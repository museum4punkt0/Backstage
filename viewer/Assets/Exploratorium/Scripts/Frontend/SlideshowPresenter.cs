using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DigitalRubyShared;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Exploratorium.Net.Shared;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using Unity.Netcode;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;

namespace Exploratorium.Frontend
{
    public class SlideshowPresenter : AssetsPresenter
    {
        [BoxGroup(Constants.ObservedEvents), SerializeField]
        private FloatEvent timeSyncEvent;

        [BoxGroup(Constants.ObservedEvents), SerializeField]
        private BoolEvent fullscreenSyncEvent;

        [BoxGroup(Constants.ObservedEvents), SerializeField]
        private BoolEvent infoSyncEvent;

        [BoxGroup(Constants.InvokedEvents), SerializeField]
        private FloatEvent timeReportEvent;

        [BoxGroup(Constants.InvokedEvents), SerializeField]
        private BoolEvent fullscreenReportEvent;

        [BoxGroup(Constants.InvokedEvents), SerializeField]
        private BoolEvent infoReportEvent;

        [BoxGroup(Constants.ReadVariables), SerializeField]
        private PawnRoleVariable roleVariable;

        [BoxGroup(Constants.ReadVariables), SerializeField]
        private BoolReference autoPlay = new BoolReference(true);

        [BoxGroup(Constants.ReadVariables), SerializeField]
        private BoolReference startWithInfoVisible = new BoolReference(true);

        [FormerlySerializedAs("disableWhileObserver")]
        [Header("Role Dependent")]
        [Tooltip("This list should reference all components that would fight inputs received from network sync")]
        [SerializeField] private Behaviour[] disableBehaviourWhileObserver;
        
        [SerializeField] private GameObject[] disableObjectWhileObserver;

        [Header("Controls")] [SerializeField]
        private RectTransform slidesParent;

        [SerializeField] private SlidePresenter slidePrefab;
        [SerializeField] private FloatReference slideDuration = new FloatReference(10f);
        [SerializeField] private float transitionDuration = 0.35f;
        [SerializeField] private Image slideTimerFill;
        [SerializeField] private CanvasGroup controls;
        [SerializeField] private GameObject activeWhilePlaying;
        [SerializeField] private GameObject activeWhilePaused;
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button copyrightButton;
        [SerializeField] private Image copyrightMarker;
        [SerializeField] private Button infoButton;
        [SerializeField] private Image infoMarker;

        [Header("Pagination")] [SerializeField]
        private RectTransform pageMarker;

        [ValidateInput(nameof(IsValidContainer),
            "Page Container is expected to have a Horizontal Layout Group component", InfoMessageType.Warning)]
        [SerializeField]
        private RectTransform pageContainer;

        [SerializeField] private Toggle pagePrefab;
        private HorizontalLayoutGroup _pageLayout;

        bool IsValidContainer => pageContainer.GetComponent<HorizontalLayoutGroup>();

        [Header("Transitions")]
        SwipeGestureRecognizerDirection _backGesture = SwipeGestureRecognizerDirection.Up;

        SwipeGestureRecognizerDirection _nextGesture = SwipeGestureRecognizerDirection.Down;
        [SerializeField] private Transition transitionType = Transition.Slide;

        [ShowIf(nameof(transitionType), Transition.Blend)] [SerializeField]
        private RectTransform.Axis transitionAxis = RectTransform.Axis.Vertical;

        [Header("Fullscreen")] [SerializeField]
        private Image fullscreenImage;

        //[SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private Vector2 minFullscreenSize = Vector2.one * 1080f;
        [SerializeField] private Vector2 maxFullscreenSize = Vector2.one * 3840f;

        [SerializeField] private Button enterFullscreenButton;
        [SerializeField] private Button exitFullscreenButton;

        [SerializeField] private SyncTransform fullscreenContentSync;
        [SerializeField] private Openable[] openWhileFullscreen;
        [SerializeField] private Openable[] closedWhileFullscreen;

        [Header("Loading")] [SerializeField]
        private Image builderProgress;

        [SerializeField] private TMP_Text cacheSize;

        [Min(1)] [SerializeField]
        private int maxCacheCount = 512;

        private List<AssetsRecord> _records = new List<AssetsRecord>();
        private readonly List<SlidePresenter> _slides = new List<SlidePresenter>();

        /// <summary>
        /// Timecode when the slideshow has started, this value drives most calculations for slide transitions and
        /// selection of the current slide
        /// </summary>
        public float ShowStart => _showStart;

        public bool IsPaused => _isPaused || roleVariable.Value == PawnRole.Observer;

        private bool IsInteractableRole => roleVariable == null ||
                                           roleVariable.Value == PawnRole.Controller ||
                                           roleVariable.Value == PawnRole.Solo ||
                                           roleVariable.Value == PawnRole.None;

        /// whether this presenter is currently in focus
        public bool PermitSync { get; set; }

        private int _slideNdx;
        private int _prevSlideNdx;
        private bool _isPaused;
        private int _nextSlideIdx;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private float _builderProgress = 0;
        private readonly List<Toggle> _pages = new List<Toggle>();
        private bool _pausedBeforeFullscreen;
        private bool _isInfoVisible;
        private bool _isCopyrightVisible;
        private float _showStart;

        // this value is supposed to be network-/server-time
        private float _networkTime;
        private float _prevNetworkTime;


        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(timeSyncEvent != null, "timeSyncEvent != null", this);
            Debug.Assert(fullscreenSyncEvent != null, "fullscreenSyncEvent != null", this);
            Debug.Assert(infoSyncEvent != null, "infoSyncEvent != null", this);
            Debug.Assert(timeReportEvent != null, "timeReportEvent != null", this);
            Debug.Assert(fullscreenReportEvent != null, "fullscreenReportEvent != null", this);
            Debug.Assert(infoReportEvent != null, "infoReportEvent != null", this);
            Debug.Assert(roleVariable != null, "roleVariable != null", this);
            Debug.Assert(slidesParent != null, "slidesParent != null", this);
            Debug.Assert(slideTimerFill != null, "slideTimerFill != null", this);
            Debug.Assert(controls != null, "controls != null", this);
            Debug.Assert(activeWhilePlaying != null, "activeWhilePlaying != null", this);
            Debug.Assert(activeWhilePaused != null, "activeWhilePaused != null", this);
            Debug.Assert(playButton != null, "playButton != null", this);
            Debug.Assert(pauseButton != null, "pauseButton != null", this);
            Debug.Assert(nextButton != null, "nextButton != null", this);
            Debug.Assert(backButton != null, "backButton != null", this);
            Debug.Assert(copyrightButton != null, "copyrightButton != null", this);
            Debug.Assert(copyrightMarker != null, "copyrightMarker != null", this);
            Debug.Assert(infoButton != null, "infoButton != null", this);
            Debug.Assert(infoMarker != null, "infoMarker != null", this);
            Debug.Assert(pageMarker != null, "pageMarker != null", this);
            Debug.Assert(pageContainer != null, "pageContainer != null", this);
            Debug.Assert(pagePrefab != null, "pagePrefab != null", this);
            Debug.Assert(fullscreenImage != null, "fullscreenImage != null", this);
            //Debug.Assert(canvasScaler != null, "canvasScaler != null", this);
            Debug.Assert(enterFullscreenButton != null, "enterFullscreenButton != null", this);
            Debug.Assert(exitFullscreenButton != null, "exitFullscreenButton != null", this);
            Debug.Assert(fullscreenContentSync != null, "fullscreenContentSync != null", this);
            Debug.Assert(openWhileFullscreen != null, "openWhileFullscreen != null", this);
            Debug.Assert(closedWhileFullscreen != null, "closedWhileFullscreen != null", this);
            Debug.Assert(builderProgress != null, "builderProgress != null", this);
            Debug.Assert(cacheSize != null, "cacheSize != null", this);
            Debug.Assert(
                closedWhileFullscreen == null || openWhileFullscreen == null ||
                !closedWhileFullscreen.Intersect(openWhileFullscreen).Any(),
                "closedWhileFullscreen is mutually exclusive with openWhileFullscreen", this);

            if (pageContainer != null)
            {
                _pageLayout = pageContainer.GetComponent<HorizontalLayoutGroup>();
                Debug.Assert(_pageLayout != null, "_pageLayout != null", this);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            enterFullscreenButton.onClick.AddListener(EnterFullscreen);
            exitFullscreenButton.onClick.AddListener(ExitFullscreen);
            playButton.onClick.AddListener(Play);
            pauseButton.onClick.AddListener(Pause);
            nextButton.onClick.AddListener(Next);
            backButton.onClick.AddListener(Back);
            copyrightButton.onClick.AddListener(ToggleCopyright);
            infoButton.onClick.AddListener(ToggleInfo);
            timeSyncEvent.Register(OnTimeSync);
            infoSyncEvent.Register(OnInfoSync);
            fullscreenSyncEvent.Register(OnFullscreenSync);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            enterFullscreenButton.onClick.RemoveListener(EnterFullscreen);
            exitFullscreenButton.onClick.RemoveListener(ExitFullscreen);
            playButton.onClick.RemoveListener(Play);
            pauseButton.onClick.RemoveListener(Pause);
            nextButton.onClick.RemoveListener(Next);
            backButton.onClick.RemoveListener(Back);
            copyrightButton.onClick.RemoveListener(ToggleCopyright);
            infoButton.onClick.RemoveListener(ToggleInfo);
            timeSyncEvent.Unregister(OnTimeSync);
            infoSyncEvent.Unregister(OnInfoSync);
            fullscreenSyncEvent.Unregister(OnFullscreenSync);
        }


        // the time set here is supposed to be network-/server-time
        protected void OnTimeSync(float t)
        {
            Debug.Log(
                $"<{nameof(SlideshowPresenter)}> : Sync time {t:F2} received ({nameof(PermitSync)}: {PermitSync})",
                this);
            if (PermitSync)
            {
                _showStart = t;
            }
        }

        protected void OnInfoSync(bool isOn)
        {
            Debug.Log(
                $"<{nameof(SlideshowPresenter)}> : Sync info = {isOn} received ({nameof(PermitSync)}: {PermitSync})",
                this);
            if (PermitSync)
            {
                if (isOn)
                    OpenInfo(false);
                else
                    CloseInfo(false);
            }
        }

        protected void OnFullscreenSync(bool isOn)
        {
            Debug.Log(
                $"<{nameof(SlideshowPresenter)}> : Sync fullscreen = {isOn} received ({nameof(PermitSync)}: {PermitSync})",
                this);
            if (PermitSync)
            {
                if (isOn)
                    EnterFullscreen(false);
                else
                    ExitFullscreen(false);
            }
        }

        protected override void OnShow(params AssetsRecord[] records)
        {
            Clear();
            _builderProgress = 0;

            if (records.Length > maxCacheCount)
                throw new Exception($"Can only display up to {maxCacheCount} items");

            _records = records.ToList();
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            _showStart = _networkTime;

            _isPaused = true;
            if (controls != null)
            {
                controls.interactable = false;
                controls.alpha = 0;
            }

            // disable components that would fight inputs received from sync
            disableBehaviourWhileObserver.ForEach(it => it.enabled = IsInteractableRole);
            disableObjectWhileObserver.ForEach(it => it.SetActive(IsInteractableRole));

            UniTask.RunOnThreadPool(BuildViewAsync, false, _cts.Token);
        }

        private async UniTask BuildViewAsync()
        {
            // load images on THREAD POOL
            await UniTask.SwitchToMainThread();
            _showStart = _networkTime;
            _builderProgress = 0;

            var sw = Stopwatch.StartNew();
            var swLoad = new Stopwatch();
            var swLoadImage = new Stopwatch();
            var stopwatchUpdate = new Stopwatch();
            var stopwatchCreate = new Stopwatch();
            for (var i = 0; i < _records.Count; i++)
            {
                _builderProgress = i / (float)_records.Count;
                swLoad.Start();

                string originalPath = DirectusManager.Instance.Connector.GetLocalFilePath(_records[i].Asset);
                int hash = await TextureImporter.Import(originalPath);

                swLoad.Stop();

                if (sw.ElapsedMilliseconds > 10)
                {
                    sw.Restart();
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }

                stopwatchCreate.Start();
                // create ui widget
                SlidePresenter slide = Instantiate(slidePrefab, slidesParent);
                slide.transform.SetAsFirstSibling();
                slide.gameObject.name = $"{slidePrefab.name} {_records[i].Id} {_records[i].Name}";
                slide.gameObject.SetActive(i == 0);
                slide.SetInfoVisible(true);
                _slides.Add(slide);

                Toggle pageToggle = Instantiate(pagePrefab, pageContainer);
                pageToggle.transform.SetAsLastSibling();
                pageToggle.gameObject.name = $"{pagePrefab.name} {_records[i].Id} {_records[i].Name}";
                int slideIndex = i;
                pageToggle.onValueChanged.AddListener(_ => Goto(slideIndex));
                pageToggle.SetIsOnWithoutNotify(i == 0);
                _pages.Add(pageToggle);

                if (TextureImporter.TryGet(hash, out var cachedTx))
                {
                    slide.Image.sprite = Sprite.Create(cachedTx, new Rect(0, 0, cachedTx.width, cachedTx.height),
                        new Vector2(cachedTx.width / 2.0f, 0), 100f, 0, SpriteMeshType.FullRect);
                    slide.Image.preserveAspect = true;
                    slide.Copyright = $"{ParsingUtils.GetRawText(_records[i].Copyright)}";
                }

                stopwatchCreate.Stop();


                stopwatchUpdate.Start();
                // update each time a sprite is ready
                // TODO: maybe only update the new one
                UpdateTranslations(LocalizationSettings.SelectedLocale);
                stopwatchUpdate.Stop();

                _isPaused = !autoPlay.Value;
            }

            _builderProgress = 1.0f;
            if (controls != null)
            {
                controls.interactable = IsInteractableRole;
                controls.alpha = IsInteractableRole ? 1.0f : 0;
            }


            if (debug)
            {
                Debug.Log(
                    $"<{nameof(SlideshowPresenter)}> : Update translations; total {stopwatchUpdate.ElapsedMilliseconds} ms, average {stopwatchUpdate.ElapsedMilliseconds / _records.Count} ms",
                    this);
                Debug.Log(
                    $"<{nameof(SlideshowPresenter)}> : Create slide; total {stopwatchCreate.ElapsedMilliseconds} ms, average {stopwatchCreate.ElapsedMilliseconds / _records.Count} ms",
                    this);
                Debug.Log(
                    $"<{nameof(SlideshowPresenter)}> : Read files; total {swLoad.ElapsedMilliseconds} ms, average {swLoad.ElapsedMilliseconds / _records.Count} ms",
                    this);
                Debug.Log(
                    $"<{nameof(SlideshowPresenter)}> : Tx load image; total {swLoadImage.ElapsedMilliseconds} ms, average {swLoadImage.ElapsedMilliseconds / _records.Count} ms",
                    this);
            }

            CloseCopyright();
            if (startWithInfoVisible.Value)
                OpenInfo();
            else
                CloseInfo();
        }

        private void UpdateTranslations(Locale locale)
        {
            for (var i = 0; i < _slides.Count; i++)
            {
                SlidePresenter slide = _slides[i];
                AssetsRecord record = _records[i];
                if (DirectusExtensions.TryGetTranslation(locale, record.Translations, out var translation))
                {
                    slide.Title = translation.Title;
                    slide.Description = translation.Text;
                }
            }
        }

        protected override void OnLocaleChanged(Locale locale) => UpdateTranslations(locale);

        public void Next()
        {
            Debug.Log($"<{nameof(SlideshowPresenter)}> : Request Next", this);
            float currentSlideElapsed = Mathf.Abs(_networkTime - ShowStart) % slideDuration;
            float remainingInCurrentSlide = slideDuration - currentSlideElapsed;
            _showStart -= remainingInCurrentSlide + 0.05f; // small offset to trigger transitions
        }

        public void Goto(int index)
        {
            Debug.Log($"<{nameof(SlideshowPresenter)}> : Goto {index}", this);
            //float currentSlideElapsed = Mathf.Abs(Time.time - _showStart) % slideDuration;
            _showStart = _networkTime - (slideDuration * index + 0.05f);
        }

        public void Back()
        {
            Debug.Log($"<{nameof(SlideshowPresenter)}> : Request Back", this);
            float currentSlideElapsed = Mathf.Abs(_networkTime - ShowStart) % slideDuration;
            _showStart += slideDuration + currentSlideElapsed - 0.05f; // small offset to trigger transitions
        }

        public void Pause() => _isPaused = true;

        public void Play() => _isPaused = false;

        public void PlayPauseToggle() => _isPaused = !_isPaused;

        public void OnGestureStateUpdated(GestureRecognizer recognizer)
        {
            switch (recognizer)
            {
                case SwipeGestureRecognizer swipe:
                    HandleSwipeGesture(swipe);
                    break;
                case TapGestureRecognizer tap:
                    HandleTapGesture(tap);
                    break;
                default:
                    throw new NotImplementedException(
                        $"No handler implemented for recognizer of type {recognizer.GetType().GetNiceName()}");
            }
        }

        private void HandleSwipeGesture(SwipeGestureRecognizer swipe)
        {
            if (swipe.State != GestureRecognizerState.Ended)
                return;


            switch (swipe.Direction)
            {
                case var direction when direction == _nextGesture:
                    Next();
                    break;
                case var direction when direction == _backGesture:
                    Back();
                    break;
                default:
                    break;
            }
        }

        private void HandleTapGesture(TapGestureRecognizer swipe)
        {
            if (swipe.State == GestureRecognizerState.Ended)
                PlayPauseToggle();
        }

        private void Update()
        {
            _prevNetworkTime = _networkTime;
            _networkTime = (float)NetworkManager.Singleton.ServerTime.TimeAsFloat;
            var networkDeltaTime = _networkTime - _prevNetworkTime;

            if (_slides.Count == 0)
                return;

            if (IsPaused)
                _showStart += networkDeltaTime;

            if (activeWhilePaused != null)
                activeWhilePaused.SetActive(IsPaused);

            if (activeWhilePaused != null)
                activeWhilePlaying.SetActive(!IsPaused);

            if (builderProgress != null && _builderProgress < 1.0f && _builderProgress > 0.0f)
            {
                builderProgress.gameObject.SetActive(true);
                builderProgress.fillAmount = _builderProgress;
            }
            else
            {
                builderProgress.gameObject.SetActive(false);
            }

            if (cacheSize != null)
            {
                cacheSize.SetText("{0} MB", TextureImporter.CacheSize / (1024 * 1024));
            }

            _prevSlideNdx = _slideNdx;
            _slideNdx = Mod((int)((_networkTime - _showStart) / slideDuration), _slides.Count);
            _nextSlideIdx = Mod((_slideNdx + 1), _slides.Count);

            if (_prevSlideNdx != _slideNdx)
            {
                if (PermitSync)
                {
                    Debug.Log($"<{nameof(SlideshowPresenter)}> : Sync report time {_showStart:F2}", this);
                    timeReportEvent.Raise(_showStart);
                }

                int direction = 0;
                int lastNdx = _slides.Count - 1;
                const int firstNdx = 0;

                if (_slideNdx == firstNdx && _prevSlideNdx == lastNdx)
                    direction = 1;
                else if (_slideNdx == lastNdx && _prevSlideNdx == firstNdx)
                    direction = -1;
                else
                    direction = _prevSlideNdx < _slideNdx ? 1 : -1;

                for (int i = 0; i < _slides.Count; i++)
                {
                    _slides[i].gameObject.SetActive(i == _slideNdx || i == _prevSlideNdx);
                }

                SlidePresenter prevPresenter = _slides[_prevSlideNdx].GetComponent<SlidePresenter>();
                SlidePresenter nextPresenter = _slides[_slideNdx].GetComponent<SlidePresenter>();

                CloseCopyright();
                UpdateInfoVisibilityOnAllSlides();

                if (direction > 0)
                {
                    if (debug)
                        Debug.Log($"<{nameof(SlideshowPresenter)}> : Forward", this);
                    // forward
                    if (_slides.Count > 0)
                        _slides[_slideNdx]?.transform.SetAsLastSibling();
                    if (_slides.Count > 1)
                        _slides[_prevSlideNdx]?.transform.SetAsLastSibling();

                    nextPresenter.In(Vector2.zero, 0, 0);
                    switch (transitionAxis)
                    {
                        case RectTransform.Axis.Vertical:
                            prevPresenter.Out(new Vector2(0, -slidesParent.rect.height), transitionDuration, 0);
                            break;
                        case RectTransform.Axis.Horizontal:
                            prevPresenter.Out(new Vector2(-slidesParent.rect.width, 0), transitionDuration, 0);
                            break;
                    }
                }
                else
                {
                    if (debug)
                        Debug.Log($"<{nameof(SlideshowPresenter)}> : Back", this);
                    // back
                    if (_slides.Count > 1)
                        _slides[_prevSlideNdx]?.transform.SetAsLastSibling();
                    if (_slides.Count > 0)
                        _slides[_slideNdx]?.transform.SetAsLastSibling();

                    prevPresenter.In(Vector2.zero, 0,
                        0); // this must be called before the other .In calls below, otherwise they would get cancelled

                    switch (transitionAxis)
                    {
                        case RectTransform.Axis.Vertical:
                            nextPresenter.In(new Vector2(0, -slidesParent.rect.height), transitionDuration, 0);
                            break;
                        case RectTransform.Axis.Horizontal:
                            nextPresenter.In(new Vector2(-slidesParent.rect.width, 0), transitionDuration, 0);
                            break;
                    }
                }

                // update pagination highlights when a the slide has changed
                if (_pages != null)
                    for (int i = 0; i < _pages.Count; i++)
                        _pages[i].SetIsOnWithoutNotify(_slideNdx == i);
            }

            // maintain timer fill
            if (slideTimerFill != null)
                slideTimerFill.fillAmount = (Mathf.Abs(_networkTime - _showStart) % slideDuration) / slideDuration;

            if (pageMarker != null && pageContainer != null && _pageLayout != null && _slides.Count > 0)
            {
                Debug.Assert(_pageLayout.childControlWidth && _pageLayout.childForceExpandWidth,
                    "All pages are equal sized and fill the rect");
                _pageLayout.childForceExpandWidth = true;
                _pageLayout.childControlWidth = true;
                var rt = _pageLayout.GetComponent<RectTransform>();
                float totalNetWidth = (rt.rect.width - _pageLayout.spacing * (_slides.Count - 1) -
                                       _pageLayout.padding.horizontal);
                float spaceNormal = _pageLayout.spacing > 0 ? _pageLayout.spacing / rt.rect.width : 0;
                float padLeftNormal = _pageLayout.padding.left > 0 ? _pageLayout.padding.left / rt.rect.width : 0;
                float pageWidth = totalNetWidth / _slides.Count;
                float pageWidthNormal = totalNetWidth / pageWidth;
                pageMarker.anchorMin = new Vector2(padLeftNormal + _slideNdx * (pageWidthNormal + spaceNormal), 0);
                pageMarker.anchorMax =
                    new Vector2(padLeftNormal + (_slideNdx + 1) * (pageWidthNormal + spaceNormal) - spaceNormal, 1.0f);
            }
        }

        protected override void OnClear()
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            _isPaused = autoPlay.Value;
            _prevSlideNdx = 0;
            _slideNdx = 0;
            if (_pages != null)
            {
                for (int i = 0; i < _pages.Count; i++)
                    Destroy(_pages[i].gameObject);
                _pages.Clear();
            }

            if (_slides != null)
            {
                for (int i = 0; i < _slides.Count; i++)
                    Destroy(_slides[i].gameObject);
                _slides.Clear();
            }
        }

        protected override void OnClose()
        {
            CancelFullscreen();
            Pause();
        }

        protected override void OnOpen()
        {
            CancelFullscreen();


            if (autoPlay.Value)
                Play();
            else
                Pause();
        }

        public void ToggleCopyright()
        {
            _isCopyrightVisible = !_isCopyrightVisible;
            UpdateCopyrightVisibilityOnAllSlides();
        }

        public void CloseCopyright()
        {
            _isCopyrightVisible = false;
            UpdateCopyrightVisibilityOnAllSlides();
        }

        public void OpenCopyright()
        {
            _isCopyrightVisible = true;
            UpdateCopyrightVisibilityOnAllSlides();
        }

        public void ToggleInfo() => ToggleInfo(true);

        public void ToggleInfo(bool notify)
        {
            _isInfoVisible = !_isInfoVisible;
            UpdateInfoVisibilityOnAllSlides(notify);
        }

        public void CloseInfo() => CloseInfo(true);

        public void CloseInfo(bool notify)
        {
            _isInfoVisible = false;
            UpdateInfoVisibilityOnAllSlides(notify);
        }

        public void OpenInfo() => OpenInfo(true);

        public void OpenInfo(bool shouldSync)
        {
            _isInfoVisible = true;
            UpdateInfoVisibilityOnAllSlides(shouldSync);
        }

        private void UpdateInfoVisibilityOnAllSlides(bool notify = true)
        {
            if (notify && PermitSync && infoReportEvent != null)
                infoReportEvent.Raise(_isInfoVisible);

            if (infoMarker != null)
                infoMarker.gameObject.SetActive(_isInfoVisible);

            _slides.ForEach(it => it.SetInfoVisible(_isInfoVisible));
        }

        private void UpdateCopyrightVisibilityOnAllSlides()
        {
            if (copyrightMarker != null)
                copyrightMarker.gameObject.SetActive(_isCopyrightVisible);
            _slides.ForEach(it => it.SetCopyrightVisible(_isCopyrightVisible));
        }

        /// <remarks>This will potentially produce unwanted side effects and should not be called during open/close.</remarks>
        public void EnterFullscreen() => EnterFullscreen(true);

        /// <remarks>This will potentially produce unwanted side effects and should not be called during open/close.</remarks>
        public void EnterFullscreen(bool shouldSync)
        {
            if (shouldSync && PermitSync && fullscreenReportEvent != null)
            {
                fullscreenReportEvent.Raise(true);
            }

            _pausedBeforeFullscreen = IsPaused;
            Pause();

            Sprite sprite = _slides[_slideNdx].Image.sprite;
            float canvasScaleFactor = fullscreenImage.canvas.scaleFactor;
            float longSide = Mathf.Max(sprite.texture.width, sprite.texture.height);
            float shortSide = Mathf.Min(sprite.texture.width, sprite.texture.height);
            float scaleToMinWidth = minFullscreenSize.x / sprite.texture.width;
            float scaleToMaxWidth = maxFullscreenSize.x / sprite.texture.width;
            float scaleToMinHeight = minFullscreenSize.y / sprite.texture.height;
            float scaleToMaxHeight = maxFullscreenSize.y / sprite.texture.height;
            float scaleToMin = Mathf.Max(scaleToMinHeight, scaleToMinWidth);
            float scaleToMax = Mathf.Min(scaleToMaxHeight, scaleToMaxWidth);
            float scale = Mathf.Max(scaleToMin, scaleToMax);
            fullscreenImage.type = Image.Type.Simple;
            fullscreenImage.sprite = sprite;
            fullscreenImage.rectTransform.anchorMin = Vector2.one * 0.5f;
            fullscreenImage.rectTransform.anchorMax = Vector2.one * 0.5f;
            fullscreenImage.rectTransform.pivot = Vector2.one * 0.5f;
            fullscreenImage.rectTransform.sizeDelta = new Vector2(
                fullscreenImage.sprite.texture.width * (scale / canvasScaleFactor),
                fullscreenImage.sprite.texture.height * (scale / canvasScaleFactor)
            );
            fullscreenImage.rectTransform.anchoredPosition = Vector2.zero;

            closedWhileFullscreen.ForEach(it => it.Close());
            openWhileFullscreen.ForEach(it => it.Open());

            // config fullscreen motion sync based on role
            fullscreenContentSync.PermitSync = PermitSync;
            fullscreenContentSync.IsConsumer = roleVariable.Value == PawnRole.Observer;
            fullscreenContentSync.IsPublisher = roleVariable.Value == PawnRole.Controller;
        }

        /// <remarks>This will potentially produce unwanted side effects and should not be called during open/close. <seealso cref="CancelFullscreen"/></remarks>
        public void ExitFullscreen() => ExitFullscreen(true);

        /// Cancel fullscreen mode without producing side-effects outside
        /// the scope of the fullscreen mode
        private void CancelFullscreen()
        {
            // disable fullscreen motion sync
            fullscreenContentSync.PermitSync = false;
            fullscreenContentSync.IsConsumer = false;
            fullscreenContentSync.IsPublisher = false;
            // close fullscreen views
            openWhileFullscreen.ForEach(it => it.Close());
        }

        /// <remarks>This will potentially produce unwanted side effects and should not be called during open/close. <seealso cref="CancelFullscreen"/></remarks>
        public void ExitFullscreen(bool shouldSync)
        {
            CancelFullscreen();
            closedWhileFullscreen.ForEach(it => it.Open());

            if (shouldSync && PermitSync && fullscreenReportEvent != null)
                fullscreenReportEvent.Raise(false);

            if (!_pausedBeforeFullscreen)
                Play();
        }
    }
}