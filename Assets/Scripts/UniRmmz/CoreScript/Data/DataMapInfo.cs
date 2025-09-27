using System;

namespace UniRmmz
{
    [Serializable]
    public class DataMapInfo
    {
        public int Id => id;
        public bool Expanded => expanded;
        public string Name => name;
        public int Order => order;
        public int ParentId => parentId;
        public float ScrollX => scrollX;
        public float ScrollY => scrollY;
        
        private int id;
        private bool expanded;
        private string name;
        private int order;
        private int parentId;
        private float scrollX;
        private float scrollY;
    }
}