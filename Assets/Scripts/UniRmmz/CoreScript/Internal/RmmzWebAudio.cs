using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace UniRmmz
{
    public sealed class RmmzWebAudio : IDisposable
    {
        private class ReserveParam
        {
            public int Pos;
        }

        private class FadeParam
        {
            public float StartVolume;
            public float EndVolume;
            public Stopwatch Timer;
            public float DurationInMiliseconds;
        }
        
        private AudioSource _audioSource;
        private CancellationTokenSource _cts = new();
        private bool _isUpdateRegistered = false;
        private Action _stopEventHandler = null;
        private ReserveParam _reserveParam;
        private FadeParam _fadeParam;
        private bool _isPlaying;

        public string Name { get; set; }
        public int FrameCount { get; set;  }

        private float _volume = 1f;
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                _audioSource.volume = value;    
                _fadeParam = null;
            }
        }

        public float Pitch
        {
            get => _audioSource.pitch;
            set => _audioSource.pitch = value;
        }
        
        public float Pan
        {
            get => _audioSource.panStereo;
            set => _audioSource.panStereo = value;
        }

        public RmmzWebAudio(string filePath)
        {
            var obj = new GameObject(filePath, typeof(AudioSource));
            _audioSource = obj.GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            obj.transform.parent = RmmzRoot.Instance.AudioRoot.transform;
            
            _ = LoadAudioAsync(filePath, clip =>
            {
                if (clip != null)
                {
                    _audioSource.clip = clip;
                    if (_reserveParam != null)
                    {
                        _audioSource.timeSamples = _reserveParam.Pos;
                        _audioSource.Play();
                        _reserveParam = null;
                    }
                }
            }, _cts.Token);
        }

        /// <summary>
        /// Initializes the audio system.
        /// </summary>
        public static bool Initialize()
        {
            // TODO
            return true;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _isUpdateRegistered = false;
            RmmzRoot.Instance.OnTick -= UpdateRmmz;
            MonoBehaviour.Destroy(_audioSource.gameObject);
        }

        public void Play(bool isLoop, int position = 0)
        {
            RegisterUpdateIfNeed();
            _audioSource.loop = isLoop;
            if (IsReady())
            {
                _audioSource.timeSamples = position;
                _audioSource.Play();    
            }
            else
            {
                _reserveParam = new ReserveParam(){ Pos = position };
            }
            _isPlaying = true;
        }

        public bool IsReady()
        {
            return _audioSource.clip != null;
        }
        
        public bool IsPlaying()
        {
            return _isPlaying;
        }

        public void FadeIn(float duration)
        {
            if (IsReady())
            {
                CreateFadeParam(0f, _volume, duration);
            }
            else
            {
                // TODO AddLoadListener(() => FadeIn(duration));
            }
        }
        
        public void FadeOut(float duration)
        {
            CreateFadeParam(_volume, 0f, duration);
            
        }
        
        public void Stop()
        {
            _isUpdateRegistered = false;
            RmmzRoot.Instance.OnTick -= UpdateRmmz;
            _isPlaying = false;
            _audioSource.Stop();
            if (_stopEventHandler != null)
            {
                var action = _stopEventHandler;
                _stopEventHandler = null;
                action();    
            }
        }
        
        public int Seek()
        {
            return _audioSource.timeSamples;
        }
        
        public void AddStopListener(Action action)
        {
            if (_stopEventHandler == null)
            {
                RegisterUpdateIfNeed();
                if (_isPlaying)
                {
                    _stopEventHandler = action;    
                }
            }
        }
        
        private static async Task LoadAudioAsync(string filePath, Action<AudioClip> callback, CancellationToken ct)
        {
            filePath = Path.Combine(Rmmz.RootPath, filePath);
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.OGGVORBIS))
            {
                var request = uwr.SendWebRequest();
                while (!request.isDone)
                {
                    if (ct.IsCancellationRequested)
                    {
                        uwr.Abort();
                        return;
                    }
                    await Task.Yield();
                }

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                    callback?.Invoke(clip);
                }
                else
                {
                    Debug.LogError($"オーディオの読み込みに失敗: {filePath} {uwr.error}");
                    callback?.Invoke(null);
                }
            }
        }

        private void RegisterUpdateIfNeed()
        {
            if (!_isUpdateRegistered)
            {
                RmmzRoot.Instance.OnTick += UpdateRmmz;
                _isUpdateRegistered = true;
            }
        }

        private void UpdateRmmz()
        {
            UpdateFadeParam();
            
            if (IsReady() && !_audioSource.isPlaying)
            {
                Stop();
            }
        }

        private void CreateFadeParam(float startVolume, float endVolume, float duration)
        {
            RegisterUpdateIfNeed();
            _fadeParam = new FadeParam();
            _fadeParam.StartVolume = startVolume;
            _fadeParam.EndVolume = endVolume;
            _fadeParam.Timer = Stopwatch.StartNew();
            _fadeParam.DurationInMiliseconds = duration * 1000;
        }
        
        private void UpdateFadeParam()
        {
            if (_fadeParam != null)
            {
                if (_audioSource.isPlaying)
                {
                    float t = Mathf.Clamp01((float)_fadeParam.Timer.ElapsedMilliseconds / _fadeParam.DurationInMiliseconds);
                    _audioSource.volume = Mathf.Lerp(_fadeParam.StartVolume, _fadeParam.EndVolume, t);
                    if (t >= 1f)
                    {
                        _fadeParam = null;
                    }
                }
                else
                {
                    _fadeParam = null;
                }
            }
        }
    }
}