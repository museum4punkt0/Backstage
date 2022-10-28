using DG.Tweening;
using Directus.Connect.v9;
using Directus.Generated;
using Exploratorium.Utility;
using Markdig;
using Sirenix.OdinInspector;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    public class SectionPresenter : RecordPresenter<SectionsRecord>
    {
        [BoxGroup("Graphics")] [SerializeField]
        private TMP_Text slug;

        [BoxGroup("Graphics")] [SerializeField]
        private TMP_Text id;

        [BoxGroup("Graphics")] [SerializeField]
        private Image thumbnail;

        [BoxGroup("Graphics")] [SerializeField]
        private TMP_Text label;

        [BoxGroup("Graphics")] [SerializeField]
        private TMP_Text description;

        [BoxGroup("Graphics")] [SerializeField]
        private Image colorSwatch;

        [BoxGroup("Graphics")] [SerializeField]
        private Image secondaryColorSwatch;

        [BoxGroup("Tabs")] [SerializeField]
        private TabContent tabContent;

        [BoxGroup("Transition")] [SerializeField]
        private float delayContentChange = 1f;

        public TabContent TabContent => tabContent;
        protected Image ColorSwatch => colorSwatch;
        protected Image SecondaryColorSwatch => secondaryColorSwatch;
        protected TMP_Text Description => description;
        protected TMP_Text Label => label;
        protected Image Thumbnail => thumbnail;
        protected TMP_Text ID => id;
        protected TMP_Text Slug => slug;

        protected override void OnSelect()
        {
        }

        protected override void OnDeselect()
        {
        }

        protected override void OnSelectedChanged(DbRecordWrapper record)
        {
        }

        protected override void OnRecordChanged()
        {
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
            }

            if (Record != null)
            {
                var isParsed = ColorUtility.TryParseHtmlString(Record.Color, out var color);
                if (debug)
                    Debug.Log($"Parsed {Record.Color} to {color}");

                if (colorSwatch != null)
                {
                    if (isParsed)
                        colorSwatch.DOColor(color, delayContentChange);
                    else
                        colorSwatch.DOColor(new Color32(0xee, 0xee, 0xee, 0xff), delayContentChange);
                }

                if (secondaryColorSwatch != null)
                {
                    if (isParsed)
                        secondaryColorSwatch.DOColor(color, delayContentChange);
                    else
                        secondaryColorSwatch.DOColor(new Color32(0xee, 0xee, 0xee, 0xff), delayContentChange);
                }

                OnLocaleChanged(LocalizationSettings.SelectedLocale);
            }
            else
            {
                if (debug)
                    Debug.LogWarning(
                        $"{nameof(SectionPresenter)} : Record change ignored. {name} has no record assigned (yet).",
                        this);
            }
        }


        protected override void OnLocaleChanged(Locale locale)
        {
            if (Record == null)
            {
                if (debug)
                    Debug.LogWarning(
                        $"{nameof(SectionPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                        this);
                return;
            }

            Debug.Assert(Record != null && Record.Translations != null,
                "Record != null && Record.Translations != null");

            if (DirectusExtensions.TryGetTranslation(locale, Record.Translations, out SectionsTranslationsRecord best))
            {
                if (label != null)
                {
                    label.DOText(best.Name, 0).SetDelay(delayContentChange);
                }

                if (description != null)
                {
                    if (description.richText)
                    {
                        string txt = best.Text?
                            .ToHtml()
                            .ParseHtml()
                            .RemoveTags(in HtmlExtensions.SkbgTags)
                            .ReplaceTag(in HtmlExtensions.SkbgTagsToTextMeshProTags)
                            .DocumentNode
                            .InnerHtml;
                        description.DOText(txt, 0).SetDelay(delayContentChange);
                    }
                    else
                    {
                        string txt = ParsingUtils.GetRawText(best.Text);
                        description.DOText(txt, 0).SetDelay(delayContentChange);
                    }
                }

                if (tabContent != null)
                {
                    tabContent.DisplayName = best.Name;
                }
            }
            else
            {
                if (label != null)
                    label.DOText("", 0).SetDelay(delayContentChange);
                if (description != null)
                    description.DOText("", 0).SetDelay(delayContentChange);
                if (tabContent != null)
                    tabContent.DisplayName = "";
            }

            RebuildOpenables();
        }


        protected override void OnClose()
        {
        }

        protected override void OnOpen()
        {
        }
    }
}