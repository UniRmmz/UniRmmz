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

        protected Dictionary<string, Bitmap> _cache = new();
        protected Dictionary<string, Bitmap> _system = new();
        protected Bitmap _emptyTexture;

        public virtual Bitmap LoadAnimation(string filename) => LoadBitmap("img/animations", filename);
        public virtual Bitmap LoadBattleback1(string filename) => LoadBitmap("img/battlebacks1", filename);
        public virtual Bitmap LoadBattleback2(string filename) => LoadBitmap("img/battlebacks2", filename);
        public virtual Bitmap LoadEnemy(string filename) => LoadBitmap("img/enemies", filename);
        public virtual Bitmap LoadCharacter(string filename) => LoadBitmap("img/characters", filename);
        public virtual Bitmap LoadFace(string filename) => LoadBitmap("img/faces", filename);
        public virtual Bitmap LoadParallax(string filename) => LoadBitmap("img/parallaxes", filename);
        public virtual Bitmap LoadPicture(string filename) => LoadBitmap("img/pictures", filename);
        public virtual Bitmap LoadSvActor(string filename) => LoadBitmap("img/sv_actors", filename);
        public virtual Bitmap LoadSvEnemy(string filename) => LoadBitmap("img/sv_enemies", filename);
        public virtual Bitmap LoadSystem(string filename) => LoadBitmap("img/system", filename);
        public virtual Bitmap LoadTileset(string filename) => LoadBitmap("img/tilesets", filename);
        public virtual Bitmap LoadTitle1(string filename) => LoadBitmap("img/titles1", filename);
        public virtual Bitmap LoadTitle2(string filename) => LoadBitmap("img/titles2", filename);

        protected virtual Bitmap LoadBitmap(string folder, string filename)
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

        protected virtual Bitmap LoadBitmapFromUrl(string url)
        {
            var cache = url.Contains("/system/") ? _system : _cache;
            if (!cache.ContainsKey(url))
            {
                cache[url] = Bitmap.Load(url);
            }

            return cache[url];
        }

        public virtual void Clear()
        {
            foreach (var pair in _cache)
            {
                pair.Value.Dispose();
            }

            _cache.Clear();
        }

        public virtual bool IsReady()
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

        protected virtual void ThrowLoadError(Bitmap bitmap)
        {
            throw new RmmzError("LoadError", bitmap.Url, () => bitmap.Retry());
        }

        public virtual bool IsObjectCharacter(string filename)
        {
            var name = Utils.ExtractFileName(filename);
            return name.StartsWith("!");
        }

        public virtual bool IsBigCharacter(string filename)
        {
            var name = Utils.ExtractFileName(filename);
            var match = System.Text.RegularExpressions.Regex.Match(name, @"^[!$]+");
            return match.Success && match.Value.Contains("$");
        }

        public virtual bool IsZeroParallax(string filename)
        {
            var name = Utils.ExtractFileName(filename);
            return name.StartsWith("!");
        }
    }
}
