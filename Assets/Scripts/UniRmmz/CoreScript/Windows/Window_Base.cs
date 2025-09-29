using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace UniRmmz
{
    public partial class Window_Base //: Window
    {
        protected bool _opening = false;
        protected bool _closing = false;
        protected Sprite _dimmerSprite;

        public static class Prototype
        {
            public static int LineHeight => 36;
        
            public static int FittingHeight(int numLines)
            {
                return numLines * LineHeight + Rmmz.gameSystem.WindowPadding() * 2;
            }
        }
        
        public class TextState
        {
            public string text;
            public int index;
            public int x;
            public int y;
            public int width;
            public int height;
            public int startX;
            public int startY;
            public bool rtl;
            public string buffer;
            public bool drawing;
            public float outputWidth;
            public float outputHeight;
        }

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            LoadWindowskin();
            Move(rect.x, rect.y, rect.width, rect.height);
            UpdatePadding();
            UpdateBackOpacity();
            UpdateTone();
            CreateContents();
            _opening = false;
            _closing = false;
            _dimmerSprite = null;
        }
        protected override void OnDestroy()
        {
            DestroyContents();
            if (_dimmerSprite != null)
            {
                _dimmerSprite.Bitmap.Dispose();
            }
            base.OnDestroy();
        }

        public virtual int LineHeight() => Prototype.LineHeight;
        public virtual int ItemWidth() => InnerWidth;
        public virtual int ItemHeight() => LineHeight(); 
        public virtual int ItemPadding() => 8;

        public virtual Rect BaseTextRect()
        {
            var rect = new Rect(0, 0, InnerWidth, InnerHeight);
            rect.Pad(-ItemPadding(), 0);
            return rect;
        }

        protected virtual void LoadWindowskin()
        {
            WindowSkin = Rmmz.ImageManager.LoadSystem("Window");
        }

        protected virtual void UpdatePadding()
        {
            Padding = Rmmz.gameSystem.WindowPadding();
        }
        
        protected virtual void UpdateBackOpacity()
        {
            BackOpacity = Rmmz.gameSystem.WindowOpacity();
        }
        
        public virtual int FittingHeight(int numLines)
        {
            return numLines * ItemHeight() + Rmmz.gameSystem.WindowPadding() * 2;
        }

        protected virtual void UpdateTone()
        {
            var tone = Rmmz.gameSystem.WindowTone();
            SetTone(tone[0], tone[1], tone[2]);
        }
        
        public virtual void CreateContents()
        {
            var width = ContentsWidth();
            var height = ContentsHeight();
            DestroyContents();
            Contents = new Bitmap(width, height);
            ContentsBack = new Bitmap(width, height);
            ResetFontSettings();
        }

        protected virtual void DestroyContents()
        {
            Contents?.Dispose();
            ContentsBack?.Dispose();
        }
        
        protected virtual int ContentsWidth() => InnerWidth;
        protected virtual int ContentsHeight() => InnerHeight;
        
        protected virtual void ResetFontSettings()
        {
            Contents.FontFace = Rmmz.gameSystem.MainFontFace();
            Contents.FontSize = Rmmz.gameSystem.MainFontSize();
            ResetTextColor();
        }
        protected virtual void ResetTextColor()
        {
            ChangeTextColor(Rmmz.ColorManager.NormalColor());
            ChangeOutlineColor(Rmmz.ColorManager.OutlineColor());
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateTone();
            UpdateOpen();
            UpdateClose();
            UpdateBackgroundDimmer();
        }

        protected virtual void UpdateOpen()
        {
            if (_opening)
            {
                Openness += 32;
                if (IsOpen())
                {
                    _opening = false;
                }
            }
        }

        protected virtual void UpdateClose()
        {
            if (_closing)
            {
                Openness -= 32;
                if (IsClosed())
                {
                    _closing = false;
                }
            }
        }

        public virtual void Open()
        {
            if (!IsOpen())
            {
                _opening = true;
            }
            _closing = false;
        }

        public virtual void Close()
        {
            if (!IsClosed())
            {
                _closing = true;
            }
            _opening = false;
        }
        
        public bool IsOpening() => _opening;
        public bool IsClosing() => _closing;

        public virtual void Show()
        {
            Visible = true;
        }
        
        public virtual void Hide()
        {
            Visible = false;
        }
        
        public virtual void Activate()
        {
            Active = true;
        }
        
        public virtual void Deactivate()
        {
            Active = false;
        }

        public Color SystemColor => Rmmz.ColorManager.SystemColor();
        
        public int TranslucentOpacity => 160;
        
        public virtual void ChangeTextColor(Color color)
        {
            Contents.TextColor = color;
        }
        
        public virtual void ChangeOutlineColor(Color color)
        {
            Contents.OutlineColor = color;
        }
        
        public virtual void ChangePaintOpacity(bool enable)
        {
            Contents.PaintOpacity = enable ? 255 : TranslucentOpacity;
        }

        protected virtual void DrawRect(int x, int y, int width, int height)
        {
            var outlineColor = Contents.OutlineColor;
            var mainColor = Contents.TextColor;
            Contents.FillRect(x, y, width, height, outlineColor);
            Contents.FillRect(x + 1, y + 1, width - 2, height - 2, mainColor);
        }

        protected virtual void DrawText(string text, int x, int y, int maxWidth = 0, Bitmap.TextAlign align = Bitmap.TextAlign.Left)
        {
            Contents.DrawText(text, x, y, maxWidth, LineHeight(), align);
        }

        protected virtual int TextWidth(string text)
        {
            return Contents.MeasureTextWidth(text);
        }
        
        public virtual float DrawTextEx(string text, int x, int y, int width)
        {
            ResetFontSettings();
            var textState = CreateTextState(text, x, y, width);
            ProcessAllText(textState);
            return textState.outputWidth;
        }
        
        public virtual Vector2 TextSizeEx(string text)
        {
            ResetFontSettings();
            var textState = CreateTextState(text, 0, 0, 0);
            textState.drawing = false;
            ProcessAllText(textState);
            return new Vector2(textState.outputWidth, textState.outputHeight);
        }
        
        protected virtual TextState CreateTextState(string text, int x, int y, int width)
        {
            bool rtl = Utils.ContainsArabic(text);
            TextState textState = new TextState();
            textState.text = ConvertEscapeCharacters(text);
            textState.index = 0;
            textState.x = rtl ? x + width : x;
            textState.y = y;
            textState.width = width;
            textState.height = CalcTextHeight(textState);
            textState.startX = textState.x;
            textState.startY = textState.y;
            textState.rtl = rtl;
            textState.buffer = CreateTextBuffer(rtl);
            textState.drawing = true;
            textState.outputWidth = 0;
            textState.outputHeight = 0;
            return textState;
        }
        protected virtual void ProcessAllText(TextState textState)
        {
            while (textState.index < textState.text.Length)
            {
                ProcessCharacter(textState);
            }
            FlushTextState(textState);
        }

        protected virtual void FlushTextState(TextState textState)
        {
            var text = textState.buffer;
            bool rtl = textState.rtl;
            int width = TextWidth(text);
            int height = textState.height;
            int x = rtl ? textState.x - width : textState.x;
            int y = textState.y;
            
            if (textState.drawing)
            {
                Contents.DrawText(text, x, y, width, height);
            }
            
            textState.x += rtl ? -width : width;
            textState.buffer = CreateTextBuffer(rtl);
            float outputWidth = Math.Abs(textState.x - textState.startX);
            
            if (textState.outputWidth < outputWidth)
            {
                textState.outputWidth = outputWidth;
            }
            
            textState.outputHeight = y - textState.startY + height;
        }

        protected virtual string CreateTextBuffer(bool rtl)
        {
            // U+202B: RIGHT-TO-LEFT EMBEDDING
            return rtl ? "\u202B" : "";
        }

        protected virtual string ConvertEscapeCharacters(string text)
        {
            // バックスラッシュをエスケープコードに変換
            text = text.Replace("\\", "\x1b");
            text = text.Replace("\x1b\x1b", "\\");
            
            // 変数の置換
            Regex varRegex = new Regex("\x1bV\\[(\\d+)\\]", RegexOptions.IgnoreCase);
            while (varRegex.IsMatch(text))
            {
                text = varRegex.Replace(text, match =>
                {
                    int varId = int.Parse(match.Groups[1].Value);
                    return Rmmz.gameVariables.Value(varId).ToString();
                });
            }
            
            // アクター名の置換
            text = Regex.Replace(text, "\x1bN\\[(\\d+)\\]", match =>
            {
                int actorId = int.Parse(match.Groups[1].Value);
                return ActorName(actorId);
            }, RegexOptions.IgnoreCase);
            
            // パーティメンバー名の置換
            text = Regex.Replace(text, "\x1bP\\[(\\d+)\\]", match =>
            {
                int memberId = int.Parse(match.Groups[1].Value);
                return PartyMemberName(memberId);
            }, RegexOptions.IgnoreCase);
            
            // 通貨単位の置換
            text = Regex.Replace(text, "\x1bG", Rmmz.TextManager.CurrencyUnit, RegexOptions.IgnoreCase);
            
            return text;
        }

        protected virtual string ActorName(int n)
        {
            var actor = n >= 1 ? Rmmz.gameActors.Actor(n) : null;
            return actor != null ? actor.Name() : "";
        }

        protected virtual  string PartyMemberName(int n)
        {
            var actor = n >= 1 ? Rmmz.gameParty.Members().ElementAt(n - 1) : null;
            return actor != null ? actor.Name() : "";
        }

        protected virtual  void ProcessCharacter(TextState textState)
        {
            char c = textState.text[textState.index++];
            if (c < 0x20) // コントロールキャラクター
            {
                FlushTextState(textState);
                ProcessControlCharacter(textState, c);
            }
            else
            {
                textState.buffer += c;
            }
        }

        protected virtual void ProcessControlCharacter(TextState textState, char c)
        {
            if (c == '\n')
            {
                ProcessNewLine(textState);
            }
            if (c == '\x1b') // エスケープコード
            {
                string code = ObtainEscapeCode(textState);
                ProcessEscapeCharacter(code, textState);
            }
        }

        protected virtual void ProcessNewLine(TextState textState)
        {
            textState.x = textState.startX;
            textState.y += textState.height;
            textState.height = CalcTextHeight(textState);
        }

        protected virtual  string ObtainEscapeCode(TextState textState)
        {
            Regex regExp = new Regex("^[$.|^!><{}\\\\]|^[A-Z]+", RegexOptions.IgnoreCase);
            Match match = regExp.Match(textState.text.Substring(textState.index));
            
            if (match.Success)
            {
                textState.index += match.Value.Length;
                return match.Value.ToUpper();
            }
            else
            {
                return "";
            }
        }

        protected virtual  string ObtainEscapeParam(TextState textState)
        {
            Regex regExp = new Regex("^\\[(\\d+)\\]");
            Match match = regExp.Match(textState.text.Substring(textState.index));
            
            if (match.Success)
            {
                textState.index += match.Value.Length;
                return match.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        protected virtual void ProcessEscapeCharacter(string code, TextState textState)
        {
            switch (code)
            {
                case "C":
                    int colorIndex = int.Parse(ObtainEscapeParam(textState));
                    ProcessColorChange(colorIndex);
                    break;
                case "I":
                    int iconIndex = int.Parse(ObtainEscapeParam(textState));
                    ProcessDrawIcon(iconIndex, textState);
                    break;
                case "PX":
                    textState.x = int.Parse(ObtainEscapeParam(textState));
                    break;
                case "PY":
                    textState.y = int.Parse(ObtainEscapeParam(textState));
                    break;
                case "FS":
                    Contents.FontSize = int.Parse(ObtainEscapeParam(textState));
                    break;
                case "{":
                    MakeFontBigger();
                    break;
                case "}":
                    MakeFontSmaller();
                    break;
            }
        }

        protected virtual void ProcessColorChange(int colorIndex)
        {
            ChangeTextColor(Rmmz.ColorManager.TextColor(colorIndex));
        }

        protected virtual void ProcessDrawIcon(int iconIndex, TextState textState)
        {
            if (textState.drawing)
            {
                DrawIcon(iconIndex, textState.x + 2, textState.y + 2);
            }
            textState.x += Rmmz.ImageManager.IconWidth + 4;
        }

        protected virtual void MakeFontBigger()
        {
            if (Contents.FontSize <= 96)
            {
                Contents.FontSize += 12;
            }
        }

        protected virtual void MakeFontSmaller()
        {
            if (Contents.FontSize >= 24)
            {
                Contents.FontSize -= 12;
            }
        }

        protected virtual int CalcTextHeight(TextState textState)
        {
            int lineSpacing = LineHeight() - Rmmz.gameSystem.MainFontSize();
            int lastFontSize = Contents.FontSize;
            string[] lines = textState.text.Substring(textState.index).Split('\n');
            int textHeight = MaxFontSizeInLine(lines[0]) + lineSpacing;
            Contents.FontSize = lastFontSize;
            return textHeight;
        }

        protected virtual int MaxFontSizeInLine(string line)
        {
            int maxFontSize = Contents.FontSize;
            Regex regExp = new Regex("\\x1b({|}|FS)(\\[(\\d+)\\])?", RegexOptions.IgnoreCase);
            
            MatchCollection matches = regExp.Matches(line);
            foreach (Match match in matches)
            {
                string code = match.Groups[1].Value.ToUpper();
                if (code == "{")
                {
                    MakeFontBigger();
                }
                else if (code == "}")
                {
                    MakeFontSmaller();
                }
                else if (code == "FS" && match.Groups[3].Success)
                {
                    Contents.FontSize = int.Parse(match.Groups[3].Value);
                }
                
                if (Contents.FontSize > maxFontSize)
                {
                    maxFontSize = Contents.FontSize;
                }
            }
            
            return maxFontSize;
        }

        protected virtual void DrawIcon(int iconIndex, int x, int y)
        {
            var bitmap = Rmmz.ImageManager.LoadSystem("IconSet");
            int pw = Rmmz.ImageManager.IconWidth;
            int ph = Rmmz.ImageManager.IconHeight;
            int sx = (iconIndex % 16) * pw;
            int sy = (iconIndex / 16) * ph;
            Contents.Blt(bitmap, sx, sy, pw, ph, x, y);
        }

        protected virtual void DrawFace(string faceName, int faceIndex, int x, int y, int width = 0, int height = 0)
        {
            width = width > 0 ? width : Rmmz.ImageManager.FaceWidth;
            height = height > 0 ? height : Rmmz.ImageManager.FaceHeight;
            
            var bitmap = Rmmz.ImageManager.LoadFace(faceName);
            int pw = Rmmz.ImageManager.FaceWidth;
            int ph = Rmmz.ImageManager.FaceHeight;
            int sw = Mathf.Min((int)width, pw);
            int sh = Mathf.Min((int)height, ph);
            int dx = Mathf.FloorToInt(x + Math.Max(width - pw, 0) / 2);
            int dy = Mathf.FloorToInt(y + Math.Max(height - ph, 0) / 2);
            int sx = Mathf.FloorToInt((faceIndex % 4) * pw + (pw - sw) / 2);
            int sy = Mathf.FloorToInt((faceIndex / 4) * ph + (ph - sh) / 2);
            
            Contents.Blt(bitmap, sx, sy, sw, sh, dx, dy);
        }

        protected virtual void DrawCharacter(string characterName, int characterIndex, int x, int y)
        {
            var bitmap = Rmmz.ImageManager.LoadCharacter(characterName);
            bool big = Rmmz.ImageManager.IsBigCharacter(characterName);
            int pw = bitmap.Width / (big ? 3 : 12);
            int ph = bitmap.Height / (big ? 4 : 8);
            int n = big ? 0 : characterIndex;
            int sx = ((n % 4) * 3 + 1) * pw;
            int sy = (n / 4) * 4 * ph;
            
            Contents.Blt(bitmap, sx, sy, pw, ph, x - pw / 2, y - ph);
        }

        protected virtual void DrawItemName(DataCommonItem item, int x, int y, int width)
        {
            if (item != null)
            {
                int iconY = y + (LineHeight() - Rmmz.ImageManager.IconHeight) / 2;
                int textMargin = Rmmz.ImageManager.IconWidth + 4;
                int itemWidth = Mathf.Max(0, width - textMargin);
                
                ResetTextColor();
                DrawIcon(item.IconIndex, x, iconY);
                DrawText(item.Name, x + textMargin, y, itemWidth);
            }
        }

        protected virtual void DrawCurrencyValue(int value, string unit, int x, int y, int width)
        {
            int unitWidth = Mathf.Min(80, TextWidth(unit));
            
            ResetTextColor();
            DrawText(value.ToString(), x, y, width - unitWidth - 6, Bitmap.TextAlign.Right);
            ChangeTextColor(Rmmz.ColorManager.SystemColor());
            DrawText(unit, x + width - unitWidth, y, unitWidth, Bitmap.TextAlign.Right);
        }
    
        public virtual void SetBackgroundType(int type)
        {
            if (type == 0)
            {
                Opacity = 255;
            }
            else
            {
                Opacity = 0;
            }

            if (type == 1)
            {
                ShowBackgroundDimmer();
            }
            else
            {
                HideBackgroundDimmer();
            }
        }

        protected virtual void ShowBackgroundDimmer()
        {
            if (_dimmerSprite == null)
            {
                CreateDimmerSprite();
            }
            var bitmap = _dimmerSprite.Bitmap;
            if (bitmap.Width != Width || bitmap.Height != Height)
            {
                RefreshDimmerBitmap();
            }
            _dimmerSprite.Visible = true;
            UpdateBackgroundDimmer();
        }

        protected virtual void CreateDimmerSprite()
        {
            _dimmerSprite = Sprite.Create("Dimmer");
            _dimmerSprite.Bitmap = new Bitmap(0, 0);
            _dimmerSprite.X = -4;
            AddChildToBack(_dimmerSprite);
        }

        protected virtual void HideBackgroundDimmer()
        {
            if (_dimmerSprite != null)
            {
                _dimmerSprite.Visible = false;    
            }
        }

        protected virtual void UpdateBackgroundDimmer()
        {
            if (_dimmerSprite != null)
            {
                _dimmerSprite.Opacity = Openness;
            }
        }
        
        protected virtual void RefreshDimmerBitmap()
        {
            if (_dimmerSprite != null)
            {
                var bitmap = _dimmerSprite.Bitmap;
                int w = Width > 0 ? (int)Width + 8 : 0;
                int h = (int)Height;
                int m = Padding;
                var c1 = Rmmz.ColorManager.DimColor1();
                var c2 = Rmmz.ColorManager.DimColor2();
                bitmap.Resize(w, h);
                bitmap.GradientFillRect(0, 0, w, m, c2, c1, true);
                bitmap.FillRect(0, m, w, h - m * 2, c1);
                bitmap.GradientFillRect(0, h - m, w, m, c1, c2, true);
                _dimmerSprite.SetFrame(0, 0, w, h);
            }
        }

        protected virtual void PlayCursorSound()
        {
            Rmmz.SoundManager.PlayCursor();
        }
        
        protected virtual void PlayOkSound()
        {
            Rmmz.SoundManager.PlayOk();
        }
        
        protected virtual void PlayBuzzerSound()
        {
            Rmmz.SoundManager.PlayBuzzer();
        }
        
        
    }
}