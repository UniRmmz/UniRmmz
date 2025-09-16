using UnityEngine;

namespace UniRmmz
{
    public partial class Window_Gold : Window_Selectable
    {
        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Refresh();
        }

        protected override int ColSpacing()
        {
            return 0;
        }

        public override void Refresh()
        {
            var rect = ItemLineRect(0);
            var x = (int)rect.x;
            var y = (int)rect.y;
            var width = (int)rect.width;
            Contents.Clear();
            DrawCurrencyValue(Value(), CurrencyUnit(), x, y, width);
        }

        public virtual int Value()
        {
            return Rmmz.gameParty.Gold();
        }

        public virtual string CurrencyUnit()
        {
            return Rmmz.TextManager.CurrencyUnit;
        }

        public override void Open()
        {
            Refresh();
            base.Open();
        }
    }
}