using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    public static partial class Input
    {
        public enum ButtonTypes
        {
            Cancel,
            PageUp,
            PageDown,
            Down,
            Up,
            Down2,
            Up2,
            Ok,
            Menu
        }
        
        public static void Initialize()
        {
            Clear();
            RmmzRoot.Instance.OnFocus += (bool focus) =>
            {
                if (!focus) OnLostFocus();
            };
        }
        
        /// <summary>
        /// The wait time of the key repeat in frames.
        /// </summary>
        public const int KeyRepeatWait = 24;
        
        /// <summary>
        /// The interval of the key repeat in frames.
        /// </summary>
        public const int KeyRepeatInterval = 6;

        /// <summary>
        /// A hash table to convert from a virtual key code to a mapped key name.
        /// </summary>
        private static Dictionary<KeyCode, string> _keyMapper = new()
        {
            {KeyCode.Tab, "tab"}, // tab
            {KeyCode.Return, "ok"}, // enter
            {KeyCode.LeftShift, "shift"}, // shift
            {KeyCode.RightShift, "shift"}, // shift
            {KeyCode.LeftControl, "control"}, // control
            {KeyCode.RightControl, "control"}, // control
            {KeyCode.LeftAlt, "control"}, // alt
            {KeyCode.RightAlt, "control"}, // alt
            {KeyCode.Escape, "escape"}, // escape
            {KeyCode.Space, "ok"}, // space
            {KeyCode.PageUp, "pageup"}, // pageup
            {KeyCode.PageDown, "pagedown"}, // pagedown
            {KeyCode.LeftArrow, "left"}, // left arrow
            {KeyCode.UpArrow, "up"}, // up arrow
            {KeyCode.RightArrow, "right"}, // right arrow
            {KeyCode.DownArrow, "down"}, // down arrow
            {KeyCode.Insert, "escape"}, // insert
            {KeyCode.Q, "pageup"}, // Q
            {KeyCode.W, "pagedown"}, // W
            {KeyCode.X, "escape"}, // X
            {KeyCode.Z, "ok"}, // Z
            {KeyCode.Keypad0, "escape"}, // numpad 0
            {KeyCode.Keypad2, "down"}, // numpad 2
            {KeyCode.Keypad4, "left"}, // numpad 4
            {KeyCode.Keypad6, "right"}, // numpad 6
            {KeyCode.Keypad8, "up"}, // numpad 8
            {KeyCode.F9, "debug"} // F9
        };

        /// <summary>
        /// A hash table to convert from a gamepad button to a mapped key name.
        /// </summary>
        private static Dictionary<int, string> _gamepadMapper = new()
        {
            { 0, "ok" }, // A
            { 1, "cancel" }, // B
            { 2, "shift" }, // X
            { 3, "menu" }, // Y
            { 4, "pageup" }, // LB
            { 5, "pagedown" }, // RB
            { 12, "up" }, // D-pad up
            { 13, "down" }, // D-pad down
            { 14, "left" }, // D-pad left
            { 15, "right" } // D-pad right
        };
        
        private static Dictionary<string, bool> _currentState = new();
        private static Dictionary<string, bool> _previousState = new();
        private static string _latestButton = null;
        private static int _pressedTime = 0;
        private static string _virtualButton = null;

        
        /// <summary>
        /// Clears all the input data.
        /// </summary>
        public static void Clear()
        {
            _currentState.Clear();
            _previousState.Clear();
            //_gamepadStates.Clear();
            _latestButton = null;
            _pressedTime = 0;
            Dir4 = 0;
            Dir8 = 0;
            //_preferredAxis = "";
            Date = 0;
            _virtualButton = null;
        }
        
        /// <summary>
        /// Updates the input data.
        /// </summary>
        public static void Update()
        {
            UpdateInternal();
            //_PollGamePads();
            if (_latestButton != null && _currentState.GetValueOrDefault(_latestButton, false))
            {
                _pressedTime++;
            }
            else
            {
                _latestButton = null;
            }

            foreach (var pair in _currentState)
            {
                var buttonName = pair.Key;
                if (_currentState.GetValueOrDefault(buttonName, false) && !_previousState.GetValueOrDefault(buttonName, false))
                {
                    _latestButton = buttonName;
                    _pressedTime = 0;
                    Date = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }

                _previousState[buttonName] = _currentState[buttonName];
            }

            if (_virtualButton != null)
            {
                _latestButton = _virtualButton;
                _pressedTime = 0;
                _virtualButton = null;
            }
            UpdateDirection();
        }

        /// <summary>
        /// Checks whether a key is currently pressed down.
        /// </summary>
        /// <param name="keyName">The mapped name of the key.</param>
        /// <returns>True if the key is pressed.</returns>
        public static bool IsPressed(string keyName)
        {
            if (IsEscapeCompatible(keyName) && IsPressed("escape"))
            {
                return true;
            }
            else
            {
                return _currentState.GetValueOrDefault(keyName);
            }
        }

        /// <summary>
        /// Checks whether a key is just pressed.
        /// </summary>
        /// <param name="keyName">The mapped name of the key.</param>
        /// <returns>True if the key is triggered.</returns>
        public static bool IsTriggered(string keyName)
        {
            if (IsEscapeCompatible(keyName) && IsTriggered("escape"))
            {
                return true;
            }
            else
            {
                return _latestButton == keyName && _pressedTime == 0; 
            }
        }

        /// <summary>
        /// Checks whether a key is just pressed or a key repeat occurred.
        /// </summary>
        /// <param name="keyName">The mapped name of the key.</param>
        /// <returns>True if the key is repeated.</returns>
        public static bool IsRepeated(string keyName)
        {
            if (IsEscapeCompatible(keyName) && IsRepeated("escape"))
            {
                return true;
            }
            else
            {
                return _latestButton == keyName &&
                    (_pressedTime == 0 || (_pressedTime >= KeyRepeatWait && _pressedTime % KeyRepeatInterval == 0)); 
            }
        }

        /// <summary>
        /// Checks whether a key is kept depressed.
        /// </summary>
        /// <param name="keyName">The mapped name of the key.</param>
        /// <returns>True if the key is long-pressed.</returns>
        public static bool IsLongPressed(string keyName)
        {
            if (IsEscapeCompatible(keyName) && IsLongPressed("escape"))
            {
                return true;
            }
            else
            {
                return _latestButton == keyName && _pressedTime >= KeyRepeatWait;
            }
        }
        
        /// <summary>
        /// The four direction value as a number of the numpad, or 0 for neutral.
        /// </summary>
        public static int Dir4 { get; private set; }
        
        /// <summary>
        /// The eight direction value as a number of the numpad, or 0 for neutral.
        /// </summary>
        public static int Dir8 { get; private set; }

        /// <summary>
        /// The time of the last input in milliseconds.
        /// </summary>
        public static long Date { get; private set; }
        
        public static void VirtualClick(ButtonTypes buttonType)
        {
            _virtualButton = buttonType switch
            {
                Input.ButtonTypes.Cancel => "cancel",
                Input.ButtonTypes.PageUp => "pageup",
                Input.ButtonTypes.PageDown => "pagedown",
                Input.ButtonTypes.Down => "down",
                Input.ButtonTypes.Up => "up",
                Input.ButtonTypes.Down2 => "down2",
                Input.ButtonTypes.Up2 => "up2",
                Input.ButtonTypes.Ok => "ok",
                Input.ButtonTypes.Menu => "menu",
                _ => "ok"
            };
        }

        private static void UpdateInternal()
        {
            foreach (var pair in _keyMapper)
            {
                var buttonName = _keyMapper.GetValueOrDefault(pair.Key);
                if (buttonName != null)
                {
                    _currentState[buttonName] = false; 
                }
            }

            foreach (var pair in _keyMapper)
            {
                var buttonName = _keyMapper.GetValueOrDefault(pair.Key);
                if (buttonName != null)
                {
                    if (UnityEngine.Input.GetKey(pair.Key))
                    {
                        _currentState[buttonName] = true; 
                    }
                }
            }
        }

        private static bool IsEscapeCompatible(string keyName)
        {
            return keyName == "cancel" || keyName == "menu";
        }

        private static void OnLostFocus()
        {
            Clear();
        }
        
        /*
         * 

Input._pollGamepads = function() {
    if (navigator.getGamepads) {
        const gamepads = navigator.getGamepads();
        if (gamepads) {
            for (const gamepad of gamepads) {
                if (gamepad && gamepad.connected) {
                    this._updateGamepadState(gamepad);
                }
            }
        }
    }
};

Input._updateGamepadState = function(gamepad) {
    const lastState = this._gamepadStates[gamepad.index] || [];
    const newState = [];
    const buttons = gamepad.buttons;
    const axes = gamepad.axes;
    const threshold = 0.5;
    newState[12] = false;
    newState[13] = false;
    newState[14] = false;
    newState[15] = false;
    for (let i = 0; i < buttons.length; i++) {
        newState[i] = buttons[i].pressed;
    }
    if (axes[1] < -threshold) {
        newState[12] = true; // up
    } else if (axes[1] > threshold) {
        newState[13] = true; // down
    }
    if (axes[0] < -threshold) {
        newState[14] = true; // left
    } else if (axes[0] > threshold) {
        newState[15] = true; // right
    }
    for (let j = 0; j < newState.length; j++) {
        if (newState[j] !== lastState[j]) {
            const buttonName = this.gamepadMapper[j];
            if (buttonName) {
                this._currentState[buttonName] = newState[j];
            }
        }
    }
    this._gamepadStates[gamepad.index] = newState;
};

         */
        
        
        private static void UpdateDirection()
        {
            int x = Sign("right") - Sign("left");
            int y = Sign("down") - Sign("up");

            Dir8 = MakeNumpadDirection(x, y);

            // prevent diagonal if both pressed
            if (x != 0 && y != 0)
            {
                y = 0; // prioritize horizontal for simplicity
            }

            Dir4 = MakeNumpadDirection(x, y);
        }

        private static int Sign(string name) => IsPressed(name) ? 1 : 0;

        private static int MakeNumpadDirection(int x, int y)
        {
            if (x == 0 && y == 0) return 0;
            return 5 - y * 3 + x;
        }
    }
}
