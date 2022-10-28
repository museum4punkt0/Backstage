// //////////////////////////////////////////////////////
// THIS IS PROTOTYPE CODE AND NOT INTENDED FOR PRODUCTION
// TODO: Make this production ready
// //////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Exploratorium.Frontend;
using Exploratorium.Utility;
using UnityAtoms.BaseAtoms;
using UnityEngine;

#pragma warning disable CS0168
namespace Exploratorium.Prototype
{
    [Obsolete]
    public class ContentBrowser : MonoBehaviour
    {
        [SerializeField] private VoidEvent initializeEvent;
        [SerializeField] private AssetPresenter assetPrefab;
        [SerializeField] private SimpleArtefactPresenter artefactPrefab;
        [SerializeField] private SectionPresenter sectionPrefab;
        [SerializeField] private RectTransform assetsContainer;
        [SerializeField] private RectTransform artefactsContainer;
        [SerializeField] private RectTransform sectionsContainer;
        [SerializeField] private VideoPresenter videoViewer;
        [SerializeField] private SlideshowPresenter slideshowViewer;
        [SerializeField] private ModelPresenter modelViewer;
        [SerializeField] private bool initializeOnStart = true;

        private Dictionary<SectionsRecord, RecordPresenter<SectionsRecord>> _sectionPresenters =
            new Dictionary<SectionsRecord, RecordPresenter<SectionsRecord>>();

        private Dictionary<AssetsRecord, RecordPresenter<AssetsRecord>> _assetPresenters =
            new Dictionary<AssetsRecord, RecordPresenter<AssetsRecord>>();

        private Dictionary<ArtefactsRecord, RecordPresenter<ArtefactsRecord>> _artefactPresenters =
            new Dictionary<ArtefactsRecord, RecordPresenter<ArtefactsRecord>>();

        private CancellationTokenSource _cts;


        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
            if (initializeEvent != null)
                initializeEvent.Register(OnInitialize);
        }

        private void OnDisable()
        {
            if (initializeEvent != null)
                initializeEvent.Unregister(OnInitialize);
            _cts.Cancel();
            _cts.Dispose();
        }

        private void OnInitialize()
        {
            try
            {
                Cancel();
                ClearAllViewers();
                UniTask.Void(RebuildAsync, _cts.Token);
            }
            catch (OperationCanceledException e)
            {
                ClearAllViewers();
            }
        }

        private void Cancel()
        {
            Debug.LogWarning($"{nameof(ContentBrowser)} : Cancelling rebuild...");
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }

        private void Start()
        {
            if (!initializeOnStart)
                return;

            OnInitialize();
        }

        private async UniTaskVoid RebuildAsync(CancellationToken ct)
        {
            await UniTask.Delay(200, cancellationToken: ct);
            await UniTask.WaitForEndOfFrame();

            if (DirectusManager.Instance.Connector.Model == null)
            {
                Debug.LogWarning(
                    $"{nameof(ContentBrowser)} : This component should only be activated after a model has been retrieved.");
                return;
            }

            var model = DirectusManager.Instance.Connector.Model;
            float sw = Time.time;
            foreach (var record in model.GetItemsOfType<AssetsRecord>())
            {
                if (ct.IsCancellationRequested)
                    throw new OperationCanceledException();
                if (record.Status != AssetsRecord.StatusChoices.Published)
                    continue;
                AssetPresenter presenter = Instantiate(assetPrefab, assetsContainer);
                presenter.Record = record;
                presenter.Selected += OnAssetSelected;
                _assetPresenters[record] = presenter;

                if (Time.time - sw > 0.002f)
                {
                    await UniTask.WaitForEndOfFrame();
                    sw = Time.time;
                }
            }

            foreach (var record in model.GetItemsOfType<ArtefactsRecord>())
            {
                if (ct.IsCancellationRequested)
                    throw new OperationCanceledException();
                if (record.Status != ArtefactsRecord.StatusChoices.Published)
                    continue;
                SimpleArtefactPresenter presenter = Instantiate(artefactPrefab, artefactsContainer);
                presenter.Record = record;
                presenter.Selected += OnArtefactSelected;
                _artefactPresenters[record] = presenter;

                if (Time.time - sw > 0.002f)
                {
                    await UniTask.WaitForEndOfFrame();
                    sw = Time.time;
                }
            }

            foreach (var record in model.GetItemsOfType<SectionsRecord>().OrderBy(it => $"{it.Parent} {it.Name}"))
            {
                if (ct.IsCancellationRequested)
                    throw new OperationCanceledException();
                if (record.Status != SectionsRecord.StatusChoices.Published)
                    continue;
                SectionPresenter presenter = Instantiate(sectionPrefab, sectionsContainer);
                presenter.Record = record;
                presenter.Selected += OnSectionSelected;
                _sectionPresenters[record] = presenter;

                if (Time.time - sw > 0.002f)
                {
                    await UniTask.WaitForEndOfFrame();
                    sw = Time.time;
                }
            }
        }

        public void ClearSelection()
        {
            ClearPresenters(_assetPresenters.Values);
            ClearPresenters(_artefactPresenters.Values);
        }

        private void ClearPresenters<T>(IEnumerable<RecordPresenter<T>> presenters) where T : DbRecord
        {
            foreach (var presenter in presenters)
                presenter.SetVisible(false);
        }

        private void OnSectionSelected(RecordPresenter<SectionsRecord> selected)
        {
            FilterPresenters(_artefactPresenters, it => it.Section == selected.Record || it.Topic == selected.Record);
        }

        private void OnArtefactSelected(RecordPresenter<ArtefactsRecord> selected)
        {
            if (selected == null)
            {
                Debug.LogWarning("Selected NULL");
                return;
            }

            if (selected.Record?.Assets == null)
            {
                Debug.LogWarning("Assets NULL");
                FilterPresenters(_assetPresenters, it => it.Artefacts.Any(jt => jt == selected.Record));
            }
            else
            {
                var assets = new HashSet<AssetsRecord>(selected.Record.Assets);
                FilterPresenters(_assetPresenters, it => assets.Contains(it));

                if (selected.Record.Layout == ArtefactsRecord.LayoutChoices.Slideshow)
                {
                    ClearAllViewers();
                    slideshowViewer.Show(assets.ToArray());
                }
            }
        }

        private void OnAssetSelected(RecordPresenter<AssetsRecord> selected)
        {
            ClearAllViewers();
            switch (selected.Record.Type)
            {
                case AssetsRecord.TypeChoices.Model:
                    modelViewer.Show(selected.Record);
                    break;
                case AssetsRecord.TypeChoices.Image:
                    slideshowViewer.Show(selected.Record);
                    break;
                case AssetsRecord.TypeChoices.Video:
                    videoViewer.Show(selected.Record);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ClearAllViewers()
        {
            modelViewer.Clear();
            slideshowViewer.Clear();
            // carouselPresenter.Clear();
            videoViewer.Clear();
        }

        private void FilterPresenters<T>(Dictionary<T, RecordPresenter<T>> dict, Func<T, bool> show) where T : DbRecord
        {
            foreach (var pair in dict)
            {
                var record = pair.Key;
                var presenter = pair.Value;
                presenter.SetVisible(show(record));
            }
        }
    }
}