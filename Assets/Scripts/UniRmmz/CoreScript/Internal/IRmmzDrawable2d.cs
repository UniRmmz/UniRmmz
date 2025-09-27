using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public interface IRmmzDrawable2d
    {
        /// <summary>
        /// The width of the window in pixels.
        /// </summary>
        public float Width { get; set; }
        
        /// <summary>
        /// The height of the window in pixels.
        /// </summary>
        public float Height { get; set; }
        
        /// <summary>
        /// The opacity of the window without contents (0 to 255).
        /// </summary>
        public int Opacity { get; set; }

        public bool Visible { get; set; }

        /// <summary>
        /// The origin point of the window for scrolling.
        /// </summary>
        public Vector2 Origin { get; set; }
        
        public float X { get; set; }
        
        public float Y { get; set; }
        
        /*
         * [Z coordinate]
         *  0 : Lower tiles
         *  1 : Lower characters
         *  3 : Normal characters
         *  4 : Upper tiles
         *  5 : Upper characters
         *  6 : Airship shadow
         *  7 : Balloon
         *  8 : Animation
         *  9 : Destination
         */
        public float Z { get; set; }
        
        public int SpriteId { get; }
        
        public Vector2 Pivot { get; set; }
        
        public Vector2 Anchor { get; set; }
        
        public Vector2 Scale { get; set; }

        public float Alpha { get; set; }

        public void UpdateRmmz();

        public IEnumerable<IRmmzFilter> Filters { get; }

        public Rect FilterArea { get; set; }

        public Transform FilterCanvasTransform { get; set; }
    }
}