using System;

namespace UniRmmz
{
    [Serializable]
    public class DataDamage
    {
        public bool Critical => critical;
        public int ElementId => elementId;
        public string Formula => formula;
        public int Type => type;
        public int Variance => variance;
        
        public bool critical;
        public int elementId;
        public string formula;
        public int type;
        public int variance;
    }
}