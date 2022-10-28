using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Directus.Connect.v9;
using Directus.Generated;
using Exploratorium.Extras;
using Exploratorium.Utility;
using Markdig;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    [Obsolete]
    public class SimpleArtefactPresenter : RecordPresenter<ArtefactsRecord>
    {
        [BoxGroup("Graphics")] [SerializeField]
        private TMP_Text slug;

        [BoxGroup("Graphics")] [SerializeField]
        private TMP_Text id;

        [BoxGroup("Graphics")] [SerializeField]
        private List<Image> thumbnails = new List<Image>();

        [BoxGroup("Graphics")] [SerializeField]
        private List<TMP_Text> titles = new List<TMP_Text>();

        [BoxGroup("Graphics")] [SerializeField]
        private List<TMP_Text> descriptions = new List<TMP_Text>();

        [SerializeField] private LayoutElement layoutElement;

        private bool HasLayoutElement => layoutElement != null;

        [SerializeField, BoxGroup("De-Selected")]
        private CanvasGroup deselectedState;

        [SerializeField, BoxGroup("De-Selected"), ShowIf(nameof(HasLayoutElement))]
        private Vector2 deselectedSize;

        [SerializeField, BoxGroup("Selected")]
        private CanvasGroup selectedState;

        [SerializeField, BoxGroup("Selected"), ShowIf(nameof(HasLayoutElement))]
        private Vector2 selectedSize;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private int _count;


        public event Action<RecordPresenter<ArtefactsRecord>> Activated;

        private void Awake()
        {
            Debug.Assert(titles.All(it => it != null), "titles.All(it => it != null)");
            Debug.Assert(descriptions.All(it => it != null), "descriptions.All(it => it != null)");
            Debug.Assert(thumbnails.All(it => it != null), "thumbnails.All(it => it != null)");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _count = 0;
        }

        protected override void OnDeselect()
        {
            if (debug)
                Debug.Log($"{nameof(SimpleArtefactPresenter)} : Deselected {typeof(ArtefactsRecord)} {Record.Name}",
                    this);

            _count = 0;
            UpdateViewState();
        }

        protected override void OnSelect()
        {
            if (debug)
                Debug.Log($"{nameof(SimpleArtefactPresenter)} :Selected {typeof(ArtefactsRecord)} {Record.Name}", this);

            _count++;
            if (_count == 2)
                Activated?.Invoke(this);

            UpdateViewState();
        }

        protected override void OnSelectedChanged(DbRecordWrapper record)
        {
            var selectedArtefact = record.Record as ArtefactsRecord;
            if (selectedArtefact == null || Record != selectedArtefact)
            {
                if (debug) Debug.Log($"{nameof(SimpleArtefactPresenter)} :Reset selected count on {name}", this);
                _count = 0;
            }

            UpdateViewState();
        }


        private void UpdateViewState()
        {
            EnableDeselectedView(_count <= 0);
            EnableSelectedView(_count > 0);

            var rt = GetComponent<RectTransform>();
            if (_count > 0)
            {
                rt.DOSizeDelta(selectedSize, 0.5f);
            }
            else
            {
                rt.DOSizeDelta(deselectedSize, 0.5f);
            }

            if (layoutElement != null)
            {
                //layoutElement.minWidth = _count > 0 ? selectedSize.x : deselectedSize.x;
                //layoutElement.minHeight = _count > 0 ? selectedSize.y : deselectedSize.y;
            }

            void EnableDeselectedView(bool isEnabled)
            {
                if (deselectedState != null)
                    deselectedState.gameObject.SetActive(isEnabled);
            }

            void EnableSelectedView(bool isEnabled)
            {
                if (selectedState != null)
                    selectedState.gameObject.SetActive(isEnabled);
            }
        }


        protected override void OnLocaleChanged(Locale locale)
        {
            if (Record == null)
            {
                Debug.LogWarning(
                    $"{nameof(SimpleArtefactPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                    this);
                return;
            }

            if (Record.Translations == null)
            {
                Debug.LogWarning(
                    $"{nameof(SimpleArtefactPresenter)} : Invalid translations on record <{Record.__Table}>[{Record.__Primary}]",
                    this);
                return;
            }

            if (DirectusExtensions.TryGetTranslation(locale, Record.Translations, out ArtefactsTranslationsRecord best))
            {
                titles?.ForEach(it => it.text = PreprocessText(best.Title));

                descriptions?.ForEach(it =>
                {
                    it.text = it.richText
                        ? best.Text?
                            .ToHtml()
                            .ParseHtml()
                            .RemoveTags(in HtmlExtensions.SkbgTags)
                            .ReplaceTag(in HtmlExtensions.SkbgTagsToTextMeshProTags)
                            .DocumentNode
                            .InnerHtml
                        : ParsingUtils.GetRawText(best.Text);
                });
            }
            else
            {
                titles?.ForEach(it => it.text = TranslationMissing);
                descriptions?.ForEach(it => it.text = TranslationMissing);
            }

            titles?.ForEach(it => it.rectTransform.CycleContentSizeFitter());
            descriptions?.ForEach(it => it.rectTransform.CycleContentSizeFitter());
        }

        protected override void OnClose()
        {
            if (debug) Debug.Log($"{GetType().GetNiceName()} : {name} onClose", this);
        }

        protected override void OnOpen()
        {
            if (debug) Debug.Log($"{GetType().GetNiceName()} : {name} onOpen", this);
        }

        protected override void OnRecordChanged()
        {
            if (slug != null)
                slug.text = Record?.Name ?? "NULL";
            if (id != null)
                id.text = $"{(Record != null ? Record.Id.ToString() : "NULL")}";
            thumbnails?.ForEach(it =>
            {
                if (it != null)
                {
                    it.sprite = Record?.Thumbnail != null && !string.IsNullOrEmpty(Record.Thumbnail.FilenameDisk)
                        ? GetPreviewSprite(Record.Thumbnail)
                        : null;
                    it.enabled = it.sprite != null;
                }
            });


            if (Record?.Thumbnail != null && !string.IsNullOrEmpty(Record.Thumbnail.FilenameDisk))
            {
            }

            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }
    }
}