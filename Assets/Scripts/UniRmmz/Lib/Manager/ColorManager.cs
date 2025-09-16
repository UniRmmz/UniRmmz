using UnityEngine;

namespace UniRmmz
{
    public partial class ColorManager
    {
        private Bitmap _windowskin;

        public void LoadWindowskin()
        {
            _windowskin = Rmmz.ImageManager.LoadSystem("Window");
        }

        public Color TextColor(int n)
        {
            int px = 96 + (n % 8) * 12 + 6;
            int py = 144 + (n / 8) * 12 + 6;

            return _windowskin.GetPixel(px, py);
        }

        public Color NormalColor() => TextColor(0);
        public Color SystemColor() => TextColor(16);
        public Color CrisisColor() => TextColor(17);
        public Color DeathColor() => TextColor(18);
        public Color GaugeBackColor() => TextColor(19);
        public Color HpGaugeColor1() => TextColor(20);
        public Color HpGaugeColor2() => TextColor(21);
        public Color MpGaugeColor1() => TextColor(22);
        public Color MpGaugeColor2() => TextColor(23);
        public Color MpCostColor() => TextColor(23);
        public Color PowerUpColor() => TextColor(24);
        public Color PowerDownColor() => TextColor(25);
        public Color CtGaugeColor1() => TextColor(26);
        public Color CtGaugeColor2() => TextColor(27);
        public Color TpGaugeColor1() => TextColor(28);
        public Color TpGaugeColor2() => TextColor(29);
        public Color TpCostColor() => TextColor(29);

        public Color PendingColor()
        {
            return _windowskin.GetPixel(120, 120);
        }

        public Color HpColor(Game_Actor actor)
        {
            if (actor == null) return NormalColor();
            if (actor.IsDead()) return DeathColor();
            if (actor.IsDying()) return CrisisColor();
            return NormalColor();
        }

        public Color MpColor(Game_Actor _) => NormalColor();
        public Color TpColor(Game_Actor _) => NormalColor();

        public Color ParamChangeTextColor(int change)
        {
            if (change > 0) return PowerUpColor();
            if (change < 0) return PowerDownColor();
            return NormalColor();
        }

        public Color DamageColor(int colorType)
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

        public Color OutlineColor() => new Color(0f, 0f, 0f, 0.6f);
        public Color DimColor1() => new Color(0f, 0f, 0f, 0.6f);
        public Color DimColor2() => new Color(0f, 0f, 0f, 0f);
        public Color ItemBackColor1() => new Color(32f / 255f, 32f / 255f, 32f / 255f, 0.5f);
        public Color ItemBackColor2() => new Color(0f, 0f, 0f, 0.5f);
    }
}