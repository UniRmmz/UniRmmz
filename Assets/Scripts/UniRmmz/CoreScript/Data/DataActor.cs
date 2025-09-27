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

        public int id;
        public string battlerName;
        public int characterIndex;
        public string characterName;
        public int classId;
        public List<int> equips;
        public int faceIndex;
        public string faceName;
        public DataTrait[] traits;
        public int initialLevel;
        public int maxLevel;
        public string name;
        public string nickname;
        public string note;
        public string profile;
    }
}