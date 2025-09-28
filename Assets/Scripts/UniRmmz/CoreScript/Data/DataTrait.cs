using System;
using UnityEngine.Serialization;

namespace UniRmmz
{
    [Serializable]
    public partial class DataTrait
    {
        public int Code => code;
        public int DataId => dataId;
        public float Value => value;
        
        protected int code;
        protected int dataId;
        protected float value;
    }
}