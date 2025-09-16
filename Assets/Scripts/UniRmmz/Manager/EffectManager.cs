using System;
using System.Collections.Generic;
using Effekseer;
using UniRmmz;

namespace UniRmmz
{
    /// <summary>
    /// The static class that loads Effekseer effects.
    /// </summary>
    public partial class EffectManager
    {
        protected readonly Dictionary<string, RmmzEffekseerAssetLoadRequest> _cache = new ();
        protected readonly List<string> _errorUrls = new List<string>();

        public RmmzEffekseerAssetLoadRequest Load(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string url = MakeUrl(filename);
                if (!_cache.ContainsKey(url))
                {
                    StartLoading(url);
                }

                return _cache.GetValueOrDefault(url);
            }
            else
            {
                return null;
            }
        }

        protected RmmzEffekseerAssetLoadRequest StartLoading(string url)
        {
            Action onLoad = () => OnLoad(url);
            Action<string, string> onError = (message, errorUrl) => OnError(errorUrl);

            var effect = UniRmmz.Graphics.Effekseer.LoadEffect(url, 1.0f, onLoad, onError);
            _cache[url] = effect;
            return effect;
        }

        public void Clear()
        {
            foreach (var kvp in _cache)
            {
                var effect = kvp.Value;
                UniRmmz.Graphics.Effekseer?.ReleaseEffect(effect);
            }

            _cache.Clear();
        }

        protected void OnLoad(string _) {}

        protected void OnError(string url)
        {
            _errorUrls.Add(url);
        }

        protected string MakeUrl(string filename)
        {
            return $"Effekseer/{Utils.EncodeUri(filename)}";
        }

        public void CheckErrors()
        {
            if (_errorUrls.Count > 0)
            {
                string url = _errorUrls[0];
                _errorUrls.RemoveAt(0);
                ThrowLoadError(url);
            }
        }

        protected void ThrowLoadError(string url)
        {
            Action retry = () => StartLoading(url);
            throw new RmmzError("failed to load effect", url, retry);
        }

        public bool IsReady()
        {
            CheckErrors();

            foreach (var effect in _cache.Values)
            {
                if (effect != null && !effect.IsLoaded)
                {
                    return false;
                }
            }

            return true;
        }
    }

}

