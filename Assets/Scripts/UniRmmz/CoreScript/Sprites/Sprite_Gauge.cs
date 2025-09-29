using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a status gauge.
    /// </summary>
    public partial class Sprite_Gauge //: Sprite
    {
        protected Game_Actor _battler;
        protected string _statusType;
        protected float _value;
        protected float _maxValue;
        protected float _targetValue;
        protected float _targetMaxValue;
        protected int _duration;
        protected int _flashingCount;

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
            CreateBitmap();
        }

        protected virtual void InitMembers()
        {
            _battler = null;
            _statusType = "";
            _value = float.NaN;
            _maxValue = float.NaN;
            _targetValue = float.NaN;
            _targetMaxValue = float.NaN;
            _duration = 0;
            _flashingCount = 0;
        }

        protected override void OnDestroy()
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
            }
            base.OnDestroy();
        }

        protected virtual void CreateBitmap()
        {
            int width = BitmapWidth();
            int height = BitmapHeight();
            Bitmap = new Bitmap(width, height);
        }

        protected virtual int BitmapWidth()
        {
            return 128;
        }

        protected virtual int BitmapHeight()
        {
            return 32;
        }

        protected virtual int TextHeight()
        {
            return 24;
        }

        protected virtual int GaugeHeight()
        {
            return 12;
        }

        protected virtual int GaugeX()
        {
            if (_statusType == "time")
            {
                return 0;
            }
            else
            {
                return MeasureLabelWidth() + 6;
            }
        }

        protected virtual int LabelY()
        {
            return 3;
        }

        protected virtual string LabelFontFace()
        {
            return Rmmz.gameSystem.MainFontFace();
        }

        protected virtual int LabelFontSize()
        {
            return Rmmz.gameSystem.MainFontSize() - 2;
        }

        protected virtual string ValueFontFace()
        {
            return Rmmz.gameSystem.NumberFontFace();
        }

        protected virtual int ValueFontSize()
        {
            return Rmmz.gameSystem.MainFontSize() - 6;
        }

        public virtual void Setup(Game_Actor battler, string statusType)
        {
            _battler = battler;
            _statusType = statusType;
            _value = CurrentValue();
            _maxValue = CurrentMaxValue();
            UpdateBitmap();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateBitmap();
        }

        protected virtual void UpdateBitmap()
        {
            float value = CurrentValue();
            float maxValue = CurrentMaxValue();
            if (value != _targetValue || maxValue != _targetMaxValue)
            {
                UpdateTargetValue(value, maxValue);
            }
            UpdateGaugeAnimation();
            UpdateFlashing();
        }

        protected virtual void UpdateTargetValue(float value, float maxValue)
        {
            _targetValue = value;
            _targetMaxValue = maxValue;
            if (float.IsNaN(_value))
            {
                _value = value;
                _maxValue = maxValue;
                Redraw();
            }
            else
            {
                _duration = Smoothness();
            }
        }

        protected virtual int Smoothness()
        {
            return _statusType == "time" ? 5 : 20;
        }

        protected virtual void UpdateGaugeAnimation()
        {
            if (_duration > 0)
            {
                float d = _duration;
                _value = (_value * (d - 1) + _targetValue) / d;
                _maxValue = (_maxValue * (d - 1) + _targetMaxValue) / d;
                _duration--;
                Redraw();
            }
        }

        protected virtual void UpdateFlashing()
        {
            if (_statusType == "time")
            {
                _flashingCount++;
                if (_battler.IsInputting())
                {
                    if (_flashingCount % 30 < 15)
                    {
                        SetBlendColor(FlashingColor1());
                    }
                    else
                    {
                        SetBlendColor(FlashingColor2());
                    }
                }
                else
                {
                    SetBlendColor(new UnityEngine.Color(0, 0, 0, 0));
                }
            }
        }

        protected virtual UnityEngine.Color FlashingColor1()
        {
            return new Color32(255, 255, 255, 64);
        }

        protected virtual UnityEngine.Color FlashingColor2()
        {
            return new Color32(0, 0, 255, 48);
        }

        protected virtual bool IsValid()
        {
            if (_battler != null)
            {
                if (_statusType == "tp" && !_battler.IsPreserveTp())
                {
                    return Rmmz.gameParty.InBattle();
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual float CurrentValue()
        {
            if (_battler != null)
            {
                switch (_statusType)
                {
                    case "hp":
                        return _battler.Hp;
                    case "mp":
                        return _battler.Mp;
                    case "tp":
                        return _battler.Tp;
                    case "time":
                        return _battler.TpbChargeTime();
                }
            }
            return float.NaN;
        }

        protected virtual float CurrentMaxValue()
        {
            if (_battler != null)
            {
                switch (_statusType)
                {
                    case "hp":
                        return _battler.Mhp;
                    case "mp":
                        return _battler.Mmp;
                    case "tp":
                        return _battler.MaxTp();
                    case "time":
                        return 1;
                }
            }
            return float.NaN;
        }

        protected virtual string Label()
        {
            switch (_statusType)
            {
                case "hp":
                    return Rmmz.TextManager.HpA;
                case "mp":
                    return Rmmz.TextManager.MpA;
                case "tp":
                    return Rmmz.TextManager.TpA;
                default:
                    return "";
            }
        }

        protected virtual UnityEngine.Color GaugeBackColor()
        {
            return Rmmz.ColorManager.GaugeBackColor();
        }

        protected virtual UnityEngine.Color GaugeColor1()
        {
            switch (_statusType)
            {
                case "hp":
                    return Rmmz.ColorManager.HpGaugeColor1();
                case "mp":
                    return Rmmz.ColorManager.MpGaugeColor1();
                case "tp":
                    return Rmmz.ColorManager.TpGaugeColor1();
                case "time":
                    return Rmmz.ColorManager.CtGaugeColor1();
                default:
                    return Rmmz.ColorManager.NormalColor();
            }
        }

        protected virtual UnityEngine.Color GaugeColor2()
        {
            switch (_statusType)
            {
                case "hp":
                    return Rmmz.ColorManager.HpGaugeColor2();
                case "mp":
                    return Rmmz.ColorManager.MpGaugeColor2();
                case "tp":
                    return Rmmz.ColorManager.TpGaugeColor2();
                case "time":
                    return Rmmz.ColorManager.CtGaugeColor2();
                default:
                    return Rmmz.ColorManager.NormalColor();
            }
        }

        protected virtual UnityEngine.Color LabelColor()
        {
            return Rmmz.ColorManager.SystemColor();
        }

        protected virtual UnityEngine.Color LabelOutlineColor()
        {
            return Rmmz.ColorManager.OutlineColor();
        }

        protected virtual int LabelOutlineWidth()
        {
            return 3;
        }

        protected virtual UnityEngine.Color ValueColor()
        {
            switch (_statusType)
            {
                case "hp":
                    return Rmmz.ColorManager.HpColor(_battler);
                case "mp":
                    return Rmmz.ColorManager.MpColor(_battler);
                case "tp":
                    return Rmmz.ColorManager.TpColor(_battler);
                default:
                    return Rmmz.ColorManager.NormalColor();
            }
        }

        protected virtual UnityEngine.Color ValueOutlineColor()
        {
            return Color.black;
        }

        protected virtual int ValueOutlineWidth()
        {
            return 2;
        }

        protected virtual void Redraw()
        {
            Bitmap.Clear();
            float currentValue = CurrentValue();
            if (!float.IsNaN(currentValue))
            {
                DrawGauge();
                if (_statusType != "time")
                {
                    DrawLabel();
                    if (IsValid())
                    {
                        DrawValue();
                    }
                }
            }
        }

        protected virtual void DrawGauge()
        {
            int gaugeX = GaugeX();
            int gaugeY = TextHeight() - GaugeHeight();
            int gaugeWidth = BitmapWidth() - gaugeX;
            int gaugeHeight = GaugeHeight();
            DrawGaugeRect(gaugeX, gaugeY, gaugeWidth, gaugeHeight);
        }

        protected virtual void DrawGaugeRect(int x, int y, int width, int height)
        {
            float rate = GaugeRate();
            int fillW = Mathf.FloorToInt((width - 2) * rate);
            int fillH = height - 2;
            var color0 = GaugeBackColor();
            var color1 = GaugeColor1();
            var color2 = GaugeColor2();
            Bitmap.FillRect(x, y, width, height, color0);
            Bitmap.GradientFillRect(x + 1, y + 1, fillW, fillH, color1, color2);
        }

        protected virtual float GaugeRate()
        {
            if (IsValid())
            {
                float value = _value;
                float maxValue = _maxValue;
                return maxValue > 0 ? value / maxValue : 0;
            }
            else
            {
                return 0;
            }
        }

        protected virtual void DrawLabel()
        {
            string label = Label();
            int x = LabelOutlineWidth() / 2;
            int y = LabelY();
            int width = BitmapWidth();
            int height = TextHeight();
            SetupLabelFont();
            Bitmap.PaintOpacity = LabelOpacity();
            Bitmap.DrawText(label, x, y, width, height, Bitmap.TextAlign.Left);
            Bitmap.PaintOpacity = 255;
        }

        protected virtual void SetupLabelFont()
        {
            Bitmap.FontFace = LabelFontFace();
            Bitmap.FontSize = LabelFontSize();
            Bitmap.TextColor = LabelColor();
            Bitmap.OutlineColor = LabelOutlineColor();
            Bitmap.OutlineWidth = LabelOutlineWidth();
        }

        protected virtual int MeasureLabelWidth()
        {
            SetupLabelFont();
            string[] labels = { Rmmz.TextManager.HpA, Rmmz.TextManager.MpA, Rmmz.TextManager.TpA };
            int[] widths = labels.Select(str => Bitmap.MeasureTextWidth(str)).ToArray();
            return Mathf.CeilToInt(widths.Max());
        }

        protected virtual int LabelOpacity()
        {
            return IsValid() ? 255 : 160;
        }

        protected virtual void DrawValue()
        {
            float currentValue = CurrentValue();
            int width = BitmapWidth();
            int height = TextHeight();
            SetupValueFont();
            Bitmap.DrawText(currentValue.ToString(), 0, 0, width, height, Bitmap.TextAlign.Right);
        }

        protected virtual void SetupValueFont()
        {
            Bitmap.FontFace = ValueFontFace();
            Bitmap.FontSize = ValueFontSize();
            Bitmap.TextColor = ValueColor();
            Bitmap.OutlineColor = ValueOutlineColor();
            Bitmap.OutlineWidth = ValueOutlineWidth();
        }
    }
}
