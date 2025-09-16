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
   
        public int id;
        public int displayType;
        public string effectName;
        public DataFlashTiming[] flashTimings;
        public string name;
        public int offsetX;
        public int offsetY;
        public DataRotation rotation;
        public int scale;
        public DataSoundTiming[] soundTimings;
        public int speed;
        public bool alignBottom;
        
        // 以下、MV版で必要な情報
        public int Animation1Hue => animation1Hue;
        public string Animation1Name => animation1Name;
        public int Animation2Hue => animation2Hue;
        public string Animation2Name => animation2Name;
        public float[][][] Frames => frames;
        public int Position => position;
        public DataAnimationTimingMV[] Timings => timings;
        
        public int animation1Hue;
        public string animation1Name;
        public int animation2Hue;
        public string animation2Name;
        public float[][][] frames;
        public int position;
        public DataAnimationTimingMV[] timings;
    }

    [Serializable]
    public class DataFlashTiming
    {
        public int Frame => frame;
        public int Duration => duration;
        public int[] Color => color;
   
        public int frame;
        public int duration;
        public int[] color;
    }

    [Serializable]
    public class DataSoundTiming
    {
        public int Frame => frame;
        public DataSystem.DataSound Se => se;
   
        public int frame;
        public DataSystem.DataSound se;
    }

    [System.Serializable]
    public class DataRotation
    {
        public float X => x;
        public float Y => y;
        public float Z => z;
   
        public float x;
        public float y;
        public float z;
    }
    
    [Serializable]
    public class DataAnimationTimingMV
    {
        public int[] FlashColor => flashColor;
        public int FlashDuration => flashDuration;
        public int FlashScope => flashScope;
        public int Frame => frame;
        public DataSystem.DataSound Se => se;
        
        public int[] flashColor;
        public int flashDuration;
        public int flashScope;
        public int frame;
        public DataSystem.DataSound se;
    }
}