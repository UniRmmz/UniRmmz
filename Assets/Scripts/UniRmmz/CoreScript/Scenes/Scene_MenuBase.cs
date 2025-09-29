using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of all the menu-type scenes.
    /// </summary>
    public partial class Scene_MenuBase //: Scene_Base
    {
        protected Window_Help _helpWindow;
        protected Sprite _backgroundSprite;
        protected Sprite_Button _cancelButton;
        protected Sprite_Button _pageupButton;
        protected Sprite_Button _pagedownButton;
        protected Game_Actor _actor;
        private IRmmzFilter _backgroundFilter;

        public override void Create()
        {
            base.Create();
            CreateBackground();
            UpdateActor();
            CreateWindowLayer();
            CreateButtons();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdatePageButtons();
        }

        protected virtual float HelpAreaTop()
        {
            if (IsBottomHelpMode())
            {
                return MainAreaBottom();
            }
            else if (IsBottomButtonMode())
            {
                return 0;
            }
            else
            {
                return ButtonAreaBottom();
            }
        }

        protected virtual float HelpAreaBottom() => HelpAreaTop() + HelpAreaHeight();

        protected virtual float HelpAreaHeight() => CalcWindowHeight(2, false);

        protected virtual float MainAreaTop()
        {
            if (!IsBottomHelpMode())
            {
                return HelpAreaBottom();
            }
            else if (IsBottomButtonMode())
            {
                return 0;
            }
            else
            {
                return ButtonAreaBottom();
            }
        }

        protected virtual float MainAreaBottom() => MainAreaTop() + MainAreaHeight();

        protected virtual float MainAreaHeight() => Graphics.BoxHeight - ButtonAreaHeight() - HelpAreaHeight();

        protected virtual Game_Actor Actor() => _actor;

        protected virtual void UpdateActor()
        {
            _actor = Rmmz.gameParty.MenuActor();
        }

        protected virtual void CreateBackground()
        {
            _backgroundFilter = new BlurFilter();
            _backgroundSprite = Sprite.Create("background");
            _backgroundSprite.Bitmap = Rmmz.SceneManager.BackgroundBitmap();
            _backgroundSprite.AddFilter(_backgroundFilter);
            this.AddChild(_backgroundSprite);
            SetBackgroundOpacity(192);
        }

        protected virtual void SetBackgroundOpacity(int opacity)
        {
            _backgroundSprite.Opacity = opacity;
        }

        protected virtual void CreateHelpWindow()
        {
            var rect = this.HelpWindowRect();
            this._helpWindow = Window_Help.Create(rect, "help");
            this.AddWindow(this._helpWindow);
        }

        protected virtual Rect HelpWindowRect()
        {
            float wx = 0;
            float wy = HelpAreaTop();
            float ww = Graphics.BoxWidth;
            float wh = HelpAreaHeight();
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateButtons()
        {
            if (Rmmz.ConfigManager.TouchUI)
            {
                if (NeedsCancelButton())
                {
                    CreateCancelButton();
                }
                if (NeedsPageButtons())
                {
                    CreatePageButtons();
                }
            }
        }

        protected virtual bool NeedsCancelButton() => true;

        protected virtual void CreateCancelButton()
        {
            _cancelButton = Sprite_Button.Create("cancel");
            _cancelButton.Initialize(Input.ButtonTypes.Cancel);
            _cancelButton.X = Graphics.BoxWidth - _cancelButton.Width - 4;
            _cancelButton.Y = ButtonY();
            AddWindow(_cancelButton);
        }

        protected virtual bool NeedsPageButtons() => false;

        protected virtual void CreatePageButtons()
        {
            _pageupButton = Sprite_Button.Create("pageup");
            _pageupButton.Initialize(Input.ButtonTypes.PageUp);
            _pageupButton.X = 4;
            _pageupButton.Y = ButtonY();
            var pageupRight = _pageupButton.X + _pageupButton.Width;
            
            _pagedownButton = Sprite_Button.Create("pagedown");
            _pagedownButton.Initialize(Input.ButtonTypes.PageDown);
            _pagedownButton.X = pageupRight + 4;
            _pagedownButton.Y = ButtonY();
            
            AddWindow(_pageupButton);
            AddWindow(_pagedownButton);
            
            _pageupButton.SetClickHandler(PreviousActor);
            _pagedownButton.SetClickHandler(NextActor);
        }

        protected virtual void UpdatePageButtons()
        {
            if (_pageupButton != null && _pagedownButton != null)
            {
                bool enabled = ArePageButtonsEnabled();
                _pageupButton.Visible = enabled;
                _pagedownButton.Visible = enabled;
            }
        }

        protected virtual bool ArePageButtonsEnabled() => true;

        protected virtual void NextActor()
        {
            Rmmz.gameParty.MakeMenuActorNext();
            UpdateActor();
            OnActorChange();
        }

        protected virtual void PreviousActor()
        {
            Rmmz.gameParty.MakeMenuActorPrevious();
            UpdateActor();
            OnActorChange();
        }

        protected virtual void OnActorChange()
        {
            Rmmz.SoundManager.PlayCursor();
        }
    }
}