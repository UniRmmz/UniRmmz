using System;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The static class that carries out graphics processing.
    /// </summary>
    public static partial class Graphics
    {
        private static int _width = 0;
        private static int _height = 0;
        private static float _defaultScale = 1;
        private static float _realScale = 1;
        private static RmmzErrorPrinterView _errorPrinter;
        private static bool _stretchEnabled = true;
        private static Scene_Base _stage;
        private static Action _tickHandler;
        private static RmmzCanvas _canvas;
        private static RmmzEffekseer _effekseer;
        private static bool _wasLoading = false;
        
        /**
         * The total frame count of the game screen.
         */
        public static int FrameCount;
        /**
         * The width of the window display area.
         */
        public static int BoxWidth { get; set; }

        /**
         * The height of the window display area.
         */
        public static int BoxHeight { get; set; }

        /**
         * Initializes the graphics system.
         * @returns True if the graphics system is available.
         */
        public static bool Initialize()
        {
            _width = 800;
            _height = 600;
            _defaultScale = 1;
            _realScale = 1;
            _stretchEnabled = true;

            BoxWidth = _width;
            BoxHeight = _height;
            UpdateRealScale();
            CreateAllElements();
            RmmzRoot.Instance.OnTick += OnTick;// TODO Ticker
            CreateEffekseerContext();
            
            return true;
        }

        public static RmmzEffekseer Effekseer
        {
            get => _effekseer;
        }
        
        /// <summary>
        /// Register a handler for tick events.
        /// </summary>
        /// <param name="handler">The listener function to be added for updates.</param>
        public static void SetTickHandler(Action handler) 
        {
            _tickHandler = handler;
        }

        /**
         * Starts the game loop.
         */
        public static void StartGameLoop()
        {
            RmmzRoot.Instance.IsGameLoopRunning = true;
        }

        /**
         * Stops the game loop.
         */
        public static void StopGameLoop()
        {
            RmmzRoot.Instance.IsGameLoopRunning = false;
        }

        /// <summary>
        /// Sets the stage to be rendered.
        /// </summary>
        /// <param name="stage">The stage object to be rendered.</param>
        public static void SetStage(Scene_Base stage)
        {
            _stage = stage;
            if (stage != null)
            {
                stage.transform.SetParent(RmmzRoot.Instance.Canvas.transform, false);
            }
        }

        /// <summary>
        /// Shows the loading spinner.
        /// </summary>
        public static void StartLoading()
        {
            // TODO
        }
        
        /// <summary>
        /// Erases the loading spinner.
        /// </summary>
        /// <returns>True if the loading spinner was active.</returns>
        public static bool EndLoading()
        {
            // TODO
            return false;
        }

        /**
         * Displays the error text to the screen.
         * @param name The name of the error.
         * @param message The message of the error.
         */
        public static void PrintError(string name, string message)
        {
            if (_errorPrinter == null)
            {
                CreateErrorPrinter();
            }

            _errorPrinter.Show(name, message);
            _wasLoading = EndLoading();
            ApplyCanvasFilter();
        }

        public static void EraseError()
        {
            if (_errorPrinter != null)
            {
                if (_wasLoading)
                {
                    StartLoading();
                }
            }
            ClearCanvasFilter();
        }

        /**
         * Converts an x coordinate on the page to the corresponding x coordinate on the canvas area.
         * @param x The x coordinate on the page to be converted.
         * @returns The x coordinate on the canvas area.
         */
        public static int PageToCanvasX(int x)
        {
            return x;
        }

        /**
         * Converts a y coordinate on the page to the corresponding y coordinate on the canvas area.
         * @param y The y coordinate on the page to be converted.
         * @returns The y coordinate on the canvas area.
         */
        public static int PageToCanvasY(int y)
        {
            return y;
        }

        /**
         * Checks whether the specified point is inside the game canvas area.
         * @param x The x coordinate on the canvas area.
         * @param y The y coordinate on the canvas area.
         * @returns True if the specified point is inside the game canvas area.
         */
        public static bool IsInsideCanvas(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        /// <summary>
        /// Shows the game screen.
        /// </summary>
        public static void ShowScreen()
        {
            _canvas.Show();
        }
        
        /// <summary>
        /// Hides the game screen.
        /// </summary>
        public static void HideScreen()
        {
            _canvas.Hide();
        }

        /**
         * Changes the size of the game screen.
         * @param width The width of the game screen.
         * @param height The height of the game screen.
         */
        public static void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            UpdateAllElements();
        }
        
        

        /**
         * The width of the game screen.
         */
        public static int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    UpdateAllElements();
                }
            }
        }
        
        /**
         * The height of the game screen.
         */
        public static int Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    UpdateAllElements();
                }
            }
        }
        
        /**
         * The default zoom scale of the game screen.
         */
        public static float DefaultScale
        {
            get => _defaultScale;
            set
            {
                if (_defaultScale != value)
                {
                    _defaultScale = value;
                    UpdateAllElements();
                }
            }
        }

        private static void CreateAllElements()
        {
            CreateErrorPrinter();
            CreateCanvas();
            /*
            CreateLoadingSpinner();
            CrateFPSCounter();
            */
        }

        private static void UpdateAllElements()
        {
            /*
            UpdateRealScale();
            UpdateErrorPrinter();
            */
            UpdateCanvas();
            UpdateVideo();
        }

        private static void OnTick()
        {
            if (_tickHandler != null)
            {
                _tickHandler();
            }
        }

        private static void UpdateRealScale()
        {
            // TODO
        }

        private static void CreateErrorPrinter()
        {
            _errorPrinter = RmmzRoot.Instance.ErrorPrinterView;
        }
        
        private static void CreateCanvas()
        {
            // todo destroy resource
            _canvas = RmmzRoot.Instance.Canvas; 
            _canvas.UpdateCanvas(_width, _height);
        }
        
        private static void UpdateCanvas()
        {
            RmmzRoot.Instance.Canvas.UpdateCanvas(_width, _height);
        }
        
        private static void UpdateVideo()
        {
            int width = Mathf.FloorToInt(_width * _realScale);
            int height = Mathf.FloorToInt(_height * _realScale);
            Video.Resize(width, height);
        }

        private static void CreateEffekseerContext()
        {
            _effekseer = new RmmzEffekseer();
        }

        private static void ApplyCanvasFilter()
        {
            RmmzRoot.Instance.Canvas.SetEnableBlurFilter(true);
        }
        
        private static void ClearCanvasFilter()
        {
            RmmzRoot.Instance.Canvas.SetEnableBlurFilter(false);
        }
    }
}

