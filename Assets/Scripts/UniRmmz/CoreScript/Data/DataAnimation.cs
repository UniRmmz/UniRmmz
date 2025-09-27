using System;

namespace UniRmmz
{
    [Serializable]
    public class DataAnimation
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
   
        private int id;
        private int displayType;
        private string effectName;
        private DataFlashTiming[] flashTimings;
        private string name;
        private int offsetX;
        private int offsetY;
        private DataRotation rotation;
        private int scale;
        private DataSoundTiming[] soundTimings;
        private int speed;
        private bool alignBottom;
        
        // 以下、MV版で必要な情報
        public int Animation1Hue => animation1Hue;
        public string Animation1Name => animation1Name;
        public int Animation2Hue => animation2Hue;
        public string Animation2Name => animation2Name;
        public float[][][] Frames => frames;
        public int Position => position;
        public DataAnimationTimingMV[] Timings => timings;
        
        private int animation1Hue;
        private string animation1Name;
        private int animation2Hue;
        private string animation2Name;
        private float[][][] frames;
        private int position;
        private DataAnimationTimingMV[] timings;
    }

    [Serializable]
    public class DataFlashTiming
    {
        public int Frame => frame;
        public int Duration => duration;
        public int[] Color => color;
   
        private int frame;
        private int duration;
        private int[] color;
    }

    [Serializable]
    public class DataSoundTiming
    {
        public int Frame => frame;
        public DataSystem.DataSound Se => se;
   
        private int frame;
        private DataSystem.DataSound se;
    }

    [System.Serializable]
    public class DataRotation
    {
        public float X => x;
        public float Y => y;
        public float Z => z;
   
        private float x;
        private float y;
        private float z;
    }
    
    [Serializable]
    public class DataAnimationTimingMV
    {
        public int[] FlashColor => flashColor;
        public int FlashDuration => flashDuration;
        public int FlashScope => flashScope;
        public int Frame => frame;
        public DataSystem.DataSound Se => se;
        
        private int[] flashColor;
        private int flashDuration;
        private int flashScope;
        private int frame;
        private DataSystem.DataSound se;
    }
}