using System;

namespace UniRmmz
{
    [Serializable]
    public partial class DataAnimation
    {
        public int Id => id;
        public int DisplayType => displayType;
        public string EffectName => effectName;
        public DataFlashTiming[] FlashTimings => flashTimings;
        public string Name => name;
        public int OffsetX => offsetX;
        public int OffsetY => offsetY;
        public DataRotation Rotation => rotation;
        public int Scale => scale;
        public DataSoundTiming[] SoundTimings => soundTimings;
        public int Speed => speed;
        public bool AlignBottom => alignBottom;
   
        protected int id;
        protected int displayType;
        protected string effectName;
        protected DataFlashTiming[] flashTimings;
        protected string name;
        protected int offsetX;
        protected int offsetY;
        protected DataRotation rotation;
        protected int scale;
        protected DataSoundTiming[] soundTimings;
        protected int speed;
        protected bool alignBottom;
        
        // 以下、MV版で必要な情報
        public int Animation1Hue => animation1Hue;
        public string Animation1Name => animation1Name;
        public int Animation2Hue => animation2Hue;
        public string Animation2Name => animation2Name;
        public float[][][] Frames => frames;
        public int Position => position;
        public DataAnimationTimingMV[] Timings => timings;
        
        protected int animation1Hue;
        protected string animation1Name;
        protected int animation2Hue;
        protected string animation2Name;
        protected float[][][] frames;
        protected int position;
        protected DataAnimationTimingMV[] timings;
    }

    [Serializable]
    public partial class DataFlashTiming
    {
        public int Frame => frame;
        public int Duration => duration;
        public int[] Color => color;
   
        protected int frame;
        protected int duration;
        protected int[] color;
    }

    [Serializable]
    public partial class DataSoundTiming
    {
        public int Frame => frame;
        public DataSystem.DataSound Se => se;
   
        protected int frame;
        protected DataSystem.DataSound se;
    }

    [System.Serializable]
    public partial class DataRotation
    {
        public float X => x;
        public float Y => y;
        public float Z => z;
   
        protected float x;
        protected float y;
        protected float z;
    }
    
    [Serializable]
    public partial class DataAnimationTimingMV
    {
        public int[] FlashColor => flashColor;
        public int FlashDuration => flashDuration;
        public int FlashScope => flashScope;
        public int Frame => frame;
        public DataSystem.DataSound Se => se;
        
        protected int[] flashColor;
        protected int flashDuration;
        protected int flashScope;
        protected int frame;
        protected DataSystem.DataSound se;
    }
}