using System;

namespace UniRmmz
{
    [Serializable]
    public class DataClass : ITraitsObject
    {
        public int Id => id;
        public int[] ExpParams => expParams;
        public DataTrait[] Traits => traits;
        public DataLearning[] Learnings => learnings;
        public string Name => name;
        public string Note => note;
        public int[][] Params => @params;
        
        public int id;
        public int[] expParams;
        public DataTrait[] traits;
        public DataLearning[] learnings;
        public string name;
        public string note;
        public int[][] @params;
    }

    [Serializable]
    public class DataLearning
    {
        public int Level => level;
        public string Note => note;
        public int SkillId => skillId;
        
        public int level;
        public string note;
        public int skillId;
    }
}