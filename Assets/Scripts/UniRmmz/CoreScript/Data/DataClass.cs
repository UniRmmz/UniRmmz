using System;

namespace UniRmmz
{
    [Serializable]
    public partial class DataClass : ITraitsObject
    {
        public int Id => id;
        public int[] ExpParams => expParams;
        public DataTrait[] Traits => traits;
        public DataLearning[] Learnings => learnings;
        public string Name => name;
        public string Note => note;
        public int[][] Params => @params;
        
        protected int id;
        protected int[] expParams;
        protected DataTrait[] traits;
        protected DataLearning[] learnings;
        protected string name;
        protected string note;
        protected int[][] @params;
    }

    [Serializable]
    public partial class DataLearning
    {
        public int Level => level;
        public string Note => note;
        public int SkillId => skillId;
        
        protected int level;
        protected string note;
        protected int skillId;
    }
}