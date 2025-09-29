using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for an actor.
    /// </summary>
    [Serializable]
    public partial class Game_Actor //: Game_Battler
    {
        protected int _actorId;
        protected string _name;
        protected string _nickname;
        protected string _profile;
        protected int _classId;
        protected int _level;
        protected string _characterName;
        protected int _characterIndex;
        protected string _faceName;
        protected int _faceIndex;
        protected string _battlerName;
        protected Dictionary<int, int> _exp;
        protected List<int> _skills;
        protected List<Game_Item> _equips;
        protected int _actionInputIndex;
        protected Game_Item _lastMenuSkill;
        protected Game_Item _lastBattleSkill;
        protected string _lastCommandSymbol;
        protected Dictionary<int, int> _stateSteps;

        public override int Level => _level;
        
        protected Game_Actor(int actorId)
        {
            Setup(actorId);
        }

        protected override void InitMembers()
        {
            base.InitMembers();
            _actorId = 0;
            _name = "";
            _nickname = "";
            _profile = "";
            _classId = 0;
            _level = 0;
            _characterName = "";
            _characterIndex = 0;
            _faceName = "";
            _faceIndex = 0;
            _battlerName = "";
            _exp = new Dictionary<int, int>();
            _skills = new List<int>();
            _equips = new List<Game_Item>();
            _actionInputIndex = 0;
            _lastMenuSkill = Game_Item.Create();
            _lastBattleSkill = Game_Item.Create();
            _lastCommandSymbol = "";
            _stateSteps = new Dictionary<int, int>();
        }

        public virtual void Setup(int actorId)
        {
            var actor = Rmmz.dataActors[actorId];
            _actorId = actorId;
            _name = actor.Name;
            _nickname = actor.Nickname;
            _profile = actor.Profile;
            _classId = actor.ClassId;
            _level = actor.InitialLevel;
            InitImages();
            InitExp();
            InitSkills();
            InitEquips(actor.Equips);
            ClearParamPlus();
            RecoverAll();
        }

        public virtual int ActorId()
        {
            return _actorId;
        }

        public virtual DataActor Actor()
        {
            return Rmmz.dataActors[_actorId];
        }

        public override string Name()
        {
            return _name;
        }

        public virtual void SetName(string name)
        {
            _name = name;
        }

        public virtual string Nickname()
        {
            return _nickname;
        }

        public virtual void SetNickname(string nickname)
        {
            _nickname = nickname;
        }

        public virtual string Profile()
        {
            return _profile;
        }

        public virtual void SetProfile(string profile)
        {
            _profile = profile;
        }

        public virtual string CharacterName()
        {
            return _characterName;
        }

        public virtual int CharacterIndex()
        {
            return _characterIndex;
        }

        public virtual string FaceName()
        {
            return _faceName;
        }

        public virtual int FaceIndex()
        {
            return _faceIndex;
        }

        public virtual string BattlerName()
        {
            return _battlerName;
        }

        protected override void ClearStates()
        {
            base.ClearStates();
            _stateSteps = new Dictionary<int, int>();
        }

        public override void EraseState(int stateId)
        {
            base.EraseState(stateId);
            _stateSteps.Remove(stateId);
        }

        public override void ResetStateCounts(int stateId)
        {
            base.ResetStateCounts(stateId);
            _stateSteps[stateId] = Rmmz.dataStates[stateId].StepsToRemove;
        }

        protected virtual void InitImages()
        {
            var actor = Actor();
            _characterName = actor.CharacterName;
            _characterIndex = actor.CharacterIndex;
            _faceName = actor.FaceName;
            _faceIndex = actor.FaceIndex;
            _battlerName = actor.BattlerName;
        }

        protected virtual int ExpForLevel(int level)
        {
            var c = CurrentClass();
            float basis = c.ExpParams[0];
            float extra = c.ExpParams[1];
            float acc_a = c.ExpParams[2];
            float acc_b = c.ExpParams[3];
            return Mathf.RoundToInt(
                (basis * Mathf.Pow(level - 1, 0.9f + acc_a / 250f) * level * (level + 1)) /
                (6 + Mathf.Pow(level, 2) / 50f / acc_b) +
                (level - 1) * extra
            );
        }

        protected virtual void InitExp()
        {
            _exp[_classId] = CurrentLevelExp();
        }

        public virtual int CurrentExp()
        {
            return _exp.ContainsKey(_classId) ? _exp[_classId] : 0;
        }

        public virtual int CurrentLevelExp()
        {
            return ExpForLevel(_level);
        }

        public virtual int NextLevelExp()
        {
            return ExpForLevel(_level + 1);
        }

        public virtual int NextRequiredExp()
        {
            return NextLevelExp() - CurrentExp();
        }

        public virtual int MaxLevel()
        {
            return Actor().MaxLevel;
        }

        public virtual bool IsMaxLevel()
        {
            return _level >= MaxLevel();
        }

        protected virtual void InitSkills()
        {
            _skills = new List<int>();
            foreach (var learning in CurrentClass().Learnings)
            {
                if (learning.Level <= _level)
                {
                    LearnSkill(learning.SkillId);
                }
            }
        }

        protected virtual void InitEquips(List<int> equips)
        {
            var slots = EquipSlots();
            int maxSlots = slots.Count;
            _equips = new List<Game_Item>();
            for (int i = 0; i < maxSlots; i++)
            {
                _equips.Add(Game_Item.Create());
            }
            for (int j = 0; j < equips.Count; j++)
            {
                if (j < maxSlots)
                {
                    _equips[j].SetEquip(slots[j] == 1, equips[j]);
                }
            }
            ReleaseUnequippableItems(true);
            Refresh();
        }

        public virtual List<int> EquipSlots()
        {
            var slots = new List<int>();
            for (int i = 1; i < Rmmz.dataSystem.EquipTypes.Length; i++)
            {
                slots.Add(i);
            }
            if (slots.Count >= 2 && IsDualWield())
            {
                slots[1] = 1;
            }
            return slots;
        }

        public virtual IEnumerable<EquipableItem> Equips()
        {
            return _equips.Select(item => item.Object<EquipableItem>());
        }

        public virtual IEnumerable<DataWeapon> Weapons()
        {
            return Equips().Where(item => item != null && Rmmz.DataManager.IsWeapon(item)).Cast<DataWeapon>();
        }

        public virtual IEnumerable<DataArmor> Armors()
        {
            return Equips().Where(item => item != null && Rmmz.DataManager.IsArmor(item)).Cast<DataArmor>();
        }

        public virtual bool HasWeapon(DataWeapon dataWeapon)
        {
            return Weapons().Contains(dataWeapon);
        }

        public virtual bool HasArmor(DataArmor dataArmor)
        {
            return Armors().Contains(dataArmor);
        }

        public virtual bool IsEquipChangeOk(int slotId)
        {
            var slots = EquipSlots();
            return !IsEquipTypeLocked(slots[slotId]) && !IsEquipTypeSealed(slots[slotId]);
        }

        public virtual void ChangeEquip(int slotId, EquipableItem item)
        {
            var equips = Equips().ToList();
            if (TradeItemWithParty(item, equips[slotId]) &&
                (item == null || EquipSlots()[slotId] == item.EtypeId))
            {
                _equips[slotId].SetObject(item);
                Refresh();
            }
        }

        public virtual void ForceChangeEquip(int slotId, EquipableItem item)
        {
            _equips[slotId].SetObject(item);
            ReleaseUnequippableItems(true);
            Refresh();
        }

        protected virtual bool TradeItemWithParty(DataCommonItem newItem, EquipableItem oldItem)
        {
            if (newItem != null && !Rmmz.gameParty.HasItem(newItem))
            {
                return false;
            }
            else
            {
                Rmmz.gameParty.GainItem(oldItem, 1);
                Rmmz.gameParty.LoseItem(newItem, 1);
                return true;
            }
        }

        public virtual void ChangeEquipById(int etypeId, int itemId)
        {
            int slotId = etypeId - 1;
            if (EquipSlots()[slotId] == 1)
            {
                ChangeEquip(slotId, Rmmz.dataWeapons[itemId]);
            }
            else
            {
                ChangeEquip(slotId, Rmmz.dataArmors[itemId]);
            }
        }

        public virtual bool IsEquipped(EquipableItem item)
        {
            return Equips().Contains(item);
        }

        public virtual void DiscardEquip(EquipableItem item)
        {
            int slotId = Equips().ToList().IndexOf(item);
            if (slotId >= 0)
            {
                _equips[slotId].SetObject(null);
            }
        }

        protected virtual void ReleaseUnequippableItems(bool forcing)
        {
            while (true)
            {
                var slots = EquipSlots();
                var equips = Equips().ToList();
                bool changed = false;
                for (int i = 0; i < equips.Count; i++)
                {
                    var item = equips[i];
                    if (item != null && (!CanEquip(item) || item.EtypeId != slots[i]))
                    {
                        if (!forcing)
                        {
                            TradeItemWithParty(null, item);
                        }
                        _equips[i].SetObject(null);
                        changed = true;
                    }
                }
                if (!changed)
                {
                    break;
                }
            }
        }

        public virtual void ClearEquipments()
        {
            int maxSlots = EquipSlots().Count;
            for (int i = 0; i < maxSlots; i++)
            {
                if (IsEquipChangeOk(i))
                {
                    ChangeEquip(i, null);
                }
            }
        }

        public virtual void OptimizeEquipments()
        {
            int maxSlots = EquipSlots().Count;
            ClearEquipments();
            for (int i = 0; i < maxSlots; i++)
            {
                if (IsEquipChangeOk(i))
                {
                    ChangeEquip(i, BestEquipItem(i));
                }
            }
        }

        protected virtual EquipableItem BestEquipItem(int slotId)
        {
            int etypeId = EquipSlots()[slotId];
            var items = Rmmz.gameParty.EquipItems()
                .Where(item => item.EtypeId == etypeId && CanEquip(item));
            EquipableItem bestItem = null;
            int bestPerformance = -1000;
            foreach (var item in items)
            {
                int performance = CalcEquipItemPerformance(item);
                if (performance > bestPerformance)
                {
                    bestPerformance = performance;
                    bestItem = item;
                }
            }
            return bestItem;
        }

        protected virtual int CalcEquipItemPerformance(EquipableItem item)
        {
            return item.Params.Sum();
        }

        public override bool IsSkillWtypeOk(DataSkill dataSkill)
        {
            int wtypeId1 = dataSkill.RequiredWtypeId1;
            int wtypeId2 = dataSkill.RequiredWtypeId2;
            if ((wtypeId1 == 0 && wtypeId2 == 0) ||
                (wtypeId1 > 0 && IsWtypeEquipped(wtypeId1)) ||
                (wtypeId2 > 0 && IsWtypeEquipped(wtypeId2)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual bool IsWtypeEquipped(int wtypeId)
        {
            return Weapons().Any(weapon => weapon.WtypeId == wtypeId);
        }

        public override void Refresh()
        {
            ReleaseUnequippableItems(false);
            base.Refresh();
        }

        public override void Hide()
        {
            base.Hide();
            Rmmz.gameTemp.RequestBattleRefresh();
        }

        public override bool IsActor()
        {
            return true;
        }

        public override Game_Unit FriendsUnit()
        {
            return Rmmz.gameParty;
        }

        public override Game_Unit OpponentsUnit()
        {
            return Rmmz.gameTroop;
        }

        public override int Index()
        {
            return Rmmz.gameParty.Members().ToList().IndexOf(this);
        }

        public override bool IsBattleMember()
        {
            return Rmmz.gameParty.BattleMembers().Contains(this);
        }

        public virtual bool IsFormationChangeOk()
        {
            return true;
        }

        public virtual DataClass CurrentClass()
        {
            return Rmmz.dataClasses[_classId];
        }

        public virtual bool IsClass(DataClass gameDataClass)
        {
            return gameDataClass != null && _classId == gameDataClass.Id;
        }

        public virtual List<int> SkillTypes()
        {
            var skillTypes = AddedSkillTypes().OrderBy(x => x).ToList();
            return skillTypes.Distinct().ToList();
        }

        public virtual List<DataSkill> Skills()
        {
            var list = new List<DataSkill>();
            var allSkillIds = _skills.Concat(AddedSkills()).ToList();
            foreach (int id in allSkillIds)
            {
                var skill = Rmmz.dataSkills[id];
                if (!list.Contains(skill))
                {
                    list.Add(skill);
                }
            }
            return list;
        }

        public virtual List<DataSkill> UsableSkills()
        {
            return Skills().Where(skill => CanUse(skill)).ToList();
        }

        protected override IEnumerable<ITraitsObject> TraitObjects()
        {
            foreach (var obj in base.TraitObjects())
            {
                yield return obj;
            }
            
            yield return Actor();
            yield return CurrentClass();
            foreach (var item in Equips())
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        public override IEnumerable<int> AttackElements()
        {
            var set = base.AttackElements();
            foreach (var element in set)
            {
                yield return element;
            }
            
            if (HasNoWeapons() && !set.Contains(BareHandsElementId()))
            {
                yield return BareHandsElementId();
            }
        }

        protected virtual bool HasNoWeapons()
        {
            return !Weapons().Any();
        }

        protected virtual int BareHandsElementId()
        {
            return 1;
        }

        protected override int ParamBase(int paramId)
        {
            return CurrentClass().Params[paramId][_level];
        }

        protected override int ParamPlus(int paramId)
        {
            int value = base.ParamPlus(paramId);
            foreach (var item in Equips())
            {
                if (item != null)
                {
                    value += item.Params[paramId];
                }
            }
            return value;
        }

        public virtual int AttackAnimationId1()
        {
            if (HasNoWeapons())
            {
                return BareHandsAnimationId();
            }
            else
            {
                var weapons = Weapons();
                return weapons.ElementAtOrDefault(0)?.AnimationId ?? 0;
            }
        }

        public virtual int AttackAnimationId2()
        {
            var weapons = Weapons();
            return weapons.ElementAtOrDefault(1)?.AnimationId ?? 0;
        }

        protected virtual int BareHandsAnimationId()
        {
            return 1;
        }

        public virtual void ChangeExp(int exp, bool show)
        {
            _exp[_classId] = Mathf.Max(exp, 0);
            int lastLevel = _level;
            var lastSkills = Skills();
            while (!IsMaxLevel() && CurrentExp() >= NextLevelExp())
            {
                LevelUp();
            }
            while (CurrentExp() < CurrentLevelExp())
            {
                LevelDown();
            }
            if (show && _level > lastLevel)
            {
                DisplayLevelUp(FindNewSkills(lastSkills));
            }
            Refresh();
        }

        protected virtual void LevelUp()
        {
            _level++;
            foreach (var learning in CurrentClass().Learnings)
            {
                if (learning.Level == _level)
                {
                    LearnSkill(learning.SkillId);
                }
            }
        }

        protected virtual void LevelDown()
        {
            _level--;
        }

        protected virtual List<DataSkill> FindNewSkills(List<DataSkill> lastSkills)
        {
            var newSkills = Skills().ToList();
            foreach (var lastSkill in lastSkills)
            {
                newSkills.Remove(lastSkill);
            }
            return newSkills;
        }

        protected virtual void DisplayLevelUp(List<DataSkill> newSkills)
        {
            string text = Rmmz.TextManager.LevelUp.RmmzFormat(_name, Rmmz.TextManager.Level, _level);
            Rmmz.gameMessage.NewPage();
            Rmmz.gameMessage.Add(text);
            foreach (var skill in newSkills)
            {
                Rmmz.gameMessage.Add(Rmmz.TextManager.ObtainSkill.RmmzFormat(skill.Name));
            }
        }

        public virtual void GainExp(int exp)
        {
            int newExp = CurrentExp() + Mathf.RoundToInt(exp * FinalExpRate());
            ChangeExp(newExp, ShouldDisplayLevelUp());
        }

        protected virtual float FinalExpRate()
        {
            return Exr * (IsBattleMember() ? 1 : BenchMembersExpRate());
        }

        protected virtual float BenchMembersExpRate()
        {
            return Rmmz.dataSystem.OptExtraExp ? 1 : 0;
        }

        protected virtual bool ShouldDisplayLevelUp()
        {
            return true;
        }

        public virtual void ChangeLevel(int level, bool show)
        {
            level = Mathf.Clamp(level, 1, MaxLevel());
            ChangeExp(ExpForLevel(level), show);
        }

        public virtual void LearnSkill(int skillId)
        {
            if (!IsLearnedSkill(skillId))
            {
                _skills.Add(skillId);
                _skills.Sort();
            }
        }

        public virtual void ForgetSkill(int skillId)
        {
            _skills.Remove(skillId);
        }

        public virtual bool IsLearnedSkill(int skillId)
        {
            return _skills.Contains(skillId);
        }

        public virtual bool HasSkill(int skillId)
        {
            return Skills().Contains(Rmmz.dataSkills[skillId]);
        }

        public virtual void ChangeClass(int classId, bool keepExp)
        {
            if (keepExp)
            {
                _exp[classId] = CurrentExp();
            }
            _classId = classId;
            _level = 0;
            ChangeExp(_exp.ContainsKey(_classId) ? _exp[_classId] : 0, false);
            Refresh();
        }

        public virtual void SetCharacterImage(string characterName, int characterIndex)
        {
            _characterName = characterName;
            _characterIndex = characterIndex;
        }

        public virtual void SetFaceImage(string faceName, int faceIndex)
        {
            _faceName = faceName;
            _faceIndex = faceIndex;
            Rmmz.gameTemp.RequestBattleRefresh();
        }

        public virtual void SetBattlerImage(string battlerName)
        {
            _battlerName = battlerName;
        }

        public override bool IsSpriteVisible()
        {
            return Rmmz.gameSystem.IsSideView();
        }

        public override void PerformActionStart(Game_Action action)
        {
            base.PerformActionStart(action);
        }

        public override void PerformAction(Game_Action action)
        {
            base.PerformAction(action);
            if (action.IsAttack())
            {
                PerformAttack();
            }
            else if (action.IsGuard())
            {
                RequestMotion("guard");
            }
            else if (action.IsMagicSkill())
            {
                RequestMotion("spell");
            }
            else if (action.IsSkill())
            {
                RequestMotion("skill");
            }
            else if (action.IsItem())
            {
                RequestMotion("item");
            }
        }

        public override void PerformActionEnd()
        {
            base.PerformActionEnd();
        }

        protected virtual void PerformAttack()
        {
            var weapons = Weapons();
            int wtypeId = weapons.ElementAtOrDefault(0)?.WtypeId ?? 0;
            var attackMotion = Rmmz.dataSystem.AttackMotions[wtypeId];
            if (attackMotion != null)
            {
                if (attackMotion.Type == 0)
                {
                    RequestMotion("thrust");
                }
                else if (attackMotion.Type == 1)
                {
                    RequestMotion("swing");
                }
                else if (attackMotion.Type == 2)
                {
                    RequestMotion("missile");
                }
                StartWeaponAnimation(attackMotion.WeaponImageId);
            }
        }

        public override void PerformDamage()
        {
            base.PerformDamage();
            if (IsSpriteVisible())
            {
                RequestMotion("damage");
            }
            else
            {
                Rmmz.gameScreen.StartShake(5, 5, 10);
            }
            Rmmz.SoundManager.PlayActorDamage();
        }

        public override void PerformEvasion()
        {
            base.PerformEvasion();
            RequestMotion("evade");
        }

        public override void PerformMagicEvasion()
        {
            base.PerformMagicEvasion();
            RequestMotion("evade");
        }

        public override void PerformCounter()
        {
            base.PerformCounter();
            PerformAttack();
        }

        public override void PerformCollapse()
        {
            base.PerformCollapse();
            if (Rmmz.gameParty.InBattle())
            {
                Rmmz.SoundManager.PlayActorCollapse();
            }
        }

        public virtual void PerformVictory()
        {
            SetActionState("done");
            if (CanMove())
            {
                RequestMotion("victory");
            }
        }

        public virtual void PerformEscape()
        {
            if (CanMove())
            {
                RequestMotion("escape");
            }
        }

        protected virtual List<Game_Action> MakeActionList()
        {
            var list = new List<Game_Action>();
            var attackAction = Game_Action.Create(this);
            attackAction.SetAttack();
            list.Add(attackAction);
            foreach (var skill in UsableSkills())
            {
                var skillAction = Game_Action.Create(this);
                skillAction.SetSkill(skill.Id);
                list.Add(skillAction);
            }
            return list;
        }

        protected virtual void MakeAutoBattleActions()
        {
            for (int i = 0; i < NumActions(); i++)
            {
                var list = MakeActionList();
                float maxValue = -float.MaxValue;
                foreach (var action in list)
                {
                    float value = action.Evaluate();
                    if (value > maxValue)
                    {
                        maxValue = value;
                        SetAction(i, action);
                    }
                }
            }
            SetActionState("waiting");
        }

        protected virtual void MakeConfusionActions()
        {
            for (int i = 0; i < NumActions(); i++)
            {
                Action(i).SetConfusion();
            }
            SetActionState("waiting");
        }

        public override void MakeActions()
        {
            base.MakeActions();
            if (NumActions() > 0)
            {
                SetActionState("undecided");
            }
            else
            {
                SetActionState("waiting");
            }
            if (IsAutoBattle())
            {
                MakeAutoBattleActions();
            }
            else if (IsConfused())
            {
                MakeConfusionActions();
            }
        }

        public virtual void OnPlayerWalk()
        {
            ClearResult();
            CheckFloorEffect();
            if (Rmmz.gamePlayer.IsNormal())
            {
                TurnEndOnMap();
                foreach (var state in States())
                {
                    UpdateStateSteps(state);
                }
                ShowAddedStates();
                ShowRemovedStates();
            }
        }

        protected virtual void UpdateStateSteps(DataState dataState)
        {
            if (dataState.RemoveByWalking)
            {
                if (_stateSteps.ContainsKey(dataState.Id) && _stateSteps[dataState.Id] > 0)
                {
                    if (--_stateSteps[dataState.Id] == 0)
                    {
                        RemoveState(dataState.Id);
                    }
                }
            }
        }

        protected virtual void ShowAddedStates()
        {
            foreach (var state in Result().AddedStateObjects())
            {
                if (!string.IsNullOrEmpty(state.Message1))
                {
                    Rmmz.gameMessage.Add(state.Message1.RmmzFormat(_name));
                }
            }
        }

        protected virtual void ShowRemovedStates()
        {
            foreach (var state in Result().RemovedStateObjects())
            {
                if (!string.IsNullOrEmpty(state.Message4))
                {
                    Rmmz.gameMessage.Add(state.Message4.RmmzFormat(_name));
                }
            }
        }

        protected virtual int StepsForTurn()
        {
            return 20;
        }

        protected virtual void TurnEndOnMap()
        {
            if (Rmmz.gameParty.Steps() % StepsForTurn() == 0)
            {
                OnTurnEnd();
                if (Result().hpDamage > 0)
                {
                    PerformMapDamage();
                }
            }
        }

        protected virtual void CheckFloorEffect()
        {
            if (Rmmz.gamePlayer.IsOnDamageFloor())
            {
                ExecuteFloorDamage();
            }
        }

        protected virtual void ExecuteFloorDamage()
        {
            int floorDamage = Mathf.FloorToInt(BasicFloorDamage() * Fdr);
            int realDamage = Mathf.Min(floorDamage, MaxFloorDamage());
            GainHp(-realDamage);
            if (realDamage > 0)
            {
                PerformMapDamage();
            }
        }

        protected virtual int BasicFloorDamage()
        {
            return 10;
        }

        protected virtual int MaxFloorDamage()
        {
            return Rmmz.dataSystem.OptFloorDeath ? Hp : Mathf.Max(Hp - 1, 0);
        }

        protected virtual void PerformMapDamage()
        {
            if (!Rmmz.gameParty.InBattle())
            {
                Rmmz.gameScreen.StartFlashForDamage();
            }
        }

        public override void ClearActions()
        {
            base.ClearActions();
            _actionInputIndex = 0;
        }

        public virtual Game_Action InputtingAction()
        {
            return Action(_actionInputIndex);
        }

        public virtual bool SelectNextCommand()
        {
            if (_actionInputIndex < NumActions() - 1)
            {
                _actionInputIndex++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool SelectPreviousCommand()
        {
            if (_actionInputIndex > 0)
            {
                _actionInputIndex--;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual DataSkill LastSkill()
        {
            if (Rmmz.gameParty.InBattle())
            {
                return LastBattleSkill();
            }
            else
            {
                return LastMenuSkill();
            }
        }

        public virtual DataSkill LastMenuSkill()
        {
            return _lastMenuSkill.Object<DataSkill>();
        }

        public virtual void SetLastMenuSkill(DataSkill dataSkill)
        {
            _lastMenuSkill.SetObject(dataSkill);
        }

        public virtual DataSkill LastBattleSkill()
        {
            return _lastBattleSkill.Object<DataSkill>();
        }

        public virtual void SetLastBattleSkill(DataSkill dataSkill)
        {
            _lastBattleSkill.SetObject(dataSkill);
        }

        public virtual string LastCommandSymbol()
        {
            return _lastCommandSymbol;
        }

        public virtual void SetLastCommandSymbol(string symbol)
        {
            _lastCommandSymbol = symbol;
        }

        protected virtual bool TestEscape(UsableItem item)
        {
            return item.Effects.Any(effect => 
                effect != null && effect.Code == Game_Action.EFFECT_SPECIAL);
        }

        public override bool MeetsUsableItemConditions(UsableItem item)
        {
            if (Rmmz.gameParty.InBattle())
            {
                if (!Rmmz.BattleManager.CanEscape() && TestEscape(item))
                {
                    return false;
                }
            }
            return base.MeetsUsableItemConditions(item);
        }

        public virtual void OnEscapeFailure()
        {
            if (Rmmz.BattleManager.IsTpb())
            {
                ApplyTpbPenalty();
            }
            ClearActions();
            RequestMotionRefresh();
        }
    }
}