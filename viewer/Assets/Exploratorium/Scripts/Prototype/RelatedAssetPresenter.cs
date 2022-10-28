// //////////////////////////////////////////////////////
// THIS IS PROTOTYPE CODE AND NOT INTENDED FOR PRODUCTION
// TODO: Make this production ready
// //////////////////////////////////////////////////////
using System;
using Directus.Generated;
using Exploratorium.Frontend;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Exploratorium.Prototype
{
    public class RelatedAssetPresenter : RecordPresenter<ArtefactsRecord>
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image image;

        protected override void OnDeselect()
        {
            throw new NotImplementedException();
        }

        protected override void OnLocaleChanged(Locale locale)
        {
            if (Record == null)
            {
                Debug.LogWarning(
                    $"{nameof(RelatedAssetPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                    this);
                return;
            }

            bool any = Record.Translations.TryGetBestAvailableCode(locale.Identifier.Code,
                out var best);
            if (any && Record.Translations.TryGetTranslation(best, out var translationsRecord))
            {
                label.text = $"{translationsRecord.Title}";
                description.text = $"{translationsRecord.Text}";
            }
        }

        protected override void OnClose()
        {
        }

        protected override void OnOpen()
        {
        }

        protected override void OnSelect()
        {
            Debug.Log($"{name} with record {Record?.Name} selected");
            MakeBig();
        }

        protected override void OnSelectedChanged(DbRecordWrapper record)
        {
        }

        private void MakeBig()
        {
            Debug.Log($"{name} with record {Record?.Name} is now big");
        }

        protected override void OnRecordChanged()
        {
            if (Record == null)
                return;

            image.sprite = GetPreviewSprite(Record.Thumbnail);
            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }
    }
}