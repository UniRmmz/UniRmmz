using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace UniRmmz
{
    /// <summary>
    /// The basic object that represents an image.
    /// </summary>
    public partial class Bitmap : IDisposable
    {
        private Texture2D _image;
        private RenderTexture _baseTexture;
        private string _url;
        private int _paintOpacity = 255;
        private bool _smooth = true;
        private Color[] _pixelCaches;
        private bool _isPixelCachesDirty = true;
        private RmmzTextRenderer.Context _textRenderContext = new();
        private event Action<Bitmap> _loadListeners = delegate { };
        
        private enum LoadingState
        {
            None,
            Loading,
            Loaded,
            Error
        }

        private LoadingState _loadingState = LoadingState.None;

        /// <summary>
        /// The width of the bitmap.
        /// </summary>
        public int Width
        {
            get => _baseTexture?.width ?? 0;
        }
        
        /// <summary>
        /// The height of the bitmap.
        /// </summary>
        public int Height
        {
            get => _baseTexture?.height ?? 0;
        }

        public string Url
        {
            get => _url;
        }

        /// <summary>
        /// Whether the smooth scaling is applied.
        /// </summary>
        public bool Smooth
        {
            get => _smooth;
            set
            {
                if (_smooth != value)
                {
                    _smooth = value;
                    UpdateScaleMode();
                }
            }
        }
        
        /// <summary>
        /// The opacity of the drawing object in the range (0, 255).
        /// </summary>
        public int PaintOpacity
        {
            get => _paintOpacity;
            set
            {
                _paintOpacity = value;
                _textRenderContext.PaintOpacity = _paintOpacity;
            }
        }
        
        /// <summary>
        /// Destroys the bitmap.
        /// </summary>
        public void Dispose()
        {
            MonoBehaviour.Destroy(_image);
            _image = null;
            if (_baseTexture != null)
            {
                _baseTexture.Release();
                MonoBehaviour.Destroy(_baseTexture);    
            }
            _baseTexture = null;
            _pixelCaches = null;
        }

        /// <summary>
        /// Resizes the bitmap.
        /// </summary>
        /// <param name="width">The new width of the bitmap.</param>
        /// <param name="height">The new height of the bitmap.</param>
        public void Resize(int width, int height)
        {
            width = Math.Max(width, 1);
            height = Math.Max(height, 1);
            CreateBaseTexture(width, height);
        }

        /// <summary>
        /// Performs a block transfer.
        /// </summary>
        /// <param name="source">The bitmap to draw.</param>
        /// <param name="sx">The x coordinate in the source.</param>
        /// <param name="sy">The y coordinate in the source.</param>
        /// <param name="sw">The width of the source image.</param>
        /// <param name="sh">The height of the source image.</param>
        /// <param name="dx">The x coordinate in the destination.</param>
        /// <param name="dy">The y coordinate in the destination.</param>
        /// <param name="dw">The width to draw the image in the destination.</param>
        /// <param name="dh">The height to draw the image in the destination.</param>
        public void Blt(Bitmap source, int sx, int sy, int sw, int sh, int dx, int dy, int dw = 0, int dh = 0)
        {
            if (dw == 0)
            {
                dw = sw;
            }
            if (dh == 0)
            {
                dh = sh;
            }
            float alpha = (float)_paintOpacity / 255;
            RmmzRoot.Instance.BitmapRenderer.Blt(source._baseTexture, new Rect(sx, sy, sw, sh), new Rect(dx, dy, dw, dh), alpha, _baseTexture);
            _isPixelCachesDirty = true;
        }

        /// <summary>
        /// The face name of the font.
        /// </summary>
        public string FontFace
        {
            get => _textRenderContext.FontFace;
            set => _textRenderContext.FontFace = value;
        }

        /// <summary>
        /// The size of the font in pixels.
        /// </summary>
        public int FontSize
        {
            get => _textRenderContext.FontSize;
            set => _textRenderContext.FontSize = value;
        }

        /// <summary>
        /// Whether the font is bold.
        /// </summary>
        public bool FontBold
        {
            get => _textRenderContext.FontBold;
            set => _textRenderContext.FontBold = value;
        }

        /// <summary>
        /// Whether the font is italic.
        /// </summary>
        public bool FontItalic
        {
            get => _textRenderContext.FontItalic;
            set => _textRenderContext.FontItalic = value;
        }

        /// <summary>
        /// The color of the text
        /// </summary>
        public Color TextColor
        {
            get => _textRenderContext.TextColor;
            set => _textRenderContext.TextColor = value;
        }
        
        /// <summary>
        /// The color of the outline of the text
        /// </summary>
        public Color OutlineColor
        {
            get => _textRenderContext.OutlineColor;
            set => _textRenderContext.OutlineColor = value;
        }
        
        /// <summary>
        /// The width of the outline of the text.
        /// </summary>
        public int OutlineWidth
        {
            get => _textRenderContext.OutlineWidth;
            set => _textRenderContext.OutlineWidth = value;
        }

        /// <summary>
        /// Checks whether the bitmap is ready to render.
        /// </summary>
        /// <returns>True if the bitmap is ready to render.</returns>
        public bool IsReady()
        {
            return _loadingState switch
            {
                LoadingState.None => true,
                LoadingState.Loaded => true,
                _ => false
            };
        }

        /// <summary>
        /// Checks whether a loading error has occurred.
        /// </summary>
        /// <returns>True if a loading error has occurred.</returns>
        public bool IsError()
        {
            return _loadingState == LoadingState.Error;
        }

        /// <summary>
        /// The base texture that holds the image.
        /// </summary>
        public Texture BaseTexture => _baseTexture;

        public Bitmap(int width = 0, int height = 0)
        {
            if (width > 0 && height > 0)
            {
                _baseTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                _baseTexture.Create();
                Clear();
            }
        }

        /// <summary>
        /// Loads a image file.
        /// </summary>
        /// <param name="path">The image path of the texture.</param>
        /// <returns></returns>
        public static Bitmap Load(string path)
        {
            var bitmap = new Bitmap(1, 1);
            bitmap._url = path;
            bitmap.StartLoading();
            return bitmap;
        }

        /// <summary>
        /// Takes a snapshot of the game screen.
        /// </summary>
        /// <returns>The new bitmap object.</returns>
        public static Bitmap Snap(Scene_Base _)
        {
            int width = Graphics.Width;
            int height = Graphics.Height;
            var bitmap = new Bitmap(width, height);
            RmmzRoot.Instance.Canvas.Snap(bitmap._baseTexture);
            return bitmap;
        }

        /// <summary>
        /// Returns pixel color at the specified point.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel in the bitmap.</param>
        /// <param name="y">The y coordinate of the pixel in the bitmap.</param>
        public Color GetPixel(int x, int y)
        {
            UpdatePixelCacheIfNeed();
            y = Height - 1 - y;
            return _pixelCaches[y * Width + x];
        }
        
        /// <summary>
        /// Clears the specified rectangle.
        /// </summary>
        /// <param name="x">The x coordinate for the upper-left corner.</param>
        /// <param name="y">The y coordinate for the upper-left corner.</param>
        /// <param name="width">The width of the rectangle to fill.</param>
        /// <param name="height">The height of the rectangle to fill.</param>
        public void ClearRect(float x, float y, float width, float height)
        {
            var clearColor = new Color(0, 0, 0, 0);
            RmmzRoot.Instance.BitmapRenderer.ClearRect(new Rect(x, y, width, height), clearColor, _baseTexture);
            _isPixelCachesDirty = true;
        }

        /// <summary>
        /// Clears the entire bitmap.
        /// </summary>
        public void Clear()
        {
            ClearRect(0, 0, Width, Height);
        }

        /// <summary>
        /// Fills the specified rectangle.
        /// </summary>
        /// <param name="x">The x coordinate for the upper-left corner.</param>
        /// <param name="y">The y coordinate for the upper-left corner.</param>
        /// <param name="width">The width of the rectangle to fill.</param>
        /// <param name="height">The height of the rectangle to fill.</param>
        /// <param name="color">The color of the rectangle in CSS format.</param>
        public void FillRect(float x, float y, float width, float height, Color color)
        {
            color.a *= (float)PaintOpacity / 255;
            RmmzRoot.Instance.BitmapRenderer.FillRect(new Rect(x, y, width, height), color, _baseTexture);
            _isPixelCachesDirty = true;
        }

        /// <summary>
        /// Fills the entire bitmap.
        /// </summary>
        /// <param name="color">The color of the rectangle in CSS format.</param>
        public void FillAll(Color color)
        {
            color.a *= (float)PaintOpacity / 255;
            RmmzRoot.Instance.BitmapRenderer.FillRect(new Rect(0, 0, Width, Height), color, _baseTexture);
            _isPixelCachesDirty = true;
        }

        /// <summary>
        /// Draws the rectangle with a gradation.
        /// </summary>
        /// <param name="x">The x coordinate for the upper-left corner.</param>
        /// <param name="y">The y coordinate for the upper-left corner.</param>
        /// <param name="width">The width of the rectangle to fill.</param>
        /// <param name="height">The height of the rectangle to fill.</param>
        /// <param name="color1">The gradient starting color.</param>
        /// <param name="color2">The gradient ending color.</param>
        /// <param name="vertical">Whether the gradient should be draw as vertical or not.</param>
        public void GradientFillRect(float x, float y, float width, float height, Color color1, Color color2, bool vertical = false)
        {
            color1.a *= (float)PaintOpacity / 255;
            color2.a *= (float)PaintOpacity / 255;
            RmmzRoot.Instance.BitmapRenderer.GradientFillRect(new Rect(x, y, width, height), color1, color2, vertical, _baseTexture);
            _isPixelCachesDirty = true;
        }
        
        public enum TextAlign
        {
            Left,
            Center,
            Right
        }
        
        /// <summary>
        /// Draws the outline text to the bitmap.
        /// </summary>
        /// <param name="text">The text that will be drawn.</param>
        /// <param name="x">The x coordinate for the left of the text.</param>
        /// <param name="y">The y coordinate for the top of the text.</param>
        /// <param name="maxWidth">The maximum allowed width of the text.</param>
        /// <param name="lineHeight">The height of the text line.</param>
        /// <param name="align">The alignment of the text.</param>
        public void DrawText(string text, int x, int y, int maxWidth, int lineHeight, TextAlign align = TextAlign.Left)
        {
            RmmzRoot.Instance.BitmapRenderer.DrawText(text, x, y, maxWidth, lineHeight, align, _textRenderContext, _baseTexture);
            _isPixelCachesDirty = true;
        }

        /// <summary>
        /// Returns the width of the specified text.
        /// </summary>
        /// <param name="text">The text to be measured.</param>
        /// <returns>The width of the text in pixels.</returns>
        public int MeasureTextWidth(string text)
        {
            return RmmzRoot.Instance.BitmapRenderer.MeasureTextWidth(text, _textRenderContext);            
        }
        
        /// <summary>
        /// Draws a bitmap in the shape of a circle.
        /// </summary>
        /// <param name="x">The x coordinate based on the circle center.</param>
        /// <param name="y">The y coordinate based on the circle center.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle</param>
        public void DrawCircle(int x, int y, int radius, Color color)
        {
            RmmzRoot.Instance.BitmapRenderer.DrawCircle(x, y, radius, color, _baseTexture);
            _isPixelCachesDirty = true;
        }

        /// <summary>
        /// Adds a callback function that will be called when the bitmap is loaded.
        /// </summary>
        /// <param name="listener">callback</param>
        public void AddLoadListener(Action<Bitmap> listener)
        {
            if (!IsReady()) 
            {
                _loadListeners += listener;
            }
            else
            {
                listener.Invoke(this);
            }
        }

        public void Retry()
        {
            StartLoading();
        }

        private static async Task LoadImageAsync(string filePath, Action<Texture2D> callback)
        {
            // TODO iamge load cache
            filePath = Path.Combine(Rmmz.RootPath, filePath);
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
            {
                var request = uwr.SendWebRequest();
                while (!request.isDone) await Task.Yield(); 

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    callback?.Invoke(texture);
                }
                else
                {
                    Debug.LogError($"画像の読み込みに失敗: {uwr.error}");
                    callback?.Invoke(null);
                }
            }
        }
        
        private void StartLoading()
        {
            _loadingState = LoadingState.Loading;
            if (Utils.HasEncryptedImages())
            {
                throw new NotImplementedException();
            }
            else
            {
                _ = LoadImageAsync(_url, (Texture2D image) =>
                {
                    _image = image;
                    OnLoad();
                });
            }
        }

        private void CreateBaseTexture(Texture2D image)
        {
            // TODO 必要になるまではテクスチャの複製を遅延させたい
            CreateBaseTexture(image.width, image.height);
            UnityEngine.Graphics.Blit(image, _baseTexture);
        }
        
        private void CreateBaseTexture(int width, int height)
        {
            var format = RenderTextureFormat.ARGB32;
            if (_baseTexture != null)
            {
                _baseTexture.Release();
                _baseTexture.descriptor = new RenderTextureDescriptor(width, height, format);
            }
            else
            {
                _baseTexture = new RenderTexture(width, height, 0, format);
            }
            _baseTexture.Create();
            _isPixelCachesDirty = true;
            this.UpdateScaleMode();
        }

        private void UpdateScaleMode()
        {
            if (_baseTexture != null)
            {
                if (_smooth)
                {
                    _baseTexture.filterMode = FilterMode.Bilinear;
                }
                else
                {
                    _baseTexture.filterMode = FilterMode.Point;
                }
            }
        }
        private void OnLoad()
        {
            if (Utils.HasEncryptedImages())
            {
                throw new NotImplementedException();
            }
            this._loadingState = LoadingState.Loaded;
            this.CreateBaseTexture(_image);
            this.CallLoadListeners();
        }
        
        private void CallLoadListeners() 
        {
            _loadListeners.Invoke(this);
            _loadListeners = delegate { };
        }

        private void UpdatePixelCacheIfNeed()
        {
            if (!_isPixelCachesDirty)
            {
                return;
            }

            var oldRt = RenderTexture.active;
            RenderTexture.active = _baseTexture;
            
            var tmpTex = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
            tmpTex.ReadPixels(new Rect(0, 0, _baseTexture.width, _baseTexture.height), 0, 0);
            tmpTex.Apply();
            _pixelCaches = tmpTex.GetPixels();
            
            Object.Destroy(tmpTex);
            _isPixelCachesDirty = false;
            RenderTexture.active = oldRt;
        }
    }
}