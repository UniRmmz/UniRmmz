using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The class that loads font files.
    /// </summary>
    public partial class FontManager
    {
        private enum FontStates
        {
            Loading,
            Loaded,
            Error,
        }
        
        private Dictionary<string, string> _urls = new();
        private Dictionary<string, FontStates> _states = new();
        private Dictionary<string, TMPro.TMP_FontAsset> _fontAssets = new();
        
        public void Load(string family, string filename)
        {
            if (_states.GetValueOrDefault(family) != FontStates.Loaded)
            {
                var url = MakeUrl(filename);
                StartLoading(family, url);
            }
            else
            {
                _urls[family] = string.Empty;
                _states[family] = FontStates.Loaded;
            }
        }

        public bool IsReady()
        {
            foreach (var pair in _states)
            {
                var state = pair.Value;
                if (state == FontStates.Loading)
                {
                    return false;
                }
                if (state == FontStates.Error)
                {
                    ThrowLoadError(pair.Key);
                }
            }
            return true;
        }

        public TMPro.TMP_FontAsset Font(string fontFace)
        {
            foreach (var family in fontFace.Split(","))
            {
                var trim = family.Trim();
                if (_fontAssets.TryGetValue(trim, out var fontAsset))
                {
                    return fontAsset;
                }
            }

            return null;
        }

        private void StartLoading(string family, string url)
        {
            System.Collections.IEnumerator LoadCoroutine(string family, string url)
            {
                _urls[family] = url;
                _states[family] = FontStates.Loading;

                var request = Resources.LoadAsync<TMPro.TMP_FontAsset>(url);
                while (!request.isDone)
                {
                    yield return null;
                }
                
                if (request.asset != null)
                {
                    _states[family] = FontStates.Loaded;
                    //var font = UnityEngine.Object.Instantiate((TMPro.TMP_FontAsset)request.asset);
                    var font = (TMPro.TMP_FontAsset)request.asset;
                    _fontAssets[family] = font;
                }
                else
                {
                    _states[family] = FontStates.Error;
                }
            }
            
            RmmzRoot.RunCoroutine(LoadCoroutine(family, url));
        }

        private void ThrowLoadError(string family)
        {
            /*
            const url = this._urls[family];
            const retry = () => this.startLoading(family, url);
            throw ["LoadError", url, retry];
            */
            throw new RmmzError("Failed to load font: " + family);
        }

        private string MakeUrl(string filename)
        {
            var tmp = Path.GetFileNameWithoutExtension(Utils.EncodeUri(filename));
            return $"Fonts/{tmp} SDF";
        }
        
        /// <summary>
        /// 読み込まれたフォントのクリーンアップ
        /// </summary>
        public void Cleanup()
        {
            foreach (var fontAsset in _fontAssets.Values)
            {
                UnityEngine.Object.DestroyImmediate(fontAsset, true);
            }
            
            _fontAssets.Clear();
            _states.Clear();
            _urls.Clear();
        }
        
    }
}