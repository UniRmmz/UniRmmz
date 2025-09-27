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
        private bool Command242(object[] parameters)
        {
            var duration = Convert.ToSingle(parameters[0]);
            Rmmz.AudioManager.FadeOutBgm(duration);
            return true;
        }
        
        // BGMの保存
        private bool Command243(object[] _)
        {
            Rmmz.gameSystem.SaveBgm();
            return true;
        }
        
        // BGMの再開
        private bool Command244(object[] _)
        {
            Rmmz.gameSystem.ReplayBgm();
            return true;
        }
        
        // BGSの演奏
        private bool Command245(object[] parameters)
        {
            var soundData = ConvertEx.ToSoundData(parameters[0]);
            Rmmz.AudioManager.PlayBgs(soundData);
            return true;
        }
        
        // BGSのフェードアウト
        private bool Command246(object[] parameters)
        {
            var duration = Convert.ToSingle(parameters[0]);
            Rmmz.AudioManager.FadeOutBgs(duration);
            return true;
        }
        
        // MEの演奏
        private bool Command249(object[] parameters)
        {
            var soundData = ConvertEx.ToSoundData(parameters[0]);
            Rmmz.AudioManager.PlayMe(soundData);
            return true;
        }
        
        // SEの演奏
        private bool Command250(object[] parameters)
        {
            var soundData = ConvertEx.ToSoundData(parameters[0]);
            Rmmz.AudioManager.PlaySe(soundData);
            return true;
        }
        
        // SEの停止
        private bool Command251(object[] parameters)
        {
            Rmmz.AudioManager.StopSe();
            return true;
        }
        
        // ムービー再生
        private bool Command261(object[] parameters)
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
        
        private string VideoFileExt()
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