using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window used for the event command [Input Number].
    /// </summary>
    public partial class Window_NumberInput : Window_Selectable
    {
        private Window_Message _messageWindow;
        private int _number = 0;
        private int _maxDigits = 1;
        private List<Sprite_Button> _buttons = new List<Sprite_Button>();

        public override void Initialize(Rect _)
        {
            base.Initialize(new Rect());
            _number = 0;
            _maxDigits = 1;
            Openness = 0;
            CreateButtons();
            Deactivate();
            _canRepeat = false;
        }

        public virtual void SetMessageWindow(Window_Message messageWindow)
        {
            _messageWindow = messageWindow;
        }

        public virtual void StartRmmz()
        {
            _maxDigits = Rmmz.gameMessage.NumInputMaxDigits();
            _number = Rmmz.gameVariables.Value(Rmmz.gameMessage.NumInputVariableId());
            _number = Mathf.Clamp(_number, 0, (int)Mathf.Pow(10, _maxDigits) - 1);
            UpdatePlacement();
            PlaceButtons();
            CreateContents();
            Refresh();
            Open();
            Activate();
            Select(0);
        }

        protected virtual void UpdatePlacement()
        {
            float messageY = _messageWindow.Y;
            float spacing = 8;
            Width = WindowWidth();
            Height = WindowHeight();
            X = (Graphics.BoxWidth - Width) / 2;
            
            if (messageY >= Graphics.BoxHeight / 2)
            {
                Y = messageY - Height - spacing;
            }
            else
            {
                Y = messageY + _messageWindow.Height + spacing;
            }
        }

        protected virtual int WindowWidth()
        {
            int totalItemWidth = MaxCols() * ItemWidth();
            int totalButtonWidth = TotalButtonWidth();
            return Mathf.Max(totalItemWidth, totalButtonWidth) + (int)(Padding * 2);
        }

        protected virtual int WindowHeight()
        {
            if (Rmmz.ConfigManager.TouchUI)
            {
                return FittingHeight(1) + ButtonSpacing() + 48;
            }
            else
            {
                return FittingHeight(1);
            }
        }

        protected override int MaxCols() => _maxDigits;

        protected override int MaxItems() => _maxDigits;

        public override int ItemWidth() => 48;

        protected override Rect ItemRect(int index)
        {
            Rect rect = base.ItemRect(index);
            float innerMargin = InnerWidth - MaxCols() * ItemWidth();
            rect.x += innerMargin / 2;
            return rect;
        }

        protected override bool IsScrollEnabled() => false;

        protected override bool IsHoverEnabled() => false;

        protected virtual void CreateButtons()
        {
            _buttons = new List<Sprite_Button>();
            if (Rmmz.ConfigManager.TouchUI)
            {
                UniRmmz.Input.ButtonTypes[] types =
                {
                    UniRmmz.Input.ButtonTypes.Down,
                    UniRmmz.Input.ButtonTypes.Up, 
                    UniRmmz.Input.ButtonTypes.Ok
                };
                foreach (var type in types)
                {
                    var button = Sprite_Button.Create("button");
                    button.Initialize(type);
                    _buttons.Add(button);
                    AddInnerChild(button);
                }
                _buttons[0].SetClickHandler(() => OnButtonDown());
                _buttons[1].SetClickHandler(() => OnButtonUp());
                _buttons[2].SetClickHandler(() => OnButtonOk());
            }
        }

        protected virtual void PlaceButtons()
        {
            int sp = ButtonSpacing();
            int totalWidth = TotalButtonWidth();
            float x = (InnerWidth - totalWidth) / 2;
            
            foreach (Sprite_Button button in _buttons)
            {
                button.X = x;
                button.Y = ButtonY();
                x += button.Width + sp;
            }
        }

        protected virtual int TotalButtonWidth()
        {
            int sp = ButtonSpacing();
            int total = -sp;
            foreach (Sprite_Button button in _buttons)
            {
                total += (int)button.Width + sp;
            }
            return total;
        }

        protected virtual int ButtonSpacing()
        {
            return 8;
        }

        protected virtual float ButtonY()
        {
            return ItemHeight() + ButtonSpacing();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            ProcessDigitChange();
        }

        protected virtual void ProcessDigitChange()
        {
            if (IsOpenAndActive())
            {
                if (Input.IsRepeated("up"))
                {
                    ChangeDigit(true);
                }
                else if (Input.IsRepeated("down"))
                {
                    ChangeDigit(false);
                }
            }
        }

        protected virtual void ChangeDigit(bool up)
        {
            int index = Index();
            int place = (int)Mathf.Pow(10, _maxDigits - 1 - index);
            int n = Mathf.FloorToInt(_number / place) % 10;
            _number -= n * place;
            
            if (up)
            {
                n = (n + 1) % 10;
            }
            else
            {
                n = (n + 9) % 10;
            }
            
            _number += n * place;
            Refresh();
            PlayCursorSound();
        }

        public override bool IsTouchOkEnabled() => false;

        protected override bool IsOkEnabled() => true;

        protected override bool IsCancelEnabled() => false;

        protected override void ProcessOk()
        {
            PlayOkSound();
            Rmmz.gameVariables.SetValue(Rmmz.gameMessage.NumInputVariableId(), _number);
            _messageWindow.TerminateMessage();
            UpdateInputData();
            Deactivate();
            Close();
        }

        public override void DrawItem(int index)
        {
            Rect rect = ItemLineRect(index);
            var align = Bitmap.TextAlign.Center;
            string s = _number.ToString().PadLeft(_maxDigits, '0');
            string c = s.Substring(index, 1);
            ResetTextColor();
            DrawText(c, (int)rect.x, (int)rect.y, (int)rect.width, align);
        }

        protected virtual void OnButtonUp()
        {
            ChangeDigit(true);
        }

        protected virtual void OnButtonDown()
        {
            ChangeDigit(false);
        }

        protected virtual void OnButtonOk()
        {
            ProcessOk();
        }
    }
}