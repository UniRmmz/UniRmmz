using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    /// <summary>
    /// he window for displaying scrolling text. No frame is displayed, but it
    /// is handled as a window for convenience.
    /// </summary>
    public partial class Window_ScrollText : Window_Base
    {
        private int _maxBitmapHeight = 2048;

        private Rect _reservedRect;
        private string _text = "";
        private int _allTextHeight = 0;
        private float _blockHeight = 0f;
        private int _blockIndex = 0;
        private float _scrollY = 0f;

        private ScrollRect _scrollRect;
        private RectTransform _contentTransform;
        private Text _textComponent;
        private ContentSizeFitter _contentSizeFitter;

        public override void Initialize(Rect rect)
        {
            base.Initialize(new Rect(0, 0, 0, 0));

            Opacity = 0;
            Hide();

            _reservedRect = rect;
            _text = "";
            _allTextHeight = 0;
            _blockHeight = 0f;
            _blockIndex = 0;
            _scrollY = 0f;
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();

            if (Rmmz.gameMessage.ScrollMode())
            {
                if (!string.IsNullOrEmpty(_text))
                {
                    UpdateMessage();
                }

                if (string.IsNullOrEmpty(_text) && Rmmz.gameMessage.HasText())
                {
                    StartMessage();
                }
            }
        }

        public void StartMessage()
        {
            _text = Rmmz.gameMessage.AllText();
            if (!string.IsNullOrEmpty(_text))
            {
                UpdatePlacement();
                _allTextHeight = (int)TextSizeEx(_text).y;
                _blockHeight = _maxBitmapHeight - Height;
                _blockIndex = 0;
                _scrollY = -Height;
                Origin = new Vector2(Origin.x, _scrollY);
                CreateContents();
                Refresh();
                Show();
            }
            else
            {
                Rmmz.gameMessage.Clear();
            }
        }

        public virtual void Refresh()
        {
            var rect = BaseTextRect();
            float y = rect.y - _scrollY + (_scrollY % _blockHeight);
            Contents.Clear();
            DrawTextEx(_text, (int)rect.x, (int)y, (int)rect.width);
        }

        private void UpdatePlacement()
        {
            var rect = _reservedRect;
            Move(rect.x, rect.y, rect.width, rect.height);
        }

        protected override int ContentsHeight()
        {
            if (_allTextHeight > 0)
            {
                return Mathf.Min(_allTextHeight, _maxBitmapHeight);
            }

            return 0;
        }

        private void UpdateMessage()
        {
            _scrollY += ScrollSpeed();
            if (_scrollY >= _allTextHeight)
            {
                TerminateMessage();
            }
            else
            {
                var blockIndex = Mathf.FloorToInt(_scrollY / _blockHeight);
                if (blockIndex > _blockIndex)
                {
                    _blockIndex = blockIndex;
                    Refresh();
                }

                Origin = new Vector2(Origin.x, _scrollY % _blockHeight);
            }
        }

        private float ScrollSpeed()
        {
            float speed = Rmmz.gameMessage.ScrollSpeed() / 2f;
            if (IsFastForward())
            {
                speed *= FastForwardRate();
            }

            return speed;
        }

        private bool IsFastForward()
        {
            if (Rmmz.gameMessage.ScrollNoFast())
            {
                return false;
            }

            return (
                Input.IsPressed("ok") ||
                Input.IsPressed("shift") ||
                TouchInput.IsPressed()
            );
        }

        private float FastForwardRate()
        {
            return 3;
        }

        private void TerminateMessage()
        {
            _text = null;
            Rmmz.gameMessage.Clear();
            Hide();
        }

    }
}