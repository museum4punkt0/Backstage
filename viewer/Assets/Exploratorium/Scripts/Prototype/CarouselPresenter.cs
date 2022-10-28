// //////////////////////////////////////////////////////
// THIS IS PROTOTYPE CODE AND NOT INTENDED FOR PRODUCTION
// TODO: Make this production ready
// //////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Exploratorium.Frontend;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Exploratorium.Prototype
{
    [Obsolete]
    public class CarouselPresenter : AssetsPresenter
    {
        [SerializeField] private Transform slidesParent;
        [SerializeField] private Image slidePrefab;

        [Min(1)] [SerializeField]
        private int maxCacheCount = 64;

        private List<AssetsRecord> _records = new List<AssetsRecord>();
        private readonly List<Image> _slides = new List<Image>();
        private readonly Dictionary<int, Texture2D> _importCache = new Dictionary<int, Texture2D>();
        private readonly Queue<int> _importFifo = new Queue<int>();

        protected override void OnLocaleChanged(Locale locale)
        {
            // update translated assets and strings
        }

        protected override void OnShow(params AssetsRecord[] records)
        {
            Clear();

            if (records.Length > maxCacheCount)
                throw new Exception($"Can only display up to {maxCacheCount} items");

            _records = records.ToList();
            foreach (var record in _records)
            {
                string path = DirectusManager.Instance.Connector.GetLocalFilePath(record.Asset);
                int hash = path.GetHashCode();


                Texture2D tx;
                if (_importCache.ContainsKey(hash))
                {
                    tx = _importCache[hash];
                }
                else
                {
                    tx = new Texture2D(32, 32, TextureFormat.RGB24, false);
                    if (File.Exists(path))
                    {
                        byte[] buffer = File.ReadAllBytes(path);
                        tx.LoadImage(buffer);
                    }
                }

                // add tx to cache
                if (_importCache.Count > maxCacheCount)
                    _importCache.Remove(_importFifo.Dequeue());
                _importCache[hash] = tx;
                _importFifo.Enqueue(hash);

                // create ui widget
                Image slide = Instantiate(slidePrefab, slidesParent);
                _slides.Add(slide);
                slide.sprite = Sprite.Create(tx, new Rect(0, 0, 1.0f, 1.0f), Vector2.one * 0.5f);
            }
        }

        protected override void OnClear()
        {
            for (int i = 0; i < _slides.Count; i++)
                Destroy(_slides[i]);
            _slides.Clear();
        }

        protected override void OnClose()
        {
        }

        protected override void OnOpen()
        {
        }
    }
}