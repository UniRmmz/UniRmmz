using System;

namespace UniRmmz
{
    [Serializable]
    public partial class DataDamage
    {
        public bool Critical => critical;
        public int ElementId => elementId;
        public string Formula => formula;
        public int Type => type;
        public int Variance => variance;
        
        protected bool critical;
        protected int elementId;
        protected string formula;
        protected int type;
        protected int variance;
    }
}