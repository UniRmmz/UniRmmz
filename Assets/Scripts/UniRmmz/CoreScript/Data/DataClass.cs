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
        
        private int id;
        private int[] expParams;
        private DataTrait[] traits;
        private DataLearning[] learnings;
        private string name;
        private string note;
        private int[][] @params;
    }

    [Serializable]
    public partial class DataLearning
    {
        public int Level => level;
        public string Note => note;
        public int SkillId => skillId;
        
        private int level;
        private string note;
        private int skillId;
    }
}