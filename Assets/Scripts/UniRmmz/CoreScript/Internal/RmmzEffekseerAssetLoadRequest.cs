using System;
using Effekseer;
using UnityEngine;

namespace UniRmmz
{
    public class RmmzEffekseerAssetLoadRequest
    {
        private ResourceRequest _request;
        private EffekseerEffectAsset _asset;
        
        public RmmzEffekseerAssetLoadRequest(ResourceRequest request)
        {
            _request = request;
        }

        public EffekseerEffectAsset Result
        {
            get
            {
                if (!IsLoaded)
                {
                    throw new Exception("Access EffekseerEffectAsset before success loading");
                }
                return _request.asset as EffekseerEffectAsset;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return _request.isDone && _request.asset != null;
            }
        }
    }
}