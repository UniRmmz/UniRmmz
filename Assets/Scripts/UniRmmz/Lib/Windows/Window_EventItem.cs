using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window used for the event command [Select Item].
    /// </summary>
    public partial class Window_EventItem : Window_ItemList
    {
        private Window_Message _messageWindow;
        private Sprite_Button _cancelButton;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            CreateCancelButton();
            Openness = 0;
            Deactivate();
            SetHandler("ok", () => OnOk());
            SetHandler("cancel", () => OnCancel());
        }

        public virtual void SetMessageWindow(Window_Message messageWindow)
        {
            _messageWindow = messageWindow;
        }

        protected virtual void CreateCancelButton()
        {
            if (Rmmz.ConfigManager.TouchUI)
            {
                _cancelButton = Sprite_Button.Create("cancel");
                _cancelButton.Initialize(Input.ButtonTypes.Cancel);
                _cancelButton.Visible = false;
                this.AddChild(_cancelButton);
            }
        }

        public virtual void StartRmmz()
        {
            Refresh();
            UpdatePlacement();
            PlaceCancelButton();
            ForceSelect(0);
            Open();
            Activate();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateCancelButton();
        }

        protected virtual void UpdateCancelButton()
        {
            if (_cancelButton != null)
            {
                _cancelButton.Visible = IsOpen();
            }
        }

        protected virtual void UpdatePlacement()
        {
            if (_messageWindow.Y >= Graphics.BoxHeight / 2)
            {
                Y = 0;
            }
            else
            {
                Y = Graphics.BoxHeight - Height;
            }
        }

        protected virtual void PlaceCancelButton()
        {
            if (_cancelButton != null)
            {
                float spacing = 8;
                Sprite_Button button = _cancelButton;
                
                if (Y == 0)
                {
                    button.Y = Height + spacing;
                }
                else if (_messageWindow.Y >= Graphics.BoxHeight / 4)
                {
                    float distance = Y - _messageWindow.Y;
                    button.Y = -button.Height - spacing - distance;
                }
                else
                {
                    button.Y = -button.Height - spacing;
                }
                
                button.X = Width - button.Width - spacing;
            }
        }

        protected override bool Includes(DataCommonItem item)
        {
            int itypeId = Rmmz.gameMessage.ItemChoiceItypeId();
            return Rmmz.DataManager.IsItem(item) && ((DataItem)item).ItypeId == itypeId;
        }

        protected override bool NeedsNumber()
        {
            int itypeId = Rmmz.gameMessage.ItemChoiceItypeId();
            
            if (itypeId == 2)
            {
                // Key Item
                return Rmmz.dataSystem.OptKeyItemsNumber;
            }
            else if (itypeId >= 3)
            {
                // Hidden Item
                return false;
            }
            else
            {
                // Normal Item
                return true;
            }
        }

        protected override bool IsEnabled(DataCommonItem _)
        {
            return true;
        }

        protected virtual void OnOk()
        {
            var item = Item();
            int itemId = item != null ? item.Id : 0;
            Rmmz.gameVariables.SetValue(Rmmz.gameMessage.ItemChoiceVariableId(), itemId);
            _messageWindow.TerminateMessage();
            Close();
        }

        protected virtual void OnCancel()
        {
            Rmmz.gameVariables.SetValue(Rmmz.gameMessage.ItemChoiceVariableId(), 0);
            _messageWindow.TerminateMessage();
            Close();
        }
    }
}
