using System;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for handling skills, items, weapons, and armor. It is
    /// required because save data should not include the database object itself.
    /// </summary>
    [Serializable]
    public partial class Game_Item
    {
        protected string _dataClass = "";
        protected int _itemId = 0;

        protected Game_Item(DataCommonItem item = null)
        {
            _dataClass = "";
            _itemId = 0;
            if (item != null)
            {
                SetObject(item);
            }
        }

        public virtual bool IsSkill()
        {
            return _dataClass == "skill";
        }

        public virtual bool IsItem()
        {
            return _dataClass == "item";
        }

        public virtual bool IsUsableItem()
        {
            return IsSkill() || IsItem();
        }

        public virtual bool IsWeapon()
        {
            return _dataClass == "weapon";
        }

        public virtual bool IsArmor()
        {
            return _dataClass == "armor";
        }

        public virtual bool IsEquipItem()
        {
            return IsWeapon() || IsArmor();
        }

        public virtual bool IsNull()
        {
            return _dataClass == "";
        }

        public virtual int ItemId()
        {
            return _itemId;
        }

        public virtual T Object<T>() where T : DataCommonItem
        {
            if (IsSkill())
            {
                return Rmmz.dataSkills[_itemId] as T;
            }
            else if (IsItem())
            {
                return Rmmz.dataItems[_itemId] as T;
            }
            else if (IsWeapon())
            {
                return Rmmz.dataWeapons[_itemId] as T;
            }
            else if (IsArmor())
            {
                return Rmmz.dataArmors[_itemId] as T;
            }
            else
            {
                return null;
            }
        }

        public virtual void SetObject(DataCommonItem item)
        {
            if (Rmmz.DataManager.IsSkill(item))
            {
                _dataClass = "skill";
            }
            else if (Rmmz.DataManager.IsItem(item))
            {
                _dataClass = "item";
            }
            else if (Rmmz.DataManager.IsWeapon(item))
            {
                _dataClass = "weapon";
            }
            else if (Rmmz.DataManager.IsArmor(item))
            {
                _dataClass = "armor";
            }
            else
            {
                _dataClass = "";
            }
            _itemId = item != null ? item.Id : 0;
        }

        public virtual void SetEquip(bool isWeapon, int itemId)
        {
            _dataClass = isWeapon ? "weapon" : "armor";
            _itemId = itemId;
        }
    }
}