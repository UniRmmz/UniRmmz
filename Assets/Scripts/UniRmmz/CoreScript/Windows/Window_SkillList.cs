using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a skill on the skill screen.
    /// </summary>
    public partial class Window_SkillList //: Window_Selectable
    {
        protected Game_Actor _actor = null;
        protected int _stypeId = 0;
        protected List<DataSkill> _data = new List<DataSkill>();

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _actor = null;
            _stypeId = 0;
            _data = new List<DataSkill>();
        }

        public virtual void SetActor(Game_Actor actor)
        {
            if (_actor != actor)
            {
                _actor = actor;
                Refresh();
                ScrollTo(0, 0);
            }
        }

        public virtual void SetStypeId(int stypeId)
        {
            if (_stypeId != stypeId)
            {
                _stypeId = stypeId;
                Refresh();
                ScrollTo(0, 0);
            }
        }

        protected override int MaxCols() => 2;

        protected override int ColSpacing() => 16;

        protected override int MaxItems()
        {
            return _data != null ? _data.Count : 1;
        }

        public virtual DataSkill Item()
        {
            return ItemAt(Index());
        }

        protected virtual DataSkill ItemAt(int index)
        {
            return _data != null && index >= 0 && index < _data.Count ? _data[index] : null;
        }

        public override bool IsCurrentItemEnabled()
        {
            return IsEnabled(_data[Index()]);
        }

        protected virtual bool Includes(DataSkill item)
        {
            return item != null && item.StypeId == _stypeId;
        }

        protected virtual bool IsEnabled(DataSkill item)
        {
            return _actor != null && _actor.CanUse(item);
        }

        protected virtual void MakeItemList()
        {
            if (_actor != null)
            {
                _data = _actor.Skills().Where(item => Includes(item)).ToList();
            }
            else
            {
                _data = new List<DataSkill>();
            }
        }

        public virtual void SelectLast()
        {
            var lastSkill = _actor?.LastSkill();
            int index = lastSkill != null ? _data.IndexOf(lastSkill) : -1;
            ForceSelect(index >= 0 ? index : 0);
        }

        public override void DrawItem(int index)
        {
            var skill = ItemAt(index);
            if (skill != null)
            {
                int costWidth = CostWidth();
                Rect rect = ItemLineRect(index);
                ChangePaintOpacity(IsEnabled(skill));
                DrawItemName(skill, (int)rect.x, (int)rect.y, (int)rect.width - costWidth);
                DrawSkillCost(skill, (int)rect.x, (int)rect.y, (int)rect.width);
                ChangePaintOpacity(true);
            }
        }

        protected virtual int CostWidth()
        {
            return TextWidth("000");
        }

        protected virtual void DrawSkillCost(DataSkill dataSkill, int x, int y, int width)
        {
            if (_actor.SkillTpCost(dataSkill) > 0)
            {
                ChangeTextColor(Rmmz.ColorManager.TpCostColor());
                DrawText(_actor.SkillTpCost(dataSkill).ToString(), x, y, width, Bitmap.TextAlign.Right);
            }
            else if (_actor.SkillMpCost(dataSkill) > 0)
            {
                ChangeTextColor(Rmmz.ColorManager.MpCostColor());
                DrawText(_actor.SkillMpCost(dataSkill).ToString(), x, y, width, Bitmap.TextAlign.Right);
            }
        }

        protected override void UpdateHelp()
        {
            SetHelpWindowItem(Item());
        }

        public override void Refresh()
        {
            MakeItemList();
            base.Refresh();
        }
    }
}