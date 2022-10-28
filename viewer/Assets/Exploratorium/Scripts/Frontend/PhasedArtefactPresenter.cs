using System;
using DG.Tweening;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Exploratorium.Frontend
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class PhasedArtefactPresenter : PhasedRecordPresenter<ArtefactsRecord>, IRevealable
    {
        [Serializable]
        public enum Layout
        {
            Unspecified,
            Small,
            Medium,
            Large
        }

        private const string InternalID = nameof(PhasedArtefactPresenter) + "INTERNAL";
        private const string Graphics = "Graphics";
        private const string SelectAnimation = "Select Animation";
        private const string RevealAnimation = "Reveal Animation";

        [SerializeField] private Layout layoutType;
        [BoxGroup(Graphics)] [SerializeField] private TMP_Text slug;
        [BoxGroup(Graphics)] [SerializeField] private TMP_Text id;
        [BoxGroup(Graphics)] [SerializeField] private TMP_Text title;
        [BoxGroup(Graphics)] [SerializeField] private TMP_Text description;
        [BoxGroup(Graphics)] [SerializeField] private Image[] swatches;
        [BoxGroup(Graphics)] [SerializeField] private Image thumbnail;

        [BoxGroup(SelectAnimation)] [Min(0), SerializeField]
        private float growVertical = 50f;

        [BoxGroup(SelectAnimation)] [Min(0), SerializeField]
        private float growHorizontal = 0;
        
        [BoxGroup(SelectAnimation)] [SerializeField]
        private bool maintainVerticalSize = false;

        [BoxGroup(SelectAnimation)] [SerializeField]
        private bool maintainHorizontalSize = true;

        [BoxGroup(SelectAnimation)] [SerializeField]
        private RectTransform maskRect;

        [BoxGroup(SelectAnimation)] [SerializeField]
        private RectTransform thumbRect;

        [BoxGroup(SelectAnimation)] [SerializeField]
        private CanvasGroup plusGroup;

        [BoxGroup(SelectAnimation)] [Min(0), SerializeField]
        private float duration = 1f;

        [BoxGroup(RevealAnimation)] [Min(0), SerializeField]
        private float delayOpen = 1f;

        [BoxGroup(RevealAnimation)] [Min(0), SerializeField]
        private float addRandomDelay = .5f;

        [BoxGroup(RevealAnimation)] [Min(0), SerializeField]
        private float delayClose = 0f;

        private RectTransform _selfRect;
        private RectTransform _plusRect;
        private RectTransform _titleRect;
        private RectTransform _descriptionRect;

        //Vector3 _originalTitlePos;
        Vector2 _originalTitleDelta;

        //Vector3 _originalDescriptionPos;
        Vector2 _originalDescriptionDelta;

        Vector2 _originalSizeDelta;
        Vector2 _originalMaskDelta;
        Vector2 _originalThumbDelta;
        Vector2 _originalPlusDelta;
        [NonSerialized]
        private bool _isInit;

        public Layout LayoutType => layoutType;


        protected void Reset()
        {
            _selfRect = GetComponent<RectTransform>();
            if (selectButton == null)
                selectButton = GetComponent<Button>();
        }

        protected void Awake() => EnsureInit();

        private void EnsureInit()
        {
            if (_isInit)
                return;
            _isInit = true;

            Debug.Assert(maskRect != null, "maskRect != null", this);
            Debug.Assert(thumbRect != null, "thumbRect != null", this);
            Debug.Assert(plusGroup != null, "plusGroup != null", this);

            _selfRect = GetComponent<RectTransform>();
            Debug.Assert(_selfRect != null, "_selfRect != null", this);

            _titleRect = title.GetComponent<RectTransform>();
            Debug.Assert(_titleRect != null, "_titleRect != null", this);

            _descriptionRect = description.GetComponent<RectTransform>();
            Debug.Assert(_descriptionRect != null, "_descriptionRect != null", this);

            _plusRect = plusGroup.GetComponent<RectTransform>();
            Debug.Assert(_plusRect != null, "_plusRect != null", this);

            if (selectButton == null)
                selectButton = GetComponent<Button>();

            // store original rects
            _originalSizeDelta = _selfRect.sizeDelta;
            _originalMaskDelta = maskRect.sizeDelta;
            _originalThumbDelta = thumbRect.sizeDelta;
            _originalPlusDelta = _plusRect.sizeDelta;
            _originalDescriptionDelta = _descriptionRect.sizeDelta;
            _originalTitleDelta = _titleRect.sizeDelta;
            //_originalDescriptionPos = _descriptionRect.localPosition;
            //_originalTitlePos = _titleRect.localPosition;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            plusGroup.alpha = IsSelected ? 1f : 0f;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DOTween.Kill(this, InternalID);
        }


        protected override void OnRecordChanged()
        {
            EnsureInit();
            ResetInternalState();

            if (Record == null)
            {
                DOTween.Kill(this, InternalID);
                SetVisible(false);
                return;
            }

            SetVisible(true);

            foreach (var swatch in swatches)
            {
                if (swatch != null)
                {
                    var isParsable = ColorUtility.TryParseHtmlString(Record.Topic.Color, out Color color);
                    swatch.color = isParsable ? color : Color.white;
                }
            }

            if (slug != null)
                slug.text = Record?.Name ?? "NULL";
            if (id != null)
                id.text = $"{(Record != null ? Record.Id.ToString() : "NULL")}";
            if (thumbnail != null)
            {
                thumbnail.sprite = Record?.Thumbnail != null && !string.IsNullOrEmpty(Record.Thumbnail.FilenameDisk)
                    ? GetPreviewSprite(Record.Thumbnail)
                    : null;
                thumbnail.enabled = thumbnail.sprite != null;
            }else
            {
                thumbnail.enabled = false;
            }

            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }

        protected override void OnLocaleChanged(Locale locale)
        {
            if (!this)
                return;

            EnsureInit();

            if (Record == null)
            {
                if (debug)
                    Debug.Log(
                    $"{nameof(PhasedArtefactPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                    this);
                return;
            }

            if (Record.Translations == null)
            {
                Debug.LogWarning(
                    $"{nameof(PhasedArtefactPresenter)} : Invalid translations on record <{Record.__Table}>[{Record.__Primary}]",
                    this);
                return;
            }

            var hasTranslation = DirectusExtensions.TryGetTranslation(locale, Record.Translations, out ArtefactsTranslationsRecord best);
            if (hasTranslation)
            {
                if (title != null)
                    title.text = best.Title;
            }
            else
            {
                if (title != null)
                    title.text = TranslationMissing;
            }

            if (description != null)
            {
                description.text = Record.Layout switch
                {
                    ArtefactsRecord.LayoutChoices.Video => "Video",
                    ArtefactsRecord.LayoutChoices.Slideshow => "Slideshow",
                    ArtefactsRecord.LayoutChoices.Model => "3D Model",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        protected override void OnClose()
        {
            
        }
        
        public void Reveal()
        {
            Open();

            float rng = Random.Range(0, addRandomDelay) + delayOpen;
            DOTween.Kill(this, InternalID);
            DOTween.Sequence()
                .Join(selfGroup
                    .DOFade(1f, duration)
                    .From(0)
                    .SetDelay(rng)
                    .SetEase(Ease.InQuad)
                )
                .Join(thumbRect
                    .GetComponent<Image>()
                    .DOFade(1f, duration)
                    .From(0)
                    .SetDelay(rng + .3f)
                    .SetEase(Ease.InQuad)
                )
                .SetTarget(this)
                .SetId(InternalID)
                .SetAutoKill(true)
                .Restart();
        }

        protected override void OnOpen()
        {
            EnsureInit();
            ResetInternalState();
        }

        protected override void OnSelectWithoutNotify()
        {
            EnsureInit();
            DOTween.Kill(this, InternalID, true);
            DOTween.Sequence()
                .Join(_selfRect
                    .DOSizeDelta(
                        new Vector2(
                            _selfRect.sizeDelta.x + (maintainHorizontalSize ? 0 : growHorizontal),
                            _selfRect.sizeDelta.y + (maintainVerticalSize ? 0 : growVertical)
                        ),
                        duration
                    )
                )
                .Join(maskRect
                    .DOSizeDelta(
                        new Vector2(maskRect.sizeDelta.x, maskRect.sizeDelta.y - growVertical),
                        duration)
                )
                .Join(thumbRect
                    .DOSizeDelta(
                        new Vector2(thumbRect.sizeDelta.x * 1.3f, thumbRect.sizeDelta.y * 1.3f),
                        duration)
                )
                .Join(_plusRect
                    .DOSizeDelta(new Vector2(_plusRect.sizeDelta.x, _plusRect.sizeDelta.y + growVertical),
                        duration)
                )
                .Join(plusGroup
                    .DOFade(1f, duration)
                )
                .Join(_titleRect
                    .DOSizeDelta(
                        new Vector2(_titleRect.sizeDelta.x - _plusRect.rect.width, _titleRect.sizeDelta.y + growVertical),
                        duration).SetEase(Ease.OutQuad))
                .Join(description.DOFade(1f, duration))
                .SetTarget(this)
                .SetId(InternalID)
                .SetAutoKill(true)
                .Restart();
        }

        protected override void OnDeSelectWithoutNotify()
        {
            EnsureInit();

            if (!enabled)
            {
                ResetInternalState();
                return;
            }

            DOTween.Kill(this, InternalID, true);
            DOTween.Sequence()
                .Join(_selfRect.DOSizeDelta(_originalSizeDelta, duration))
                .Join(_plusRect.DOSizeDelta(_originalPlusDelta, duration))
                .Join(maskRect.DOSizeDelta(_originalMaskDelta, duration))
                .Join(thumbRect.DOSizeDelta(_originalThumbDelta, duration))
                .Join(_titleRect.DOSizeDelta(_originalTitleDelta, duration))
                .Join(plusGroup.DOFade(0f, duration))
                .Join(description.DOFade(0f, duration))
                .SetTarget(this)
                .SetId(InternalID)
                .SetAutoKill(true)
                .Restart();
        }

        private void ResetInternalState()
        {
            EnsureInit();
            _selfRect.sizeDelta = _originalSizeDelta;
            _plusRect.sizeDelta = _originalPlusDelta;
            maskRect.sizeDelta = _originalMaskDelta;
            thumbRect.sizeDelta = _originalThumbDelta;
            _descriptionRect.sizeDelta = _originalDescriptionDelta;
            _titleRect.sizeDelta = _originalTitleDelta;
            //_descriptionRect.localPosition = _originalDescriptionPos;
            //_titleRect.localPosition = _originalTitlePos;
            plusGroup.alpha = 0;
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}