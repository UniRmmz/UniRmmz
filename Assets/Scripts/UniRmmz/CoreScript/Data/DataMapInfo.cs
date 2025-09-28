using System;

namespace UniRmmz
{
    [Serializable]
    public partial class DataMapInfo
    {
        public int Id => id;
        public bool Expanded => expanded;
        public string Name => name;
        public int Order => order;
        public int ParentId => parentId;
        public float ScrollX => scrollX;
        public float ScrollY => scrollY;
        
        protected int id;
        protected bool expanded;
        protected string name;
        protected int order;
        protected int parentId;
        protected float scrollX;
        protected float scrollY;
    }
}