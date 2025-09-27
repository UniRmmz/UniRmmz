using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
   /// <summary>
   /// The scene class of the battle screen.
   /// </summary>
   public partial class Scene_Battle : Scene_Message
   {
       protected Spriteset_Battle _spriteset;
       protected Window_BattleLog _logWindow;
       protected Window_BattleStatus _statusWindow;
       protected Window_PartyCommand _partyCommandWindow;
       protected Window_ActorCommand _actorCommandWindow;
       protected Window_Help _helpWindow;
       protected Window_BattleSkill _skillWindow;
       protected Window_BattleItem _itemWindow;
       protected Window_BattleActor _actorWindow;
       protected Window_BattleEnemy _enemyWindow;
       protected Sprite_Button _cancelButton;

       public override void Initialize()
       {
           base.Initialize();
       }

       public override void Create()
       {
           base.Create();
           CreateDisplayObjects();
       }

       public override void StartScene()
       {
           base.StartScene();
           Rmmz.BattleManager.PlayBattleBgm();
           Rmmz.BattleManager.StartBattle();
           _statusWindow.Refresh();
           StartFadeIn(FadeSpeed(), false);
       }

       public override void UpdateRmmz()
       {
           bool active = IsSceneActive();
           Rmmz.gameTimer.Update(active);
           Rmmz.gameScreen.Update();
           UpdateVisibility();
           if (active && !IsBusy())
           {
               UpdateBattleProcess();
           }
           base.UpdateRmmz();
       }

       protected virtual void UpdateVisibility()
       {
           UpdateLogWindowVisibility();
           UpdateStatusWindowVisibility();
           UpdateInputWindowVisibility();
           UpdateCancelButton();
       }

       protected virtual void UpdateBattleProcess()
       {
           Rmmz.BattleManager.Update(IsTimeActive());
       }

       protected virtual bool IsTimeActive()
       {
           if (Rmmz.BattleManager.IsActiveTpb())
           {
               return !_skillWindow.Active && !_itemWindow.Active;
           }
           else
           {
               return !IsAnyInputWindowActive();
           }
       }

       protected virtual bool IsAnyInputWindowActive()
       {
           return (
               _partyCommandWindow.Active ||
               _actorCommandWindow.Active ||
               _skillWindow.Active ||
               _itemWindow.Active ||
               _actorWindow.Active ||
               _enemyWindow.Active
           );
       }

       protected virtual void ChangeInputWindow()
       {
           HideSubInputWindows();
           if (Rmmz.BattleManager.IsInputting())
           {
               if (Rmmz.BattleManager.Actor() != null)
               {
                   StartActorCommandSelection();
               }
               else
               {
                   StartPartyCommandSelection();
               }
           }
           else
           {
               EndCommandSelection();
           }
       }

       public override void StopScene()
       {
           base.StopScene();
           if (NeedsSlowFadeOut())
           {
               StartFadeOut(SlowFadeSpeed(), false);
           }
           else
           {
               StartFadeOut(FadeSpeed(), false);
           }
           _statusWindow.Close();
           _partyCommandWindow.Close();
           _actorCommandWindow.Close();
       }

       public override void Terminate()
       {
           base.Terminate();
           Rmmz.gameParty.OnBattleEnd();
           Rmmz.gameTroop.OnBattleEnd();
           Rmmz.AudioManager.StopMe();
           if (ShouldAutosave())
           {
               RequestAutosave();
           }
       }

       protected virtual bool ShouldAutosave()
       {
           return Rmmz.SceneManager.IsNextScene(typeof (Scene_Map));
       }

       protected virtual bool NeedsSlowFadeOut()
       {
           return (
               Rmmz.SceneManager.IsNextScene(typeof (Scene_Title)) ||
               Rmmz.SceneManager.IsNextScene(typeof (Scene_Gameover))
           );
       }

       protected virtual void UpdateLogWindowVisibility()
       {
           _logWindow.Visible = !_helpWindow.Visible;
       }

       protected virtual void UpdateStatusWindowVisibility()
       {
           if (Rmmz.gameMessage.IsBusy())
           {
               _statusWindow.Close();
           }
           else if (ShouldOpenStatusWindow())
           {
               _statusWindow.Open();
           }
           UpdateStatusWindowPosition();
       }

       protected virtual bool ShouldOpenStatusWindow()
       {
           return (
               IsSceneActive() &&
               !IsMessageWindowClosing() &&
               !Rmmz.BattleManager.IsBattleEnd()
           );
       }

       protected virtual void UpdateStatusWindowPosition()
       {
           var statusWindow = _statusWindow;
           float targetX = StatusWindowX();
           if (statusWindow.X < targetX)
           {
               statusWindow.X = Mathf.Min(statusWindow.X + 16, targetX);
           }
           if (statusWindow.X > targetX)
           {
               statusWindow.X = Mathf.Max(statusWindow.X - 16, targetX);
           }
       }

       protected virtual float StatusWindowX()
       {
           if (IsAnyInputWindowActive())
           {
               return StatusWindowRect().x;
           }
           else
           {
               return _partyCommandWindow.Width / 2;
           }
       }

       protected virtual void UpdateInputWindowVisibility()
       {
           if (Rmmz.gameMessage.IsBusy())
           {
               CloseCommandWindows();
               HideSubInputWindows();
           }
           else if (NeedsInputWindowChange())
           {
               ChangeInputWindow();
           }
       }

       protected virtual bool NeedsInputWindowChange()
       {
           bool windowActive = IsAnyInputWindowActive();
           bool inputting = Rmmz.BattleManager.IsInputting();
           if (windowActive && inputting)
           {
               return _actorCommandWindow.Actor() != Rmmz.BattleManager.Actor();
           }
           return windowActive != inputting;
       }

       protected virtual void UpdateCancelButton()
       {
           if (_cancelButton != null)
           {
               _cancelButton.Visible =
                   IsAnyInputWindowActive() && !_partyCommandWindow.Active;
           }
       }

       protected virtual void CreateDisplayObjects()
       {
           CreateSpriteset();
           CreateWindowLayer();
           CreateAllWindows();
           CreateButtons();
           Rmmz.BattleManager.SetLogWindow(_logWindow);
           Rmmz.BattleManager.SetSpriteset(_spriteset);
           _logWindow.SetSpriteset(_spriteset);
       }

       protected virtual void CreateSpriteset()
       {
           _spriteset = Spriteset_Battle.Create("spriteset");
           this.AddChild(_spriteset);
       }

       protected override void CreateAllWindows()
       {
           CreateLogWindow();
           CreateStatusWindow();
           CreatePartyCommandWindow();
           CreateActorCommandWindow();
           CreateHelpWindow();
           CreateSkillWindow();
           CreateItemWindow();
           CreateActorWindow();
           CreateEnemyWindow();
           base.CreateAllWindows();
       }

       protected virtual void CreateLogWindow()
       {
           var rect = LogWindowRect();
           _logWindow = Window_BattleLog.Create(rect, "logWindow");
           AddWindow(_logWindow);
       }

       protected virtual Rect LogWindowRect()
       {
           float wx = 0;
           float wy = 0;
           float ww = Graphics.BoxWidth;
           float wh = CalcWindowHeight(10, false);
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual void CreateStatusWindow()
       {
           var rect = StatusWindowRect();
           _statusWindow = Window_BattleStatus.Create(rect, "statusWindow");
           AddWindow(_statusWindow);
       }

       protected virtual Rect StatusWindowRect()
       {
           int extra = 10;
           float ww = Graphics.BoxWidth - 192;
           float wh = WindowAreaHeight() + extra;
           float wx = IsRightInputMode() ? 0 : Graphics.BoxWidth - ww;
           float wy = Graphics.BoxHeight - wh + extra - 4;
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual void CreatePartyCommandWindow()
       {
           var rect = PartyCommandWindowRect();
           _partyCommandWindow = Window_PartyCommand.Create(rect, "partyCommandWindow");
           _partyCommandWindow.SetHandler("fight", () => CommandFight());
           _partyCommandWindow.SetHandler("escape", () => CommandEscape());
           _partyCommandWindow.Deselect();
           AddWindow(_partyCommandWindow);
       }

       protected virtual Rect PartyCommandWindowRect()
       {
           float ww = 192;
           float wh = WindowAreaHeight();
           float wx = IsRightInputMode() ? Graphics.BoxWidth - ww : 0;
           float wy = Graphics.BoxHeight - wh;
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual void CreateActorCommandWindow()
       {
           var rect = ActorCommandWindowRect();
           _actorCommandWindow = Window_ActorCommand.Create(rect, "actorCommandWindow");
           _actorCommandWindow.Y = Graphics.BoxHeight - _actorCommandWindow.Height;
           _actorCommandWindow.SetHandler("attack", () => CommandAttack());
           _actorCommandWindow.SetHandler("skill", () => CommandSkill());
           _actorCommandWindow.SetHandler("guard", () => CommandGuard());
           _actorCommandWindow.SetHandler("item", () => CommandItem());
           _actorCommandWindow.SetHandler("cancel", () => CommandCancel());
           AddWindow(_actorCommandWindow);
       }

       protected virtual Rect ActorCommandWindowRect()
       {
           float ww = 192;
           float wh = WindowAreaHeight();
           float wx = IsRightInputMode() ? Graphics.BoxWidth - ww : 0;
           float wy = Graphics.BoxHeight - wh;
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual void CreateHelpWindow()
       {
           var rect = HelpWindowRect();
           _helpWindow = Window_Help.Create(rect, "helpWindow");
           _helpWindow.Hide();
           AddWindow(_helpWindow);
       }

       protected virtual Rect HelpWindowRect()
       {
           float wx = 0;
           float wy = HelpAreaTop();
           float ww = Graphics.BoxWidth;
           float wh = HelpAreaHeight();
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual void CreateSkillWindow()
       {
           var rect = SkillWindowRect();
           _skillWindow = Window_BattleSkill.Create(rect, "skillWindow");
           _skillWindow.SetHelpWindow(_helpWindow);
           _skillWindow.SetHandler("ok", () => OnSkillOk());
           _skillWindow.SetHandler("cancel", () => OnSkillCancel());
           AddWindow(_skillWindow);
       }

       protected virtual Rect SkillWindowRect()
       {
           float ww = Graphics.BoxWidth;
           float wh = WindowAreaHeight();
           float wx = 0;
           float wy = Graphics.BoxHeight - wh;
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual void CreateItemWindow()
       {
           var rect = ItemWindowRect();
           _itemWindow = Window_BattleItem.Create(rect, "itemWindow");
           _itemWindow.SetHelpWindow(_helpWindow);
           _itemWindow.SetHandler("ok", () => OnItemOk());
           _itemWindow.SetHandler("cancel", () => OnItemCancel());
           AddWindow(_itemWindow);
       }

       protected virtual Rect ItemWindowRect()
       {
           return SkillWindowRect();
       }

       protected virtual void CreateActorWindow()
       {
           var rect = ActorWindowRect();
           _actorWindow = Window_BattleActor.Create(rect, "actorWindow");
           _actorWindow.SetHandler("ok", () => OnActorOk());
           _actorWindow.SetHandler("cancel", () => OnActorCancel());
           AddWindow(_actorWindow);
       }

       protected virtual Rect ActorWindowRect()
       {
           return StatusWindowRect();
       }

       protected virtual void CreateEnemyWindow()
       {
           var rect = EnemyWindowRect();
           _enemyWindow = Window_BattleEnemy.Create(rect, "enemyWindow");
           _enemyWindow.SetHandler("ok", () => OnEnemyOk());
           _enemyWindow.SetHandler("cancel", () => OnEnemyCancel());
           AddWindow(_enemyWindow);
       }

       protected virtual Rect EnemyWindowRect()
       {
           float wx = _statusWindow.X;
           float ww = _statusWindow.Width;
           float wh = WindowAreaHeight();
           float wy = Graphics.BoxHeight - wh;
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual int HelpAreaTop()
       {
           return 0;
       }

       protected virtual int HelpAreaBottom()
       {
           return HelpAreaTop() + HelpAreaHeight();
       }

       protected virtual int HelpAreaHeight()
       {
           return CalcWindowHeight(2, false);
       }

       public override int ButtonAreaTop()
       {
           return HelpAreaBottom();
       }

       protected virtual float WindowAreaHeight()
       {
           return CalcWindowHeight(4, true);
       }

       protected virtual void CreateButtons()
       {
           if (Rmmz.ConfigManager.TouchUI)
           {
               CreateCancelButton();
           }
       }

       protected virtual void CreateCancelButton()
       {
           _cancelButton = Sprite_Button.Create("cancelButton");
           _cancelButton.Initialize(Input.ButtonTypes.Cancel);
           _cancelButton.X = Graphics.BoxWidth - _cancelButton.Width - 4;
           _cancelButton.Y = ButtonY();
           AddWindow(_cancelButton);
       }

       protected virtual void CloseCommandWindows()
       {
           _partyCommandWindow.Deactivate();
           _actorCommandWindow.Deactivate();
           _partyCommandWindow.Close();
           _actorCommandWindow.Close();
       }

       protected virtual void HideSubInputWindows()
       {
           _actorWindow.Deactivate();
           _enemyWindow.Deactivate();
           _skillWindow.Deactivate();
           _itemWindow.Deactivate();
           _actorWindow.Hide();
           _enemyWindow.Hide();
           _skillWindow.Hide();
           _itemWindow.Hide();
       }

       protected virtual void StartPartyCommandSelection()
       {
           _statusWindow.Deselect();
           _statusWindow.Show();
           _statusWindow.Open();
           _actorCommandWindow.Setup(null);
           _actorCommandWindow.Close();
           _partyCommandWindow.Setup();
       }

       protected virtual void CommandFight()
       {
           SelectNextCommand();
       }

       protected virtual void CommandEscape()
       {
           Rmmz.BattleManager.ProcessEscape();
           ChangeInputWindow();
       }

       protected virtual void StartActorCommandSelection()
       {
           _statusWindow.Show();
           _statusWindow.SelectActor(Rmmz.BattleManager.Actor());
           _partyCommandWindow.Close();
           _actorCommandWindow.Show();
           _actorCommandWindow.Setup(Rmmz.BattleManager.Actor());
       }

       protected virtual void CommandAttack()
       {
           var action = Rmmz.BattleManager.InputtingAction();
           action.SetAttack();
           OnSelectAction();
       }

       protected virtual void CommandSkill()
       {
           _skillWindow.SetActor(Rmmz.BattleManager.Actor());
           _skillWindow.SetStypeId((int)_actorCommandWindow.CurrentExt());
           _skillWindow.Refresh();
           _skillWindow.Show();
           _skillWindow.Activate();
           _statusWindow.Hide();
           _actorCommandWindow.Hide();
       }

       protected virtual void CommandGuard()
       {
           var action = Rmmz.BattleManager.InputtingAction();
           action.SetGuard();
           OnSelectAction();
       }

       protected virtual void CommandItem()
       {
           _itemWindow.Refresh();
           _itemWindow.Show();
           _itemWindow.Activate();
           _statusWindow.Hide();
           _actorCommandWindow.Hide();
       }

       protected virtual void CommandCancel()
       {
           SelectPreviousCommand();
       }

       protected virtual void SelectNextCommand()
       {
           Rmmz.BattleManager.SelectNextCommand();
           ChangeInputWindow();
       }

       protected virtual void SelectPreviousCommand()
       {
           Rmmz.BattleManager.SelectPreviousCommand();
           ChangeInputWindow();
       }

       protected virtual void StartActorSelection()
       {
           _actorWindow.Refresh();
           _actorWindow.Show();
           _actorWindow.Activate();
       }

       protected virtual void OnActorOk()
       {
           var action = Rmmz.BattleManager.InputtingAction();
           action.SetTarget(_actorWindow.Index());
           HideSubInputWindows();
           SelectNextCommand();
       }

       protected virtual void OnActorCancel()
       {
           _actorWindow.Hide();
           switch (_actorCommandWindow.CurrentSymbol())
           {
               case "skill":
                   _skillWindow.Show();
                   _skillWindow.Activate();
                   break;
               case "item":
                   _itemWindow.Show();
                   _itemWindow.Activate();
                   break;
           }
       }

       protected virtual void StartEnemySelection()
       {
           _enemyWindow.Refresh();
           _enemyWindow.Show();
           _enemyWindow.Select(0);
           _enemyWindow.Activate();
           _statusWindow.Hide();
       }

       protected virtual void OnEnemyOk()
       {
           var action = Rmmz.BattleManager.InputtingAction();
           action.SetTarget(_enemyWindow.EnemyIndex());
           HideSubInputWindows();
           SelectNextCommand();
       }

       protected virtual void OnEnemyCancel()
       {
           _enemyWindow.Hide();
           switch (_actorCommandWindow.CurrentSymbol())
           {
               case "attack":
                   _statusWindow.Show();
                   _actorCommandWindow.Activate();
                   break;
               case "skill":
                   _skillWindow.Show();
                   _skillWindow.Activate();
                   break;
               case "item":
                   _itemWindow.Show();
                   _itemWindow.Activate();
                   break;
           }
       }

       protected virtual void OnSkillOk()
       {
           var skill = _skillWindow.Item();
           var action = Rmmz.BattleManager.InputtingAction();
           action.SetSkill(skill.Id);
           Rmmz.BattleManager.Actor().SetLastBattleSkill(skill);
           OnSelectAction();
       }

       protected virtual void OnSkillCancel()
       {
           _skillWindow.Hide();
           _statusWindow.Show();
           _actorCommandWindow.Show();
           _actorCommandWindow.Activate();
       }

       protected virtual void OnItemOk()
       {
           var item = _itemWindow.Item();
           var action = Rmmz.BattleManager.InputtingAction();
           action.SetItem(item.Id);
           Rmmz.gameParty.SetLastItem(item as DataItem);
           OnSelectAction();
       }

       protected virtual void OnItemCancel()
       {
           _itemWindow.Hide();
           _statusWindow.Show();
           _actorCommandWindow.Show();
           _actorCommandWindow.Activate();
       }

       protected virtual void OnSelectAction()
       {
           var action = Rmmz.BattleManager.InputtingAction();
           if (!action.NeedsSelection())
           {
               SelectNextCommand();
           }
           else if (action.IsForOpponent())
           {
               StartEnemySelection();
           }
           else
           {
               StartActorSelection();
           }
       }

       protected virtual void EndCommandSelection()
       {
           CloseCommandWindows();
           HideSubInputWindows();
           _statusWindow.Deselect();
           _statusWindow.Show();
       }
   }
}