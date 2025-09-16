using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The static class that manages battle progress.
    /// </summary>
    public partial class BattleManager
    {
        // Battle state
        private string _phase;
        private bool _inputting;
        private bool _canEscape;
        private bool _canLose;
        private bool _battleTest;
        private Action<int> _eventCallback;
        private bool _preemptive;
        private bool _surprise;
        private Game_Actor _currentActor;
        private Game_Battler _actionForcedBattler;

        // Audio state
        private AudioManager.Sound _mapBgm;
        private AudioManager.Sound _mapBgs;

        // Battle progress
        private List<Game_Battler> _actionBattlers;
        private Game_Battler _subject;
        private Game_Action _action;
        private List<Game_Battler> _targets;

        // UI references
        private Window_BattleLog _logWindow;
        private Spriteset_Battle _spriteset;

        // Battle results
        private float _escapeRatio;
        private bool _escaped;
        private BattleRewards _rewards;
        private bool _tpbNeedsPartyCommand;

        public void Setup(int troopId, bool canEscape, bool canLose)
        {
            InitMembers();
            _canEscape = canEscape;
            _canLose = canLose;
            Rmmz.gameTroop.Setup(troopId);
            Rmmz.gameScreen.OnBattleStart();
            MakeEscapeRatio();
        }

        public void InitMembers()
        {
            _phase = "";
            _inputting = false;
            _canEscape = false;
            _canLose = false;
            _battleTest = false;
            _eventCallback = null;
            _preemptive = false;
            _surprise = false;
            _currentActor = null;
            _actionForcedBattler = null;
            _mapBgm = null;
            _mapBgs = null;
            _actionBattlers = new List<Game_Battler>();
            _subject = null;
            _action = null;
            _targets = new List<Game_Battler>();
            _logWindow = null;
            _spriteset = null;
            _escapeRatio = 0;
            _escaped = false;
            _rewards = new BattleRewards();
            _tpbNeedsPartyCommand = true;
        }

        public bool IsTpb()
        {
            return Rmmz.DataSystem.BattleSystem >= 1;
        }

        public bool IsActiveTpb()
        {
            return Rmmz.DataSystem.BattleSystem == 1;
        }

        public bool IsBattleTest()
        {
            return _battleTest;
        }

        public void SetBattleTest(bool battleTest)
        {
            _battleTest = battleTest;
        }

        public void SetEventCallback(Action<int> callback)
        {
            _eventCallback = callback;
        }

        public void SetLogWindow(Window_BattleLog logWindow)
        {
            _logWindow = logWindow;
        }

        public void SetSpriteset(Spriteset_Battle spriteset)
        {
            _spriteset = spriteset;
        }

        public void OnEncounter()
        {
            _preemptive = UnityEngine.Random.value < RatePreemptive();
            _surprise = UnityEngine.Random.value < RateSurprise() && !_preemptive;
        }

        public float RatePreemptive()
        {
            return Rmmz.gameParty.RatePreemptive(Rmmz.gameTroop.Agility());
        }

        public float RateSurprise()
        {
            return Rmmz.gameParty.RateSurprise(Rmmz.gameTroop.Agility());
        }

        public void SaveBgmAndBgs()
        {
            _mapBgm = Rmmz.AudioManager.SaveBgm();
            _mapBgs = Rmmz.AudioManager.SaveBgs();
        }

        public void PlayBattleBgm()
        {
            Rmmz.AudioManager.PlayBgm(Rmmz.gameSystem.BattleBgm());
            Rmmz.AudioManager.StopBgs();
        }

        public void PlayVictoryMe()
        {
            Rmmz.AudioManager.PlayMe(Rmmz.gameSystem.VictoryMe());
        }

        public void PlayDefeatMe()
        {
            Rmmz.AudioManager.PlayMe(Rmmz.gameSystem.DefeatMe());
        }

        public void ReplayBgmAndBgs()
        {
            if (_mapBgm != null)
            {
                Rmmz.AudioManager.ReplayBgm(_mapBgm);
            }
            else
            {
                Rmmz.AudioManager.StopBgm();
            }

            if (_mapBgs != null)
            {
                Rmmz.AudioManager.ReplayBgs(_mapBgs);
            }
        }

        public void MakeEscapeRatio()
        {
            _escapeRatio = (0.5f * Rmmz.gameParty.Agility()) / Rmmz.gameTroop.Agility();
        }

        public void Update(bool timeActive)
        {
            if (!IsBusy() && !UpdateEvent())
            {
                UpdatePhase(timeActive);
            }

            if (IsTpb())
            {
                UpdateTpbInput();
            }
        }

        public void UpdatePhase(bool timeActive)
        {
            switch (_phase)
            {
                case "start":
                    UpdateStart();
                    break;
                case "turn":
                    UpdateTurn(timeActive);
                    break;
                case "action":
                    UpdateAction();
                    break;
                case "turnEnd":
                    UpdateTurnEnd();
                    break;
                case "battleEnd":
                    UpdateBattleEnd();
                    break;
            }
        }

        public bool UpdateEvent()
        {
            switch (_phase)
            {
                case "start":
                case "turn":
                case "turnEnd":
                    if (IsActionForced())
                    {
                        ProcessForcedAction();
                        return true;
                    }
                    else
                    {
                        return UpdateEventMain();
                    }
            }

            return CheckAbort();
        }

        public bool UpdateEventMain()
        {
            Rmmz.gameTroop.UpdateInterpreter();
            Rmmz.gameParty.RequestMotionRefresh();
            if (Rmmz.gameTroop.IsEventRunning() || CheckBattleEnd())
            {
                return true;
            }

            Rmmz.gameTroop.SetupBattleEvent();
            if (Rmmz.gameTroop.IsEventRunning() || Rmmz.SceneManager.IsSceneChanging())
            {
                return true;
            }

            return false;
        }

        public bool IsBusy()
        {
            return Rmmz.gameMessage.IsBusy() ||
                   _spriteset.IsBusy() ||
                   _logWindow.IsBusy();
        }

        public void UpdateTpbInput()
        {
            if (_inputting)
            {
                CheckTpbInputClose();
            }
            else
            {
                CheckTpbInputOpen();
            }
        }

        public void CheckTpbInputClose()
        {
            if (!IsPartyTpbInputtable() || NeedsActorInputCancel())
            {
                CancelActorInput();
                _currentActor = null;
                _inputting = false;
            }
        }

        public void CheckTpbInputOpen()
        {
            if (IsPartyTpbInputtable())
            {
                if (_tpbNeedsPartyCommand)
                {
                    _inputting = true;
                    _tpbNeedsPartyCommand = false;
                }
                else
                {
                    SelectNextCommand();
                }
            }
        }

        public bool IsPartyTpbInputtable()
        {
            return Rmmz.gameParty.CanInput() && IsTpbMainPhase();
        }

        public bool NeedsActorInputCancel()
        {
            return _currentActor != null && !_currentActor.CanInput();
        }

        public bool IsTpbMainPhase()
        {
            return new[] { "turn", "turnEnd", "action" }.Contains(_phase);
        }

        public bool IsInputting()
        {
            return _inputting;
        }

        public bool IsInTurn()
        {
            return _phase == "turn";
        }

        public bool IsTurnEnd()
        {
            return _phase == "turnEnd";
        }

        public bool IsAborting()
        {
            return _phase == "aborting";
        }

        public bool IsBattleEnd()
        {
            return _phase == "battleEnd";
        }

        public bool CanEscape()
        {
            return _canEscape;
        }

        public bool CanLose()
        {
            return _canLose;
        }

        public bool IsEscaped()
        {
            return _escaped;
        }

        public Game_Actor Actor()
        {
            return _currentActor;
        }

        public void StartBattle()
        {
            _phase = "start";
            Rmmz.gameSystem.OnBattleStart();
            Rmmz.gameParty.OnBattleStart(_preemptive);
            Rmmz.gameTroop.OnBattleStart(_surprise);
            DisplayStartMessages();
        }

        public void DisplayStartMessages()
        {
            foreach (string name in Rmmz.gameTroop.EnemyNames())
            {
                Rmmz.gameMessage.Add(Rmmz.TextManager.Emerge.RmmzFormat(name));
            }

            if (_preemptive)
            {
                Rmmz.gameMessage.Add(Rmmz.TextManager.Preemptive.RmmzFormat(Rmmz.gameParty.Name()));
            }
            else if (_surprise)
            {
                Rmmz.gameMessage.Add(Rmmz.TextManager.Surprise.RmmzFormat(Rmmz.gameParty.Name()));
            }
        }

        public void StartInput()
        {
            _phase = "input";
            _inputting = true;
            Rmmz.gameParty.MakeActions();
            Rmmz.gameTroop.MakeActions();
            _currentActor = null;
            if (_surprise || !Rmmz.gameParty.CanInput())
            {
                StartTurn();
            }
        }

        public Game_Action InputtingAction()
        {
            return _currentActor?.InputtingAction();
        }

        public void SelectNextCommand()
        {
            if (_currentActor != null)
            {
                if (_currentActor.SelectNextCommand())
                {
                    return;
                }

                FinishActorInput();
            }

            SelectNextActor();
        }

        public void SelectNextActor()
        {
            ChangeCurrentActor(true);
            if (_currentActor == null)
            {
                if (IsTpb())
                {
                    ChangeCurrentActor(true);
                }
                else
                {
                    StartTurn();
                }
            }
        }

        public void SelectPreviousCommand()
        {
            if (_currentActor != null)
            {
                if (_currentActor.SelectPreviousCommand())
                {
                    return;
                }

                CancelActorInput();
            }

            SelectPreviousActor();
        }

        public void SelectPreviousActor()
        {
            if (IsTpb())
            {
                ChangeCurrentActor(true);
                if (_currentActor == null)
                {
                    _inputting = Rmmz.gameParty.CanInput();
                }
            }
            else
            {
                ChangeCurrentActor(false);
            }
        }

        public void ChangeCurrentActor(bool forward)
        {
            var members = Rmmz.gameParty.BattleMembers().ToList();
            Game_Actor actor = _currentActor;

            while (true)
            {
                int currentIndex = members.IndexOf(actor);
                int nextIndex = currentIndex + (forward ? 1 : -1);
                actor = (nextIndex >= 0 && nextIndex < members.Count) ? members[nextIndex] : null;

                if (actor == null || actor.CanInput())
                {
                    break;
                }
            }

            _currentActor = actor;
            StartActorInput();
        }

        public void StartActorInput()
        {
            if (_currentActor != null)
            {
                _currentActor.SetActionState("inputting");
                _inputting = true;
            }
        }

        public void FinishActorInput()
        {
            if (_currentActor != null)
            {
                if (IsTpb())
                {
                    _currentActor.StartTpbCasting();
                }

                _currentActor.SetActionState("waiting");
            }
        }

        public void CancelActorInput()
        {
            if (_currentActor != null)
            {
                _currentActor.SetActionState("undecided");
            }
        }

        public void UpdateStart()
        {
            if (IsTpb())
            {
                _phase = "turn";
            }
            else
            {
                StartInput();
            }
        }

        public void StartTurn()
        {
            _phase = "turn";
            Rmmz.gameTroop.IncreaseTurn();
            Rmmz.gameParty.RequestMotionRefresh();
            if (!IsTpb())
            {
                MakeActionOrders();
                _logWindow.StartTurn();
                _inputting = false;
            }
        }

        public void UpdateTurn(bool timeActive)
        {
            Rmmz.gameParty.RequestMotionRefresh();
            if (IsTpb() && timeActive)
            {
                UpdateTpb();
            }

            if (_subject == null)
            {
                _subject = GetNextSubject();
            }

            if (_subject != null)
            {
                ProcessTurn();
            }
            else if (!IsTpb())
            {
                EndTurn();
            }
        }

        public void UpdateTpb()
        {
            Rmmz.gameParty.UpdateTpb();
            Rmmz.gameTroop.UpdateTpb();
            UpdateAllTpbBattlers();
            CheckTpbTurnEnd();
        }

        public void UpdateAllTpbBattlers()
        {
            foreach (var battler in AllBattleMembers())
            {
                UpdateTpbBattler(battler);
            }
        }

        public void UpdateTpbBattler(Game_Battler battler)
        {
            if (battler.IsTpbTurnEnd())
            {
                battler.OnTurnEnd();
                battler.StartTpbTurn();
                DisplayBattlerStatus(battler, false);
            }
            else if (battler.IsTpbReady())
            {
                battler.StartTpbAction();
                _actionBattlers.Add(battler);
            }
            else if (battler.IsTpbTimeout())
            {
                battler.OnTpbTimeout();
                DisplayBattlerStatus(battler, true);
            }
        }

        public void CheckTpbTurnEnd()
        {
            if (Rmmz.gameTroop.IsTpbTurnEnd())
            {
                EndTurn();
            }
        }

        public void ProcessTurn()
        {
            var subject = _subject;
            var action = subject.CurrentAction();
            if (action != null)
            {
                action.Prepare();
                if (action.IsValid())
                {
                    StartAction();
                }

                subject.RemoveCurrentAction();
            }
            else
            {
                EndAction();
                _subject = null;
            }
        }

        public void EndBattlerActions(Game_Battler battler)
        {
            battler.SetActionState(IsTpb() ? "undecided" : "done");
            battler.OnAllActionsEnd();
            battler.ClearTpbChargeTime();
            DisplayBattlerStatus(battler, true);
        }

        public void EndTurn()
        {
            _phase = "turnEnd";
            _preemptive = false;
            _surprise = false;
        }

        public void UpdateTurnEnd()
        {
            if (IsTpb())
            {
                StartTurn();
            }
            else
            {
                EndAllBattlersTurn();
                _phase = "start";
            }
        }

        public void EndAllBattlersTurn()
        {
            foreach (var battler in AllBattleMembers())
            {
                battler.OnTurnEnd();
                DisplayBattlerStatus(battler, false);
            }
        }

        public void DisplayBattlerStatus(Game_Battler battler, bool current)
        {
            _logWindow.DisplayAutoAffectedStatus(battler);
            if (current)
            {
                _logWindow.DisplayCurrentState(battler);
            }

            _logWindow.DisplayRegeneration(battler);
        }

        public Game_Battler GetNextSubject()
        {
            while (_actionBattlers.Count > 0)
            {
                var battler = _actionBattlers[0];
                _actionBattlers.RemoveAt(0);

                if (battler.IsBattleMember() && battler.IsAlive())
                {
                    return battler;
                }
            }

            return null;
        }

        public List<Game_Battler> AllBattleMembers()
        {
            var result = new List<Game_Battler>();
            result.AddRange(Rmmz.gameParty.BattleMembers().Cast<Game_Battler>());
            result.AddRange(Rmmz.gameTroop.Members());
            return result;
        }

        public void MakeActionOrders()
        {
            var battlers = new List<Game_Battler>();
            if (!_surprise)
            {
                battlers.AddRange(Rmmz.gameParty.BattleMembers().Cast<Game_Battler>());
            }

            if (!_preemptive)
            {
                battlers.AddRange(Rmmz.gameTroop.Members());
            }

            foreach (var battler in battlers)
            {
                battler.MakeSpeed();
            }

            battlers.Sort((a, b) => b.Speed().CompareTo(a.Speed()));
            _actionBattlers = battlers;
        }

        public void StartAction()
        {
            var subject = _subject;
            var action = subject.CurrentAction();
            var targets = action.MakeTargets();
            _phase = "action";
            _action = action;
            _targets = targets;
            subject.CancelMotionRefresh();
            subject.UseItem(action.Item());
            _action.ApplyGlobal();
            _logWindow.StartAction(subject, action, targets);
        }

        public void UpdateAction()
        {
            if (_targets.Count > 0)
            {
                var target = _targets[0];
                _targets.RemoveAt(0);
                InvokeAction(_subject, target);
            }
            else
            {
                EndAction();
            }
        }

        public void EndAction()
        {
            _logWindow.EndAction(_subject);
            _phase = "turn";
            if (_subject.NumActions() == 0)
            {
                EndBattlerActions(_subject);
                _subject = null;
            }
        }

        public void InvokeAction(Game_Battler subject, Game_Battler target)
        {
            _logWindow.Push("pushBaseLine");
            if (UnityEngine.Random.value < _action.ItemCnt(target))
            {
                InvokeCounterAttack(subject, target);
            }
            else if (UnityEngine.Random.value < _action.ItemMrf(target))
            {
                InvokeMagicReflection(subject, target);
            }
            else
            {
                InvokeNormalAction(subject, target);
            }

            subject.SetLastTarget(target);
            _logWindow.Push("popBaseLine");
        }

        public void InvokeNormalAction(Game_Battler subject, Game_Battler target)
        {
            var realTarget = ApplySubstitute(target);
            _action.Apply(realTarget);
            _logWindow.DisplayActionResults(subject, realTarget);
        }

        public void InvokeCounterAttack(Game_Battler subject, Game_Battler target)
        {
            var action = Game_Action.Create(target);
            action.SetAttack();
            action.Apply(subject);
            _logWindow.DisplayCounter(target);
            _logWindow.DisplayActionResults(target, subject);
        }

        public void InvokeMagicReflection(Game_Battler subject, Game_Battler target)
        {
            _action.SetReflectionTarget(target);
            _logWindow.DisplayReflection(target);
            _action.Apply(subject);
            _logWindow.DisplayActionResults(target, subject);
        }

        public Game_Battler ApplySubstitute(Game_Battler target)
        {
            if (CheckSubstitute(target))
            {
                var substitute = target.FriendsUnit().SubstituteBattler();
                if (substitute != null && target != substitute)
                {
                    _logWindow.DisplaySubstitute(substitute, target);
                    return substitute;
                }
            }

            return target;
        }

        public bool CheckSubstitute(Game_Battler target)
        {
            return target.IsDying() && !_action.IsCertainHit();
        }

        public bool IsActionForced()
        {
            return _actionForcedBattler != null;
        }

        public void ForceAction(Game_Battler battler)
        {
            if (battler.NumActions() > 0)
            {
                _actionForcedBattler = battler;
                _actionBattlers.Remove(battler);
            }
        }

        public void ProcessForcedAction()
        {
            if (_actionForcedBattler != null)
            {
                if (_subject != null)
                {
                    EndBattlerActions(_subject);
                }

                _subject = _actionForcedBattler;
                _actionForcedBattler = null;
                StartAction();
                _subject.RemoveCurrentAction();
            }
        }

        public void Abort()
        {
            _phase = "aborting";
        }

        public bool CheckBattleEnd()
        {
            if (!string.IsNullOrEmpty(_phase))
            {
                if (Rmmz.gameParty.IsEscaped())
                {
                    ProcessPartyEscape();
                    return true;
                }
                else if (Rmmz.gameParty.IsAllDead())
                {
                    ProcessDefeat();
                    return true;
                }
                else if (Rmmz.gameTroop.IsAllDead())
                {
                    ProcessVictory();
                    return true;
                }
            }

            return false;
        }

        public bool CheckAbort()
        {
            if (IsAborting())
            {
                ProcessAbort();
                return true;
            }

            return false;
        }

        public void ProcessVictory()
        {
            Rmmz.gameParty.RemoveBattleStates();
            Rmmz.gameParty.PerformVictory();
            PlayVictoryMe();
            ReplayBgmAndBgs();
            MakeRewards();
            DisplayVictoryMessage();
            DisplayRewards();
            GainRewards();
            EndBattle(0);
        }

        public bool ProcessEscape()
        {
            Rmmz.gameParty.PerformEscape();
            Rmmz.SoundManager.PlayEscape();
            bool success = _preemptive || UnityEngine.Random.value < _escapeRatio;
            if (success)
            {
                OnEscapeSuccess();
            }
            else
            {
                OnEscapeFailure();
            }

            return success;
        }

        public void OnEscapeSuccess()
        {
            DisplayEscapeSuccessMessage();
            _escaped = true;
            ProcessAbort();
        }

        public void OnEscapeFailure()
        {
            Rmmz.gameParty.OnEscapeFailure();
            DisplayEscapeFailureMessage();
            _escapeRatio += 0.1f;
            if (!IsTpb())
            {
                StartTurn();
            }
        }

        public void ProcessPartyEscape()
        {
            _escaped = true;
            ProcessAbort();
        }

        public void ProcessAbort()
        {
            Rmmz.gameParty.RemoveBattleStates();
            _logWindow.Clear();
            ReplayBgmAndBgs();
            EndBattle(1);
        }

        public void ProcessDefeat()
        {
            DisplayDefeatMessage();
            PlayDefeatMe();
            if (_canLose)
            {
                ReplayBgmAndBgs();
            }
            else
            {
                Rmmz.AudioManager.StopBgm();
            }

            EndBattle(2);
        }

        public void EndBattle(int result)
        {
            _phase = "battleEnd";
            CancelActorInput();
            _inputting = false;
            if (_eventCallback != null)
            {
                _eventCallback(result);
            }

            if (result == 0)
            {
                Rmmz.gameSystem.OnBattleWin();
            }
            else if (_escaped)
            {
                Rmmz.gameSystem.OnBattleEscape();
            }

            Rmmz.gameTemp.ClearCommonEventReservation();
        }

        public void UpdateBattleEnd()
        {
            if (IsBattleTest())
            {
                Rmmz.AudioManager.StopBgm();
                Rmmz.SceneManager.Exit();
            }
            else if (!_escaped && Rmmz.gameParty.IsAllDead())
            {
                if (_canLose)
                {
                    Rmmz.gameParty.ReviveBattleMembers();
                    Rmmz.SceneManager.Pop();
                }
                else
                {
                    Scene_Gameover.Goto();
                }
            }
            else
            {
                Rmmz.SceneManager.Pop();
            }

            _phase = "";
        }

        public void MakeRewards()
        {
            _rewards = new BattleRewards
            {
                Gold = Rmmz.gameTroop.GoldTotal(),
                Exp = Rmmz.gameTroop.ExpTotal(),
                Items = Rmmz.gameTroop.MakeDropItems()
            };
        }

        public void DisplayVictoryMessage()
        {
            Rmmz.gameMessage.Add(Rmmz.TextManager.Victory.RmmzFormat(Rmmz.gameParty.Name()));
        }

        public void DisplayDefeatMessage()
        {
            Rmmz.gameMessage.Add(Rmmz.TextManager.Defeat.RmmzFormat(Rmmz.gameParty.Name()));
        }

        public void DisplayEscapeSuccessMessage()
        {
            Rmmz.gameMessage.Add(Rmmz.TextManager.EscapeStart.RmmzFormat(Rmmz.gameParty.Name()));
        }

        public void DisplayEscapeFailureMessage()
        {
            Rmmz.gameMessage.Add(Rmmz.TextManager.EscapeStart.RmmzFormat(Rmmz.gameParty.Name()));
            Rmmz.gameMessage.Add("\\." + Rmmz.TextManager.EscapeFailure);
        }

        public void DisplayRewards()
        {
            DisplayExp();
            DisplayGold();
            DisplayDropItems();
        }

        public void DisplayExp()
        {
            int exp = _rewards.Exp;
            if (exp > 0)
            {
                string text = Rmmz.TextManager.ObtainExp.RmmzFormat(exp, Rmmz.TextManager.Exp);
                Rmmz.gameMessage.Add("\\." + text);
            }
        }

        public void DisplayGold()
        {
            int gold = _rewards.Gold;
            if (gold > 0)
            {
                Rmmz.gameMessage.Add("\\." + Rmmz.TextManager.ObtainGold.RmmzFormat(gold));
            }
        }

        public void DisplayDropItems()
        {
            var items = _rewards.Items;
            if (items.Count > 0)
            {
                Rmmz.gameMessage.NewPage();
                foreach (var item in items)
                {
                    Rmmz.gameMessage.Add(Rmmz.TextManager.ObtainItem.RmmzFormat(item.Name));
                }
            }
        }

        public void GainRewards()
        {
            GainExp();
            GainGold();
            GainDropItems();
        }

        public void GainExp()
        {
            int exp = _rewards.Exp;
            foreach (var actor in Rmmz.gameParty.AllMembers())
            {
                actor.GainExp(exp);
            }
        }

        public void GainGold()
        {
            Rmmz.gameParty.GainGold(_rewards.Gold);
        }

        public void GainDropItems()
        {
            foreach (var item in _rewards.Items)
            {
                Rmmz.gameParty.GainItem(item, 1);
            }
        }
    }

    /// <summary>
    /// Battle rewards data structure
    /// </summary>
    [Serializable]
    public class BattleRewards
    {
        public int Gold { get; set; } = 0;
        public int Exp { get; set; } = 0;
        
        public List<DataCommonItem> Items { get; set; } = new();
    }
}