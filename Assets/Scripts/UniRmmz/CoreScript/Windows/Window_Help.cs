using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying the description of the selected item.
    /// </summary>
    public partial class Window_Help //: Window_Base
    {
        protected string _text = String.Empty;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _text = String.Empty;
        }

        public virtual void SetText(string text)
        {
            if (_text != text)
            {
                _text = text;
                Refresh();
            }
        }

        public virtual void Clear()
        {
            SetText(String.Empty);
        }

        public virtual void SetItem<T>(T item) where T : DataCommonItem 
        {
            SetText(item?.Description ?? String.Empty);
        }

        public virtual void Refresh()
        {
            var rect = BaseTextRect();
            Contents.Clear();
            DrawTextEx(_text, (int)rect.x, (int)rect.y, (int)rect.width);
        }
    }
}