using System;
using System.Collections.Generic;
using System.Linq;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Exploratorium.Utility;
using Markdig;
using Exploratorium.Net.Shared;
using Piglet;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    public class LayerAttribute : PropertyAttribute
    {
        // NOTHING - just oxygen.
    }

    public class ModelPresenter : AssetsPresenter
    {
        [BoxGroup(Constants.ObservedEvents), SerializeField]
        private IntEvent syncIndexEvent;

        [BoxGroup(Constants.ObservedEvents), SerializeField]
        private BoolEvent syncInfoEvent;

        [BoxGroup(Constants.InvokedEvents), SerializeField]
        private IntEvent reportIndexEvent;

        [BoxGroup(Constants.InvokedEvents), SerializeField]
        private BoolEvent reportInfoEvent;

        [BoxGroup(Constants.ReadVariables), SerializeField]
        private PawnRoleVariable roleVariable;

        [BoxGroup(Constants.ReadVariables), SerializeField]
        private BoolReference startWithInfoVisible = new BoolReference(true);

        [Title("Model")]
        [Tooltip("This transform will be detached from the hierarchy to become a root object.")]
        [SerializeField]
        private Transform modelSceneRoot;

        [Tooltip("The transform under which objects will be spawned")] [SerializeField]
        private Transform modelParent;

        [Tooltip("The auto scale parameter passed to the glTF importer")] [SerializeField] [Min(1f)]
        private float autoScaleSize = 1.0f;

        [Title("Loading")] [SerializeField] private CanvasGroup loadingGroup;

        [SerializeField] private Image loadingProgress;
        [SerializeField] private CanvasGroup error;
        [SerializeField] private TMP_Text errorText;

        [Tooltip("Camera that renders the model view")] [SerializeField]
        private Camera modelViewCamera;

        [Tooltip("Layer to be assigned to loaded models")] [SerializeField, Layer]
        private int layer;

        [Title("Navigation")] [SerializeField] private Button nextButton;

        [SerializeField] private Button previousButton;
        [SerializeField] private Toggle pagePrefab;
        [SerializeField] private RectTransform pageContainer;

        [Title("Copyright")] [SerializeField] private Button toggleCopyrightButton;

        [SerializeField] private Image toggleCopyrightMarker;
        [SerializeField] private Openable copyrightGroup;

        [Title("Info")] [SerializeField] private TMP_Text assetTitle;

        [SerializeField] private TMP_Text assetDescription;
        [SerializeField] private TMP_Text assetCopyright;
        [SerializeField] private TMP_Text scientificName;
        [SerializeField] private TMP_Text discoveryYear;
        [SerializeField] private TMP_Text locationCountry;
        [SerializeField] private TMP_Text locationPlace;
        [SerializeField] private Button toggleInfoButton;
        [SerializeField] private Image toggleInfoMarker;
        [SerializeField] private bool alwaysOpenInfoOnNext = false;

        [SerializeField] private Openable[] whileInfoVisible;

        //[SerializeField] private ShowPositionOnSphere geolocation;
        [Title("Fullscreen")] [SerializeField] private Button enterFullscreenButton;

        [SerializeField] private Button exitFullscreenButton;
        [SerializeField] private Openable[] openWhileFullscreen;
        [SerializeField] private Openable[] closedWhileFullscreen;

        [Title("Role")] [SerializeField] private GameObject[] disabledWhileObserver;

        [Title("Motion Sync")] [SerializeField]
        private SyncTransform contentMotionSync;

        [SerializeField] private ExploratoriumPanOrbitComponentScript panOrbitComponent;
        [SerializeField] private CameraFramer cameraFramer;

        private static readonly RuntimeGltfImportCache ImportCache = new RuntimeGltfImportCache();
        private List<Uri> _playlistPaths = new List<Uri>();
        private GameObject _gltfModel;
        private GltfImportTask _importTask;
        private int _prevPageIndex;
        private int _pageIndex;
        private readonly List<Toggle> _pages = new List<Toggle>();
        private bool _isFullscreen;
        private bool _isInfoVisible;
        private bool _isCopyrightVisible;
        private bool _permitSync;
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private bool IsInteractableRole => roleVariable == null ||
                                           roleVariable.Value == PawnRole.Controller ||
                                           roleVariable.Value == PawnRole.Solo ||
                                           roleVariable.Value == PawnRole.None;


        /// whether this presenter is currently in focus
        public bool PermitSync
        {
            get => _permitSync;
            set
            {
                _permitSync = value;
                if (contentMotionSync != null)
                    contentMotionSync.PermitSync = value;
            }
        }

        public bool IsFullscreen => _isFullscreen;

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(modelSceneRoot != null, "modelSceneRoot != null");
            Debug.Assert(syncIndexEvent != null, "syncIndexEvent != null");
            Debug.Assert(syncInfoEvent != null, "syncInfoEvent != null");
            Debug.Assert(reportIndexEvent != null, "reportIndexEvent != null");
            Debug.Assert(reportInfoEvent != null, "reportInfoEvent != null");
            Debug.Assert(roleVariable != null, "roleVariable != null");
            Debug.Assert(modelSceneRoot != null, "modelSceneRoot != null");
            Debug.Assert(modelParent != null, "modelParent != null");
            Debug.Assert(loadingGroup != null, "loadingGroup != null");
            Debug.Assert(loadingProgress != null, "loadingProgress != null");
            Debug.Assert(error != null, "error != null");
            Debug.Assert(errorText != null, "errorText != null");
            Debug.Assert(modelViewCamera != null, "modelViewCamera != null");
            Debug.Assert(disabledWhileObserver.Any(), "disabledWhileObserver.Any()");
            Debug.Assert(nextButton != null, "nextButton != null");
            Debug.Assert(previousButton != null, "previousButton != null");
            Debug.Assert(pagePrefab != null, "pagePrefab != null");
            Debug.Assert(pageContainer != null, "pageContainer != null");
            Debug.Assert(toggleCopyrightButton != null, "toggleCopyrightButton != null");
            Debug.Assert(toggleCopyrightMarker != null, "toggleCopyrightMarker != null");
            Debug.Assert(copyrightGroup != null, "copyrightGroup != null");
            Debug.Assert(assetTitle != null, "assetTitle != null");
            Debug.Assert(assetDescription != null, "assetDescription != null");
            Debug.Assert(assetCopyright != null, "assetCopyright != null");
            Debug.Assert(scientificName != null, "scientificName != null");
            Debug.Assert(discoveryYear != null, "discoveryYear != null");
            Debug.Assert(locationCountry != null, "locationCountry != null");
            Debug.Assert(locationPlace != null, "locationPlace != null");
            Debug.Assert(toggleInfoButton != null, "toggleInfoButton != null");
            Debug.Assert(toggleInfoMarker != null, "toggleInfoMarker != null");
            Debug.Assert(whileInfoVisible != null, "infoGroups != null");
            //Debug.Assert(geolocation != null, "geolocation != null");
            Debug.Assert(enterFullscreenButton != null, "enterFullscreenButton != null");
            Debug.Assert(exitFullscreenButton != null, "exitFullscreenButton != null");
            Debug.Assert(openWhileFullscreen.Any(), "openWhileFullscreen.Any()");
            Debug.Assert(closedWhileFullscreen.Any(), "closedWhileFullscreen.Any()");
            Debug.Assert(contentMotionSync != null, "contentMotionSync != null");
            Debug.Assert(panOrbitComponent != null, "panOrbitComponent != null");
            Debug.Assert(cameraFramer != null, "cameraFramer != null");

            // ensure the camera is rendering the layer we assign to models
            if (modelViewCamera != null)
                modelViewCamera.cullingMask |= 1 << layer;
        }

        protected void Start()
        {
            // to fix lighting issues while the model is under a CanvasScaler
            // we detach the render scene from the canvas hierarchy
            modelSceneRoot.SetParent(null);
            modelSceneRoot.localScale = Vector3.one;
            modelSceneRoot.transform.position = Vector3.zero;
            modelSceneRoot.gameObject.SetActive(false);
            _startPosition = panOrbitComponent.Orbiter.position;
            _startRotation = panOrbitComponent.Orbiter.rotation;
        }

        protected override void OnLocaleChanged(Locale locale)
        {
            if (Records != null && Records.Length > 0)
            {
                var hasAssetTranslation =
                    DirectusExtensions.TryGetTranslation(locale, Records[_pageIndex].Translations,
                        out var assetTranslation);
                if (hasAssetTranslation)
                {
                    if (assetTitle != null)
                        assetTitle.text = $"{assetTranslation.Title}\n{Records[_pageIndex].ScientificName}";
                    if (assetDescription != null)
                    {
                        assetDescription.text = assetDescription.richText
                            ? assetTranslation.Text?
                                .ToHtml()
                                .ParseHtml()
                                .RemoveTags(in HtmlExtensions.SkbgTags)
                                .ReplaceTag(in HtmlExtensions.SkbgTagsToTextMeshProTags)
                                .DocumentNode
                                .InnerHtml
                            : ParsingUtils.GetRawText(assetTranslation.Text);
                    }
                }

                if (Records[_pageIndex].DiscoveryLocation != null)
                {
                    var hasLocationTranslation = DirectusExtensions.TryGetTranslation(
                        locale,
                        Records.First().DiscoveryLocation.Translations,
                        out var locationTranslation
                    );
                    if (hasLocationTranslation)
                    {
                        if (locationCountry != null)
                            locationCountry.text = locationTranslation.Country;
                        if (locationPlace != null)
                            locationCountry.text = locationTranslation.Place;
                    }
                }
            }
        }

        protected override void OnShow(params AssetsRecord[] records)
        {
            Debug.Assert(modelParent != null, "modelParent != null", this);
            if (modelParent == null)
                return;

            OnClear();

            if (disabledWhileObserver.Any())
                disabledWhileObserver.ForEach(it => it.SetActive(IsInteractableRole));

            _playlistPaths = Records
                .Select(it => new Uri(DirectusManager.Instance.Connector.GetLocalFilePath(it.Asset)))
                .ToList();

            for (int i = 0; i < Records.Length; i++)
            {
                Toggle page = Instantiate(pagePrefab, pageContainer);
                page.transform.SetAsLastSibling();
                page.gameObject.name = $"{pagePrefab.name} {Records[i].Id} {Records[i].Name}";
                int slideIndex = i;
                page.onValueChanged.AddListener(isOn => GoToIndex(slideIndex));
                page.SetIsOnWithoutNotify(i == 0);
                _pages.Add(page);
            }

            contentMotionSync.IsConsumer = roleVariable.Value == PawnRole.Observer;
            contentMotionSync.IsPublisher = roleVariable.Value == PawnRole.Controller;
            panOrbitComponent.enabled = roleVariable.Value != PawnRole.Observer;

            GoToIndex(0);

            if (startWithInfoVisible.Value)
                OpenInfo();
            else
                CloseInfo();

            CancelFullscreen();
            /* part of exit fullscreen
            OpenInfo();
            CloseCopyright();
            */
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            nextButton.onClick.AddListener(GoToNext);
            previousButton.onClick.AddListener(GoToPrevious);
            toggleCopyrightButton.onClick.AddListener(ToggleCopyright);
            toggleInfoButton.onClick.AddListener(ToggleInfo);
            enterFullscreenButton.onClick.AddListener(EnterFullscreen);
            exitFullscreenButton.onClick.AddListener(ExitFullscreen);

            if (syncIndexEvent != null)
                syncIndexEvent.Register(OnSyncIndex);
            if (syncInfoEvent != null)
                syncInfoEvent.Register(OnSyncInfo);
            modelSceneRoot.gameObject.SetActive(true);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            nextButton.onClick.RemoveListener(GoToNext);
            previousButton.onClick.RemoveListener(GoToPrevious);
            toggleCopyrightButton.onClick.RemoveListener(ToggleCopyright);
            toggleInfoButton.onClick.RemoveListener(ToggleInfo);
            enterFullscreenButton.onClick.RemoveListener(EnterFullscreen);
            exitFullscreenButton.onClick.RemoveListener(ExitFullscreen);
            if (syncIndexEvent != null)
                syncIndexEvent.Unregister(OnSyncIndex);
            if (syncInfoEvent != null)
                syncInfoEvent.Unregister(OnSyncInfo);
            modelSceneRoot.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_importTask is {State: GltfImportTask.ExecutionState.Running})
                _importTask.MoveNext();
        }

        private void OnImportCompleted(GameObject model)
        {
            _gltfModel = model;
            _gltfModel.transform.parent = modelParent;
            _gltfModel.transform.localPosition = Vector3.zero;
            _gltfModel.GetComponentsInChildren<Transform>().ForEach(it => it.gameObject.layer = layer);
            panOrbitComponent.Orbiter.rotation = _startRotation;
            panOrbitComponent.Orbiter.position = _startPosition;
            cameraFramer.RecalculateFraming();

            /*
        // center the glTF object based on its AABB
        var renderers = _gltfModel.GetComponentsInChildren<MeshRenderer>();
        var objBounds = new Bounds();
        foreach (var r in renderers)
        {
            objBounds.Encapsulate(r.bounds);
            Debug.Log(objBounds);
        }
        Vector3 centerOffset = objBounds.center - _gltfModel.transform.position;
        Debug.Log(centerOffset);
        _gltfModel.transform.localPosition = Vector3.up * objBounds.extents.y;
        */
            if (loadingGroup != null)
                loadingGroup.alpha = 0.0f;
        }

        private void OnImportProgress(GltfImportStep step, int completed, int total)
        {
            if (debug)
                Debug.Log($"{nameof(ModelPresenter)} : glTF import progress {step} {completed}/{total}", this);
            loadingProgress.fillAmount = completed / (float) total;
            if (loadingGroup != null)
                loadingGroup.alpha = 1.0f;
        }

        private void OnImportException(Exception exception)
        {
            Debug.LogException(exception);
            if (error != null)
                error.alpha = 1.0f;
            if (errorText != null)
                errorText.text = exception.Message;
            if (loadingGroup != null)
                loadingGroup.alpha = 0.0f;
        }

        private void OnImportAborted()
        {
            Debug.LogWarning($"{nameof(ModelPresenter)} : glTF import aborted", this);
            if (loadingGroup != null)
                loadingGroup.alpha = 0.0f;
        }


        protected override void OnClear()
        {
            if (_pages != null)
            {
                for (int i = 0; i < _pages.Count; i++)
                    Destroy(_pages[i].gameObject);
                _pages.Clear();
            }

            _playlistPaths.Clear();
            Destroy(_gltfModel);
            _gltfModel = null;
        }

        protected override void OnClose()
        {
            modelSceneRoot.gameObject.SetActive(false);
            contentMotionSync.PermitSync = false;
            contentMotionSync.IsConsumer = false;
            contentMotionSync.IsPublisher = false;
            CancelFullscreen();
        }

        protected override void OnOpen()
        {
            CancelFullscreen();
        }

        private void OnSyncIndex(int ndx)
        {
            if (PermitSync)
                GoToIndexWithoutSync(ndx);
        }

        private void OnSyncInfo(bool isOn)
        {
            if (PermitSync)
                if (isOn)
                    OpenInfoWithoutSync();
                else
                    CloseInfoWithoutSync();
        }

        public void GoToNext()
        {
            if (debug)
                Debug.Log($"{nameof(ModelPresenter)} : Request Next", this);
            GoToIndex(Mod(_pageIndex + 1, Records.Length));
        }

        public void GoToPrevious() => GoToIndex(Mod(_pageIndex - 1, Records.Length));

        public void GoToIndex(int index)
        {
            if (GoToIndexWithoutSync(index) && PermitSync && reportIndexEvent != null)
                reportIndexEvent.Raise(_pageIndex);
        }

        public bool GoToIndexWithoutSync(int index)
        {
            if (debug)
                Debug.Log($"{nameof(ModelPresenter)} : Go to page {index}", this);

            if (Records == null || !Records.Any())
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(ModelPresenter)} : No records assigned (yet); goto command ignored; This is expected behaviour when receiving a sync event before the presenter was initialized due to the undeterministic order of the Flow-FSM's entry actions",
                        this);
                return false;
            }

            panOrbitComponent.Orbiter.rotation = _startRotation;
            panOrbitComponent.Orbiter.position = _startPosition;
            modelSceneRoot.gameObject.SetActive(true);

            _prevPageIndex = _pageIndex;
            _pageIndex = Mod(index, Records.Length);

            // update pagination highlights when a the slide has changed
            if (_pages != null && _pageIndex != _prevPageIndex)
                for (int i = 0; i < _pages.Count; i++)
                    _pages[i].SetIsOnWithoutNotify(_pageIndex == i);

            Destroy(_gltfModel);
            _gltfModel = null;

            if (loadingGroup != null)
                loadingGroup.alpha = 0.0f;
            if (error != null)
                error.alpha = 0.0f;

            var options = new GltfImportOptions
            {
                AutoScale = true,
                AutoScaleSize = autoScaleSize,
                ImportAnimations = false,
                ShowModelAfterImport = true
            };

            _importTask = RuntimeGltfImporter.GetImportTask(_playlistPaths[_pageIndex], options);
            _importTask.OnCompleted += OnImportCompleted;
            _importTask.OnProgress += OnImportProgress;
            _importTask.OnException += OnImportException;
            _importTask.OnAborted += OnImportAborted;

            var record = Records[_pageIndex];

            if (scientificName != null)
                scientificName.text = record.ScientificName;

            if (discoveryYear != null)
                discoveryYear.text = $"{record.DiscoveryYear}";

            if (assetCopyright != null)
                assetCopyright.text = $"{record.Copyright}";

            /* UNUSED
            if (geolocation != null &&
                record.IsArtefact &&
                record.DiscoveryLocation != null)
            {
                geolocation.gameObject.SetActive(true);
                geolocation.ClearAll();
                if (record.DiscoveryLocation.Longitude != 0 && record.DiscoveryLocation.Latitude != 0)
                {
                    geolocation.gameObject.SetActive(true);
                    geolocation.ShowPosition(
                        record.DiscoveryLocation.Longitude,
                        record.DiscoveryLocation.Latitude
                    );
                }
                else
                {
                    geolocation.gameObject.SetActive(false);
                }
            }
            else if (geolocation != null)
            {
                geolocation.gameObject.SetActive(false);
            }
            */

            OnLocaleChanged(LocalizationSettings.SelectedLocale);

            CloseCopyright();
            if (alwaysOpenInfoOnNext)
                OpenInfo();

            return true;
        }

        public void OpenInfo() => reportInfoEvent.Raise(OpenInfoWithoutSync());

        public void CloseInfo() => reportInfoEvent.Raise(CloseInfoWithoutSync());

        public void ToggleInfo() => reportInfoEvent.Raise(ToggleInfoWithoutSync());

        public bool OpenInfoWithoutSync()
        {
            _isInfoVisible = true;
            UpdateInfoView();
            return _isInfoVisible;
        }

        public bool CloseInfoWithoutSync()
        {
            _isInfoVisible = false;
            UpdateInfoView();
            return _isInfoVisible;
        }

        public bool ToggleInfoWithoutSync()
        {
            _isInfoVisible = !_isInfoVisible;
            UpdateInfoView();
            return _isInfoVisible;
        }

        public void ToggleCopyright()
        {
            _isCopyrightVisible = !_isCopyrightVisible;
            UpdateCopyrightView();
        }

        public void CloseCopyright()
        {
            _isCopyrightVisible = false;
            UpdateCopyrightView();
        }

        public void OpenCopyright()
        {
            _isCopyrightVisible = true;
            UpdateCopyrightView();
        }

        private void UpdateCopyrightView()
        {
            if (toggleCopyrightMarker != null)
                toggleCopyrightMarker.gameObject.SetActive(_isCopyrightVisible);

            if (copyrightGroup != null)
            {
                if (_isCopyrightVisible)
                    copyrightGroup.Open();
                else
                    copyrightGroup.Close();
            }
        }

        private void UpdateInfoView()
        {
            if (toggleInfoMarker != null)
                toggleInfoMarker.gameObject.SetActive(_isInfoVisible);

            whileInfoVisible.ForEach(it =>
            {
                if (it != null)
                {
                    if (_isInfoVisible)
                        it.Open();
                    else
                        it.Close();
                }
            });
        }

        public void ToggleFullscreen()
        {
            if (_isFullscreen)
                ExitFullscreen();
            else
                EnterFullscreen();
        }

        public void EnterFullscreen()
        {
            openWhileFullscreen.ForEach(it => it.Open());
            closedWhileFullscreen.ForEach(it => it.Close());
            _isFullscreen = true;
            CloseInfo();
            CloseCopyright();
        }

        public void ExitFullscreen()
        {
            closedWhileFullscreen.ForEach(it => it.Open());
            OpenInfo();
            CloseCopyright();
            CancelFullscreen();
        }

        /// Cancel fullscreen mode without producing side-effects outside
        /// the scope of the fullscreen mode
        private void CancelFullscreen()
        {
            openWhileFullscreen.ForEach(it => it.Close());
            _isFullscreen = false;
        }
    }
}