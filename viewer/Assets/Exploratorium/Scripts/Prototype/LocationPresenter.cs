// //////////////////////////////////////////////////////
// THIS IS PROTOTYPE CODE AND NOT INTENDED FOR PRODUCTION
// TODO: Make this production ready
// //////////////////////////////////////////////////////
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
    internal class LocationPresenter : RecordPresenter<LocationsRecord>
    {
        [SerializeField] private TMP_Text slug;
        [SerializeField] private TMP_Text id;
        [SerializeField] private RawImage thumbnail;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text coordinates;

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
            if (thumbnail != null && Record?.Thumbnail != null && !string.IsNullOrEmpty(Record.Thumbnail.FilenameDisk))
            {
                thumbnail.texture = GetPreviewTx(Record.Thumbnail);
                thumbnail.enabled = Record.Thumbnail != null;
            }
            else
            {
                thumbnail.enabled = false;
            }

            if (coordinates != null)
                coordinates.text = $"Coordinates: {Record.Longitude}, {Record.Latitude}";

            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }

        protected override void OnLocaleChanged(Locale locale)
        {
            if (Record == null)
            {
                Debug.LogWarning(
                    $"{nameof(LocationPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                    this);
                return;
            }
            
            if (DirectusExtensions.TryGetTranslation(locale, Record.Translations,
                out LocationsTranslationsRecord best))
            {
                if (description != null)
                    description.text = $"Country: {best.Country}\nPlace: {best.Place}";
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