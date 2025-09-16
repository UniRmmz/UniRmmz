using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The static class that handles BGM, BGS, ME and SE.
    /// </summary>
    public partial class AudioManager
    {
        private int _bgmVolume = 100;
        private int _bgsVolume = 100;
        private int _meVolume = 100;
        private int _seVolume = 100;
        private RmmzWebAudio _bgmBuffer;
        private RmmzWebAudio _bgsBuffer;
        private RmmzWebAudio _meBuffer;
        private List<RmmzWebAudio> _seBuffers = new();
        private List<RmmzWebAudio> _staticBuffers = new();
        private float _replayFadeTime = 0.5f;
        private string _path = "audio/";

        private Sound _currentBgm; 
        private Sound _currentBgs;
        private Sound _currentMe;
        
        public class Sound
        {
            public string Name;
            public int Pan;
            public int Pitch;
            public int Volume;
            public int Pos;
            public static implicit operator Sound(DataSystem.DataSound dataSound)
            {
                return new Sound { Name = dataSound.Name, Pan = dataSound.Pan, Pitch = dataSound.Pitch, Volume = dataSound.Volume, Pos = 0 };
            }
        }

        public int BgmVolume
        {
            get => _bgmVolume;
            set
            {
                _bgmVolume = value;
                UpdateBgmParameters(_currentBgm);
            }
        }
        
        public int BgsVolume
        {
            get => _bgsVolume;
            set
            {
                _bgsVolume = value;
                UpdateBgsParameters(_currentBgs);
            }
        }
        
        public int MeVolume
        {
            get => _meVolume;
            set
            {
                _meVolume = value;
                UpdateMeParameters(_currentMe);
            }
        }
        
        public int SeVolume
        {
            get => _seVolume;
            set
            {
                _seVolume = value;
            }
        }

        public void PlayBgm(Sound bgm, int pos = 0)
        {
            if (IsCurrentBgm(bgm))
            {
                UpdateBgmParameters(bgm);
            }
            else
            {
                StopBgm();
                if (!string.IsNullOrEmpty(bgm.Name)) 
                {
                    _bgmBuffer = CreateBuffer("bgm/", bgm.Name);
                    UpdateBgmParameters(bgm);
                    if (_meBuffer == null)
                    {
                        _bgmBuffer.Play(true, pos);
                    }
                }
            }
            UpdateCurrentBgm(bgm, pos);
        }
        
        public void ReplayBgm(Sound bgm)
        {
            if (IsCurrentBgm(bgm))
            {
                UpdateBgmParameters(bgm);
            }
            else
            {
                PlayBgm(bgm, bgm.Pos);
                if (_bgmBuffer != null)
                {
                    _bgmBuffer.FadeIn(_replayFadeTime);
                }
            }
        }
        
        public bool IsCurrentBgm(Sound bgm)
        {
            return _currentBgm != null &&
                   _bgmBuffer != null &&
                   _currentBgm.Name == bgm.Name;
        }
        
        public void UpdateBgmParameters(Sound bgm)
        {
            UpdateBufferParameters(_bgmBuffer, _bgmVolume, bgm);
        }
        
        public void UpdateCurrentBgm(Sound bgm, int pos)
        {
            _currentBgm = new Sound()
            {
                Name = bgm.Name,
                Volume = bgm.Volume,
                Pitch = bgm.Pitch,
                Pan = bgm.Pan,
                Pos = pos
            };
        }
        
        public void StopBgm()
        {
            if (_bgmBuffer != null)
            {
                _bgmBuffer.Dispose();
                _bgmBuffer = null;
                _currentBgm = null;
            }
        }

        public void FadeOutBgm(float duration)
        {
            if (_bgmBuffer != null && _currentBgm != null)
            {
                _bgmBuffer.FadeOut(duration);
                _currentBgm = null;
            }
        }
        
        public void FadeInBgm(float duration)
        {
            if (_bgmBuffer != null && _currentBgm != null)
            {
                _bgmBuffer.FadeIn(duration);
            }
        }
        
        public void PlayBgs(Sound bgs, int pos = 0)
        {
            if (IsCurrentBgs(bgs))
            {
                UpdateBgsParameters(bgs);
            }
            else
            {
                StopBgs();
                if (!string.IsNullOrEmpty(bgs.Name)) 
                {
                    _bgsBuffer = CreateBuffer("bgs/", bgs.Name);
                    UpdateBgsParameters(bgs);
                    _bgsBuffer.Play(true, pos);
                }
            }
            UpdateCurrentBgs(bgs, pos);
        }
        
        public void ReplayBgs(Sound bgs)
        {
            if (IsCurrentBgs(bgs))
            {
                UpdateBgsParameters(bgs);
            }
            else
            {
                PlayBgs(bgs, bgs.Pos);
                if (_bgsBuffer != null)
                {
                    _bgsBuffer.FadeIn(_replayFadeTime);
                }
            }
        }
        
        public bool IsCurrentBgs(Sound bgs)
        {
            return _currentBgs != null &&
                   _bgsBuffer != null &&
                   _currentBgs.Name == bgs.Name;
        }
        
        public void UpdateBgsParameters(Sound bgs)
        {
            UpdateBufferParameters(_bgsBuffer, _bgsVolume, bgs);
        }
        
        public void UpdateCurrentBgs(Sound bgs, int pos)
        {
            _currentBgs = new Sound()
            {
                Name = bgs.Name,
                Volume = bgs.Volume,
                Pitch = bgs.Pitch,
                Pan = bgs.Pan,
                Pos = pos
            };
        }
        
        public void StopBgs()
        {
            if (_bgsBuffer != null)
            {
                _bgsBuffer.Dispose();
                _bgsBuffer = null;
                _currentBgs = null;
            }
        }

        public void FadeOutBgs(float duration)
        {
            if (_bgsBuffer != null && _currentBgs != null)
            {
                _bgsBuffer.FadeOut(duration);
                _currentBgs = null;
            }
        }
        
        public void FadeInBgs(float duration)
        {
            if (_bgsBuffer != null && _currentBgs != null)
            {
                _bgsBuffer.FadeIn(duration);
            }
        }
        
        public void PlayMe(Sound me, int pos = 0)
        {
            StopMe();
            if (!string.IsNullOrEmpty(me.Name)) 
            {
                if (_bgmBuffer != null && _currentBgm != null)
                {
                    _currentBgm.Pos = _bgmBuffer.Seek();
                    _bgmBuffer.Stop();
                }
                _meBuffer = CreateBuffer("me/", me.Name);
                UpdateMeParameters(me);
                _meBuffer.Play(false);
                _meBuffer.AddStopListener(() => StopMe());
            }
        }
        
        public void UpdateMeParameters(Sound me)
        {
            UpdateBufferParameters(_meBuffer, _meVolume, me);
        }
        
        public void FadeOutMe(float duration)
        {
            if (_meBuffer != null)
            {
                _meBuffer.FadeOut(duration);
            }
        }
        
        public void StopMe()
        {
            if (_meBuffer != null)
            {
                _meBuffer.Dispose();
                _meBuffer = null;
                if (_bgmBuffer != null &&
                    _currentBgm != null &&
                    !_bgmBuffer.IsPlaying())
                {
                    _bgmBuffer.Play(true, _currentBgm.Pos);
                    _bgmBuffer.FadeIn(_replayFadeTime);
                }
            }
        }
        
        public void PlaySe(Sound se)
        {
            if (!string.IsNullOrEmpty(se.Name)) 
            {
                // [Note] Do not play the same sound in the same frame.
                var lastestBuffers = _seBuffers.Where(
                    buffer => buffer.FrameCount == UniRmmz.Graphics.FrameCount
                );
                if (lastestBuffers.Any(buffer => buffer.Name == se.Name))
                {
                    return;
                }
                var buffer = CreateBuffer("se/", se.Name);
                UpdateSeParameters(buffer, se);
                buffer.Play(false);
                _seBuffers.Add(buffer);
                CleanupSe();
            }
        }
        
        private void UpdateSeParameters(RmmzWebAudio buffer, Sound se)
        {
            UpdateBufferParameters(buffer, _seVolume, se);
        }
        
        public void CleanupSe()
        {
            foreach (var buffer in _seBuffers)
            {
                if (!buffer.IsPlaying())
                {
                    buffer.Dispose();
                }
            }

            _seBuffers = _seBuffers.Where(buffer => buffer.IsPlaying()).ToList();
        }
        
        public void StopSe()
        {
            foreach (var buffer in _seBuffers)
            {
                buffer.Dispose();
            }
            _seBuffers.Clear();
        }
        
        public void PlayStaticSe(Sound se)
        {
            if (!string.IsNullOrEmpty(se.Name))
            {
                LoadStaticSe(se);
                foreach (var buffer in _staticBuffers)
                {
                    if (buffer.Name == se.Name)
                    {
                        buffer.Stop();
                        UpdateSeParameters(buffer, se);
                        buffer.Play(false);
                        break;    
                    }
                }
            }
        }

        public void LoadStaticSe(Sound se)
        {
            if (!string.IsNullOrEmpty(se.Name) && !IsStaticSe(se))
            {
                var buffer = CreateBuffer("se/", se.Name);
                _staticBuffers.Add(buffer);
            }
        }

        public bool IsStaticSe(Sound se)
        {
            foreach (var buffer in _staticBuffers)
            {
                if (buffer.Name == se.Name)
                {
                    return true;
                }
            }

            return false;
        }
        
        public void StopAll()
        {
            StopMe();
            StopBgm();
            StopBgs();
            StopSe();
        }
        
        public Sound SaveBgm()
        {
            if (_currentBgm != null)
            {
                var bgm = _currentBgm;
                return new Sound
                {
                    Name = bgm.Name,
                    Volume = bgm.Volume,
                    Pitch = bgm.Pitch,
                    Pan = bgm.Pan,
                    Pos = _bgmBuffer?.Seek() ?? 0,
                };    
            }
            else
            {
                return MakeEmptyAudioObject();
            }
        }
        
        public Sound SaveBgs()
        {
            if (_currentBgs != null)
            {
                var bgs = _currentBgs;
                return new Sound
                {
                    Name = bgs.Name,
                    Volume = bgs.Volume,
                    Pitch = bgs.Pitch,
                    Pan = bgs.Pan,
                    Pos = _bgsBuffer?.Seek() ?? 0,
                };    
            }
            else
            {
                return MakeEmptyAudioObject();
            }
        }

        private Sound MakeEmptyAudioObject()
        {
            return new Sound
            {
                Name = string.Empty,
                Volume = 0,
                Pitch = 0,
            };
        }

        private RmmzWebAudio CreateBuffer(string folder, string name)
        {
            var ext = AudioFileExt();
            var url = _path + folder + Utils.EncodeUri(name) + ext;
            var buffer = new RmmzWebAudio(url);
            buffer.Name = name;
            buffer.FrameCount = Graphics.FrameCount;
            return buffer;
        }

        private void UpdateBufferParameters(RmmzWebAudio buffer, int configVolume, Sound audio)
        {
            if (buffer != null && audio != null) 
            {
                buffer.Volume = (float)(configVolume * audio.Volume) / 10000;
                buffer.Pitch = (float)audio.Pitch / 100;
                buffer.Pan = (float)audio.Pan / 100;
            }
        }
        private string AudioFileExt()
        {
            return ".ogg";
        }
    }
}