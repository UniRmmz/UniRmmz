using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a save file on the save and load screens.
    /// </summary>
    public partial class Window_SavefileList //: Window_Selectable
    {
        protected string _mode = null;
        protected bool _autosave = false;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Activate();
            _mode = null;
            _autosave = false;
        }

        public virtual void SetMode(string mode, bool autosave)
        {
            _mode = mode;
            _autosave = autosave;
            Refresh();
        }

        protected override int MaxItems()
        {
            return Rmmz.DataManager.MaxSavefiles() - (_autosave ? 0 : 1);
        }

        protected virtual int NumVisibleRows()
        {
            return 5;
        }

        public override int ItemHeight()
        {
            return InnerHeight / NumVisibleRows();
        }

        public override void DrawItem(int index)
        {
            int savefileId = IndexToSavefileId(index);
            var info = Rmmz.DataManager.SavefileInfo(savefileId);
            Rect rect = ItemRectWithPadding(index);
            ResetTextColor();
            ChangePaintOpacity(IsEnabled(savefileId));
            DrawTitle(savefileId, (int)rect.x, (int)rect.y + 4);
            if (info != null)
            {
                DrawContents(info, rect);
            }
        }

        protected virtual int IndexToSavefileId(int index)
        {
            return index + (_autosave ? 0 : 1);
        }

        protected virtual int SavefileIdToIndex(int savefileId)
        {
            return savefileId - (_autosave ? 0 : 1);
        }

        public virtual bool IsEnabled(int savefileId)
        {
            if (_mode == "save")
            {
                return savefileId > 0;
            }
            else
            {
                return Rmmz.DataManager.SavefileInfo(savefileId) != null;
            }
        }

        public virtual int SavefileId()
        {
            return IndexToSavefileId(Index());
        }

        public virtual void SelectSavefile(int savefileId)
        {
            int index = Mathf.Max(0, SavefileIdToIndex(savefileId));
            Select(index);
            SetTopRow(index - 2);
        }

        protected virtual void DrawTitle(int savefileId, int x, int y)
        {
            if (savefileId == 0)
            {
                DrawText(Rmmz.TextManager.Autosave, x, y, 180);
            }
            else
            {
                DrawText(Rmmz.TextManager.File + " " + savefileId, x, y, 180);
            }
        }

        protected virtual void DrawContents(DataManager.GlobalInfoElement info, Rect rect)
        {
            float bottom = rect.y + rect.height;
            if (rect.width >= 420)
            {
                DrawPartyCharacters(info, (int)rect.x + 220, (int)bottom - 8);
            }
            float lineHeight = LineHeight();
            float y2 = bottom - lineHeight - 4;
            if (y2 >= lineHeight)
            {
                DrawPlaytime(info, (int)rect.x, (int)y2, (int)rect.width);
            }
        }

        protected virtual void DrawPartyCharacters(DataManager.GlobalInfoElement info, int x, int y)
        {
            if (info.characters != null)
            {
                int characterX = x;
                foreach (var data in info.characters)
                {
                    DrawCharacter(data[0], Convert.ToInt32(data[1]), characterX, y);
                    characterX += 48;
                }
            }
        }

        protected virtual void DrawPlaytime(DataManager.GlobalInfoElement info, int x, int y, int width)
        {
            if (!string.IsNullOrEmpty(info.playtime))
            {
                DrawText(info.playtime, x, y, width, Bitmap.TextAlign.Right);
            }
        }

        protected override void PlayOkSound()
        {
            // Override to prevent sound in some cases
        }
    }

}