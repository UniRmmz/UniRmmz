using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The class that handles terms and messages.
    /// </summary>
    public partial class TextManager
    {
        public virtual string Basic(int basicId) => Rmmz.dataSystem.Terms.Basic[basicId];

        public virtual string Param(int paramId) => Rmmz.dataSystem.Terms.Params[paramId];

        public virtual string Command(int commandId) => Rmmz.dataSystem.Terms.Commands[commandId];

        public virtual DataSystem.DataMessages Message() => Rmmz.dataSystem.Terms.Messages;

        public virtual string CurrencyUnit => Rmmz.dataSystem.CurrencyUnit ?? string.Empty;

        public virtual string Level => Basic(0);
        public virtual string LevelA => Basic(1);
        public virtual string Hp => Basic(2);
        public virtual string HpA => Basic(3);
        public virtual string Mp => Basic(4);
        public virtual string MpA => Basic(5);
        public virtual string Tp => Basic(6);
        public virtual string TpA => Basic(7);
        public virtual string Exp => Basic(8);
        public virtual string ExpA => Basic(9);

        public virtual string Fight => Command(0);
        public virtual string Escape => Command(1);
        public virtual string Attack => Command(2);
        public virtual string Guard => Command(3);
        public virtual string Item => Command(4);
        public virtual string Skill => Command(5);
        public virtual string Equip => Command(6);
        public virtual string Status => Command(7);
        public virtual string Formation => Command(8);
        public virtual string Save => Command(9);
        public virtual string GameEnd => Command(10);
        public virtual string Options => Command(11);
        public virtual string Weapon => Command(12);
        public virtual string Armor => Command(13);
        public virtual string KeyItem => Command(14);
        public virtual string Equip2 => Command(15);
        public virtual string Optimize => Command(16);
        public virtual string Clear => Command(17);
        public virtual string NewGame => Command(18);
        public virtual string Continue => Command(19);
        public virtual string ToTitle => Command(21);
        public virtual string Cancel => Command(22);
        public virtual string Buy => Command(24);
        public virtual string Sell => Command(25);
        
        public virtual string AlwaysDash => Message().AlwaysDash;
        public virtual string CommandRemember => Message().CommandRemember;
        public virtual string TouchUI => Message().TouchUI;
        public virtual string BgmVolume => Message().BgmVolume;
        public virtual string BgsVolume => Message().BgsVolume;
        public virtual string MeVolume => Message().MeVolume;
        public virtual string SeVolume => Message().SeVolume;
        public virtual string Possession => Message().Possession;
        public virtual string ExpTotal => Message().ExpTotal;
        public virtual string ExpNext => Message().ExpNext;
        public virtual string SaveMessage => Message().SaveMessage;
        public virtual string LoadMessage => Message().LoadMessage;
        public virtual string File => Message().File;
        public virtual string Autosave => Message().Autosave;
        public virtual string PartyName => Message().PartyName;
        public virtual string Emerge => Message().Emerge;
        public virtual string Preemptive => Message().Preemptive;
        public virtual string Surprise => Message().Surprise;
        public virtual string EscapeStart => Message().EscapeStart;
        public virtual string EscapeFailure => Message().EscapeFailure;
        public virtual string Victory => Message().Victory;
        public virtual string Defeat => Message().Defeat;
        public virtual string ObtainExp => Message().ObtainExp;
        public virtual string ObtainGold => Message().ObtainGold;
        public virtual string ObtainItem => Message().ObtainItem;
        public virtual string LevelUp => Message().LevelUp;
        public virtual string ObtainSkill => Message().ObtainSkill;
        public virtual string UseItem => Message().UseItem;
        public virtual string CriticalToEnemy => Message().CriticalToEnemy;
        public virtual string CriticalToActor => Message().CriticalToActor;
        public virtual string ActorDamage => Message().ActorDamage;
        public virtual string ActorRecovery => Message().ActorRecovery;
        public virtual string ActorGain => Message().ActorGain;
        public virtual string ActorLoss => Message().ActorLoss;
        public virtual string ActorDrain => Message().ActorDrain;
        public virtual string ActorNoDamage => Message().ActorNoDamage;
        public virtual string ActorNoHit => Message().ActorNoHit;
        public virtual string EnemyDamage => Message().EnemyDamage;
        public virtual string EnemyRecovery => Message().EnemyRecovery;
        public virtual string EnemyGain => Message().EnemyGain;
        public virtual string EnemyLoss => Message().EnemyLoss;
        public virtual string EnemyDrain => Message().EnemyDrain;
        public virtual string EnemyNoDamage => Message().EnemyNoDamage;
        public virtual string EnemyNoHit => Message().EnemyNoHit;
        public virtual string Evasion => Message().Evasion;
        public virtual string MagicEvasion => Message().MagicEvasion;
        public virtual string MagicReflection => Message().MagicReflection;
        public virtual string CounterAttack => Message().CounterAttack;
        public virtual string Substitute => Message().Substitute;
        public virtual string BuffAdd => Message().BuffAdd;
        public virtual string DebuffAdd => Message().DebuffAdd;
        public virtual string BuffRemove => Message().BuffRemove;
        public virtual string ActionFailure => Message().ActionFailure;
    }
}