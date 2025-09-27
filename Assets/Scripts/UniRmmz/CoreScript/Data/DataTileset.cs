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
        
        private int id;
        private int[] flags;
        private int mode;
        private string name;
        private string note;
        private string[] tilesetNames;
    }
}