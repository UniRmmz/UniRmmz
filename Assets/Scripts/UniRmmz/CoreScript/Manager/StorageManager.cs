using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The class that manages storage for saving game data.
    /// </summary>
    public partial class StorageManager
    {
        protected string _basePath = Application.persistentDataPath;
        
        protected SynchronizationContext _mainThreadContext = SynchronizationContext.Current;
        public virtual bool IsLocalMode() => true;
        
        public virtual void SaveObject<T>(string saveName, T data, Action resolve = null, Action reject = null) where T : class
        {
            Task.Run(() =>
            {
                try
                {
                    string json = ObjectToJson(data);
                    byte[] zipData = JsonToZip(json);
                    SaveZip(saveName, zipData);
                    _mainThreadContext.Post(_ => resolve?.Invoke(), null);
                }
                catch (Exception)
                {
                    _mainThreadContext.Post(_ => reject?.Invoke(), null);
                }
            });
        }
        
        public virtual void LoadObject<T>(string saveName, Action<T> resolve = null, Action reject = null) where T : class
        {
            Task.Run(() =>
            {
                try
                {
                    var zipData = LoadZip(saveName);
                    string json = ZipToJson(zipData);
                    var obj = JsonToObject<T>(json);
                    _mainThreadContext.Post(_ => resolve?.Invoke(obj), null);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    _mainThreadContext.Post(_ => reject?.Invoke(), null);
                }
            });
        }

        protected virtual string ObjectToJson<T>(T data) where T : class
        {
            return JsonEx.Stringify(data);
        }
        
        protected virtual T JsonToObject<T>(string json) where T : class
        {
            return JsonEx.Parse<T>(json);
        }
        
        protected virtual byte[] JsonToZip(string json)
        {
            using var memoryStream = new MemoryStream();
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            using (var writer = new StreamWriter(gzipStream, new UTF8Encoding(false)))
            {
                writer.Write(json);
                writer.Flush();
            }
            
            return memoryStream.ToArray();
        }

        protected virtual string ZipToJson(byte[] compressedData)
        {
            using (var memoryStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzipStream))
            {
                return reader.ReadToEnd();
            }
        }
        protected virtual void SaveZip(string fileName, byte[] data)
        {
            string dirPath = FileDirectoryPath();
            string filePath = FilePath(fileName);
            string backupFilePath = fileName + "_";
            
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (File.Exists(backupFilePath))
            {
                File.Delete(backupFilePath);
            }

            if (File.Exists(filePath))
            {
                File.Move(filePath, backupFilePath);
            }

            File.WriteAllBytes(filePath, data);

            if (File.Exists(backupFilePath))
            {
                File.Delete(backupFilePath);
            }
        }
        
        protected virtual byte[] LoadZip(string fileName)
        {
            string filePath = FilePath(fileName);
            return File.ReadAllBytes(filePath);
        }

        public virtual bool Exists(string saveName)
        {
            string filePath = FilePath(saveName);
            return File.Exists(filePath);
        }

        public virtual void Remove(string saveName)
        {
            string filePath = FilePath(saveName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        protected virtual string FileDirectoryPath()
        {
            return Path.Combine(_basePath, "save");
        }
        
        protected virtual string FilePath(string saveName)
        {
            var dir = FileDirectoryPath();
            return Path.GetFullPath(Path.Combine(dir, saveName) + ".rmmzsave");
        }

        public virtual void UpdateForageKeys()
        {
            // not implemented
        }

        public virtual bool ForageKeysUpdated()
        {
            return true;// not implemented
        }

    }
}