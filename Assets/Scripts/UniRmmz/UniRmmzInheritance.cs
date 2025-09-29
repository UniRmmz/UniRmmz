namespace UniRmmz
{
    #region Game～
    public partial class Game_Event : Game_Character {}
    public partial class Game_Player : Game_Character {}
    public partial class Game_Follower : Game_Character {}
    public partial class Game_Vehicle : Game_Character {}
    public partial class Game_Character : Game_CharacterBase {}
    public partial class Game_Actor : Game_Battler {}
    public partial class Game_Enemy : Game_Battler {}
    public partial class Game_Battler : Game_BattlerBase {}
    public partial class Game_Party : Game_Unit {}
    public partial class Game_Troop : Game_Unit {}
    #endregion
    #region Scene～
    public partial class Scene_Battle : Scene_Message {}
    public partial class Scene_Boot : Scene_Base {}
    public partial class Scene_Equip : Scene_MenuBase {}
    public partial class Scene_File : Scene_MenuBase {}
    public partial class Scene_GameEnd : Scene_MenuBase {}
    public partial class Scene_Gameover : Scene_Base {}
    public partial class Scene_Gameover : Scene_Base {}
    public partial class Scene_Item : Scene_ItemBase<Window_ItemList> {}
    public abstract partial class Scene_ItemBase<T> : Scene_MenuBase where T : Window_Selectable {}
    public partial class Scene_Load : Scene_File {}
    public partial class Scene_Map : Scene_Message {}
    public partial class Scene_Menu : Scene_MenuBase {}
    public partial class Scene_MenuBase : Scene_Base {}
    public partial class Scene_Message : Scene_Base {}
    public partial class Scene_Name : Scene_MenuBase {}
    public partial class Scene_Options : Scene_MenuBase {}
    public partial class Scene_Save : Scene_File {}
    public partial class Scene_Shop : Scene_MenuBase {}
    public partial class Scene_Skill : Scene_ItemBase<Window_SkillList> {}
    public partial class Scene_Status : Scene_MenuBase {}
    public partial class Scene_Title : Scene_Base {}
    #endregion
    #region Sprite～
    public partial class Sprite_Actor : Sprite_Battler {}
    public partial class Sprite_Animation : Sprite_AnimationBase, IEffekseerContainer {}
    public abstract partial class Sprite_AnimationBase : Sprite {}
    public partial class Sprite_AnimationMV : Sprite_AnimationBase {}
    public partial class Sprite_Balloon : Sprite {}
    public partial class Sprite_Battleback : TilingSprite {}
    public partial class Sprite_Battler : Sprite_Clickable {}
    public partial class Sprite_Button : Sprite_Clickable {}
    public partial class Sprite_Character : Sprite {}
    public abstract partial class Sprite_Clickable : Sprite {}
    public partial class Sprite_Damage : Sprite {}
    public partial class Sprite_Destination : Sprite {}
    public partial class Sprite_Enemy : Sprite_Battler {}
    public partial class Sprite_Gauge : Sprite {}
    public partial class Sprite_Name : Sprite {}
    public partial class Sprite_Picture : Sprite_Clickable {}
    public partial class Sprite_StateIcon : Sprite {}
    public partial class Sprite_StateOverlay : Sprite {}
    public partial class Sprite_Timer : Sprite {}
    public partial class Sprite_Weapon : Sprite {}
    public partial class Spriteset_Base : Sprite {}
    public partial class Spriteset_Battle : Spriteset_Base {}
    public partial class Spriteset_Map : Spriteset_Base {}
    #endregion
    #region Window～
    public partial class Window_Base : Window {}
    public partial class Window_ActorCommand : Window_Command {}
    public partial class Window_BattleActor : Window_BattleStatus {}
    public partial class Window_BattleEnemy : Window_Selectable {}
    public partial class Window_BattleItem : Window_ItemList {}
    public partial class Window_BattleLog : Window_Base {}
    public partial class Window_BattleSkill : Window_SkillList {}
    public partial class Window_BattleStatus : Window_StatusBase {}
    public partial class Window_ChoiceList : Window_Command {}
    public abstract partial class Window_Command : Window_Selectable {}
    public partial class Window_EquipCommand : Window_HorzCommand {}
    public partial class Window_EquipItem : Window_ItemList {}
    public partial class Window_EquipSlot : Window_StatusBase {}
    public partial class Window_EquipStatus : Window_StatusBase {}
    public partial class Window_EventItem : Window_ItemList {}
    public partial class Window_GameEnd : Window_Command {}
    public partial class Window_Gold : Window_Selectable {}
    public partial class Window_Help : Window_Base {}
    public partial class Window_HorzCommand : Window_Command {}
    public partial class Window_ItemCategory : Window_HorzCommand {}
    public partial class Window_ItemList : Window_Selectable {}
    public partial class Window_MapName : Window_Base {}
    public partial class Window_MenuActor : Window_MenuStatus {}
    public partial class Window_MenuCommand : Window_Command {}
    public partial class Window_MenuStatus : Window_StatusBase {}
    public partial class Window_Message : Window_Base {}
    public partial class Window_NameBox : Window_Base {}
    public partial class Window_NameEdit : Window_StatusBase {}
    public partial class Window_NameInput : Window_Selectable {}
    public partial class Window_NumberInput : Window_Selectable {}
    public partial class Window_Options : Window_Command {}
    public partial class Window_PartyCommand : Window_Command {}
    public partial class Window_SavefileList : Window_Selectable {}
    public abstract partial class Window_Scrollable : Window_Base {}
    public partial class Window_ScrollText : Window_Base {}
    public abstract partial class Window_Selectable : Window_Scrollable {}
    public partial class Window_ShopBuy : Window_Selectable {}
    public partial class Window_ShopCommand : Window_HorzCommand {}
    public partial class Window_ShopNumber : Window_Selectable {}
    public partial class Window_ShopSell : Window_ItemList {}
    public partial class Window_ShopStatus : Window_StatusBase {}
    public partial class Window_SkillList : Window_Selectable {}
    public partial class Window_SkillStatus : Window_StatusBase {}
    public partial class Window_SkillType : Window_Command {}
    public partial class Window_Status : Window_StatusBase {}
    public abstract partial class Window_StatusBase : Window_Selectable {}
    public partial class Window_StatusEquip : Window_StatusBase {}
    public partial class Window_StatusParams : Window_StatusBase {}
    public partial class Window_TitleCommand : Window_Command {}
        
    #endregion
}