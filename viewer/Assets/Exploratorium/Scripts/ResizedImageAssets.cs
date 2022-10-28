using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Directus.Connect.v9;
using UnityEngine;
using UnityEngine.Networking;

namespace Exploratorium
{
    public static class ResizedImageAssets
    {
        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        private static long _cacheSize;
        private const int SizeOfPreviewTextureFormat = 3;
        private const TextureFormat PreviewTextureFormat = TextureFormat.RGB24;
        private const string PreviewSuffix = "_preview";
        private const int LongEdge = 1024;

        public static long CacheSize => _cacheSize;

        public static Texture2D GetTx(string pathToOriginal) => _textures[pathToOriginal];
        public static Sprite GetSprite(string pathToOriginal)
        {
            if (!_sprites.ContainsKey(pathToOriginal))
                Debug.LogError($"Key not found '{pathToOriginal}'");
            return _sprites[pathToOriginal];
        }

        public static bool Contains(string pathToOriginal) => _textures.ContainsKey(pathToOriginal) && _sprites.ContainsKey(pathToOriginal);

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        private static void Init()
        {
            _textures = new Dictionary<string, Texture2D>();
        }

        public static async UniTask AddAsync(string pathToOriginal, bool cropToSquare = false)
        {
            var originalFileInfo = new FileInfo(pathToOriginal);
            var originalDirName = Path.GetDirectoryName(pathToOriginal);
            var originalFileName = Path.GetFileNameWithoutExtension(pathToOriginal);
            var previewFileInfo = new FileInfo(GetPreviewFilePath(originalDirName, originalFileName));
            if (string.IsNullOrWhiteSpace(originalDirName) || string.IsNullOrWhiteSpace(originalFileName))
            {
                Debug.LogError($"{nameof(ResizedImageAssets)} : Not a valid path '{pathToOriginal}'");
                return;
            }

            Texture2D previewTx;// = new Texture2D(LongEdge, LongEdge, PreviewTextureFormat, false);
            bool isPreviewUpToDate = previewFileInfo.Exists && previewFileInfo.Length > 0 &&
                                     originalFileInfo.LastWriteTimeUtc < previewFileInfo.LastWriteTimeUtc;
            if (isPreviewUpToDate)
            {
                // load resized
                UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(new Uri(previewFileInfo.FullName));
                webRequest.certificateHandler = AcceptAllCertificates.Handler;
                await webRequest.SendWebRequest().ToUniTask();
                previewTx = DownloadHandlerTexture.GetContent(webRequest);
                //previewTx.LoadImage(File.ReadAllBytes(previewFileInfo.FullName));
            }
            else
            {
                // create resized
                previewTx = await ResizeImage.GetResized(pathToOriginal, new Vector2Int(LongEdge, LongEdge));
                if (!previewTx)
                    return;

                // save resized
                ResizeImage.Save(previewTx, previewFileInfo.FullName);
                var pfi = new FileInfo(previewFileInfo.FullName);
                Debug.Log($"{nameof(ResizedImageAssets)} : '{originalFileInfo.Name}' preview stored on disk ({pfi.Length} bytes)");
            }

            // store resized textures
            _textures[pathToOriginal] = previewTx;
            _sprites[pathToOriginal] = Sprite.Create(
                previewTx,
                new Rect(0, 0, previewTx.width, previewTx.height),
                Vector2.one * 0.5f
            );
            _cacheSize += previewTx.GetRawTextureData().Length;
            //Debug.Log($"{nameof(ResizedPictures)} : '{originalFileInfo.Name}' preview stored in memory, total cache size is now {_cacheSize / 1048576:F1} MB");
        }

        private static string GetPreviewFilePath(string dirName, string fileName) =>
            Path.Combine(dirName, $"{fileName}{PreviewSuffix}.jpg");
    }
}