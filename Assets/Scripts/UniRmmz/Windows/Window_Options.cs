using UnityEngine;
using System.Collections.Generic;

namespace UniRmmz
{
    public partial class Window_Options : Window_Command
    {
        protected override void MakeCommandList()
        {
            AddGeneralOptions();
            AddVolumeOptions();
        }

        private void AddGeneralOptions()
        {
            AddCommand(Rmmz.TextManager.AlwaysDash, "alwaysDash");
            AddCommand(Rmmz.TextManager.CommandRemember, "commandRemember");
            AddCommand(Rmmz.TextManager.TouchUI, "touchUI");
        }

        private void AddVolumeOptions()
        {
            AddCommand(Rmmz.TextManager.BgmVolume, "bgmVolume");
            AddCommand(Rmmz.TextManager.BgsVolume, "bgsVolume");
            AddCommand(Rmmz.TextManager.MeVolume, "meVolume");
            AddCommand(Rmmz.TextManager.SeVolume, "seVolume");
        }

        public override void DrawItem(int index)
        {
            var title = CommandName(index);
            var status = StatusText(index);
            var rect = ItemLineRect(index);
            int statusWidth = StatusWidth();
            float titleWidth = rect.width - statusWidth;

            ResetTextColor();
            ChangePaintOpacity(IsCommandEnabled(index));
            DrawText(title, (int)rect.x, (int)rect.y, (int)titleWidth, Bitmap.TextAlign.Left);
            DrawText(status, (int)(rect.x + titleWidth), (int)rect.y, (int)statusWidth, Bitmap.TextAlign.Right);
        }

        protected int StatusWidth() => 120;

        protected string StatusText(int index)
        {
            var symbol = CommandSymbol(index);
            if (IsVolumeSymbol(symbol))
            {
                return VolumeStatusText(GetConfigValueAsInt(symbol));
            }
            else
            {
                return BooleanStatusText(GetConfigValueAsBool(symbol));
            }
        }

        private bool IsVolumeSymbol(string symbol) => symbol.Contains("Volume");

        private string BooleanStatusText(bool value) => value ? "ON" : "OFF";

        private string VolumeStatusText(int value) => value + "%";

        protected override void ProcessOk()
        {
            int index = Index();
            var symbol = CommandSymbol(index);
            if (IsVolumeSymbol(symbol))
            {
                ChangeVolume(symbol, true, true);
            }
            else
            {
                ChangeValue(symbol, GetConfigValueAsBool(symbol) ? 0 : 1);
            }
        }

        protected override void CursorRight(bool wrap)
        {
            int index = Index();
            var symbol = CommandSymbol(index);
            if (IsVolumeSymbol(symbol))
            {
                ChangeVolume(symbol, true, false);
            }
            else
            {
                ChangeValue(symbol, 1);
            }
        }

        protected override void CursorLeft(bool wrap)
        {
            int index = Index();
            var symbol = CommandSymbol(index);
            if (IsVolumeSymbol(symbol))
            {
                ChangeVolume(symbol, false, false);
            }
            else
            {
                ChangeValue(symbol, 0);
            }
        }

        private void ChangeVolume(string symbol, bool forward, bool wrap)
        {
            var lastValue = GetConfigValueAsInt(symbol);
            var offset = VolumeOffset();
            int value = lastValue + (forward ? offset : -offset);
            if (value > 100 && wrap)
            {
                ChangeValue(symbol, 0);
            }
            else
            {
                ChangeValue(symbol, Mathf.Clamp(value, 0, 100));
            }
        }

        private int VolumeOffset() => 20;

        private void ChangeValue(string symbol, int value)
        {
            bool wasChanged = false;
            bool boolValue = value != 0;
            switch (symbol)
            {
                case "alwaysDash":
                    if (Rmmz.ConfigManager.AlwaysDash != boolValue)
                    {
                        Rmmz.ConfigManager.AlwaysDash = boolValue;
                        wasChanged = true;
                    }

                    break;

                case "commandRemember":
                    if (Rmmz.ConfigManager.CommandRemember != boolValue)
                    {
                        Rmmz.ConfigManager.CommandRemember = boolValue;
                        wasChanged = true;
                    }

                    break;

                case "touchUI":
                    if (Rmmz.ConfigManager.TouchUI != boolValue)
                    {
                        Rmmz.ConfigManager.TouchUI = boolValue;
                        wasChanged = true;
                    }

                    break;

                case "bgmVolume":
                    if (Rmmz.ConfigManager.BgmVolume != value)
                    {
                        Rmmz.ConfigManager.BgmVolume = value;
                        wasChanged = true;
                    }

                    break;

                case "bgsVolume":
                    if (Rmmz.ConfigManager.BgsVolume != value)
                    {
                        Rmmz.ConfigManager.BgsVolume = value;
                        wasChanged = true;
                    }

                    break;


                case "meVolume":
                    if (Rmmz.ConfigManager.MeVolume != value)
                    {
                        Rmmz.ConfigManager.MeVolume = value;
                        wasChanged = true;
                    }

                    break;

                case "seVolume":
                    if (Rmmz.ConfigManager.SeVolume != value)
                    {
                        Rmmz.ConfigManager.SeVolume = value;
                        wasChanged = true;
                    }

                    break;
            }

            if (wasChanged)
            {
                RedrawItem(FindSymbol(symbol));
                PlayCursorSound();
            }
        }

        private bool GetConfigValueAsBool(string symbol)
        {
            switch (symbol)
            {
                case "alwaysDash":
                    return Rmmz.ConfigManager.AlwaysDash;
                case "commandRemember":
                    return Rmmz.ConfigManager.CommandRemember;
                case "touchUI":
                    return Rmmz.ConfigManager.TouchUI;
                default:
                    return false;
            }
        }

        private int GetConfigValueAsInt(string symbol)
        {
            switch (symbol)
            {
                case "bgmVolume":
                    return Rmmz.ConfigManager.BgmVolume;
                case "bgsVolume":
                    return Rmmz.ConfigManager.BgsVolume;
                case "meVolume":
                    return Rmmz.ConfigManager.MeVolume;
                case "seVolume":
                    return Rmmz.ConfigManager.SeVolume;
                default:
                    return 0;
            }
        }
    }
}
