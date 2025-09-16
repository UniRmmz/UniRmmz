
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace UniRmmz
{
    /// <summary>
    /// The static class that defines utility methods.
    /// </summary>
    public static partial class Utils
    {
        /// <summary>
        /// The name of the RPG Maker. "MZ" in the current version.
        /// </summary>
        public const string RpgMakerName = "MZ";

        /// <summary>
        /// The version of the RPG Maker.
        /// </summary>
        public const string RpgMakerVersion = "1.7.0";

        /// <summary>
        /// Checks whether the current RPG Maker version is greater than or equal to the given version.
        /// </summary>
        /// <param name="version">version - The "x.x.x" format string to compare.</param>
        /// <returns>True if the current version is greater than or equal to the given version.</returns>
        public static bool CheckRMVersion(string version)
        {
            var array1 = RpgMakerVersion.Split(".");
            var array2 = version.Split(".");
            for (int i = 0; i < array1.Length; ++i)
            {
                int v1 = int.Parse(array1[i]);
                int v2 = int.Parse(array2[i]);
                if (v1 > v2)
                {
                    return true;
                }
                else if (v1 < v2)
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Checks whether the option is in the query string.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>True if the option is in the query string.</returns>
        public static bool IsOptionValid(string name)
        {
            return false;// not implemented
        }

        /// <summary>
        /// Encodes a URI component without escaping slash characters.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>Encoded string.</returns>
        public static string EncodeUri(string str)
        {
            return Uri.EscapeDataString(str).Replace("%2F", "/");
        }

        /// <summary>
        /// Gets the filename that does not include subfolders.
        /// </summary>
        /// <param name="filename">The filename with subfolders.</param>
        /// <returns>The filename without subfolders.</returns>
        public static string ExtractFileName(string filename)
        {
            return Path.GetFileName(filename);
        }

        /// <summary>
        /// Escapes special characters for HTML.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>Escaped string.</returns>
        public static string EscapeHtml(string str)
        {
            return System.Net.WebUtility.HtmlEncode(str);
        }

        /// <summary>
        /// Checks whether the string contains any Arabic characters.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>True if the string contains any Arabic characters.</returns>
        public static bool ContainsArabic(string str)
        {
            return Regex.IsMatch(str, "[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF]");
        }

        private static bool _hasEncryptedImages;
        private static bool _hasEncryptedAudio;
        private static string _encryptionKey = "";

        /// <summary>
        /// Sets information related to encryption.
        /// </summary>
        /// <param name="hasImages">Whether the image files are encrypted.</param>
        /// <param name="hasAudio">Whether the audio files are encrypted.</param>
        /// <param name="key">The encryption key.</param>
        public static void SetEncryptionInfo(bool hasImages, bool hasAudio, string key)
        {
            _hasEncryptedImages = hasImages;
            _hasEncryptedAudio = hasAudio;
            _encryptionKey = key;
        }

        /// <summary>
        /// Checks whether the image files in the game are encrypted.
        /// </summary>
        /// <returns>True if the image files are encrypted.</returns>
        public static bool HasEncryptedImages() => _hasEncryptedImages;
        
        /// <summary>
        /// Checks whether the audio files in the game are encrypted.
        /// </summary>
        /// <returns>True if the audio files are encrypted.</returns>
        public static bool HasEncryptedAudio() => _hasEncryptedAudio;

        /// <summary>
        /// Decrypts encrypted data.
        /// </summary>
        /// <param name="source">The data to be decrypted.</param>
        /// <returns>The decrypted data.</returns>
        public static byte[] DecryptArrayBuffer(byte[] source)
        {
            if (source.Length < 16) throw new Exception("Decryption error");
            string headerHex = BitConverter.ToString(source.Take(16).ToArray()).Replace("-", ",");
            if (headerHex != "52,50,47,4D,56,00,00,00,00,03,01,00,00,00,00,00")
            {
                throw new Exception("Decryption error");
            }
            
            byte[] body = source.Skip(16).ToArray();
            byte[] keyBytes = Enumerable.Range(0, _encryptionKey.Length / 2)
                                        .Select(i => Convert.ToByte(_encryptionKey.Substring(i * 2, 2), 16))
                                        .ToArray();

            for (int i = 0; i < Math.Min(16, body.Length); i++)
            {
                body[i] ^= keyBytes[i];
            }

            return body;
        }

        /// <summary>
        /// Checks whether the browser can play webm files.
        /// </summary>
        /// <returns>True if the browser can play webm files.</returns>
        public static bool CanPlayWebm()
        {
            return true;
        }
    }    
}
