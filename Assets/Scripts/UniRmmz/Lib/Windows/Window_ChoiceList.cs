using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window used for the event command [Show Choices].
    /// </summary>
    public partial class Window_ChoiceList : Window_Command
    {
        private Window_Message _messageWindow;
        private int _background = 0;
        private Sprite_Button _cancelButton;

        public override void Initialize(Rect _)
        {
            base.Initialize(new Rect());
            CreateCancelButton();
            Openness = 0;
            Deactivate();
            _background = 0;
            _canRepeat = false;
        }

        public void SetMessageWindow(Window_Message messageWindow)
        {
            _messageWindow = messageWindow;
        }

        private void CreateCancelButton()
        {
            if (Rmmz.ConfigManager.TouchUI)
            {
                _cancelButton = Sprite_Button.Create( "cancel");
                _cancelButton.Initialize(Input.ButtonTypes.Cancel);
                _cancelButton.Visible = false;
                this.AddChild(_cancelButton);
            }
        }

        public void StartRmmz()
        {
            UpdatePlacement();
            UpdateBackground();
            PlaceCancelButton();
            CreateContents();
            Refresh();
            ScrollTo(0, 0);
            SelectDefault();
            Open();
            Activate();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateCancelButton();
        }

        private void UpdateCancelButton()
        {
            if (_cancelButton != null)
            {
                _cancelButton.Visible = NeedsCancelButton() && IsOpen();
            }
        }

        private void SelectDefault()
        {
            Select(Rmmz.gameMessage.ChoiceDefaultType());
        }

        private void UpdatePlacement()
        {
            X = WindowX();
            Y = WindowY();
            Width = WindowWidth();
            Height = WindowHeight();
        }

        private void UpdateBackground()
        {
            _background = Rmmz.gameMessage.ChoiceBackground();
            SetBackgroundType(_background);
        }

        private void PlaceCancelButton()
        {
            if (_cancelButton != null)
            {
                const float spacing = 8;
                Sprite_Button button = _cancelButton;
                float right = X + Width;
                
                if (right < Graphics.BoxWidth - button.Width + spacing)
                {
                    button.X = Width + spacing;
                }
                else
                {
                    button.X = -button.Width - spacing;
                }
                
                button.Y = Height / 2 - button.Height / 2;
            }
        }

        private float WindowX()
        {
            int positionType = Rmmz.gameMessage.ChoicePositionType();
            
            if (positionType == 1)
            {
                return (Graphics.BoxWidth - WindowWidth()) / 2;
            }
            else if (positionType == 2)
            {
                return Graphics.BoxWidth - WindowWidth();
            }
            else
            {
                return 0;
            }
        }

        private float WindowY()
        {
            float messageY = _messageWindow.Y;
            
            if (messageY >= Graphics.BoxHeight / 2)
            {
                return messageY - WindowHeight();
            }
            else
            {
                return messageY + _messageWindow.Height;
            }
        }

        private float WindowWidth()
        {
            float width = MaxChoiceWidth() + ColSpacing() + Padding * 2;
            return Mathf.Min(width, Graphics.BoxWidth);
        }

        private float WindowHeight()
        {
            return FittingHeight(NumVisibleRows());
        }

        private int NumVisibleRows()
        {
            List<string> choices = Rmmz.gameMessage.Choices();
            return Math.Min(choices.Count, MaxLines());
        }

        private int MaxLines()
        {
            Window_Message messageWindow = _messageWindow;
            float messageY = messageWindow != null ? messageWindow.Y : 0;
            float messageHeight = messageWindow != null ? messageWindow.Height : 0;
            float centerY = Graphics.BoxHeight / 2;
            
            if (messageY < centerY && messageY + messageHeight > centerY)
            {
                return 4;
            }
            else
            {
                return 8;
            }
        }

        private int MaxChoiceWidth()
        {
            int maxWidth = 96;
            List<string> choices = Rmmz.gameMessage.Choices();
            
            foreach (string choice in choices)
            {
                float textWidth = TextSizeEx(choice).x;
                int choiceWidth = Mathf.CeilToInt(textWidth) + ItemPadding() * 2;
                
                if (maxWidth < choiceWidth)
                {
                    maxWidth = choiceWidth;
                }
            }
            
            return maxWidth;
        }

        protected override void MakeCommandList()
        {
            List<string> choices = Rmmz.gameMessage.Choices();
            foreach (string choice in choices)
            {
                AddCommand(choice, "choice");
            }
        }

        public override void DrawItem(int index)
        {
            var rect = ItemLineRect(index);
            DrawTextEx(CommandName(index), (int)rect.x, (int)rect.y, (int)rect.width);
        }

        protected override bool IsCancelEnabled()
        {
            return Rmmz.gameMessage.ChoiceCancelType() != -1;
        }

        public bool NeedsCancelButton()
        {
            return Rmmz.gameMessage.ChoiceCancelType() == -2;
        }

        protected override void CallOkHandler()
        {
            Rmmz.gameMessage.OnChoice(Index());
            _messageWindow.TerminateMessage();
            Close();
        }

        protected override void CallCancelHandler()
        {
            Rmmz.gameMessage.OnChoice(Rmmz.gameMessage.ChoiceCancelType());
            _messageWindow.TerminateMessage();
            Close();
        }
    }
}
