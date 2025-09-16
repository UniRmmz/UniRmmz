using System;

namespace UniRmmz
{
    public class ShopGoods
    {
        public int ItemType;
        public int ItemId;
        public bool IsOverwritePrice;
        public int Price;

        public ShopGoods(object[] @params)
        {
            ItemType = Convert.ToInt32(@params[0]);
            ItemId = Convert.ToInt32(@params[1]);
            IsOverwritePrice = Convert.ToInt32(@params[2]) == 1;
            Price = Convert.ToInt32(@params[3]);
        }

    }
}