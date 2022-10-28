using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Directus.Generated;
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
    internal class ShortcutsPresenter : RecordPresenter<SectionsRecord>
    {
        [SerializeField] private PhasedSectionPresenter sectionPrefab;
        //[SerializeField] private RectTransform spawnParent;
        [BoxGroup("Phasing")] [SerializeField] private PhaseGroup phaseGroup;

        [BoxGroup("Graphics")] [SerializeField]
        private Image cloudsVfx;

        [BoxGroup("Graphics")] [SerializeField]
        private TMP_Text currentSection;

        [BoxGroup("Transition")] [SerializeField]
        private float delayContentChange = 1f;

        private static readonly int _ColorID = Shader.PropertyToID("_Color");


        public event Action<SectionsRecord> ShortcutActivated;
        public event Action<SectionsRecord> ShortcutSelected;

        private void Awake()
        {
            Debug.Assert(phaseGroup != null, "phaseGroup != null", this);
            //Debug.Assert(spawnParent != null, "spawnParent != null");
            Debug.Assert(sectionPrefab != null, "sectionPrefab != null", this);
        }

        private void OnSelected(RecordPresenter<SectionsRecord> source)
        {
            if (debug) Debug.Log($"{nameof(HomePresenter)} : Section {source.Record.Id} {source.Record.Name} selected", this);
            ShortcutSelected?.Invoke(source.Record);
        }

        private void OnActivated(RecordPresenter<SectionsRecord> source)
        {
            if (debug)
                Debug.Log($"{nameof(HomePresenter)} : Section {source.Record.Id} {source.Record.Name} activated", this);
            ShortcutActivated?.Invoke(source.Record);
        }

        protected override void OnDeselect()
        {
            // nothing to select
        }

        protected override void OnSelect()
        {
            // nothing to select
        }

        protected override void OnSelectedChanged(DbRecordWrapper record)
        {
            // nothing to select
        }

        protected override void OnRecordChanged()
        {
            OnClear();

            if (Record?.Parent == null)
            {
                Debug.LogWarning(
                    $"{nameof(ShortcutsPresenter)} : Cannot display sibling shortcuts without a valid parent", this);
                return;
            }

            Debug.Assert(phaseGroup != null, "phaseGroup != null", this);
            Debug.Assert(phaseGroup.Slots.Count > 0, "phaseGroup.Slots.Count > 0", this);
            Debug.Assert(phaseGroup.Slots.All(it => it != null), "phaseGroup.Slots.All(it => it != null)", this);
            
            // get slots and records in a correlated order
            // we want records to be sorted into slots based on sorting from top to bottom
            PhasedSectionPresenter[] slots = phaseGroup.Slots
                .OfType<PhasedSectionPresenter>()
                .OrderBy(it => it.transform.localPosition.y)
                .ToArray();
            SectionsRecord[] siblings = Record.Parent.Children
                .Except(new[] { Record })
                .OrderBy(it => it.Sort)
                .ToArray();
            
            // fill slots, break when either slots or records run out, hide/reset unused slots.
            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (i < siblings.Length)
                {
                    var record = siblings[i];
                    Debug.Assert(record != null, "section != null", this);
                    slot.SetVisible(true);
                    slot.name = $"SLOT -- {record.Id}:{record.Name}";
                    slot.Record = record;
                    slot.Activated += OnActivated;
                    slot.Selected += OnSelected;
                    slot.Open();
                }
                else
                {
                    slot.name = $"SLOT -- empty";
                    slot.Record = null;
                    slot.SetVisible(false); // not necessary but more explicit this way
                }
            }

            if (cloudsVfx != null && Record != null)
            {
                var isParsed = ColorUtility.TryParseHtmlString(Record.Color, out var color);
                if (debug)
                    Debug.Log($"Parsed {Record.Color} to {color}");
                if (isParsed)
                {
                    cloudsVfx.material.DOColor(color, delayContentChange);
                    //cloudsVfx.material.SetColor(_ColorID, color);
                }
                else
                {
                    cloudsVfx.material.DOColor(new Color(.4f, .42f, .5f), delayContentChange);
                    //cloudsVfx.material.SetColor(_ColorID, new Color(.4f, .42f, .5f));
                }
            }

            OnLocaleChanged(LocalizationSettings.SelectedLocale);

            //LayoutRebuilder.MarkLayoutForRebuild(spawnParent);
        }


        protected override void OnLocaleChanged(Locale locale)
        {
            if (Record == null)
            {
                if (debug) 
                    Debug.LogWarning(
                        $"{nameof(ShortcutsPresenter)} : Locale change ignored. {name} has no record assigned (yet).",
                        this);
                
                return;
            }
            
            if (debug) Debug.Log($"{nameof(ShortcutsPresenter)} : Locale changed to {locale.LocaleName}", this);
            if (DirectusExtensions.TryGetTranslation(LocalizationSettings.SelectedLocale, Record.Translations,
                out var best))
            {
                if (currentSection != null)
                    currentSection.DOText(best.Name, 0).SetDelay(delayContentChange);
            }
            else
            {
                if (currentSection != null)
                    currentSection.text = "[?]";
            }
        }

        protected override void OnClose()
        {
            phaseGroup.RecenterViews();

            phaseGroup.Slots
                .OfType<PhasedSectionPresenter>()
                .Where(it => it.Record != null)
                .ForEach(it => it.Close());
        }

        protected override void OnOpen()
        {
            phaseGroup.RecenterViews();
            IEnumerable<PhasedSectionPresenter> slots = phaseGroup.Slots
                .OfType<PhasedSectionPresenter>()
                .Where(it => it.Record != null);

            slots.ForEach(it => it.Open());
        }
        
        private void OnClear()
        {
            phaseGroup.Slots
                .Where(it => it.IsValid)
                .OfType<PhasedSectionPresenter>()
                .ForEach(it =>
                {
                    it.Activated -= OnActivated;
                    it.Selected -= OnSelected;
                    it.Close();
                    it.Record = null;
                });
        }
    }
}