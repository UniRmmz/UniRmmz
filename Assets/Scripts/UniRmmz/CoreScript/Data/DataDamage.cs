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
        
        private bool critical;
        private int elementId;
        private string formula;
        private int type;
        private int variance;
    }
}