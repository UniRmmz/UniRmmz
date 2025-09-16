using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace UniRmmz
{
   public static partial class Video
   {
       private static VideoPlayer _videoPlayer;
       private static bool _loading = false;
       private static float _volume = 1f;
       private static bool _isVisible = false;
       private static RenderTexture _renderTarget;

       public static void Initialize(int width, int height)
       {
           _loading = false;
           _volume = 1f;
           CreateVideoPlayer();
           //SetupEventHandlers();
           Resize(width, height);
       }

       public static void Resize(int width, int height)
       {
           if (_renderTarget != null)
           {
               ResizeRenderTexture(_videoPlayer, RmmzRoot.Instance.VideoImage, _renderTarget, GetRenderTextureDescriptor(width, height));
           }
       }

       public static void Play(string src)
       {
           if (_videoPlayer == null) return;

           var filePath = Path.Combine(Rmmz.RootPath, src);
           _videoPlayer.url = filePath;
           _videoPlayer.prepareCompleted += OnLoad;
           _videoPlayer.errorReceived += OnError;
           _videoPlayer.loopPointReached += OnEnd;
           _videoPlayer.Prepare();
           _loading = true;
       }

       public static bool IsPlaying()
       {
           return _loading || _isVisible;
       }

       public static void SetVolume(float volume)
       {
           _volume = Mathf.Clamp01(volume);
           if (_videoPlayer != null)
           {
               _videoPlayer.SetDirectAudioVolume(0, _volume);
           }
       }

       private static void CreateVideoPlayer()
       {
           var videoObject = new GameObject("VideoPlayer");
           _videoPlayer = videoObject.AddComponent<VideoPlayer>();
           _videoPlayer.playOnAwake = false;
           _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
           _videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
           _videoPlayer.aspectRatio = VideoAspectRatio.FitHorizontally;
           videoObject.transform.parent = RmmzRoot.Instance.VideoRoot.transform;
           
           _renderTarget = new RenderTexture(GetRenderTextureDescriptor(1, 1));
           _renderTarget.Create();
           _videoPlayer.targetTexture = _renderTarget;
       }

       private static void OnLoad(VideoPlayer player)
       {
           player.SetDirectAudioVolume(0, _volume);
           player.Play();
           UpdateVisibility(true);
           _loading = false;
       }

       private static void OnError(VideoPlayer player, string message)
       {
           UpdateVisibility(false);
           Debug.LogError($"Video load error: {message}");
           throw new RmmzError($"Failed to load video: {player.url}", null, () => {
               player.Prepare();
           });
       }

       private static void OnEnd(VideoPlayer player)
       {
           UpdateVisibility(false);
       }

       private static void UpdateVisibility(bool videoVisible)
       {
           _isVisible = videoVisible;
           
           if (videoVisible)
           {
               Graphics.HideScreen();
               RmmzRoot.Instance.SetVideoActive(true);
           }
           else
           {
               Graphics.ShowScreen();
               RmmzRoot.Instance.SetVideoActive(false);
           }
       }
       
       public static void Cleanup()
       {
           if (_videoPlayer != null)
           {
               _videoPlayer.Stop();
               _videoPlayer.prepareCompleted -= OnLoad;
               _videoPlayer.errorReceived -= OnError;
               _videoPlayer.loopPointReached -= OnEnd;
               
               if (_videoPlayer.gameObject != null)
               {
                   GameObject.DestroyImmediate(_videoPlayer.gameObject);
               }
               
               if (_renderTarget != null)
               {
                   _renderTarget.Release();
                   GameObject.Destroy(_renderTarget);
                   _renderTarget = null;
               }
               
               _videoPlayer = null;
           }
           
           _loading = false;
           _isVisible = false;
       }
       
       private static RenderTextureDescriptor GetRenderTextureDescriptor(int width, int height)
       {
           var desc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32);
           return desc;
       }
        
       /// <summary>
       /// VideoPlayerとRawImageに参照されているRenderTextureを正しくリサイズする
       /// </summary>
       private static void ResizeRenderTexture(VideoPlayer videoPlayer, RawImage image, RenderTexture renderTexture, RenderTextureDescriptor newDesc)
       {
           // 一度参照を外さないと、正しく反映されない
           videoPlayer.targetTexture = null;
           image.texture = null;
            
           // RenderTextureを作り直す
           renderTexture.Release();
           renderTexture.descriptor = newDesc; 
           renderTexture.Create();
           
           // 変更後のサイズ変更
           RmmzRoot.Instance.SetVideoResolution(_renderTarget.width, _renderTarget.height);
            
           // 参照を付けなおす
           videoPlayer.targetTexture = renderTexture;
           image.texture = renderTexture;
       }
   }
}