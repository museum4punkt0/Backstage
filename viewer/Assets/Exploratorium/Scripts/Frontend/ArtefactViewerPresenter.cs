using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Directus.Connect.v9;
using Directus.Generated;
using Exploratorium.Utility;
using Markdig;
using Markdig.Renderers;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Exploratorium.Frontend
{
    public class ArtefactViewerPresenter : RecordPresenter<ArtefactsRecord>
    {
        [BoxGroup("Debug"), SerializeField]
        private TMP_Text slug;

        [BoxGroup("Debug"), SerializeField]
        private TMP_Text id;

        [BoxGroup("Debug"), SerializeField]
        private Image thumbnail;

        [BoxGroup("Viewers"), SerializeField]
        private VideoPresenter videoPresenter;

        [BoxGroup("Viewers"), SerializeField]
        private SlideshowPresenter slideshowPresenter;

        [BoxGroup("Viewers"), SerializeField]
        private ModelPresenter modelPresenter;

        [BoxGroup("Info Tab"), SerializeField]
        private TabContent infoTab;

        [BoxGroup("Info Tab"), SerializeField]
        private TMP_Text title;

        [BoxGroup("Info Tab"), SerializeField]
        private TMP_Text descriptionCol1;

        [BoxGroup("Info Tab"), SerializeField]
        private TMP_Text descriptionCol2;

        [BoxGroup("Other Tabs"), SerializeField]
        private TabGroup tabGroup;

        [BoxGroup("Other Tabs"), SerializeField]
        private PersonPresenter personContentPrefab;

        [BoxGroup("Other Tabs"), SerializeField]
        private SectionPresenter sectionContentPrefab;

        [BoxGroup("Other Tabs"), SerializeField]
        private RectTransform tabContentContainer;

        [InfoBox(
            "Layout passes should match the depth of nesting of ContentSizeFitters below the Rebuild Layout Groups")]
        [BoxGroup("Layout"), SerializeField]
        private int layoutPasses;

        [BoxGroup("Layout"), SerializeField]
        private bool immediateRebuild;

        [BoxGroup("Layout"), SerializeField]
        private LayoutGroup[] rebuildLayoutGroups;

        private readonly List<TabContent> _tabs = new List<TabContent>();

        protected override void OnDeselect()
        {
            if (Record != null)
                if (debug)
                    Debug.Log($"{nameof(ArtefactViewerPresenter)} : Deselected {typeof(ArtefactsRecord)} {Record.Name}",
                        this);
        }

        protected override void OnLocaleChanged(Locale locale)
        {
            if (Record == null)
            {
                Debug.LogWarning(
                    $"{nameof(ArtefactViewerPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                    this);
                return;
            }

            if (Record.Translations == null)
            {
                Debug.LogWarning(
                    $"{nameof(ArtefactViewerPresenter)} : Invalid translations on record <{Record.__Table}>[{Record.__Primary}]",
                    this);
                return;
            }

            if (DirectusExtensions.TryGetTranslation(locale, Record.Translations, out ArtefactsTranslationsRecord best))
            {
                HtmlRenderer htmlRenderer = new HtmlRenderer(new StringWriter());
                htmlRenderer.Write(best.Text);
                if (title != null)
                {
                    title.text = PreprocessText(best.Title);
                }

                if (descriptionCol1 != null)
                {
                    descriptionCol1.text = descriptionCol1.richText
                        ? best.Text?
                            .ToHtml()
                            .ParseHtml()
                            .RemoveTags(in HtmlExtensions.SkbgTags)
                            .ReplaceTag(in HtmlExtensions.SkbgTagsToTextMeshProTags)
                            .DocumentNode
                            .InnerHtml
                        : ParsingUtils.GetRawText(best.Text);
                }

                if (descriptionCol2 != null)
                {
                    descriptionCol2.text = descriptionCol2.richText
                        ? best.Text?
                            .ToHtml()
                            .ParseHtml()
                            .RemoveTags(in HtmlExtensions.SkbgTags)
                            .ReplaceTag(in HtmlExtensions.SkbgTagsToTextMeshProTags)
                            .DocumentNode
                            .InnerHtml
                        : ParsingUtils.GetRawText(best.Text);
                }
            }
            else
            {
                if (title != null)
                    title.text = TranslationMissing;
                if (descriptionCol1 != null)
                    descriptionCol1.text = TranslationMissing;
                if (descriptionCol2 != null)
                    descriptionCol2.text = TranslationMissing;
            }

            UniTask.Void(async () =>
            {
                // rebuild layouts once for each level of nested content size fitters
                // TODO: this is the currently the only known way to make layouts update correctly in a cascade of fitters
                // TODO: there is probably a way to make this less hacky
                if (debug) Debug.Log($"STARTING TO REBUILD LAYOUT {layoutPasses} TIMES ", this);
                for (int i = 0; i < layoutPasses; i++)
                {
                    if (debug) Debug.Log($"STARTING LAYOUT REBUILD PASS {i} ", this);
                    RebuildLayoutGroups();
                    if (!immediateRebuild)
                        await UniTask.NextFrame();
                    if (debug) Debug.Log($"FINISHED LAYOUT REBUILD PASS {i}", this);
                }

                if (debug) Debug.Log($"FINISHED REBUILDING LAYOUTS {layoutPasses} TIMES", this);
                RebuildOpenableTweens(); // rebuild only the openables that have their parent layout changed
            });
        }

        protected override void OnClose()
        {
            // cascade close signal to all dependent viewers
            slideshowPresenter.CloseAsync().Forget();
            videoPresenter.CloseAsync().Forget();
            modelPresenter.CloseAsync().Forget();
        }

        protected override void OnOpen()
        {
            // ignore
        }

        protected override void OnSelect()
        {
            if (debug)
                Debug.Log($"{nameof(ArtefactViewerPresenter)} : Selected {typeof(ArtefactsRecord)} {Record.Name}",
                    this);
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
            if (thumbnail != null && Record?.Thumbnail != null && !string.IsNullOrEmpty(Record.Thumbnail.FilenameDisk))
            {
                if (Record?.Thumbnail != null && !string.IsNullOrEmpty(Record.Thumbnail.FilenameDisk))
                {
                    Texture2D tx = GetPreviewTx(Record.Thumbnail);
                    float spriteAspect = thumbnail.rectTransform.rect.width / thumbnail.rectTransform.rect.height;
                    thumbnail.sprite = ResizeImage.CreateFilledSprite(spriteAspect, tx);
                }
                else
                {
                    thumbnail.sprite = null;
                }

                thumbnail.enabled = thumbnail.sprite != null;
            }
            
            
            if (tabGroup != null)
                tabGroup.DefaultTab = infoTab;

            // remove existing tabs
            foreach (var tab in _tabs)
                Destroy(tab.gameObject);
            _tabs.Clear();

            // set highlight color
            if (tabGroup != null)
            {
                Color color = Color.white;
                bool hasColor = Record.Topic != null && ColorUtility.TryParseHtmlString(Record.Topic.Color, out color);
                //tabGroup.SetOnColor(color);
            }

            // create new tabs
            if (Record.Section != null && personContentPrefab.TabContent != null)
            {
                SectionPresenter presenter = Instantiate(sectionContentPrefab, tabContentContainer);
                presenter.Record = Record.Section;
                presenter.TabContent.SetGroup(tabGroup);
                presenter.TabContent.SetActiveWithoutNotify(false);
                _tabs.Add(presenter.TabContent);
            }

            if (Record.Persons.Length > 0)
            {
                foreach (var personRecord in Record.Persons.Where(it => it != null).Take(2))
                {
                    if (personContentPrefab.TabContent == null)
                        continue;

                    PersonPresenter presenter = Instantiate(personContentPrefab, tabContentContainer);
                    presenter.Record = personRecord;
                    presenter.TabContent.SetGroup(tabGroup);
                    presenter.TabContent.SetActiveWithoutNotify(false);
                    _tabs.Add(presenter.TabContent);
                }
            }

            OnLocaleChanged(locale: LocalizationSettings.SelectedLocale);

            switch (Record.Layout)
            {
                case ArtefactsRecord.LayoutChoices.Video:
                    slideshowPresenter.CloseAsync().Forget();
                    modelPresenter.CloseAsync().Forget();
                    videoPresenter.Clear();
                    videoPresenter.Show(Record.Assets);
                    videoPresenter.OpenAsync().Forget();
                    break;
                case ArtefactsRecord.LayoutChoices.Slideshow:
                    videoPresenter.CloseAsync().Forget();
                    modelPresenter.CloseAsync().Forget();
                    slideshowPresenter.Clear();
                    slideshowPresenter.Show(Record.Assets);
                    slideshowPresenter.OpenAsync().Forget();
                    break;
                case ArtefactsRecord.LayoutChoices.Model:
                    videoPresenter.CloseAsync().Forget();
                    slideshowPresenter.CloseAsync().Forget();
                    modelPresenter.Clear();
                    modelPresenter.Show(Record.Assets);
                    modelPresenter.OpenAsync().Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (tabGroup != null)
                tabGroup.RequestTabActivation(tabGroup.DefaultTab);
        }

        [Button]
        private void RebuildLayoutGroups()
        {
            if (debug)
                Debug.LogWarning($"STARTING TO REBUILD LAYOUTS ----------------------------------------- ", this);
            if (immediateRebuild)
            {
                rebuildLayoutGroups?
                    .ForEach(it =>
                    {
                        if (debug) Debug.LogWarning($"REBUILDING {it.name} IMMEDIATELY", this);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(it.GetComponent<RectTransform>());
                    });
            }
            else
            {
                rebuildLayoutGroups?
                    .ForEach(it =>
                    {
                        if (debug) Debug.LogWarning($"REBUILDING {it.name}", this);
                        LayoutRebuilder.MarkLayoutForRebuild(it.GetComponent<RectTransform>());
                    });
            }

            if (debug)
                Debug.LogWarning($"FINISHED REBUILDING LAYOUTS ----------------------------------------- ", this);
        }

        [Button]
        private void RebuildOpenableTweens()
        {
            if (debug)
                Debug.LogWarning($"STARTING TO REBUILD TWEENS ++++++++++++++++++++++++++++++++++++++++++++++++ ", this);
            foreach (var layoutGroup in rebuildLayoutGroups)
            {
                // this has to be a forced rebuild because the Openable component will not be marked dirty and would
                // otherwise ignore the rebuild
                layoutGroup.GetComponentsInChildren<OpenableGroup>().ForEach(it => it.RebuildTweens(true));
            }

            if (debug)
                Debug.LogWarning($"FINISHED REBUILDING TWEENS ++++++++++++++++++++++++++++++++++++++++++++++++ ", this);
        }

        public void MarkAsForeground()
        {
            slideshowPresenter.PermitSync = true;
            videoPresenter.PermitSync = true;
            modelPresenter.PermitSync = true;
            Debug.Log($"{nameof(ArtefactViewerPresenter)} : {name} is now in foreground", this);
        }

        public void MarkAsBackground()
        {
            slideshowPresenter.PermitSync = false;
            videoPresenter.PermitSync = false;
            modelPresenter.PermitSync = false;
            Debug.Log($"{nameof(ArtefactViewerPresenter)} : {name} is now in background", this);
        }
    }
}