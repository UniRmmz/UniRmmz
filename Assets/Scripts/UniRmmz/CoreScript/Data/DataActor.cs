using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataActor : ITraitsObject, IMetadataContainer
    {
        public int Id => id;
        public string BattlerName => battlerName;
        public int CharacterIndex => characterIndex;
        public string CharacterName => characterName;
        public int ClassId => classId;
        public List<int> Equips => equips;
        public int FaceIndex => faceIndex;
        public string FaceName => faceName;
        public DataTrait[] Traits => traits;
        public int InitialLevel => initialLevel;
        public int MaxLevel => maxLevel;
        public string Name => name;
        public string Nickname => nickname;
        public string Note => note;
        public RmmzMetadata Meta { get; set; }
        public string Profile => profile;

        private int id;
        private string battlerName;
        private int characterIndex;
        private string characterName;
        private int classId;
        private List<int> equips;
        private int faceIndex;
        private string faceName;
        private DataTrait[] traits;
        private int initialLevel;
        private int maxLevel;
        private string name;
        private string nickname;
        private string note;
        private string profile;
    }
}