using System;
using System.Collections.Generic;
using Effekseer;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    public sealed class RmmzEffekseer
    {
        public RmmzEffekseer()
        {
            InitializeLayer();
        }
        
        public RmmzEffekseerAssetLoadRequest LoadEffect(string url , float scale, Action onLoad, Action<string, string> onError)
        {
            var request = Resources.LoadAsync<EffekseerEffectAsset>(url);
            var task = new RmmzEffekseerAssetLoadRequest(request);
            System.Collections.IEnumerator LoadCoroutine(Action onLoaded, Action<string, string> onError)
            {
                yield return new WaitUntil(() => request.isDone);
                if (request.asset != null)
                {
                    onLoaded?.Invoke();    
                }
                else
                {
                    onError?.Invoke(url, "failed to load effect asset");
                }
            }
            RmmzRoot.RunCoroutine(LoadCoroutine(onLoad, onError));

            return task;
        }
        
        public void ReleaseEffect(RmmzEffekseerAssetLoadRequest asset)
        {
            if (asset.IsLoaded)
            {
                Resources.UnloadAsset(asset.Result);    
            }
        }

        public EffekseerHandle Play(RmmzEffekseerAssetLoadRequest asset)
        {
            var handle = EffekseerSystem.PlayEffect(asset.Result, Vector3.zero);
            return AssignEffectLayer(handle);
        }
        
        #region UniRmmz

        private void InitializeLayer()
        {
            // Camera.Render()でEffkseerエフェクトを描画する際に、関係ないエフェクトを非表示にしたいが、
            // EffekseerHandle.layerや、EffekseerHandle.shown が即時反映されないので、仕方なく、
            // エフェクトインスタンス毎にレイヤーを割り当てすることで実現している。もっといい方法ないか？
            var effekseerLayers = new int[]
            {
                Rmmz.RmmzRmmzEffekseer1,
                Rmmz.RmmzRmmzEffekseer2,
                Rmmz.RmmzRmmzEffekseer3,
                Rmmz.RmmzRmmzEffekseer4,
                Rmmz.RmmzRmmzEffekseer5,
                Rmmz.RmmzRmmzEffekseer6,
                Rmmz.RmmzRmmzEffekseer7,
                Rmmz.RmmzRmmzEffekseer8,
            };
            foreach (var layer in effekseerLayers)
            {
                _layerMap.Add(new LayerData(){ Handle = new EffekseerHandle(-1), Layer = layer });    
            }
        }

        /// <summary>
        /// レイヤー管理陽データ
        /// </summary>
        private class LayerData
        {
            public EffekseerHandle Handle;
            public int Layer;
        }

        private List<LayerData> _layerMap = new();

        private EffekseerHandle AssignEffectLayer(EffekseerHandle handle)
        {
            foreach (var data in _layerMap)
            {
                if (!(data.Handle.enabled && data.Handle.exists))
                {
                    data.Handle = handle;
                    handle.layer = data.Layer;
                    return handle;
                }
            }

            throw new Exception("Maximum number of concurrent Effekseer effects exceeded");
        }
        
        #endregion
    }
}