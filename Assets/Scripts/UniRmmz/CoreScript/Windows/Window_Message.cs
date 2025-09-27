using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying text messages.
    /// </summary>
    public partial class Window_Message : Window_Base
    {
        private int _background;
        private int _positionType;
        private int _waitCount;
        private Bitmap _faceBitmap;
        private TextState _textState;
        private Window_Gold _goldWindow;
        private Window_NameBox _nameBoxWindow;
        private Window_ChoiceList _choiceListWindow;
        private Window_NumberInput _numberInputWindow;
        private Window_EventItem _eventItemWindow;
        private bool _showFast;
        private bool _lineShowFast;
        private bool _pauseSkip;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            this.Openness = 0;
            InitMembers();
        }
        
        protected void InitMembers()
        {
            _background = 0;
            _positionType = 2;
            _waitCount = 0;
            _faceBitmap = null;
            _textState = null;
            _goldWindow = null;
            _nameBoxWindow = null;
            _choiceListWindow = null;
            _numberInputWindow = null;
            _eventItemWindow = null;
            ClearFlags();
        }

        public void SetGoldWindow(Window_Gold goldWindow)
        {
            _goldWindow = goldWindow;
        }

        public void SetNameBoxWindow(Window_NameBox nameBoxWindow)
        {
            _nameBoxWindow = nameBoxWindow;
        }
        
        public void SetChoiceListWindow(Window_ChoiceList choiceListWindow)
        {
            _choiceListWindow = choiceListWindow;
        }

        public void SetNumberInputWindow(Window_NumberInput numberInputWindow)
        {
            _numberInputWindow = numberInputWindow;
        }

        public void SetEventItemWindow(Window_EventItem eventItemWindow)
        {
            _eventItemWindow = eventItemWindow;
        }

        public void ClearFlags()
        {
            _showFast = false;
            _lineShowFast = false;
            _pauseSkip = false;
        }

        public override void UpdateRmmz()
        {
            CheckToNotClose();
            base.UpdateRmmz();
            SynchronizeNameBox();
            while (!IsOpening() && !IsClosing())
            {
                if (UpdateWait())
                {
                    return;
                }
                else if (UpdateLoading())
                {
                    return;
                }
                else if (UpdateInput())
                {
                    return;
                }
                else if (UpdateMessage())
                {
                    return;
                }
                else if (CanStart())
                {
                    StartMessage();
                }
                else
                {
                    StartInput();
                    return;
                }
            }
        }

        protected void CheckToNotClose()
        {
            if (IsOpen() && IsClosing() && DoesContinue())
            {
                Open();
            }
        }

        protected void SynchronizeNameBox()
        {
            _nameBoxWindow.Openness = this.Openness;
        }

        public bool CanStart()
        {
            return Rmmz.gameMessage.HasText() && !Rmmz.gameMessage.ScrollMode();
        }

        public void StartMessage()
        {
            string text = Rmmz.gameMessage.AllText();
            TextState textState = CreateTextState(text, 0, 0, 0);
            textState.x = NewLineX(textState);
            textState.startX = textState.x;
            _textState = textState;
            NewPage(_textState);
            UpdatePlacement();
            UpdateBackground();
            Open();
            _nameBoxWindow.StartRmmz();
        }

        protected int NewLineX(TextState textState)
        {
            bool faceExists = Rmmz.gameMessage.FaceName() != "";
            int faceWidth = Rmmz.ImageManager.FaceWidth;
            int spacing = 20;
            int margin = faceExists ? faceWidth + spacing : 4;
            return textState.rtl ? InnerWidth - margin : margin;
        }

        protected void UpdatePlacement()
        {
            _positionType = Rmmz.gameMessage.PositionType();
            this.Y = (_positionType * (Graphics.BoxHeight - this.Height)) / 2;
            if (_goldWindow != null)
            {
                _goldWindow.Y = this.Y > 0 ? 0 : Graphics.BoxHeight - _goldWindow.Height;
            }
        }

        protected void UpdateBackground()
        {
            _background = Rmmz.gameMessage.Background();
            SetBackgroundType(_background);
        }

        public void TerminateMessage()
        {
            Close();
            _goldWindow.Close();
            Rmmz.gameMessage.Clear();
        }

        protected bool UpdateWait()
        {
            if (_waitCount > 0)
            {
                _waitCount--;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool UpdateLoading()
        {
            if (_faceBitmap != null)
            {
                if (_faceBitmap.IsReady()) 
                {
                    DrawMessageFace();
                    _faceBitmap = null;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        protected bool UpdateInput()
        {
            if (IsAnySubWindowActive())
            {
                return true;
            }
            if (Pause)
            {
                if (IsTriggered())
                {
                    Input.Update();
                    Pause = false;
                    if (_textState == null)
                    {
                        TerminateMessage();
                    }
                }
                return true;
            }
            return false;
        }

        public bool IsAnySubWindowActive()
        {
            return (
                _choiceListWindow.Active ||
                _numberInputWindow.Active ||
                _eventItemWindow.Active
            );
        }

        protected bool UpdateMessage()
        {
            TextState textState = _textState;
            if (textState != null)
            {
                while (!IsEndOfText(textState))
                {
                    if (NeedsNewPage(textState))
                    {
                        NewPage(textState);
                    }
                    UpdateShowFast();
                    ProcessCharacter(textState);
                    if (ShouldBreakHere(textState))
                    {
                        break;
                    }
                }
                FlushTextState(textState);
                if (IsEndOfText(textState) && !IsWaiting())
                {
                    OnEndOfText();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool ShouldBreakHere(TextState textState)
        {
            if (CanBreakHere(textState))
            {
                if (!_showFast && !_lineShowFast)
                {
                    return true;
                }
                if (IsWaiting())
                {
                    return true;
                }
            }
            return false;
        }

        protected bool CanBreakHere(TextState textState)
        {
            if (!IsEndOfText(textState))
            {
                char c = textState.text[textState.index];
                if (c >= 0xdc00 && c <= 0xdfff)
                {
                    // サロゲートペア
                    return false;
                }
                if (textState.rtl && c > 0x20)
                {
                    return false;
                }
            }
            return true;
        }

        protected void OnEndOfText()
        {
            if (!StartInput())
            {
                if (!_pauseSkip)
                {
                    StartPause();
                }
                else
                {
                    TerminateMessage();
                }
            }
            _textState = null;
        }

        protected bool StartInput()
        {
            if (Rmmz.gameMessage.IsChoice())
            {
                _choiceListWindow.StartRmmz();
                return true;
            }
            else if (Rmmz.gameMessage.IsNumberInput())
            {
                _numberInputWindow.StartRmmz();
                return true;
            }
            else if (Rmmz.gameMessage.IsItemChoice())
            {
                _eventItemWindow.StartRmmz();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool IsTriggered()
        {
            return (
                Input.IsRepeated("ok") ||
                Input.IsRepeated("cancel") ||
                TouchInput.IsRepeated()
            );
        }

        protected bool DoesContinue()
        {
            return (
                Rmmz.gameMessage.HasText() &&
                !Rmmz.gameMessage.ScrollMode() &&
                !AreSettingsChanged()
            );
        }

        protected bool AreSettingsChanged()
        {
            return (
                _background != Rmmz.gameMessage.Background() ||
                _positionType != Rmmz.gameMessage.PositionType()
            );
        }

        protected void UpdateShowFast()
        {
            if (IsTriggered())
            {
                _showFast = true;
            }
        }

        protected void NewPage(TextState textState)
        {
            Contents.Clear();
            ResetFontSettings();
            ClearFlags();
            UpdateSpeakerName();
            LoadMessageFace();
            textState.x = textState.startX;
            textState.y = 0;
            textState.height = CalcTextHeight(textState);
        }

        protected void UpdateSpeakerName()
        {
            _nameBoxWindow.SetName(Rmmz.gameMessage.SpeakerName());
        }

        protected void LoadMessageFace()
        {
            _faceBitmap = Rmmz.ImageManager.LoadFace(Rmmz.gameMessage.FaceName());
        }

        protected void DrawMessageFace()
        {
            string faceName = Rmmz.gameMessage.FaceName();
            int faceIndex = Rmmz.gameMessage.FaceIndex();
            bool rtl = Rmmz.gameMessage.IsRTL();
            int width = Rmmz.ImageManager.FaceWidth;
            int height = InnerHeight;
            int x = rtl ? InnerWidth - width - 4 : 4;
            DrawFace(faceName, faceIndex, x, 0, width, height);
        }

        protected override void ProcessControlCharacter(TextState textState, char c)
        {
            base.ProcessControlCharacter(textState, c);
            if (c == '\f')
            {
                ProcessNewPage(textState);
            }
        }

        protected override void ProcessNewLine(TextState textState)
        {
            _lineShowFast = false;
            base.ProcessNewLine(textState);
            if (NeedsNewPage(textState))
            {
                StartPause();
            }
        }

        protected void ProcessNewPage(TextState textState)
        {
            if (textState.text[textState.index] == '\n')
            {
                textState.index++;
            }
            textState.y = Contents.Height;
            StartPause();
        }

        protected bool IsEndOfText(TextState textState)
        {
            return textState.index >= textState.text.Length;
        }

        protected bool NeedsNewPage(TextState textState)
        {
            return (
                !IsEndOfText(textState) &&
                textState.y + textState.height > Contents.Height
            );
        }

        protected override void ProcessEscapeCharacter(string code, TextState textState)
        {
            switch (code)
            {
                case "$":
                    _goldWindow.Open();
                    break;
                case ".":
                    StartWait(15);
                    break;
                case "|":
                    StartWait(60);
                    break;
                case "!":
                    StartPause();
                    break;
                case ">":
                    _lineShowFast = true;
                    break;
                case "<":
                    _lineShowFast = false;
                    break;
                case "^":
                    _pauseSkip = true;
                    break;
                default:
                    base.ProcessEscapeCharacter(code, textState);
                    break;
            }
        }

        protected void StartWait(int count)
        {
            _waitCount = count;
        }

        protected void StartPause()
        {
            StartWait(10);
            Pause = true;
        }

        public bool IsWaiting()
        {
            return Pause || _waitCount > 0;
        }
    }
}