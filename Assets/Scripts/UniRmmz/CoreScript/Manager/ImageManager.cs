using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public partial class ImageManager
    {
        public readonly int IconWidth = 32;
        public readonly int IconHeight = 32;
        public readonly int FaceWidth = 144;
        public readonly int FaceHeight = 144;

        private Dictionary<string, Bitmap> _cache = new();
        private Dictionary<string, Bitmap> _system = new();
        private Bitmap _emptyTexture;

        public Bitmap LoadAnimation(string filename) => LoadBitmap("img/animations", filename);
        public Bitmap LoadBattleback1(string filename) => LoadBitmap("img/battlebacks1", filename);
        public Bitmap LoadBattleback2(string filename) => LoadBitmap("img/battlebacks2", filename);
        public Bitmap LoadEnemy(string filename) => LoadBitmap("img/enemies", filename);
        public Bitmap LoadCharacter(string filename) => LoadBitmap("img/characters", filename);
        public Bitmap LoadFace(string filename) => LoadBitmap("img/faces", filename);
        public Bitmap LoadParallax(string filename) => LoadBitmap("img/parallaxes", filename);
        public Bitmap LoadPicture(string filename) => LoadBitmap("img/pictures", filename);
        public Bitmap LoadSvActor(string filename) => LoadBitmap("img/sv_actors", filename);
        public Bitmap LoadSvEnemy(string filename) => LoadBitmap("img/sv_enemies", filename);
        public Bitmap LoadSystem(string filename) => LoadBitmap("img/system", filename);
        public Bitmap LoadTileset(string filename) => LoadBitmap("img/tilesets", filename);
        public Bitmap LoadTitle1(string filename) => LoadBitmap("img/titles1", filename);
        public Bitmap LoadTitle2(string filename) => LoadBitmap("img/titles2", filename);

        private Bitmap LoadBitmap(string folder, string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string url = $"{folder}/{filename}.png";
                return LoadBitmapFromUrl(url);
            }
            else
            {
                _emptyTexture ??= new Bitmap(1, 1);
                return _emptyTexture;
            }
        }

        private Bitmap LoadBitmapFromUrl(string url)
        {
            var cache = url.Contains("/system/") ? _system : _cache;
            if (!cache.ContainsKey(url))
            {
                cache[url] = Bitmap.Load(url);
            }

            return cache[url];
        }

        public void Clear()
        {
            foreach (var pair in _cache)
            {
                pair.Value.Dispose();
            }

            _cache.Clear();
        }

        public bool IsReady()
        {
            foreach (var dict in new[] { _cache, _system })
            {
                foreach (var pair in dict)
                {
                    var bitmap = pair.Value;
                    if (bitmap.IsError())
                    {
                        ThrowLoadError(bitmap);
                    }

                    if (!bitmap.IsReady())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ThrowLoadError(Bitmap bitmap)
        {
            throw new RmmzError("LoadError", bitmap.Url, () => bitmap.Retry());
        }

        public bool IsObjectCharacter(string filename)
        {
            var name = Utils.ExtractFileName(filename);
            return name.StartsWith("!");
        }

        public bool IsBigCharacter(string filename)
        {
            var name = Utils.ExtractFileName(filename);
            var match = System.Text.RegularExpressions.Regex.Match(name, @"^[!$]+");
            return match.Success && match.Value.Contains("$");
        }

        public bool IsZeroParallax(string filename)
        {
            var name = Utils.ExtractFileName(filename);
            return name.StartsWith("!");
        }
    }
}
