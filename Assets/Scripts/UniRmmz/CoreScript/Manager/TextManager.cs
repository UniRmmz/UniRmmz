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
        public string Basic(int basicId) => Rmmz.dataSystem.Terms.Basic[basicId];

        public string Param(int paramId) => Rmmz.dataSystem.Terms.Params[paramId];

        public string Command(int commandId) => Rmmz.dataSystem.Terms.Commands[commandId];

        public DataSystem.DataMessages Message() => Rmmz.dataSystem.Terms.Messages;

        public string CurrencyUnit => Rmmz.dataSystem.CurrencyUnit ?? string.Empty;

        public string Level => Basic(0);
        public string LevelA => Basic(1);
        public string Hp => Basic(2);
        public string HpA => Basic(3);
        public string Mp => Basic(4);
        public string MpA => Basic(5);
        public string Tp => Basic(6);
        public string TpA => Basic(7);
        public string Exp => Basic(8);
        public string ExpA => Basic(9);

        public string Fight => Command(0);
        public string Escape => Command(1);
        public string Attack => Command(2);
        public string Guard => Command(3);
        public string Item => Command(4);
        public string Skill => Command(5);
        public string Equip => Command(6);
        public string Status => Command(7);
        public string Formation => Command(8);
        public string Save => Command(9);
        public string GameEnd => Command(10);
        public string Options => Command(11);
        public string Weapon => Command(12);
        public string Armor => Command(13);
        public string KeyItem => Command(14);
        public string Equip2 => Command(15);
        public string Optimize => Command(16);
        public string Clear => Command(17);
        public string NewGame => Command(18);
        public string Continue => Command(19);
        public string ToTitle => Command(21);
        public string Cancel => Command(22);
        public string Buy => Command(24);
        public string Sell => Command(25);
        
        public string AlwaysDash => Message().AlwaysDash;
        public string CommandRemember => Message().CommandRemember;
        public string TouchUI => Message().TouchUI;
        public string BgmVolume => Message().BgmVolume;
        public string BgsVolume => Message().BgsVolume;
        public string MeVolume => Message().MeVolume;
        public string SeVolume => Message().SeVolume;
        public string Possession => Message().Possession;
        public string ExpTotal => Message().ExpTotal;
        public string ExpNext => Message().ExpNext;
        public string SaveMessage => Message().SaveMessage;
        public string LoadMessage => Message().LoadMessage;
        public string File => Message().File;
        public string Autosave => Message().Autosave;
        public string PartyName => Message().PartyName;
        public string Emerge => Message().Emerge;
        public string Preemptive => Message().Preemptive;
        public string Surprise => Message().Surprise;
        public string EscapeStart => Message().EscapeStart;
        public string EscapeFailure => Message().EscapeFailure;
        public string Victory => Message().Victory;
        public string Defeat => Message().Defeat;
        public string ObtainExp => Message().ObtainExp;
        public string ObtainGold => Message().ObtainGold;
        public string ObtainItem => Message().ObtainItem;
        public string LevelUp => Message().LevelUp;
        public string ObtainSkill => Message().ObtainSkill;
        public string UseItem => Message().UseItem;
        public string CriticalToEnemy => Message().CriticalToEnemy;
        public string CriticalToActor => Message().CriticalToActor;
        public string ActorDamage => Message().ActorDamage;
        public string ActorRecovery => Message().ActorRecovery;
        public string ActorGain => Message().ActorGain;
        public string ActorLoss => Message().ActorLoss;
        public string ActorDrain => Message().ActorDrain;
        public string ActorNoDamage => Message().ActorNoDamage;
        public string ActorNoHit => Message().ActorNoHit;
        public string EnemyDamage => Message().EnemyDamage;
        public string EnemyRecovery => Message().EnemyRecovery;
        public string EnemyGain => Message().EnemyGain;
        public string EnemyLoss => Message().EnemyLoss;
        public string EnemyDrain => Message().EnemyDrain;
        public string EnemyNoDamage => Message().EnemyNoDamage;
        public string EnemyNoHit => Message().EnemyNoHit;
        public string Evasion => Message().Evasion;
        public string MagicEvasion => Message().MagicEvasion;
        public string MagicReflection => Message().MagicReflection;
        public string CounterAttack => Message().CounterAttack;
        public string Substitute => Message().Substitute;
        public string BuffAdd => Message().BuffAdd;
        public string DebuffAdd => Message().DebuffAdd;
        public string BuffRemove => Message().BuffRemove;
        public string ActionFailure => Message().ActionFailure;
    }
}