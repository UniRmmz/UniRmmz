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
        
        public int id;
        public bool expanded;
        public string name;
        public int order;
        public int parentId;
        public float scrollX;
        public float scrollY;
    }
}