using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public partial class DataActor : ITraitsObject, IMetadataContainer
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

        protected int id;
        protected string battlerName;
        protected int characterIndex;
        protected string characterName;
        protected int classId;
        protected List<int> equips;
        protected int faceIndex;
        protected string faceName;
        protected DataTrait[] traits;
        protected int initialLevel;
        protected int maxLevel;
        protected string name;
        protected string nickname;
        protected string note;
        protected string profile;
    }
}