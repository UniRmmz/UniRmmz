using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public partial class DataTileset : IMetadataContainer
    {
        public int Id => id;
        public int[] Flags => flags;
        public int Mode => mode;
        public string Name => name;
        public string Note => note;
        public RmmzMetadata Meta { get; set; }
        public string[] TilesetNames => tilesetNames;
        
        protected int id;
        protected int[] flags;
        protected int mode;
        protected string name;
        protected string note;
        protected string[] tilesetNames;
    }
}