using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the item screen.
    /// </summary>
    public partial class Scene_Item //: Scene_ItemBase<Window_ItemList>
    {
        protected Window_ItemCategory _categoryWindow;

        public override void Create()
        {
            base.Create();
            CreateHelpWindow();
            CreateCategoryWindow();
            CreateItemWindow();
            CreateActorWindow();
        }

        protected virtual void CreateCategoryWindow()
        {
            Rect rect = CategoryWindowRect();
            _categoryWindow = Window_ItemCategory.Create(rect, "itemCategory");
            _categoryWindow.SetHelpWindow(_helpWindow);
            _categoryWindow.SetHandler("ok", OnCategoryOk);
            _categoryWindow.SetHandler("cancel", PopScene);
            AddWindow(_categoryWindow);
        }

        protected virtual Rect CategoryWindowRect()
        {
            float wx = 0;
            float wy = MainAreaTop();
            float ww = Graphics.BoxWidth;
            float wh = CalcWindowHeight(1, true);
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateItemWindow()
        {
            Rect rect = ItemWindowRect();
            _itemWindow = Window_ItemList.Create(rect, "itemList");
            _itemWindow.SetHelpWindow(_helpWindow);
            _itemWindow.SetHandler("ok", OnItemOk);
            _itemWindow.SetHandler("cancel", OnItemCancel);
            AddWindow(_itemWindow);
            _categoryWindow.SetItemWindow(_itemWindow);

            if (!_categoryWindow.NeedsSelection())
            {
                _itemWindow.Y -= _categoryWindow.Height;
                _itemWindow.Height += _categoryWindow.Height;
                _itemWindow.CreateContents();
                _categoryWindow.UpdateRmmz();
                _categoryWindow.Hide();
                _categoryWindow.Deactivate();
                OnCategoryOk();
            }
        }

        protected virtual Rect ItemWindowRect()
        {
            float wx = 0;
            float wy = _categoryWindow.Y + _categoryWindow.Height;
            float ww = Graphics.BoxWidth;
            float wh = MainAreaBottom() - wy;
            return new Rect(wx, wy, ww, wh);
        }

        protected override Game_Battler User()
        {
            var members = Rmmz.gameParty.MovableMembers();
            if (!members.Any())
            {
                return null;
            }
            
            float bestPha = members.Max(member => member.Pha);
            return members.FirstOrDefault(member => member.Pha == bestPha);
        }

        protected virtual void OnCategoryOk()
        {
            _itemWindow.Activate();
            _itemWindow.SelectLast();
        }

        protected virtual void OnItemOk()
        {
            Rmmz.gameParty.SetLastItem(Item() as DataItem);
            DetermineItem();
        }

        protected virtual void OnItemCancel()
        {
            if (_categoryWindow.NeedsSelection())
            {
                _itemWindow.Deselect();
                _categoryWindow.Activate();
            }
            else
            {
                PopScene();
            }
        }

        protected override void PlaySeForItem()
        {
            Rmmz.SoundManager.PlayUseItem();
        }

        protected override void UseItem()
        {
            base.UseItem();
            _itemWindow.RedrawCurrentItem();
        }
        
        protected override UsableItem Item()
        {
            return _itemWindow.Item() as UsableItem;
        }
    }
}
