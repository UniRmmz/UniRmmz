using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The static class that handles input data from the mouse and touchscreen.
    /// </summary>
    public static partial class TouchInput
    {
        /// <summary>
        /// The wait time of the pseudo key repeat in frames.
        /// </summary>
        public const int KeyRepeatWait = 24;
        
        /// <summary>
        /// The interval of the pseudo key repeat in frames.
        /// </summary>
        public const int KeyRepeatInterval = 6;
        
        /// <summary>
        /// The threshold number of pixels to treat as moved.
        /// </summary>
        public const float MoveThreshold = 10f;

        public static float X { get; private set; }
        public static float Y { get; private set; }
        public static float WheelX { get; private set; }
        public static float WheelY { get; private set; }
        public static long Date { get; private set; }

        private static bool _mousePressed = false;
        private static bool _screenPressed = false;
        private static bool _clicked = false;
        private static bool _moved = false;

        private static Vector2 _triggerPos;
        private static int _pressedTime = 0;

        private static State _currentState = new();
        private static State _nextState = new();

        private struct State
        {
            public bool Triggered;
            public bool Released;
            public bool Cancelled;
            public bool Moved;
            public bool Hovered;
            public float WheelX;
            public float WheelY;
            public bool LeftHolding;
            public bool RightHolding;
        }

        public static void Initialize()
        {
            Clear();
        }

        public static void Clear()
        {
            _mousePressed = false;
            _screenPressed = false;
            _pressedTime = 0;
            _clicked = false;
            _currentState = new();
            _nextState = new();
            X = Y = 0;
            _triggerPos = Vector2.zero;
            _moved = false;
            Date = 0;
        }

        public static void Update()
        {
            _currentState = _nextState;
            _nextState = new();

            WheelX = _currentState.WheelX;
            WheelY = _currentState.WheelY;

            _clicked = _currentState.Released && !_moved;

            if (IsPressed())
            {
                _pressedTime++;
            }
            else
            {
                _pressedTime = 0;
            }

            // マウス入力取得
            UpdateMouse();
        }

        private static void UpdateMouse()
        {
            Vector2 mousePos = UnityEngine.Input.mousePosition;
            mousePos = RmmzRoot.Instance.Canvas.UnityScreenPointToRmmzScreenPoint(mousePos);
            X = mousePos.x;
            Y = mousePos.y;

            if (UnityEngine.Input.GetMouseButton(0))
            {
                _nextState.LeftHolding = true;
                if (!_currentState.LeftHolding)
                {
                    _mousePressed = true;
                    _pressedTime = 0;
                    _nextState.Triggered = true;
                    _triggerPos = mousePos;
                    _moved = false;
                    Date = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
            }
            else
            {
                _nextState.LeftHolding = false;
                if (_currentState.LeftHolding)
                {
                    _mousePressed = false;
                    _nextState.Released = true;
                }
            }

            if (_mousePressed)
            {
                float dx = Mathf.Abs(mousePos.x - _triggerPos.x);
                float dy = Mathf.Abs(mousePos.y - _triggerPos.y);
                if (dx > MoveThreshold || dy > MoveThreshold)
                {
                    _moved = true;
                    _nextState.Moved = true;
                }
            }

            // ホイール
            _nextState.WheelY = UnityEngine.Input.mouseScrollDelta.y;
            _nextState.WheelX = UnityEngine.Input.mouseScrollDelta.x;

            if (UnityEngine.Input.GetMouseButton(1))
            {
                _nextState.RightHolding = true;
                if (!_currentState.RightHolding)
                {
                    _nextState.Cancelled = true;
                }
            }
            else
            {
                _nextState.RightHolding = false;
                if (_currentState.RightHolding)
                {
                    _nextState.Hovered = true;
                }
            }
        }

        public static bool IsClicked() => _clicked;
        public static bool IsPressed() => _mousePressed || _screenPressed;
        public static bool IsTriggered() => _currentState.Triggered;
        public static bool IsRepeated() =>
            IsPressed() &&
            (_currentState.Triggered || (_pressedTime >= KeyRepeatWait && _pressedTime % KeyRepeatInterval == 0));
        public static bool IsLongPressed() => IsPressed() && _pressedTime >= KeyRepeatWait;
        public static bool IsCancelled() => _currentState.Cancelled;
        public static bool IsMoved() => _currentState.Moved;
        public static bool IsHovered() => _currentState.Hovered;
        public static bool IsReleased() => _currentState.Released;
    }
}