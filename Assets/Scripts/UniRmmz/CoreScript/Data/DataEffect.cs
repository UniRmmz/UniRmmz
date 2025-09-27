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
        
        private int code;
        private int dataId;
        private float value1;
        private float value2;
    }
}