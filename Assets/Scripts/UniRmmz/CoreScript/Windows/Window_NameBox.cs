using System.Drawing;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying a speaker name above the message window.
    /// </summary>
    public partial class Window_NameBox : Window_Base
    {
        protected Window_Message _messageWindow;
        protected string _name = string.Empty;

        public override void Initialize(Rect _)
        {
            base.Initialize(Rect.zero);
            Openness = 0;
            _name = "";
        }

        public virtual void SetMessageWindow(Window_Message messageWindow)
        {
            _messageWindow = messageWindow;
        }

        public virtual void SetName(string name)
        {
            if (_name != name)
            {
                _name = name;
                Refresh();
            }
        }

        public virtual void Clear()
        {
            SetName("");
        }

        public void StartRmmz()
        {
            UpdatePlacement();
            UpdateBackground();
            CreateContents();
            Refresh();
        }

        protected virtual void UpdatePlacement()
        {
            Width = WindowWidth();
            Height = WindowHeight();
            Window_Message messageWindow = _messageWindow;
            
            if (Rmmz.gameMessage.IsRTL())
            {
                X = messageWindow.X + messageWindow.Width - Width;
            }
            else
            {
                X = messageWindow.X;
            }
            
            if (messageWindow.Y > 0)
            {
                Y = messageWindow.Y - Height;
            }
            else
            {
                Y = messageWindow.Y + messageWindow.Height;
            }
        }

        protected virtual void UpdateBackground()
        {
            SetBackgroundType(Rmmz.gameMessage.Background());
        }

        protected virtual int WindowWidth()
        {
            if (!string.IsNullOrEmpty(_name))
            {
                var textSize = TextSizeEx(_name);
                float textWidth = textSize.x;
                float padding = this.Padding + ItemPadding();
                int width = Mathf.CeilToInt(textWidth) + (int)(padding * 2);
                return Mathf.Min(width, Graphics.BoxWidth);
            }
            else
            {
                return 0;
            }
        }

        protected virtual int WindowHeight()
        {
            return FittingHeight(1);
        }

        public virtual void Refresh()
        {
            Rect rect = BaseTextRect();
            Contents.Clear();
            DrawTextEx(_name, (int)rect.x, (int)rect.y, (int)rect.width);
        }
    }
}