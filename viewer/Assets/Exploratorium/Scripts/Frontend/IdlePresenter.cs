using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Exploratorium.Utility;
using JetBrains.Annotations;
using Exploratorium.Net.Shared;
using RenderHeads.Media.AVProVideo;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Exploratorium.Frontend
{
    public class IdlePresenter : MonoBehaviour
    {
        private const string TweenId = "a42b6D6E52A35fCc9556c997";

        [BoxGroup(Constants.ReadVariables)] [SerializeField]
        private PawnRoleVariable roleVariable;

        [BoxGroup("Observer")] [SerializeField]
        private Openable observerMode;

        [BoxGroup("Observer")] [SerializeField]
        private Image imageA;

        [BoxGroup("Observer")] [SerializeField]
        private Image imageB;

        [BoxGroup("Solo")] [SerializeField] private MediaPlayer mediaPlayer;

        [BoxGroup("Solo")] [SerializeField] private CanvasGroup mediaPlayerFadeGroup;

        [BoxGroup("Solo")] [SerializeField] private Openable soloMode;

        [BoxGroup("IO")] [SerializeField] private Button touch;


        [Min(0)] [SerializeField] private float fadeDuration = 1f;

        [Min(1f)] [SerializeField] private float slideDuration = 10f;

        [SerializeField] private OpenableGroup[] openables;
        private List<MediaPath> _videoPlaylist = new List<MediaPath>();
        private List<int> _imagePlaylist = new List<int>();
        private SettingsRecord _settings;


        public event Action UserDetected;

        private DirectusModel _model;
        private DirectusConnector _connector;
        private CancellationTokenSource _builderCts = new CancellationTokenSource();
        private CancellationTokenSource _showCts = new CancellationTokenSource();
        private readonly SemaphoreSlim _builderLock = new SemaphoreSlim(1, 1);
        private const float SoloMinimumDelay = 5.0f;

        private void Awake()
        {
            Debug.Assert(roleVariable != null, "roleVariable != null");
            Debug.Assert(observerMode != null, "observerMode != null");
            Debug.Assert(imageA != null, "imageA != null");
            Debug.Assert(imageB != null, "imageB != null");
            Debug.Assert(mediaPlayer != null, "mediaPlayer != null");
            Debug.Assert(mediaPlayerFadeGroup != null, "mediaPlayerFadeGroup != null");
            Debug.Assert(soloMode != null, "soloMode != null");
            Debug.Assert(touch != null, "touch != null");
        }

        private void Start()
        {
            DirectusManager.Instance.ModelChanged += Init;
            if (DirectusManager.Instance.IsReady)
                Init(DirectusManager.Instance.Connector);
        }

        private void Init(DirectusConnector directusConnector)
        {
            _model = directusConnector.Model;
            _connector = directusConnector;
            _settings = _model.GetItemsOfType<SettingsRecord>().First();
            UniTask.Void(BuildAsync);
        }

        private void OnEnable()
        {
            UniTask.Void(EnableAsync);
        }

        private void OnDisable()
        {
            _builderCts.Cancel();
            _builderCts = new CancellationTokenSource();
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        private void OnWakeup()
        {
            UserDetected?.Invoke();
        }

        private void OnLocaleChanged(Locale selectedLocale)
        {
        }

        private async UniTaskVoid EnableAsync()
        {
            await _builderLock.WaitAsync(_builderCts.Token);
            try
            {
                LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
                OnLocaleChanged(LocalizationSettings.SelectedLocale);

                if (touch != null)
                    touch.onClick.AddListener(OnWakeup);
            }
            finally
            {
                _builderLock.Release();
            }
        }

        private async UniTaskVoid BuildAsync()
        {
            await _builderLock.WaitAsync(_builderCts.Token);
            try
            {
                _settings = _model.GetItemsOfType<SettingsRecord>().First();

                bool hasTranslation = DirectusExtensions.TryGetTranslation(
                    LocalizationSettings.SelectedLocale,
                    _settings.Translations,
                    out SettingsTranslationsRecord assetTranslation);
                if (hasTranslation)
                {
                    if (assetTranslation.IdleAssetpoolSolo == null || !assetTranslation.IdleAssetpoolSolo.Any())
                    {
                        Debug.LogWarning($"{nameof(IdlePresenter)} : Video playlist is empty");
                    }
                    else
                    {
                        _videoPlaylist = assetTranslation.IdleAssetpoolSolo
                            .Where(it =>
                                it.Status == AssetsRecord.StatusChoices.Published)
                            .Select(it =>
                                new MediaPath(_connector.GetLocalFilePath(it.Asset), MediaPathType.AbsolutePathOrURL))
                            .ToList();
                        Debug.Log($"{nameof(IdlePresenter)} : Video playlist has {_videoPlaylist.Count} entries");
                    }
                }
                else
                {
                    Debug.LogWarning($"{nameof(IdlePresenter)} : Settings data missing");
                }

                if (assetTranslation.IdleAssetpoolSolo == null || !assetTranslation.IdleAssetpoolSolo.Any())
                {
                    Debug.LogWarning($"{nameof(IdlePresenter)} : Image playlist is empty");
                }
                else
                {
                    _imagePlaylist.Clear();
                    IEnumerable<string> paths = _settings.IdleAssetpoolObserver
                        .Where(it => it.Status == AssetsRecord.StatusChoices.Published)
                        .Select(it => _connector.GetLocalFilePath(it.Asset));
                    foreach (var path in paths)
                    {
                        int hash = await TextureImporter.Import(path);
                        _imagePlaylist.Add(hash);
                    }
                }
            }
            finally
            {
                _builderLock.Release();
            }
        }

        private void CancelShow()
        {
            _showCts.Cancel();
            _showCts = new CancellationTokenSource();
        }

        private async UniTaskVoid SoloShowAsync(CancellationToken ct)
        {
            try
            {
                Debug.Log($"{nameof(IdlePresenter)} : Solo mode started");
                if (observerMode != null)
                    observerMode.CloseAsync().Forget();
                if (soloMode != null)
                    soloMode.OpenAsync().Forget();
                mediaPlayerFadeGroup.alpha = 0;
                int playNdx = 0;
                while (true)
                {
                    await UniTask.NextFrame(ct);
                    if (ct.IsCancellationRequested)
                        break;
                    
                    float delay;
                    if (_settings == null)
                    {
                        Debug.LogWarning($"{nameof(IdlePresenter)} : No settings available, using minimum delay {SoloMinimumDelay}");
                        delay = SoloMinimumDelay;
                    }
                    else
                    {
                        delay = Random.Range(0, Mathf.Max(SoloMinimumDelay, _settings.IdleIntervalRandom)) +
                                Mathf.Max(SoloMinimumDelay, _settings.IdleIntervalMin);
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);
                    {
                        if (_videoPlaylist == null || _videoPlaylist.Count == 0)
                        {
                            Debug.LogWarning($"{nameof(IdlePresenter)} : Video playlist is empty, cancelling playback");
                            break;
                        }

                        if (playNdx == _videoPlaylist.Count)
                        {
                            _videoPlaylist.Shuffle();
                            playNdx = 0;
                        }

                        mediaPlayer.OpenMedia(_videoPlaylist[playNdx], true);
                        playNdx++;
                    }
                    mediaPlayer.AudioVolume = 0;
                    mediaPlayerFadeGroup.DOFade(1f, fadeDuration).SetId(TweenId).SetEase(Ease.InQuad);
                    await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: ct);
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(Mathf.Max(0, (float) mediaPlayer.Info.GetDuration() - fadeDuration * 2f)),
                        cancellationToken: ct);
                    mediaPlayerFadeGroup.DOFade(0f, fadeDuration).SetId(TweenId).SetEase(Ease.OutQuad);
                    await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: ct);
                    mediaPlayer.Stop();
                }
            }
            finally
            {
                DOTween.Kill(TweenId);
                mediaPlayer.Stop();
                Debug.Log($"{nameof(IdlePresenter)} : Solo mode cancelled");
            }
        }

        private async UniTaskVoid ObserverShowAsync(CancellationToken ct)
        {
            try
            {
                Debug.Log($"{nameof(IdlePresenter)} : Observer mode started");
                if (soloMode != null)
                    soloMode.CloseAsync().Forget();
                if (observerMode != null)
                    observerMode.OpenAsync().Forget();
                if (imageA != null && _imagePlaylist.Any())
                    FillImage(ref imageA, TextureImporter.Get(_imagePlaylist.First()));

                int slideNdx = 0;
                Image back = imageA;
                Image front = imageB;
                front.transform.SetAsLastSibling();
                front.DOFade(1f, 0).SetId(TweenId);
                back.DOFade(0, 0).SetId(TweenId);

                while (true)
                {
                    if (ct.IsCancellationRequested)
                        break;

                    front = slideNdx % 2 == 0 ? imageB : imageA;
                    back = slideNdx % 2 == 0 ? imageA : imageB;
                    DOTween.Sequence()
                        .Append(front.DOFade(1f, fadeDuration).SetEase(Ease.InQuad))
                        .Append(back.DOFade(0, 0))
                        .SetId(TweenId)
                        .Restart();
                    front.transform.SetAsLastSibling();
                    if (_imagePlaylist.Count > 0)
                    {
                        FillImage(ref front, TextureImporter.Get(_imagePlaylist[slideNdx]));
                    }
                    else
                    {
                        Debug.LogWarning($"{nameof(IdlePresenter)} : Image playlist is empty");
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(slideDuration), cancellationToken: ct);
                    slideNdx = (slideNdx + 1) % _imagePlaylist.Count;
                }
            }
            finally
            {
                DOTween.Kill(TweenId);
                Debug.Log($"{nameof(IdlePresenter)} : Observer mode cancelled");
            }
        }

        private void FillImage([NotNull] ref Image image, [NotNull] in Texture2D tx)
        {
            var rect = image.rectTransform.rect;
            image.sprite = ResizeImage.CreateFilledSprite(
                rect.width / rect.height,
                tx);
        }

        public void Close()
        {
            CancelShow();
            openables.ForEach(it => it.CloseAsync().Forget());
        }

        public void Open()
        {
            switch (roleVariable.Value)
            {
                case PawnRole.Observer:
                    CancelShow();
                    ObserverShowAsync(_showCts.Token).Forget();
                    break;
                case PawnRole.None:
                case PawnRole.Controller:
                case PawnRole.Solo:
                    CancelShow();
                    SoloShowAsync(_showCts.Token).Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            openables.ForEach(it => it.OpenAsync().Forget());
        }
    }
}