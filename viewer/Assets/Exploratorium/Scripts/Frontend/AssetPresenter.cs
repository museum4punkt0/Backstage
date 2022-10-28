using Directus.Generated;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    public class AssetPresenter : RecordPresenter<AssetsRecord>
    {
        [SerializeField] private TMP_Text slug;
        [SerializeField] private TMP_Text id;
        [SerializeField] private RawImage thumbnail;

        protected override void OnDeselect()
        {
        }

        protected override void OnLocaleChanged(Locale locale)
        {
        }

        protected override void OnClose()
        {
        }

        protected override void OnOpen()
        {
        }

        protected override void OnSelect()
        {
            Debug.Log($"Selected {typeof(AssetsRecord)} {Record.Name}");
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
        }
    }
}