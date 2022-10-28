using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Exploratorium;
using Exploratorium.Frontend;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public static class TextureImporter
{
    private static readonly Dictionary<int, Texture2D> ImportCache = new Dictionary<int, Texture2D>();
    private static readonly Queue<int> ImportFifo = new Queue<int>();
    private const int MAXCacheCount = 800;
    private const long MAXCacheSize = 8589934592; // 8 GB
    private static long _cacheSize;

    public static long CacheSize => _cacheSize;

    /// <summary>
    /// Import the image at the given path as a Texture2D.
    /// </summary>
    public static async UniTask<int> Import(string originalPath)
    {
        int hash = originalPath.GetHashCode();
        if (!ImportCache.ContainsKey(hash))
        {
            var format = Regex.Match(Path.GetExtension(originalPath), "(jpg|JPG|jpeg|JPEG)").Success
                ? TextureFormat.DXT1
                : TextureFormat.DXT5;
            
            if (File.Exists(originalPath))
            {
                // OPTION A: load the texture on the main thread (will cause hitches)
                /*
                Texture2D tx = new Texture2D(32, 32, format, false);
                byte[] originalBuffer = File.ReadAllBytes(originalPath);
                tx.LoadImage(originalBuffer);
                */
                
                // OPTION B: defer loading and texture creation to a background worker
                /*
                */
                UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(new Uri(originalPath));
                webRequest.certificateHandler = AcceptAllCertificates.Handler;
                await webRequest.SendWebRequest().ToUniTask();
                
                
                Texture2D tx = DownloadHandlerTexture.GetContent(webRequest);
                // optionally compress the texture to DXT
                //tx.Compress(false);

                while (ImportFifo.Count > 0 && (_cacheSize > MAXCacheSize || ImportCache.Count > MAXCacheCount))
                {
                    int key = ImportFifo.Dequeue();
                    _cacheSize = CacheSize - ImportCache[key].GetRawTextureData().Length;
                    Object.Destroy(ImportCache[key]); // mark as invalid and allow garbage collection
                    ImportCache.Remove(key); // remove the now invalid reference from the cache
                    Debug.Log($"{nameof(SlidePresenter)} : Cached texture evicted for {key}");
                }

                ImportCache[hash] = tx;
                ImportFifo.Enqueue(hash);
                _cacheSize = CacheSize + tx.GetRawTextureData().Length;
                Debug.Log($"{nameof(SlidePresenter)} : Texture cached for {originalPath}");
            }
        }
        else
        {
            Debug.Log($"{nameof(SlidePresenter)} : Cached texture found for {originalPath}");
        }

        return hash;
    }

    public static void Clear()
    {
        ImportCache.Clear();
    }
    public static bool Contains(int hash) => ImportCache.ContainsKey(hash);
    public static bool TryGet(int hash, out Texture2D texture) => ImportCache.TryGetValue(hash, out texture);
    public static Texture2D Get(int hash) => ImportCache[hash];
}