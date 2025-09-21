using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataTileset : IMetadataContainer
    {
        public int Id => id;
        public int[] Flags => flags;
        public int Mode => mode;
        public string Name => name;
        public string Note => note;
        public RmmzMetadata Meta { get; set; }
        public string[] TilesetNames => tilesetNames;
        
        public int id;
        public int[] flags;
        public int mode;
        public string name;
        public string note;
        public string[] tilesetNames;
    }
}