using UnityEngine;

namespace UniRmmz
{
    public partial class ColorManager
    {
        protected Bitmap _windowskin;

        public virtual void LoadWindowskin()
        {
            _windowskin = Rmmz.ImageManager.LoadSystem("Window");
        }

        public virtual Color TextColor(int n)
        {
            int px = 96 + (n % 8) * 12 + 6;
            int py = 144 + (n / 8) * 12 + 6;

            return _windowskin.GetPixel(px, py);
        }

        public virtual Color NormalColor() => TextColor(0);
        public virtual Color SystemColor() => TextColor(16);
        public virtual Color CrisisColor() => TextColor(17);
        public virtual Color DeathColor() => TextColor(18);
        public virtual Color GaugeBackColor() => TextColor(19);
        public virtual Color HpGaugeColor1() => TextColor(20);
        public virtual Color HpGaugeColor2() => TextColor(21);
        public virtual Color MpGaugeColor1() => TextColor(22);
        public virtual Color MpGaugeColor2() => TextColor(23);
        public virtual Color MpCostColor() => TextColor(23);
        public virtual Color PowerUpColor() => TextColor(24);
        public virtual Color PowerDownColor() => TextColor(25);
        public virtual Color CtGaugeColor1() => TextColor(26);
        public virtual Color CtGaugeColor2() => TextColor(27);
        public virtual Color TpGaugeColor1() => TextColor(28);
        public virtual Color TpGaugeColor2() => TextColor(29);
        public virtual Color TpCostColor() => TextColor(29);

        public virtual Color PendingColor()
        {
            return _windowskin.GetPixel(120, 120);
        }

        public virtual Color HpColor(Game_Actor actor)
        {
            if (actor == null) return NormalColor();
            if (actor.IsDead()) return DeathColor();
            if (actor.IsDying()) return CrisisColor();
            return NormalColor();
        }

        public virtual Color MpColor(Game_Actor _) => NormalColor();
        public virtual Color TpColor(Game_Actor _) => NormalColor();

        public virtual Color ParamChangeTextColor(int change)
        {
            if (change > 0) return PowerUpColor();
            if (change < 0) return PowerDownColor();
            return NormalColor();
        }

        public virtual Color DamageColor(int colorType)
        {
            return colorType switch
            {
                0 => Color.white,
                1 => new Color(0.73f, 1f, 0.71f), // #b9ffb5
                2 => new Color(1f, 1f, 0.56f),   // #ffff90
                3 => new Color(0.5f, 0.69f, 1f), // #80b0ff
                _ => Color.gray
            };
        }

        public virtual Color OutlineColor() => new Color(0f, 0f, 0f, 0.6f);
        public virtual Color DimColor1() => new Color(0f, 0f, 0f, 0.6f);
        public virtual Color DimColor2() => new Color(0f, 0f, 0f, 0f);
        public virtual Color ItemBackColor1() => new Color(32f / 255f, 32f / 255f, 32f / 255f, 0.5f);
        public virtual Color ItemBackColor2() => new Color(0f, 0f, 0f, 0.5f);
    }
}