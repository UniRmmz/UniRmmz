using System;
using UnityEngine;

namespace UniRmmz
{
   public partial class Scene_Name : Scene_MenuBase
   {
       protected int _actorId;
       protected int _maxLength;
       protected Window_NameEdit _editWindow;
       protected Window_NameInput _inputWindow;

       public override void Prepare(params object[] args)
       {
           _actorId = Convert.ToInt32(args[0]);
           _maxLength = Convert.ToInt32(args[1]);
       }

       public override void Create()
       {
           base.Create();
           _actor = Rmmz.gameActors.Actor(_actorId);
           CreateEditWindow();
           CreateInputWindow();
       }

       public override void StartScene()
       {
           base.StartScene();
           _editWindow.Refresh();
       }

       protected virtual void CreateEditWindow()
       {
           Rect rect = EditWindowRect();
           _editWindow = Window_NameEdit.Create(rect, "editWindow");
           _editWindow.Setup(_actor, _maxLength);
           AddWindow(_editWindow);
       }

       protected virtual Rect EditWindowRect()
       {
           float inputWindowHeight = CalcWindowHeight(9, true);
           float padding = Rmmz.gameSystem.WindowPadding();
           float ww = 600f;
           float wh = Rmmz.ImageManager.FaceHeight + padding * 2;
           float wx = (Graphics.BoxWidth - ww) / 2f;
           float wy = (Graphics.BoxHeight - (wh + inputWindowHeight + 8f)) / 2f;
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual void CreateInputWindow()
       {
           Rect rect = InputWindowRect();
           _inputWindow = Window_NameInput.Create(rect, "inputWindow");
           _inputWindow.SetEditWindow(_editWindow);
           _inputWindow.SetHandler("ok", OnInputOk);
           AddWindow(_inputWindow);
       }

       protected virtual Rect InputWindowRect()
       {
           float wx = _editWindow.X;
           float wy = _editWindow.Y + _editWindow.Height + 8f;
           float ww = _editWindow.Width;
           float wh = CalcWindowHeight(9, true);
           return new Rect(wx, wy, ww, wh);
       }

       protected virtual void OnInputOk()
       {
           _actor.SetName(_editWindow.Name());
           PopScene();
       }
   }
}