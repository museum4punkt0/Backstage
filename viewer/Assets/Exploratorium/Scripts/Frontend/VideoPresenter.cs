using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Exploratorium.Net.Shared;
using RenderHeads.Media.AVProVideo;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;

#pragma warning disable CS0414
#pragma warning disable CS0168

namespace Exploratorium.Frontend
{
    public class VideoPresenter : AssetsPresenter
    {
        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private FloatEvent syncTimeEvent;

        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private BoolEvent syncPlayEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private FloatEvent reportTimeEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private BoolEvent reportPlayEvent;

        [BoxGroup(Constants.ReadVariables)] [SerializeField]
        private PawnRoleVariable roleVariable;

        [BoxGroup(Constants.ReadVariables)]
        [SerializeField] private BoolReference autoPlay = new BoolReference(true);

        [Title("Misc")]
        [SerializeField] private float autoPlayDelay = 1f;

        [SerializeField] private RectTransform canvasTrs;

        [Title("Tip")]
        [SerializeField] private bool enableTip;

        [SerializeField, ShowIf(nameof(enableTip))]
        private MediaPlayer tipPlayer;

        [SerializeField, ShowIf(nameof(enableTip))]
        private RectTransform tipRoot;

        [SerializeField, ShowIf(nameof(enableTip))]
        private CanvasGroup tipGroup;

        [SerializeField, ShowIf(nameof(enableTip))]
        private TMP_Text tipInfo;

        [Title("Main")] [SerializeField]
        private MediaPlayer mainPlayer;


        [Title("Fullscreen")] [SerializeField]
        private CanvasGroup maximizedView;

        [SerializeField] private CanvasGroup minimizedView;
        [SerializeField] private Button maximise;
        [SerializeField] private Button minimize;

        [Title("Feedback")] [SerializeField]
        private CanvasGroup feedbackGroup;

        [SerializeField] private bool enableFeedback = true;
        [SerializeField] private CanvasGroup leftFeedbackGroup;
        [SerializeField] private CanvasGroup middleFeedbackGroup;
        [SerializeField] private CanvasGroup rightFeedbackGroup;
        [SerializeField] private TMP_Text leftFeedbackText;
        [SerializeField] private TMP_Text middleFeedbackText;
        [SerializeField] private TMP_Text rightFeedbackText;
        [SerializeField] private CanvasGroup playFeedbackGroup;
        [SerializeField] private CanvasGroup pauseFeedbackGroup;
        [SerializeField] private Image progress;

        [FormerlySerializedAs("subtitlesStatus")] [Header("Subtitles")] [SerializeField]
        private CanvasGroup whileSubtitlesAvailable;

        [Title("Playback Controls")] [SerializeField]
        private Button play;

        [SerializeField] private Button pause;
        [SerializeField] private Button rewindToStart;
        [SerializeField] private Button fastForward;
        [SerializeField] private Button replay;
        [SerializeField] private Button skipToNext;
        [SerializeField] private Button skipToPrevious;
        [SerializeField] private GameObject[] disabledWhileObserver;
        [SerializeField] private bool scaleHandleOnHover = false;

        [Title("Status indicators")] [SerializeField]
        private GameObject isPlaying;

        [SerializeField] private GameObject isPaused;

        //[SerializeField] private Button close;
        [SerializeField] private Slider position;
        [SerializeField] private EventTrigger sliderEventTrigger;

        [Min(0)] [SerializeField]
        private float fastForwardInterval = 5f;

        [Min(0)] [SerializeField]
        private float replayInterval = 5f;

        [Title("Video Touch")] [SerializeField]
        private Button videoOverlayRight;

        [SerializeField] private Button videoOverlayLeft;

        [Min(0)] [SerializeField]
        private float doubleClickInterval = 0.5f;

        [Title("Timeline")] [SerializeField]
        private TMP_Text currentTime;

        [SerializeField] private TMP_Text remainingTime;

        [Title("Content")] [SerializeField]
        private TMP_Text assetTitle;

        [SerializeField] private TMP_Text assetDescription;
        [SerializeField] private TMP_Text assetCopyright;
        [SerializeField] private Button toggleInfo;
        [SerializeField] private Button closeInfo;
        [SerializeField] private Image toggleInfoMarker;
        [SerializeField] private Openable infoGroup;
        [SerializeField] private Button toggleCopyright;
        [SerializeField] private Image toggleCopyrightMarker;
        [SerializeField] private Openable copyrightGroup;

        [Title("Debug")] [SerializeField]
        private TMP_Text mediaInfo;

        private List<AssetsRecord> _playlist = new List<AssetsRecord>();
        private List<MediaPath> _playlistVideoPaths = new List<MediaPath>();
        private List<MediaPath> _playlistSubtitlePaths = new List<MediaPath>();

        private float _leftClick;
        private float _rightClick;
        private float _lastPlausedTime;
        private float _lastPlayingTime;
        private float _lastMiddleFeedbackTime;
        private float _lastRightFeedbackTime;
        private float _lastLeftFeedbackTime;
        private bool _wasPlayingBeforeDrag;
        private bool _isHoveringOverTimeline;
        private bool _isDragging;

        [Min(0)] [SerializeField]
        private float feedbackDuration = 1f;

        private bool _isCopyrightVisible;
        private bool _isInfoVisible;

        /// whether this presenter is currently in focus
        public bool PermitSync { get; set; }

        private bool IsInteractableRole => roleVariable == null || roleVariable.Value == PawnRole.Controller ||
                                           roleVariable.Value == PawnRole.Solo;

        public event Action FinishedPlaying;
        public event Action Next;


        protected override void OnLocaleChanged(Locale locale)
        {
            if (Records == null || !Records.Any())
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(VideoPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                        this);
                return;
            }

            if (_playlist != null && _playlist.Count > 0)
            {
                // SUBTITLES
                {
                    _playlistSubtitlePaths = _playlist
                        .Select(it =>
                        {
                            bool any = it.Translations.TryGetBestAvailableCode(
                                LocalizationSettings.SelectedLocale.Identifier.Code, out var best);
                            if (any && it.Translations.TryGetTranslation(best, out var translationsRecord))
                            {
                                try
                                {
                                    string path =
                                        DirectusManager.Instance.Connector.GetLocalFilePath(translationsRecord.Subtitles);
                                    return new MediaPath(path, MediaPathType.AbsolutePathOrURL);
                                }
                                catch (NullReferenceException e)
                                {
                                    Debug.LogError(
                                        $"{nameof(VideoPresenter)} : Broken FilePath detected in {translationsRecord.GetType().GetNiceName()}[{translationsRecord.__Primary}].{nameof(translationsRecord.Subtitles)}; Filename was '{(translationsRecord.Subtitles != null ? translationsRecord.Subtitles.FilenameDisk : "null")}'",
                                        this);
                                    return null;
                                }
                            }

                            return null;
                        })
                        .Where(it => it != null && !string.IsNullOrWhiteSpace(it.Path))
                        .ToList();

                    if (_playlistSubtitlePaths.Count != _playlist.Count)
                        Debug.Log(
                            $"{nameof(VideoPresenter)} : Found {_playlistSubtitlePaths.Count} subtitle paths for {_playlist.Count} playlist entries",
                            this);
                    if (_playlistSubtitlePaths.Count > 0)
                    {
                        mainPlayer.SideloadSubtitles = true;
                        MediaPath path = _playlistSubtitlePaths.First();
                        if (mainPlayer.EnableSubtitles(path))
                        {
                            if (debug)
                                Debug.Log(
                                    $"{nameof(VideoPresenter)} : Sideloading subtitles for record {_playlist.FirstOrDefault()?.Name} and locale {locale.Identifier.Code} from {_playlistSubtitlePaths.FirstOrDefault()?.Path}",
                                    this);
                        }
                    }
                    else
                    {
                        mainPlayer.SideloadSubtitles = false;
                        mainPlayer.DisableSubtitles();
                        Debug.LogWarning(
                            $"{nameof(VideoPresenter)} : No subtitles are available for record {_playlist.First().Name} and locale {locale.Identifier.Code}",
                            this);
                    }
                }

                var hasTranslation =
                    DirectusExtensions.TryGetTranslation(locale, _playlist.First().Translations, out var translation);
                if (assetTitle != null && hasTranslation)
                    assetTitle.text = string.IsNullOrWhiteSpace(translation.Title) ? "[?]" : translation.Title;
                if (assetDescription != null && hasTranslation)
                    assetDescription.text = ParsingUtils.GetRawText(translation.Text);
                if (assetCopyright != null)
                    assetCopyright.text = ParsingUtils.GetRawText(_playlist.First().Copyright);
                if (whileSubtitlesAvailable != null)
                {
                    bool available = mainPlayer.Subtitles != null;
                    whileSubtitlesAvailable.gameObject.SetActive(available);
                    whileSubtitlesAvailable.alpha = available ? 1.0f : 0;
                }

                if (disabledWhileObserver.Any())
                    disabledWhileObserver.ForEach(it => it.gameObject.SetActive(IsInteractableRole));
            }
        }

        protected override void OnShow(params AssetsRecord[] records)
        {
            Debug.Assert(mainPlayer != null, "mediaPlayer != null", this);
            if (mainPlayer == null)
                return;

            mainPlayer.gameObject.SetActive(true);

            _playlist = records.ToList();
            Debug.Assert(_playlist != null, "_playlist != null", this);
            _playlistVideoPaths = _playlist
                .Select(it => new MediaPath(DirectusManager.Instance.Connector.GetLocalFilePath(it.Asset),
                    MediaPathType.AbsolutePathOrURL))
                .ToList();

            if (_playlistVideoPaths.Any())
            {
                mainPlayer.OpenMedia(_playlistVideoPaths.First(), false);
                if (autoPlay.Value)
                {
                    UniTask.Void(async () =>
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(autoPlayDelay));
                        mainPlayer.Play();
                    });
                }
                else
                {
                    mainPlayer.Pause();
                }
            
                if (mediaInfo != null)
                    mediaInfo.text = $"Asset: {_playlist.First().Name}\n" +
                                     $"File: {_playlist.First().Asset.FilenameDisk}\n";

                if (enableTip && tipPlayer != null)
                {
                    tipPlayer.OpenMedia(_playlistVideoPaths.First(), false);
                    tipPlayer.Pause();
                    tipPlayer.AudioVolume = 0;
                }
            }

            OnLocaleChanged(LocalizationSettings.SelectedLocale);
            Debug.Log($"{nameof(VideoPresenter)} : Showing {_playlistVideoPaths.First().Path}", this);
            _lastMiddleFeedbackTime = 0;
            _lastPlayingTime = 0;
            _lastPlausedTime = 0;
            _lastPlausedTime = mainPlayer.Control.IsPaused() ? Time.time : 0;
            _lastPlayingTime = mainPlayer.Control.IsPlaying() ? Time.time : 0;
            _wasPlayingBeforeDrag = true;

            CloseInfo();
            CloseCopyright();
        
            if (!_playlistVideoPaths.Any())
                FinishedPlaying?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();
            if (mainPlayer != null)
            {
                mainPlayer.CloseMedia();
                mainPlayer.AudioVolume = 1f;
            }

            if (tipPlayer != null)
            {
                tipPlayer.CloseMedia();
                tipPlayer.AudioVolume = 0;
            }

            SetupTriggers();
        }

        private void SetupTriggers()
        {
            if (sliderEventTrigger != null)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener(data => OnTimelineBeginDrag((PointerEventData)data));
                sliderEventTrigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.Drag;
                entry.callback.AddListener(data => OnTimelineDrag((PointerEventData)data));
                sliderEventTrigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener(data => OnTimelineEndDrag((PointerEventData)data));
                sliderEventTrigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener(data => OnTimelineEnter((PointerEventData)data));
                sliderEventTrigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerExit;
                entry.callback.AddListener(data => OnTimelineExit((PointerEventData)data));
                sliderEventTrigger.triggers.Add(entry);
            }
        }

        private void OnTimelineBeginDrag(PointerEventData data)
        {
            _isDragging = true;
            if (mainPlayer && mainPlayer.Control != null)
            {
                _wasPlayingBeforeDrag = mainPlayer.Control.IsPlaying();
                if (_wasPlayingBeforeDrag)
                {
                    mainPlayer.Pause();
                }

                OnTimelineDrag(data);
            }
        }

        private void OnTimelineDrag(PointerEventData data)
        {
            if (mainPlayer && mainPlayer.Control != null)
            {
                SafeSeek(position.value);
                _isHoveringOverTimeline = true;
            }
        }

        private void OnTimelineEndDrag(PointerEventData data)
        {
            _isDragging = false;
            if (mainPlayer && mainPlayer.Control != null)
            {
                if (_wasPlayingBeforeDrag)
                {
                    mainPlayer.Play();
                    _wasPlayingBeforeDrag = false;
                }
            }
        }

        private void OnTimelineEnter(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                _isHoveringOverTimeline = true;
                if (scaleHandleOnHover)
                {
                    position.fillRect.localScale = new Vector3(1f, 2f, 1f);
                    position.handleRect.localScale = new Vector3(1f, 2f, 1f);
                }
            }
        }

        private void OnTimelineExit(PointerEventData eventData)
        {
            _isHoveringOverTimeline = false;
            position.fillRect.localScale = new Vector3(1f, 1f, 1f);
            position.handleRect.localScale = new Vector3(1f, 1f, 1f);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            mainPlayer.Events.AddListener(OnMediaPlayerEvent);
            if (position != null) position.onValueChanged.AddListener(OnPositionChanged);
            if (play != null) play.onClick.AddListener(OnPlayClicked);
            if (pause != null) pause.onClick.AddListener(OnPauseClicked);
            if (maximise != null) maximise.onClick.AddListener(OnMaxClicked);
            if (minimize != null) minimize.onClick.AddListener(OnMinClicked);
            if (rewindToStart != null) rewindToStart.onClick.AddListener(OnRewindClicked);
            if (skipToNext != null) skipToNext.onClick.AddListener(OnNextClicked);
            if (skipToPrevious != null) skipToPrevious.onClick.AddListener(OnPreviousClicked);
            if (fastForward != null) fastForward.onClick.AddListener(OnForwardClicked);
            if (replay != null) replay.onClick.AddListener(OnBackClicked);
            if (videoOverlayLeft != null) videoOverlayLeft.onClick.AddListener(OnOverlayLeftClicked);
            if (videoOverlayRight != null) videoOverlayRight.onClick.AddListener(OnOverlayRightClicked);
            if (toggleCopyright != null) toggleCopyright.onClick.AddListener(ToggleCopyright);
            if (toggleInfo != null) toggleInfo.onClick.AddListener(ToggleInfo);
            if (closeInfo != null) closeInfo.onClick.AddListener(CloseInfo);
            if (syncPlayEvent != null) syncPlayEvent.Register(OnSyncPlayEvent);
            if (syncTimeEvent != null) syncTimeEvent.Register(OnSyncTimeEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            mainPlayer.Events.RemoveListener(OnMediaPlayerEvent);
            if (position != null) position.onValueChanged.RemoveListener(OnPositionChanged);
            if (play != null) play.onClick.RemoveListener(OnPlayClicked);
            if (pause != null) pause.onClick.RemoveListener(OnPauseClicked);
            if (maximise != null) maximise.onClick.RemoveListener(OnMaxClicked);
            if (minimize != null) minimize.onClick.RemoveListener(OnMinClicked);
            if (rewindToStart != null) rewindToStart.onClick.RemoveListener(OnRewindClicked);
            if (skipToNext != null) skipToNext.onClick.RemoveListener(OnNextClicked);
            if (skipToPrevious != null) skipToPrevious.onClick.RemoveListener(OnPreviousClicked);
            if (fastForward != null) fastForward.onClick.RemoveListener(OnForwardClicked);
            if (replay != null) replay.onClick.RemoveListener(OnBackClicked);
            if (videoOverlayLeft != null) videoOverlayLeft.onClick.RemoveListener(OnOverlayLeftClicked);
            if (videoOverlayRight != null) videoOverlayRight.onClick.RemoveListener(OnOverlayRightClicked);
            if (toggleCopyright != null) toggleCopyright.onClick.RemoveListener(ToggleCopyright);
            if (toggleInfo != null) toggleInfo.onClick.RemoveListener(ToggleInfo);
            if (closeInfo != null) closeInfo.onClick.RemoveListener(CloseInfo);
            if (syncPlayEvent != null) syncPlayEvent.Unregister(OnSyncPlayEvent);
            if (syncTimeEvent != null) syncTimeEvent.Unregister(OnSyncTimeEvent);
        }

        private void Start()
        {
            if (tipRoot != null)
                tipRoot.gameObject.SetActive(enableTip);
        }

        private void Update()
        {
            if (enableTip && tipRoot != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTrs, Input.mousePosition, null,
                    out var canvasPos);
                Vector3 mousePos = canvasTrs.TransformPoint(canvasPos);
                tipRoot.position = new Vector2(mousePos.x, tipRoot.position.y);

                if (position != null && _isHoveringOverTimeline)
                {
                    // Work out position on the timeline
                    Bounds bounds = RectTransformUtility
                        .CalculateRelativeRectTransformBounds(position.GetComponent<RectTransform>());
                    float x = Mathf.Clamp01((canvasPos.x - bounds.min.x) / bounds.size.x);
                    double time = (double)x * position.maxValue;

                    // Seek to the new position
                    if (tipPlayer != null && tipPlayer.Control != null)
                    {
                        tipPlayer.Control.SeekFast(time);
                    }
                }

                if (tipGroup != null)
                {
                    tipGroup.gameObject.SetActive(_isHoveringOverTimeline);
                }

                if (tipInfo != null && tipPlayer.Control != null)
                {
                    TimeSpan t = TimeSpan.FromSeconds(tipPlayer.Control.GetCurrentTime());
                    tipInfo.text = $"{t.TotalMinutes:F0}:{t.Seconds:00}";
                }
            }

            if (position != null)
            {
                if (mainPlayer.Control != null && mainPlayer.Control.CanPlay())
                    position.SetValueWithoutNotify((float)mainPlayer.Control.GetCurrentTime());
                else
                    position.SetValueWithoutNotify(0);
            }

            if (progress != null)
            {
                progress.fillAmount =
                    mainPlayer.Control != null &&
                    mainPlayer.Control.CanPlay() &&
                    mainPlayer.Info != null
                        ? (float)mainPlayer.Control.GetBufferedTimes().MaxTime
                        : 0;
            }

            if (remainingTime != null)
            {
                if (mainPlayer.Control != null && mainPlayer.Control.CanPlay() && mainPlayer.Info != null)
                {
                    var remaining =
                        TimeSpan.FromSeconds(mainPlayer.Info.GetDuration() - mainPlayer.Control.GetCurrentTime());
                    remainingTime.text = $"-{remaining.Minutes:F0}:{remaining.Seconds:00}";
                }
                else
                {
                    remainingTime.text = "0:00";
                }
            }

            if (currentTime != null)
            {
                if (mainPlayer.Control != null && mainPlayer.Control.CanPlay())
                {
                    var current = TimeSpan.FromSeconds(mainPlayer.Control.GetCurrentTime());
                    currentTime.text = $"{current.Minutes:F0}:{current.Seconds:00}";
                }
                else
                {
                    currentTime.text = "0:00";
                }
            }

            if (play != null)
            {
                var isVisible = mainPlayer.Control != null && mainPlayer.Control.CanPlay() &&
                                !mainPlayer.Control.IsPlaying();
                play.gameObject.SetActive(isVisible);
            }

            if (pause != null)
            {
                var isVisible = mainPlayer.Control != null && mainPlayer.Control.CanPlay() &&
                                mainPlayer.Control.IsPlaying();
                pause.gameObject.SetActive(isVisible);
            }

            if (middleFeedbackGroup != null)
                middleFeedbackGroup.alpha = feedbackDuration > 0
                    ? Mathf.Lerp(1f, 0, (Time.time - _lastMiddleFeedbackTime) / feedbackDuration)
                    : 0;

            if (rightFeedbackGroup != null)
                rightFeedbackGroup.alpha = feedbackDuration > 0
                    ? Mathf.Lerp(1f, 0, (Time.time - _lastRightFeedbackTime) / feedbackDuration)
                    : 0;

            if (leftFeedbackGroup != null)
                leftFeedbackGroup.alpha = feedbackDuration > 0
                    ? Mathf.Lerp(1f, 0, (Time.time - _lastLeftFeedbackTime) / feedbackDuration)
                    : 0;

            if (pauseFeedbackGroup != null)
                pauseFeedbackGroup.alpha = feedbackDuration > 0
                    ? Mathf.Lerp(1f, 0, (Time.time - _lastPlayingTime) / feedbackDuration)
                    : 0; // we track playing time to fade out the overlay!!

            if (playFeedbackGroup != null)
                playFeedbackGroup.alpha = feedbackDuration > 0
                    ? Mathf.Lerp(1f, 0,
                        (Time.time - _lastPlausedTime) / feedbackDuration)
                    : 0; // we track paused time to fade out the overlay!!

            if (feedbackGroup != null)
                feedbackGroup.alpha = enableFeedback ? 1.0f : 0;

            if (mainPlayer.Control != null)
            {
                _lastPlausedTime = mainPlayer.Control.IsPaused() ? Time.time : _lastPlausedTime;
                _lastPlayingTime = mainPlayer.Control.IsPlaying() ? Time.time : _lastPlayingTime;
            }
            else
            {
                _lastPlausedTime = 0;
                _lastPlayingTime = 0;
            }

            if (isPlaying != null)
                isPlaying.gameObject.SetActive(mainPlayer.Control.IsPlaying());
            if (isPaused != null)
                isPaused.gameObject.SetActive(mainPlayer.Control.IsPaused());
        }

        private void OnPreviousClicked() => throw new NotImplementedException();

        private void OnNextClicked()
        {
            Next?.Invoke();
            //mainPlayer.Control.SeekFast(mainPlayer.Info.GetDuration());
        }

        public void ToggleCopyright()
        {
            _isCopyrightVisible = !_isCopyrightVisible;
            UpdateCopyrightState();
        }

        public void CloseCopyright()
        {
            _isCopyrightVisible = false;
            UpdateCopyrightState();
        }

        public void OpenCopyright()
        {
            _isCopyrightVisible = true;
            UpdateCopyrightState();
        }

        private void UpdateCopyrightState()
        {
            if (toggleCopyrightMarker != null)
            {
                toggleCopyrightMarker.gameObject.SetActive(_isCopyrightVisible);
            }

            if (copyrightGroup != null)
            {
                if (_isCopyrightVisible)
                    copyrightGroup.Open();
                else
                    copyrightGroup.Close();
            }
        }

        public void ToggleInfo()
        {
            _isInfoVisible = !_isInfoVisible;
            UpdateInfoState();
        }

        public void CloseInfo()
        {
            _isInfoVisible = false;
            UpdateInfoState();
        }

        public void OpenInfo()
        {
            _isInfoVisible = true;
            UpdateInfoState();
        }

        private void UpdateInfoState()
        {
            if (toggleCopyrightMarker != null)
            {
                toggleCopyrightMarker.gameObject.SetActive(_isCopyrightVisible);
            }

            if (infoGroup != null)
            {
                if (_isInfoVisible)
                    infoGroup.Open();
                else
                    infoGroup.Close();
            }
        }

        private void OnSyncTimeEvent(float value)
        {
            if (!PermitSync)
                return;
            Debug.Log($"{nameof(VideoPresenter)} : {nameof(OnSyncTimeEvent)} = {value:F2}", this);
            SafeSeek(value, false);
        }

        private void OnSyncPlayEvent(bool value)
        {
            if (!PermitSync)
                return;
            Debug.Log($"{nameof(VideoPresenter)} : {nameof(OnSyncPlayEvent)} = {value}", this);
            if (value)
                PlayWithoutNotify();
            else
                PauseWithoutNotify();
        }

        private void OnOverlayRightClicked()
        {
            if (!mainPlayer.Control.CanPlay())
                return;

            var isDoubleClick = Time.time - _rightClick < doubleClickInterval;
            _rightClick = Time.time;
            if (isDoubleClick)
            {
                SafeRelativeSeek(fastForwardInterval);
            }

            if (mainPlayer.Control.IsPlaying())
                OnPauseClicked();
            else
                OnPlayClicked();
        }

        private void OnOverlayLeftClicked()
        {
            if (!mainPlayer.Control.CanPlay())
                return;

            var isDoubleClick = Time.time - _leftClick < doubleClickInterval;
            _leftClick = Time.time;
            if (isDoubleClick)
            {
                SafeRelativeSeek(-replayInterval);
            }

            if (mainPlayer.Control.IsPlaying())
                OnPauseClicked();
            else
                OnPlayClicked();
        }


        private void OnForwardClicked()
        {
            if (mainPlayer.Control.CanPlay())
            {
                SafeRelativeSeek(fastForwardInterval);
            }
            else
            {
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnForwardClicked)} ignored", this);
            }
        }

        private void OnBackClicked()
        {
            if (mainPlayer.Control.CanPlay())
            {
                SafeRelativeSeek(-replayInterval);
            }
            else
            {
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnBackClicked)} ignored", this);
            }
        }

        private void OnRewindClicked()
        {
            if (mainPlayer.Control.CanPlay())
            {
                mainPlayer.Control.Rewind();
                if (reportTimeEvent != null && PermitSync)
                    reportTimeEvent.Raise(0);
            }
            else
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnRewindClicked)} ignored", this);
        }

        private void PauseWithoutNotify()
        {
            if (mainPlayer.Control.CanPlay())
                mainPlayer.Control.Pause();
            else
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnPauseClicked)} ignored", this);
        }

        private void OnPauseClicked()
        {
            if (mainPlayer.Control.CanPlay())
            {
                mainPlayer.Control.Pause();
                if (reportPlayEvent != null && PermitSync)
                    reportPlayEvent.Raise(false);
            }
            else
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnPauseClicked)} ignored", this);
        }

        private void PlayWithoutNotify()
        {
            if (mainPlayer.Control.CanPlay())
                mainPlayer.Control.Play();
            else
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnPlayClicked)} ignored", this);
        }

        private void OnPlayClicked()
        {
            if (mainPlayer.Control.CanPlay())
            {
                mainPlayer.Control.Play();
                if (reportTimeEvent != null && PermitSync)
                    reportTimeEvent.Raise((float)mainPlayer.Control.GetCurrentTime());
                if (reportPlayEvent != null && PermitSync)
                    reportPlayEvent.Raise(true);
            }
            else
            {
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnPlayClicked)} ignored", this);
            }
        }

        private void OnPositionChanged(float time)
        {
            if (mainPlayer.Control.CanPlay())
            {
                Debug.Log($"{nameof(VideoPresenter)} : Seeking {time}", this);
                SafeSeek(time);
            }
            else
            {
                Debug.LogWarning($"{nameof(VideoPresenter)} : Seek {time} ignored", this);
            }
        }

        private void OnMinClicked()
        {
            if (minimizedView != null && maximizedView != null)
            {
                minimizedView.gameObject.SetActive(true);
                maximizedView.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnMinClicked)} ignored", this);
            }
        }

        private void OnMaxClicked()
        {
            if (minimizedView != null && maximizedView != null)
            {
                minimizedView.gameObject.SetActive(false);
                maximizedView.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"{nameof(VideoPresenter)} : {nameof(OnMinClicked)} ignored", this);
            }
        }

        private void OnMediaPlayerEvent(MediaPlayer player, MediaPlayerEvent.EventType eventType, ErrorCode error)
        {
            switch (eventType)
            {
                case MediaPlayerEvent.EventType.Error:
                    Debug.LogError($"{nameof(VideoPresenter)} : Player event {eventType} {error}", this);
                    break;
                case MediaPlayerEvent.EventType.Started:
                    if (position != null)
                    {
                        position.SetValueWithoutNotify(0);
                        position.direction = Slider.Direction.LeftToRight;
                        position.minValue = 0;
                        position.maxValue = (float)mainPlayer.Info.GetDuration();
                    }

                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:
                    FinishedPlaying?.Invoke();
                    mainPlayer.Stop();
                    break;
                case MediaPlayerEvent.EventType.MetaDataReady:
                case MediaPlayerEvent.EventType.FirstFrameReady:
                case MediaPlayerEvent.EventType.ReadyToPlay:
                case MediaPlayerEvent.EventType.Closing:
                case MediaPlayerEvent.EventType.SubtitleChange:
                case MediaPlayerEvent.EventType.Stalled:
                case MediaPlayerEvent.EventType.Unstalled:
                case MediaPlayerEvent.EventType.ResolutionChanged:
                case MediaPlayerEvent.EventType.StartedSeeking:
                case MediaPlayerEvent.EventType.FinishedSeeking:
                case MediaPlayerEvent.EventType.StartedBuffering:
                case MediaPlayerEvent.EventType.FinishedBuffering:
                case MediaPlayerEvent.EventType.PropertiesChanged:
                case MediaPlayerEvent.EventType.PlaylistItemChanged:
                case MediaPlayerEvent.EventType.PlaylistFinished:
                case MediaPlayerEvent.EventType.TextTracksChanged:
                    if (debug)
                        Debug.Log($"{nameof(VideoPresenter)} : Player event {eventType}", this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }


        protected override void OnClear()
        {
            _playlist.Clear();
            _playlistSubtitlePaths.Clear();
            mainPlayer.Stop();
            mainPlayer.CloseMedia();
        }

        protected override void OnClose()
        {
            if (mainPlayer != null && mainPlayer.Control != null)
                mainPlayer.Control.Stop();
        }

        protected override void OnOpen()
        {
        }

        private void SafeSeek(float time, bool notify = true)
        {
            float seekTo = Mathf.Clamp(
                time,
                0,
                (float)mainPlayer.Info.GetDuration()
            );
            mainPlayer.Control.SeekFast(seekTo);

            if (PermitSync && notify && reportTimeEvent != null)
            {
                reportTimeEvent.Raise(seekTo);
                Debug.Log($"{nameof(VideoPresenter)} : {nameof(SafeSeek)} reported", this);
            }
        }

        private void SafeRelativeSeek(float offset, bool notify = true)
        {
            if (rightFeedbackText != null && offset > 0)
            {
                _lastRightFeedbackTime = Time.time;
                rightFeedbackText.text = $"{offset:+#;-#;0}";
            }
            else if (leftFeedbackText != null && offset < 0)
            {
                _lastLeftFeedbackTime = Time.time;
                leftFeedbackText.text = $"{offset:+#;-#;0}";
            }
            else if (middleFeedbackText != null)
            {
                _lastMiddleFeedbackTime = Time.time;
                middleFeedbackText.text = $"{offset:+#;-#;0}";
            }

            SafeSeek((float)mainPlayer.Control.GetCurrentTime() + offset);
        }
    }
}