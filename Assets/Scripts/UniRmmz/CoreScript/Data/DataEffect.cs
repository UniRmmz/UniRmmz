using System;

namespace UniRmmz
{
    [Serializable]
    public partial class DataEffect
    {
        public int Code => code;
        public int DataId => dataId;
        public float Value1 => value1;
        public float Value2 => value2;
        
        protected int code;
        protected int dataId;
        protected float value1;
        protected float value2;
    }
}