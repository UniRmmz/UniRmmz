using System;

namespace UniRmmz
{
    [Serializable]
    public class DataEffect
    {
        public int Code => code;
        public int DataId => dataId;
        public float Value1 => value1;
        public float Value2 => value2;
        
        public int code;
        public int dataId;
        public float value1;
        public float value2;
    }
}