using UnityEngine;

namespace UniRmmz
{
    public partial class Scene_Gameover : Scene_Base
    {
        private Sprite _backSprite;
        
        public override void Create()
        {
            base.Create();
            PlayGameoverMusic();
            CreateBackground();
        }

        public override void StartScene()
        {
            base.StartScene();
            AdjustBackgrond();
            StartFadeIn(SlowFadeSpeed(), false);
        }

        public override void UpdateRmmz()
        {
            if (IsSceneActive() && !IsBusy() && IsTriggered())
            {
                GotoTitle();
            }
            base.UpdateRmmz();
        }

        public override void StopScene()
        {
            base.StopScene();
            FadeOutAll();
        }

        public override void Terminate()
        {
            base.Terminate();
            Rmmz.AudioManager.StopAll();
        }

        private void PlayGameoverMusic()
        {
            Rmmz.AudioManager.StopBgm();
            Rmmz.AudioManager.StopBgs();
            Rmmz.AudioManager.PlayMe(Rmmz.DataSystem.GameoverMe);
        }
        
        private void CreateBackground()
        {
            _backSprite = Sprite.Create("backSprite");
            _backSprite.Bitmap = Rmmz.ImageManager.LoadSystem("GameOver");
            this.AddChild(_backSprite);
        }

        private void AdjustBackgrond()
        {
            ScaleSprite(_backSprite);
            CenterSprite(_backSprite);
        }
        
        private bool IsTriggered()
        {
            return Input.IsTriggered("ok") || TouchInput.IsTriggered();
        }
        
        private void GotoTitle()
        {
            Scene_Title.Goto();
        }
    }
}