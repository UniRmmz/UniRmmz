using System;
using UnityEngine.Serialization;

namespace UniRmmz
{
    [Serializable]
    public class DataTrait
    {
        public int Code => code;
        public int DataId => dataId;
        public float Value => value;
        
        public int code;
        public int dataId;
        public float value;
    }
}