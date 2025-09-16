using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying battle progress. No frame is displayed, but it is
    /// handled as a window for convenience.
    /// </summary>
    public partial class Window_BattleLog : Window_Base
    {
        protected List<string> _lines = new List<string>();
        protected Queue<MethodCall> _methods = new Queue<MethodCall>();
        protected int _waitCount = 0;
        protected string _waitMode = "";
        protected Stack<int> _baseLineStack = new Stack<int>();
        protected Spriteset_Battle _spriteset = null;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Opacity = 0;
            _lines = new List<string>();
            _methods = new Queue<MethodCall>();
            _waitCount = 0;
            _waitMode = "";
            _baseLineStack = new Stack<int>();
            _spriteset = null;
            Refresh();
        }

        public virtual void SetSpriteset(Spriteset_Battle spriteset)
        {
            _spriteset = spriteset;
        }

        protected virtual int MaxLines()
        {
            return 10;
        }

        protected virtual int NumLines()
        {
            return _lines.Count;
        }

        protected virtual int MessageSpeed()
        {
            return 16;
        }

        public virtual bool IsBusy()
        {
            return _waitCount > 0 || !string.IsNullOrEmpty(_waitMode) || _methods.Count > 0;
        }

        public override void UpdateRmmz()
        {
            if (!UpdateWait())
            {
                CallNextMethod();
            }
        }

        protected virtual bool UpdateWait()
        {
            return UpdateWaitCount() || UpdateWaitMode();
        }

        protected virtual bool UpdateWaitCount()
        {
            if (_waitCount > 0)
            {
                _waitCount -= IsFastForward() ? 3 : 1;
                if (_waitCount < 0)
                {
                    _waitCount = 0;
                }
                return true;
            }
            return false;
        }

        protected virtual bool UpdateWaitMode()
        {
            bool waiting = false;
            switch (_waitMode)
            {
                case "effect":
                    waiting = _spriteset.IsEffecting();
                    break;
                case "movement":
                    waiting = _spriteset.IsAnyoneMoving();
                    break;
            }
            if (!waiting)
            {
                _waitMode = "";
            }
            return waiting;
        }

        protected virtual void SetWaitMode(string waitMode)
        {
            _waitMode = waitMode;
        }

        protected virtual void CallNextMethod()
        {
            if (_methods.Count > 0)
            {
                var method = _methods.Dequeue();
                try
                {
                    InvokeMethod(method);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Method not found or failed: {method.Name} - {ex.Message}");
                }
            }
        }

        protected virtual void InvokeMethod(MethodCall method)
        {
            switch (method.Name)
            {
                case "wait":
                    Wait();
                    break;
                case "waitForEffect":
                    WaitForEffect();
                    break;
                case "waitForMovement":
                    WaitForMovement();
                    break;
                case "addText":
                    AddText((string)method.Params[0]);
                    break;
                case "pushBaseLine":
                    PushBaseLine();
                    break;
                case "popBaseLine":
                    PopBaseLine();
                    break;
                case "waitForNewLine":
                    WaitForNewLine();
                    break;
                case "clear":
                    Clear();
                    break;
                case "popupDamage":
                    PopupDamage((Game_Battler)method.Params[0]);
                    break;
                case "performActionStart":
                    PerformActionStart((Game_Battler)method.Params[0], (Game_Action)method.Params[1]);
                    break;
                case "performAction":
                    PerformAction((Game_Battler)method.Params[0], (Game_Action)method.Params[1]);
                    break;
                case "performActionEnd":
                    PerformActionEnd((Game_Battler)method.Params[0]);
                    break;
                case "performDamage":
                    PerformDamage((Game_Battler)method.Params[0]);
                    break;
                case "performMiss":
                    PerformMiss((Game_Battler)method.Params[0]);
                    break;
                case "performRecovery":
                    PerformRecovery((Game_Battler)method.Params[0]);
                    break;
                case "performEvasion":
                    PerformEvasion((Game_Battler)method.Params[0]);
                    break;
                case "performMagicEvasion":
                    PerformMagicEvasion((Game_Battler)method.Params[0]);
                    break;
                case "performCounter":
                    PerformCounter((Game_Battler)method.Params[0]);
                    break;
                case "performReflection":
                    PerformReflection((Game_Battler)method.Params[0]);
                    break;
                case "performSubstitute":
                    PerformSubstitute((Game_Battler)method.Params[0], (Game_Battler)method.Params[1]);
                    break;
                case "performCollapse":
                    PerformCollapse((Game_Battler)method.Params[0]);
                    break;
                case "showAnimation":
                    ShowAnimation((Game_Battler)method.Params[0], (List<Game_Battler>)method.Params[1], (int)method.Params[2]);
                    break;
                default:
                    Debug.LogWarning($"Unknown method: {method.Name}");
                    break;
            }
        }

        protected virtual bool IsFastForward()
        {
            return Input.IsLongPressed("ok") ||
                   Input.IsPressed("shift") ||
                   TouchInput.IsLongPressed();
        }

        public virtual void Push(string methodName, params object[] parameters)
        {
            _methods.Enqueue(new MethodCall(methodName, parameters));
        }

        public virtual void Clear()
        {
            _lines.Clear();
            _baseLineStack.Clear();
            Refresh();
        }

        protected virtual void Wait()
        {
            _waitCount = MessageSpeed();
        }

        protected virtual void WaitForEffect()
        {
            SetWaitMode("effect");
        }

        protected virtual void WaitForMovement()
        {
            SetWaitMode("movement");
        }

        protected virtual void AddText(string text)
        {
            _lines.Add(text);
            Refresh();
            Wait();
        }

        protected virtual void PushBaseLine()
        {
            _baseLineStack.Push(_lines.Count);
        }

        protected virtual void PopBaseLine()
        {
            if (_baseLineStack.Count > 0)
            {
                int baseLine = _baseLineStack.Pop();
                while (_lines.Count > baseLine)
                {
                    _lines.RemoveAt(_lines.Count - 1);
                }
            }
        }

        protected virtual void WaitForNewLine()
        {
            int baseLine = 0;
            if (_baseLineStack.Count > 0)
            {
                baseLine = _baseLineStack.Peek();
            }
            if (_lines.Count > baseLine)
            {
                Wait();
            }
        }

        protected virtual void PopupDamage(Game_Battler target)
        {
            if (target.ShouldPopupDamage())
            {
                target.StartDamagePopup();
            }
        }

        protected virtual void PerformActionStart(Game_Battler subject, Game_Action action)
        {
            subject.PerformActionStart(action);
        }

        protected virtual void PerformAction(Game_Battler subject, Game_Action action)
        {
            subject.PerformAction(action);
        }

        protected virtual void PerformActionEnd(Game_Battler subject)
        {
            subject.PerformActionEnd();
        }

        protected virtual void PerformDamage(Game_Battler target)
        {
            target.PerformDamage();
        }

        protected virtual void PerformMiss(Game_Battler target)
        {
            target.PerformMiss();
        }

        protected virtual void PerformRecovery(Game_Battler target)
        {
            target.PerformRecovery();
        }

        protected virtual void PerformEvasion(Game_Battler target)
        {
            target.PerformEvasion();
        }

        protected virtual void PerformMagicEvasion(Game_Battler target)
        {
            target.PerformMagicEvasion();
        }

        protected virtual void PerformCounter(Game_Battler target)
        {
            target.PerformCounter();
        }

        protected virtual void PerformReflection(Game_Battler target)
        {
            target.PerformReflection();
        }

        protected virtual void PerformSubstitute(Game_Battler substitute, Game_Battler target)
        {
            substitute.PerformSubstitute(target);
        }

        protected virtual void PerformCollapse(Game_Battler target)
        {
            target.PerformCollapse();
        }

        protected virtual void ShowAnimation(Game_Battler subject, List<Game_Battler> targets, int animationId)
        {
            if (animationId < 0)
            {
                ShowAttackAnimation(subject, targets);
            }
            else
            {
                ShowNormalAnimation(targets, animationId);
            }
        }

        protected virtual void ShowAttackAnimation(Game_Battler subject, List<Game_Battler> targets)
        {
            if (subject.IsActor())
            {
                ShowActorAttackAnimation((Game_Actor)subject, targets);
            }
            else
            {
                ShowEnemyAttackAnimation(subject, targets);
            }
        }

        protected virtual void ShowActorAttackAnimation(Game_Actor subject, List<Game_Battler> targets)
        {
            ShowNormalAnimation(targets, subject.AttackAnimationId1(), false);
            ShowNormalAnimation(targets, subject.AttackAnimationId2(), true);
        }

        protected virtual void ShowEnemyAttackAnimation(Game_Battler subject, List<Game_Battler> targets)
        {
            Rmmz.SoundManager.PlayEnemyAttack();
        }

        protected virtual void ShowNormalAnimation(List<Game_Battler> targets, int animationId, bool mirror = false)
        {
            var animation = Rmmz.dataAnimations[animationId];
            if (animation != null)
            {
                Rmmz.gameTemp.RequestAnimation(targets.Cast<object>().ToList(), animationId, mirror);
            }
        }

        public virtual void Refresh()
        {
            DrawBackground();
            Contents.Clear();
            for (int i = 0; i < _lines.Count; i++)
            {
                DrawLineText(i);
            }
        }

        protected virtual void DrawBackground()
        {
            Rect rect = BackRect();
            Color color = BackColor();
            ContentsBack.Clear();
            ContentsBack.PaintOpacity = BackPaintOpacity();
            ContentsBack.FillRect(rect.x, rect.y, rect.width, rect.height, color);
            ContentsBack.PaintOpacity = 255;
        }

        protected virtual Rect BackRect()
        {
            float height = NumLines() * ItemHeight();
            return new Rect(0, 0, InnerWidth, height);
        }

        protected virtual Rect LineRect(int index)
        {
            float itemHeight = ItemHeight();
            float padding = ItemPadding();
            float x = padding;
            float y = index * itemHeight;
            float width = InnerWidth - padding * 2;
            float height = itemHeight;
            return new Rect(x, y, width, height);
        }

        protected virtual Color BackColor()
        {
            return Color.black;
        }

        protected virtual int BackPaintOpacity()
        {
            return 64;
        }

        protected virtual void DrawLineText(int index)
        {
            Rect rect = LineRect(index);
            Contents.ClearRect(rect.x, rect.y, rect.width, rect.height);
            DrawTextEx(_lines[index], (int)rect.x, (int)rect.y, (int)rect.width);
        }

        public virtual void StartTurn()
        {
            Push("wait");
        }

        public virtual void StartAction(Game_Battler subject, Game_Action action, List<Game_Battler> targets)
        {
            var item = action.Item();
            Push("performActionStart", subject, action);
            Push("waitForMovement");
            Push("performAction", subject, action);
            Push("showAnimation", subject, new List<Game_Battler>(targets), item.AnimationId);
            DisplayAction(subject, item);
        }

        public virtual void EndAction(Game_Battler subject)
        {
            Push("waitForNewLine");
            Push("clear");
            Push("performActionEnd", subject);
        }

        public virtual void DisplayCurrentState(Game_Battler subject)
        {
            string stateText = subject.MostImportantStateText();
            if (!string.IsNullOrEmpty(stateText))
            {
                Push("addText", stateText.RmmzFormat(subject.Name()));
                Push("wait");
                Push("clear");
            }
        }

        public virtual void DisplayRegeneration(Game_Battler subject)
        {
            Push("popupDamage", subject);
        }

        public virtual void DisplayAction(Game_Battler subject, UsableItem item)
        {
            int numMethods = _methods.Count;
            if (Rmmz.DataManager.IsSkill(item))
            {
                var skill = item as DataSkill;
                DisplayItemMessage(skill.Message1, subject, item);
                DisplayItemMessage(skill.Message2, subject, item);
            }
            else
            {
                DisplayItemMessage(Rmmz.TextManager.UseItem, subject, item);
            }
            if (_methods.Count == numMethods)
            {
                Push("wait");
            }
        }

        protected virtual void DisplayItemMessage(string fmt, Game_Battler subject, UsableItem item)
        {
            if (!string.IsNullOrEmpty(fmt))
            {
                Push("addText", fmt.RmmzFormat(subject.Name(), item.Name));
            }
        }

        public virtual void DisplayCounter(Game_Battler target)
        {
            Push("performCounter", target);
            Push("addText", Rmmz.TextManager.CounterAttack.RmmzFormat(target.Name()));
        }

        public virtual void DisplayReflection(Game_Battler target)
        {
            Push("performReflection", target);
            Push("addText", Rmmz.TextManager.MagicReflection.RmmzFormat(target.Name()));
        }

        public virtual void DisplaySubstitute(Game_Battler substitute, Game_Battler target)
        {
            string substName = substitute.Name();
            string text = Rmmz.TextManager.Substitute.RmmzFormat(substName, target.Name());
            Push("performSubstitute", substitute, target);
            Push("addText", text);
        }

        public virtual void DisplayActionResults(Game_Battler subject, Game_Battler target)
        {
            if (target.Result().used)
            {
                Push("pushBaseLine");
                DisplayCritical(target);
                Push("popupDamage", target);
                Push("popupDamage", subject);
                DisplayDamage(target);
                DisplayAffectedStatus(target);
                DisplayFailure(target);
                Push("waitForNewLine");
                Push("popBaseLine");
            }
        }

        protected virtual void DisplayFailure(Game_Battler target)
        {
            if (target.Result().IsHit() && !target.Result().success)
            {
                Push("addText", Rmmz.TextManager.ActionFailure.RmmzFormat(target.Name()));
            }
        }

        protected virtual void DisplayCritical(Game_Battler target)
        {
            if (target.Result().critical)
            {
                if (target.IsActor())
                {
                    Push("addText", Rmmz.TextManager.CriticalToActor);
                }
                else
                {
                    Push("addText", Rmmz.TextManager.CriticalToEnemy);
                }
            }
        }

        protected virtual void DisplayDamage(Game_Battler target)
        {
            if (target.Result().missed)
            {
                DisplayMiss(target);
            }
            else if (target.Result().evaded)
            {
                DisplayEvasion(target);
            }
            else
            {
                DisplayHpDamage(target);
                DisplayMpDamage(target);
                DisplayTpDamage(target);
            }
        }

        protected virtual void DisplayMiss(Game_Battler target)
        {
            string fmt;
            if (target.Result().physical)
            {
                bool isActor = target.IsActor();
                fmt = isActor ? Rmmz.TextManager.ActorNoHit : Rmmz.TextManager.EnemyNoHit;
                Push("performMiss", target);
            }
            else
            {
                fmt = Rmmz.TextManager.ActionFailure;
            }
            Push("addText", fmt.RmmzFormat(target.Name()));
        }

        protected virtual void DisplayEvasion(Game_Battler target)
        {
            string fmt;
            if (target.Result().physical)
            {
                fmt = Rmmz.TextManager.Evasion;
                Push("performEvasion", target);
            }
            else
            {
                fmt = Rmmz.TextManager.MagicEvasion;
                Push("performMagicEvasion", target);
            }
            Push("addText", fmt.RmmzFormat(target.Name()));
        }

        protected virtual void DisplayHpDamage(Game_Battler target)
        {
            if (target.Result().hpAffected)
            {
                if (target.Result().hpDamage > 0 && !target.Result().drain)
                {
                    Push("performDamage", target);
                }
                if (target.Result().hpDamage < 0)
                {
                    Push("performRecovery", target);
                }
                Push("addText", MakeHpDamageText(target));
            }
        }

        protected virtual void DisplayMpDamage(Game_Battler target)
        {
            if (target.IsAlive() && target.Result().mpDamage != 0)
            {
                if (target.Result().mpDamage < 0)
                {
                    Push("performRecovery", target);
                }
                Push("addText", MakeMpDamageText(target));
            }
        }

        protected virtual void DisplayTpDamage(Game_Battler target)
        {
            if (target.IsAlive() && target.Result().tpDamage != 0)
            {
                if (target.Result().tpDamage < 0)
                {
                    Push("performRecovery", target);
                }
                Push("addText", MakeTpDamageText(target));
            }
        }

        protected virtual void DisplayAffectedStatus(Game_Battler target)
        {
            if (target.Result().IsStatusAffected())
            {
                Push("pushBaseLine");
                DisplayChangedStates(target);
                DisplayChangedBuffs(target);
                Push("waitForNewLine");
                Push("popBaseLine");
            }
        }

        public virtual void DisplayAutoAffectedStatus(Game_Battler target)
        {
            if (target.Result().IsStatusAffected())
            {
                DisplayAffectedStatus(target);
                Push("clear");
            }
        }

        protected virtual void DisplayChangedStates(Game_Battler target)
        {
            DisplayAddedStates(target);
            DisplayRemovedStates(target);
        }

        protected virtual void DisplayAddedStates(Game_Battler target)
        {
            var result = target.Result();
            var states = result.AddedStateObjects();
            foreach (var state in states)
            {
                string stateText = target.IsActor() ? state.Message1 : state.Message2;
                if (state.Id == target.DeathStateId())
                {
                    Push("performCollapse", target);
                }
                if (!string.IsNullOrEmpty(stateText))
                {
                    Push("popBaseLine");
                    Push("pushBaseLine");
                    Push("addText", stateText.RmmzFormat(target.Name()));
                    Push("waitForEffect");
                }
            }
        }

        protected virtual void DisplayRemovedStates(Game_Battler target)
        {
            var result = target.Result();
            var states = result.RemovedStateObjects();
            foreach (var state in states)
            {
                if (!string.IsNullOrEmpty(state.Message4))
                {
                    Push("popBaseLine");
                    Push("pushBaseLine");
                    Push("addText", state.Message4.RmmzFormat(target.Name()));
                }
            }
        }

        protected virtual void DisplayChangedBuffs(Game_Battler target)
        {
            var result = target.Result();
            DisplayBuffs(target, result.addedBuffs, Rmmz.TextManager.BuffAdd);
            DisplayBuffs(target, result.addedDebuffs, Rmmz.TextManager.DebuffAdd);
            DisplayBuffs(target, result.removedBuffs, Rmmz.TextManager.BuffRemove);
        }

        protected virtual void DisplayBuffs(Game_Battler target, List<int> buffs, string fmt)
        {
            foreach (int paramId in buffs)
            {
                string text = fmt.RmmzFormat(target.Name(), Rmmz.TextManager.Param(paramId));
                Push("popBaseLine");
                Push("pushBaseLine");
                Push("addText", text);
            }
        }

        protected virtual string MakeHpDamageText(Game_Battler target)
        {
            var result = target.Result();
            int damage = result.hpDamage;
            bool isActor = target.IsActor();
            string fmt;

            if (damage > 0 && result.drain)
            {
                fmt = isActor ? Rmmz.TextManager.ActorDrain : Rmmz.TextManager.EnemyDrain;
                return fmt.RmmzFormat(target.Name(), Rmmz.TextManager.Hp, damage);
            }
            else if (damage > 0)
            {
                fmt = isActor ? Rmmz.TextManager.ActorDamage : Rmmz.TextManager.EnemyDamage;
                return fmt.RmmzFormat(target.Name(), damage);
            }
            else if (damage < 0)
            {
                fmt = isActor ? Rmmz.TextManager.ActorRecovery : Rmmz.TextManager.EnemyRecovery;
                return fmt.RmmzFormat(target.Name(), Rmmz.TextManager.Hp, -damage);
            }
            else
            {
                fmt = isActor ? Rmmz.TextManager.ActorNoDamage : Rmmz.TextManager.EnemyNoDamage;
                return fmt.RmmzFormat(target.Name());
            }
        }

        protected virtual string MakeMpDamageText(Game_Battler target)
        {
            var result = target.Result();
            int damage = result.mpDamage;
            bool isActor = target.IsActor();
            string fmt;

            if (damage > 0 && result.drain)
            {
                fmt = isActor ? Rmmz.TextManager.ActorDrain : Rmmz.TextManager.EnemyDrain;
                return fmt.RmmzFormat(target.Name(), Rmmz.TextManager.Mp, damage);
            }
            else if (damage > 0)
            {
                fmt = isActor ? Rmmz.TextManager.ActorLoss : Rmmz.TextManager.EnemyLoss;
                return fmt.RmmzFormat(target.Name(), Rmmz.TextManager.Mp, damage);
            }
            else if (damage < 0)
            {
                fmt = isActor ? Rmmz.TextManager.ActorRecovery : Rmmz.TextManager.EnemyRecovery;
                return fmt.RmmzFormat(target.Name(), Rmmz.TextManager.Mp, -damage);
            }
            else
            {
                return "";
            }
        }

        protected virtual string MakeTpDamageText(Game_Battler target)
        {
            var result = target.Result();
            int damage = result.tpDamage;
            bool isActor = target.IsActor();
            string fmt;

            if (damage > 0)
            {
                fmt = isActor ? Rmmz.TextManager.ActorLoss : Rmmz.TextManager.EnemyLoss;
                return fmt.RmmzFormat(target.Name(), Rmmz.TextManager.Tp, damage);
            }
            else if (damage < 0)
            {
                fmt = isActor ? Rmmz.TextManager.ActorGain : Rmmz.TextManager.EnemyGain;
                return fmt.RmmzFormat(target.Name(), Rmmz.TextManager.Tp, -damage);
            }
            else
            {
                return "";
            }
        }
    }

    /// <summary>
    /// Method call data structure for deferred execution
    /// </summary>
    [Serializable]
    public class MethodCall
    {
        public string Name { get; set; }
        public object[] Params { get; set; }

        public MethodCall(string name, params object[] parameters)
        {
            Name = name;
            Params = parameters ?? new object[0];
        }
    }
}
