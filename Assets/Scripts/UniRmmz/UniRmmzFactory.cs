
using System;
using System.Linq.Expressions;
using UnityEngine;

namespace UniRmmz
{
    #region Game_～
    
    public partial class Game_Temp
    {
        public static Game_Temp Create() => new Game_Temp();
    }
    
    public partial class Game_System
    {
        public static Game_System Create() => new Game_System();
    }
    
    public partial class Game_Screen
    {
        public static Game_Screen Create() => new Game_Screen();
    }
    
    public partial class Game_Timer
    {
        public static Game_Timer Create() => new Game_Timer();
    }
    
    public partial class Game_Message
    {
        public static Game_Message Create() => new Game_Message();
    }
    
    public partial class Game_Switches
    {
        public static Game_Switches Create() => new Game_Switches();
    }
    
    public partial class Game_Variables
    {
        public static Game_Variables Create() => new Game_Variables();
    }
    
    public partial class Game_SelfSwitches
    {
        public static Game_SelfSwitches Create() => new Game_SelfSwitches();
    }
    
    public partial class Game_Actors
    {
        public static Game_Actors Create() => new Game_Actors();
    }
    
    public partial class Game_Party
    {
        public static Game_Party Create() => new Game_Party();
    }
    
    public partial class Game_Troop
    {
        public static Game_Troop Create() => new Game_Troop();
    }
    
    public partial class Game_Map
    {
        public static Game_Map Create() => new Game_Map();
    }
    
    public partial class Game_Player
    {
        public static Game_Player Create() => new Game_Player();
    }
    
    public partial class Game_Action
    {
        public static Game_Action Create(Game_Battler battler = null, bool forcing = false) 
            => new Game_Action(battler, forcing);
    }
    
    public partial class Game_ActionResult
    {
        public static Game_ActionResult Create() => new Game_ActionResult();
    }
    
    public partial class Game_Actor
    {
        public static Game_Actor Create(int actorId) => new Game_Actor(actorId);
    }
    
    public partial class Game_CommonEvent
    {
        public static Game_CommonEvent Create(int commonEventId) => new Game_CommonEvent(commonEventId);
    }
    
    public partial class Game_Enemy
    {
        public static Game_Enemy Create(int enemyid, float x, float y) => new Game_Enemy(enemyid, x, y);
    }
    
    public partial class Game_Event
    {
        public static Game_Event Create(int mapId, int eventId) => new Game_Event(mapId, eventId);
    }
    
    public partial class Game_Follower
    {
        public static Game_Follower Create(int memberIndex) => new Game_Follower(memberIndex);
    }
    
    public partial class Game_Followers
    {
        public static Game_Followers Create() => new Game_Followers();
    }
    
    public partial class Game_Item
    {
        public static Game_Item Create(DataCommonItem item = null) => new Game_Item(item);
    }
    
    public partial class Game_Picture
    {
        public static Game_Picture Create() => new Game_Picture();
    }
    
    public partial class Game_Interpreter
    {
        public static Game_Interpreter Create(int depth = 0) => new Game_Interpreter(depth);
    }
    
    public partial class Game_Vehicle
    {
        public static Game_Vehicle Create(VehicleTypes type) => new Game_Vehicle(type);
    }
 
    #endregion
    #region Scene_～

    public partial class Scene_Boot
    {
        private static readonly Type _instanceType = typeof(Scene_Boot);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
        public static void Run() => Rmmz.SceneManager._Run(_instanceType);
    }

    public partial class Scene_Battle
    {
        private static readonly Type _instanceType = typeof(Scene_Battle);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Equip
    {
        private static readonly Type _instanceType = typeof(Scene_Equip);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_GameEnd
    {
        private static readonly Type _instanceType = typeof(Scene_GameEnd);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Gameover
    {
        private static readonly Type _instanceType = typeof(Scene_Gameover);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Item
    {
        private static readonly Type _instanceType = typeof(Scene_Item);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Load
    {
        private static readonly Type _instanceType = typeof(Scene_Load);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Map
    {
        private static readonly Type _instanceType = typeof(Scene_Map);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Menu
    {
        private static readonly Type _instanceType = typeof(Scene_Menu);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Name
    {
        private static readonly Type _instanceType = typeof(Scene_Name);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Optiions
    {
        private static readonly Type _instanceType = typeof(Scene_Options);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Save
    {
        private static readonly Type _instanceType = typeof(Scene_Save);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Shop
    {
        private static readonly Type _instanceType = typeof(Scene_Shop);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Skill
    {
        private static readonly Type _instanceType = typeof(Scene_Skill);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Status
    {
        private static readonly Type _instanceType = typeof(Scene_Status);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_Title
    {
        private static readonly Type _instanceType = typeof(Scene_Title);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    public partial class Scene_ItemBook
    {
        private static readonly Type _instanceType = typeof(Scene_ItemBook);
        public static void Goto() => Rmmz.SceneManager._Goto(_instanceType);
        public static void Push() => Rmmz.SceneManager._Push(_instanceType);
    }
    
    #endregion
    #region Sprite_～

    public partial class Sprite
    {
        public static Sprite Create(string name = "") => Sprite._Create<Sprite>(name);
    }
    
    public partial class ScreenSprite
    {
        public static ScreenSprite Create(string name = "") => RmmzContainer._Create<ScreenSprite>(name);
    }
    
    public partial class TilingSprite
    {
        public static TilingSprite Create(string name = "") => RmmzContainer._Create<TilingSprite>(name);
    }
    
    public partial class WindowLayer
    {
        public static WindowLayer Create(string name = "") => RmmzContainer._Create<WindowLayer>(name);
    }
    
    public partial class Weather
    {
        public static Weather Create(string name = "") => RmmzContainer._Create<Weather>(name);
    }
    
    public partial class Sprite_Actor
    {
        new public static Sprite_Actor Create(string name = "") => Sprite._Create<Sprite_Actor>(name);
    }
    
    public partial class Sprite_Animation
    {
        new public static Sprite_Animation Create(string name = "") => Sprite._Create<Sprite_Animation>(name);
    }
    
    public partial class Sprite_AnimationMV
    {
        new public static Sprite_AnimationMV Create(string name = "") => Sprite._Create<Sprite_AnimationMV>(name);
    }
    
    public partial class Sprite_Balloon
    {
        new public static Sprite_Balloon Create(string name = "") => Sprite._Create<Sprite_Balloon>(name);
    }
    
    public partial class Sprite_Battleback
    {
        new public static Sprite_Battleback Create(string name = "") => TilingSprite._Create<Sprite_Battleback>(name);
    }
    
    public partial class Sprite_Button
    {
        new public static Sprite_Button Create(string name = "") => Sprite._Create<Sprite_Button>(name);
    }
    
    public partial class Sprite_Character
    {
        new public static Sprite_Character Create(string name = "") => Sprite._Create<Sprite_Character>(name);
    }
    
    public partial class Sprite_Damage
    {
        new public static Sprite_Damage Create(string name = "") => Sprite._Create<Sprite_Damage>(name);
    }
    
    public partial class Sprite_Destination
    {
        new public static Sprite_Destination Create(string name = "") => Sprite._Create<Sprite_Destination>(name);
    }
    
    public partial class Sprite_Enemy
    {
        new public static Sprite_Enemy Create(string name = "") => Sprite._Create<Sprite_Enemy>(name);
    }
    
    public partial class Sprite_Gauge
    {
        new public static Sprite_Gauge Create(string name = "") => Sprite._Create<Sprite_Gauge>(name);
    }
    
    public partial class Sprite_Name
    {
        new public static Sprite_Name Create(string name = "") => Sprite._Create<Sprite_Name>(name);
    }
    
    public partial class Sprite_Picture
    {
        new public static Sprite_Picture Create(string name = "") => Sprite._Create<Sprite_Picture>(name);
    }
    
    public partial class Sprite_StateIcon
    {
        new public static Sprite_StateIcon Create(string name = "") => Sprite._Create<Sprite_StateIcon>(name);
    }
    
    public partial class Sprite_StateOverlay
    {
        new public static Sprite_StateOverlay Create(string name = "") => Sprite._Create<Sprite_StateOverlay>(name);
    }
    
    public partial class Sprite_Timer
    {
        new public static Sprite_Timer Create(string name = "") => Sprite._Create<Sprite_Timer>(name);
    }
    
    public partial class Sprite_Weapon
    {
        new public static Sprite_Weapon Create(string name = "") => Sprite._Create<Sprite_Weapon>(name);
    }
    
    public partial class Spriteset_Battle
    {
        new public static Spriteset_Battle Create(string name = "") => Sprite._Create<Spriteset_Battle>(name);
    }
    
    public partial class Spriteset_Map
    {
        new public static Spriteset_Map Create(string name = "") => Sprite._Create<Spriteset_Map>(name);
    }

    #endregion
    #region Window_～
    
    public partial class Window_Base
    {
        public static Window_Base Create(Rect rect, string name = "") => Window._Create<Window_Base>(rect, name);
    }
    
    public partial class Window_ActorCommand
    {
        new public static Window_ActorCommand Create(Rect rect, string name = "") => Window._Create<Window_ActorCommand>(rect, name);
    }
    
    public partial class Window_BattleActor
    {
        new public static Window_BattleActor Create(Rect rect, string name = "") => Window._Create<Window_BattleActor>(rect, name);
    }
    
    public partial class Window_BattleEnemy
    {
        new public static Window_BattleEnemy Create(Rect rect, string name = "") => Window._Create<Window_BattleEnemy>(rect, name);
    }
    
    public partial class Window_BattleItem
    {
        new public static Window_BattleItem Create(Rect rect, string name = "") => Window._Create<Window_BattleItem>(rect, name);
    }
    
    public partial class Window_BattleLog
    {
        new public static Window_BattleLog Create(Rect rect, string name = "") => Window._Create<Window_BattleLog>(rect, name);
    }
    
    public partial class Window_BattleSkill
    {
        new public static Window_BattleSkill Create(Rect rect, string name = "") => Window._Create<Window_BattleSkill>(rect, name);
    }
    
    public partial class Window_BattleStatus
    {
        new public static Window_BattleStatus Create(Rect rect, string name = "") => Window._Create<Window_BattleStatus>(rect, name);
    }
    
    public partial class Window_ChoiceList
    {
        new public static Window_ChoiceList Create(Rect rect, string name = "") => Window._Create<Window_ChoiceList>(rect, name);
    }
    
    public partial class Window_EquipCommand
    {
        new public static Window_EquipCommand Create(Rect rect, string name = "") => Window._Create<Window_EquipCommand>(rect, name);
    }
    
    public partial class Window_EquipItem
    {
        new public static Window_EquipItem Create(Rect rect, string name = "") => Window._Create<Window_EquipItem>(rect, name);
    }
    
    public partial class Window_EquipSlot
    {
        new public static Window_EquipSlot Create(Rect rect, string name = "") => Window._Create<Window_EquipSlot>(rect, name);
    }
    
    public partial class Window_EquipStatus
    {
        new public static Window_EquipStatus Create(Rect rect, string name = "") => Window._Create<Window_EquipStatus>(rect, name);
    }
    
    public partial class Window_EventItem
    {
        new public static Window_EventItem Create(Rect rect, string name = "") => Window._Create<Window_EventItem>(rect, name);
    }
    
    public partial class Window_GameEnd
    {
        new public static Window_GameEnd Create(Rect rect, string name = "") => Window._Create<Window_GameEnd>(rect, name);
    }
    
    public partial class Window_Gold
    {
        new public static Window_Gold Create(Rect rect, string name = "") => Window._Create<Window_Gold>(rect, name);
    }
    
    public partial class Window_Help
    {
        new public static Window_Help Create(Rect rect, string name = "") => Window._Create<Window_Help>(rect, name);
    }
    
    public partial class Window_HorzCommand
    {
        new public static Window_HorzCommand Create(Rect rect, string name = "") => Window._Create<Window_HorzCommand>(rect, name);
    }
    
    public partial class Window_ItemCategory
    {
        new public static Window_ItemCategory Create(Rect rect, string name = "") => Window._Create<Window_ItemCategory>(rect, name);
    }
    
    public partial class Window_ItemList
    {
        new public static Window_ItemList Create(Rect rect, string name = "") => Window._Create<Window_ItemList>(rect, name);
    }
    
    public partial class Window_MapName
    {
        new public static Window_MapName Create(Rect rect, string name = "") => Window._Create<Window_MapName>(rect, name);
    }
    
    public partial class Window_MenuActor
    {
        new public static Window_MenuActor Create(Rect rect, string name = "") => Window._Create<Window_MenuActor>(rect, name);
    }
    
    public partial class Window_MenuCommand
    {
        new public static Window_MenuCommand Create(Rect rect, string name = "") => Window._Create<Window_MenuCommand>(rect, name);
    }
    
    public partial class Window_MenuStatus
    {
        new public static Window_MenuStatus Create(Rect rect, string name = "") => Window._Create<Window_MenuStatus>(rect, name);
    }
    
    public partial class Window_Message
    {
        new public static Window_Message Create(Rect rect, string name = "") => Window._Create<Window_Message>(rect, name);
    }
    
    public partial class Window_NameBox
    {
        new public static Window_NameBox Create(Rect rect, string name = "") => Window._Create<Window_NameBox>(rect, name);
    }
    
    public partial class Window_NameEdit
    {
        new public static Window_NameEdit Create(Rect rect, string name = "") => Window._Create<Window_NameEdit>(rect, name);
    }
    
    public partial class Window_NameInput
    {
        new public static Window_NameInput Create(Rect rect, string name = "") => Window._Create<Window_NameInput>(rect, name);
    }
    
    public partial class Window_NumberInput
    {
        new public static Window_NumberInput Create(Rect rect, string name = "") => Window._Create<Window_NumberInput>(rect, name);
    }
    
    public partial class Window_Options
    {
        new public static Window_Options Create(Rect rect, string name = "") => Window._Create<Window_Options>(rect, name);
    }
    
    public partial class Window_PartyCommand
    {
        new public static Window_PartyCommand Create(Rect rect, string name = "") => Window._Create<Window_PartyCommand>(rect, name);
    }
    
    public partial class Window_SavefileList
    {
        new public static Window_SavefileList Create(Rect rect, string name = "") => Window._Create<Window_SavefileList>(rect, name);
    }
    
    public partial class Window_ScrollText
    {
        new public static Window_ScrollText Create(Rect rect, string name = "") => Window._Create<Window_ScrollText>(rect, name);
    }
    
    public partial class Window_ShopBuy
    {
        new public static Window_ShopBuy Create(Rect rect, string name = "") => Window._Create<Window_ShopBuy>(rect, name);
    }
    
    public partial class Window_ShopCommand
    {
        new public static Window_ShopCommand Create(Rect rect, string name = "") => Window._Create<Window_ShopCommand>(rect, name);
    }
    
    public partial class Window_ShopNumber
    {
        new public static Window_ShopNumber Create(Rect rect, string name = "") => Window._Create<Window_ShopNumber>(rect, name);
    }
    
    public partial class Window_ShopSell
    {
        new public static Window_ShopSell Create(Rect rect, string name = "") => Window._Create<Window_ShopSell>(rect, name);
    }

    public partial class Window_ShopStatus
    {
        new public static Window_ShopStatus Create(Rect rect, string name = "") => Window._Create<Window_ShopStatus>(rect, name);
    }

    public partial class Window_SkillList
    {
        new public static Window_SkillList Create(Rect rect, string name = "") => Window._Create<Window_SkillList>(rect, name);
    }
    
    public partial class Window_SkillType
    {
        new public static Window_SkillType Create(Rect rect, string name = "") => Window._Create<Window_SkillType>(rect, name);
    }
    
    public partial class Window_SkillStatus
    {
        new public static Window_SkillStatus Create(Rect rect, string name = "") => Window._Create<Window_SkillStatus>(rect, name);
    }
    
    public partial class Window_Status
    {
        new public static Window_Status Create(Rect rect, string name = "") => Window._Create<Window_Status>(rect, name);
    }
    
    public partial class Window_StatusEquip
    {
        new public static Window_StatusEquip Create(Rect rect, string name = "") => Window._Create<Window_StatusEquip>(rect, name);
    }
    
    public partial class Window_StatusParams
    {
        new public static Window_StatusParams Create(Rect rect, string name = "") => Window._Create<Window_StatusParams>(rect, name);
    }

    public partial class Window_TitleCommand
    {
        new public static Window_TitleCommand Create(Rect rect, string name = "") => Window._Create<Window_TitleCommand>(rect, name);
    }
    
    #endregion
    #region StaticClass

    public partial class AudioManager
    {
        public static AudioManager Create() => Rmmz.AudioManager = new AudioManager();
    }
    
    public partial class BattleManager
    {
        public static BattleManager Create() => Rmmz.BattleManager = new BattleManager();
    }
    
    public partial class ColorManager
    {
        public static ColorManager Create() => Rmmz.ColorManager = new ColorManager();
    }
    
    public partial class ConfigManager
    {
        public static ConfigManager Create() => Rmmz.ConfigManager = new ConfigManager();
    }
    
    public partial class DataManager
    {
        public static DataManager Create() => Rmmz.DataManager = new DataManager();
    }
    
    public partial class EffectManager
    {
        public static EffectManager Create() => Rmmz.EffectManager = new EffectManager();
    }
    
    public partial class FontManager
    {
        public static FontManager Create() => Rmmz.FontManager = new FontManager();
    }
    
    public partial class ImageManager
    {
        public static ImageManager Create() => Rmmz.ImageManager = new ImageManager();
    }
    
    public partial class SceneManager
    {
        public static SceneManager Create() => Rmmz.SceneManager = new SceneManager();
    }
    
    public partial class SoundManager
    {
        public static SoundManager Create() => Rmmz.SoundManager = new SoundManager();
    }
    
    public partial class StorageManager
    {
        public static StorageManager Create() => Rmmz.StorageManager = new StorageManager();
    }
    
    public partial class TextManager
    {
        public static TextManager Create() => Rmmz.TextManager = new TextManager();
    }
    
    #endregion
}