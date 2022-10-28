using Directus.Connect.v9;
using Directus.Generated;
using Exploratorium.Utility;
using Markdig;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    internal class PersonPresenter : RecordPresenter<PersonsRecord>
    {
        [SerializeField] private TMP_Text slug;
        [SerializeField] private TMP_Text id;
        [SerializeField] private Image thumbnail;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TabContent tabContent;
        public TabContent TabContent => tabContent;

        protected override void OnDeselect()
        {
        }

        protected override void OnSelect()
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

            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }

        protected override void OnLocaleChanged(Locale locale)
        {
            if (Record == null)
            {
                if (debug)
                    Debug.LogWarning(
                        $"{nameof(PersonPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                        this);
                return;
            }

            if (DirectusExtensions.TryGetTranslation(locale, Record.Translations, out PersonsTranslationsRecord best))
            {
                if (description != null)
                {
                    description.text = description.richText
                        ? best.Role?
                            .ToHtml()
                            .ParseHtml()
                            .RemoveTags(in HtmlExtensions.SkbgTags)
                            .ReplaceTag(in HtmlExtensions.SkbgTagsToTextMeshProTags)
                            .DocumentNode
                            .InnerHtml
                        : ParsingUtils.GetRawText(best.Role);
                }

                if (tabContent != null)
                {
                    tabContent.DisplayName = best.Title;
                }
            }
        }

        protected override void OnClose()
        {
        }

        protected override void OnOpen()
        {
        }
    }
}