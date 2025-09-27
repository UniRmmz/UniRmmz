using System;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // BGMの演奏
        private bool Command241(object[] parameters)
        {
            var soundData = ConvertEx.ToSoundData(parameters[0]);
            Rmmz.AudioManager.PlayBgm(soundData);
            return true;
        }
        
        // BGMのフェードアウト
        protected virtual bool Command242(object[] parameters)
        {
            var duration = Convert.ToSingle(parameters[0]);
            Rmmz.AudioManager.FadeOutBgm(duration);
            return true;
        }
        
        // BGMの保存
        protected virtual bool Command243(object[] _)
        {
            Rmmz.gameSystem.SaveBgm();
            return true;
        }
        
        // BGMの再開
        protected virtual bool Command244(object[] _)
        {
            Rmmz.gameSystem.ReplayBgm();
            return true;
        }
        
        // BGSの演奏
        protected virtual bool Command245(object[] parameters)
        {
            var soundData = ConvertEx.ToSoundData(parameters[0]);
            Rmmz.AudioManager.PlayBgs(soundData);
            return true;
        }
        
        // BGSのフェードアウト
        protected virtual bool Command246(object[] parameters)
        {
            var duration = Convert.ToSingle(parameters[0]);
            Rmmz.AudioManager.FadeOutBgs(duration);
            return true;
        }
        
        // MEの演奏
        protected virtual bool Command249(object[] parameters)
        {
            var soundData = ConvertEx.ToSoundData(parameters[0]);
            Rmmz.AudioManager.PlayMe(soundData);
            return true;
        }
        
        // SEの演奏
        protected virtual bool Command250(object[] parameters)
        {
            var soundData = ConvertEx.ToSoundData(parameters[0]);
            Rmmz.AudioManager.PlaySe(soundData);
            return true;
        }
        
        // SEの停止
        protected virtual bool Command251(object[] parameters)
        {
            Rmmz.AudioManager.StopSe();
            return true;
        }
        
        // ムービー再生
        protected virtual bool Command261(object[] parameters)
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                return false;
            }

            var name = Convert.ToString(parameters[0]);
            if (name.Length > 0)
            {
                var ext = VideoFileExt();
                Video.Play("movies/" + name + ext);
                SetWaitMode("video");
            }
            return true;
        }
        
        protected virtual string VideoFileExt()
        {
            if (Utils.CanPlayWebm())
            {
                return ".webm";
            }
            else
            {
                return ".mp4";
            }
        }
    }
}