using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Directus.Generated;
using Exploratorium.Utility;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    public class CloudArtefactsPresenter : PhasedRecordsPresenter<ArtefactsRecord>
    {
        [SerializeField] private float openDelayIncrement = 0f;
        [SerializeField] private float closeDelayIncrement = 0f;
        [SerializeField] private ScrollRect[] scrollRects;

        [SerializeField] private RectTransform viewport;

        private bool _isReveal;

        protected override void OnLocaleChanged(Locale locale)
        {
        }

        private Vector3 GetRectCenter(RectTransform rect)
        {
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            return (corners[0] + corners[1] + corners[2] + corners[3]) * 0.25f;
        }

        protected override void OnShow(params ArtefactsRecord[] records)
        {
            OnClear();
            if (debug) Debug.Log($"{nameof(CloudArtefactsPresenter)} : Showing {records.Length} artefacts");
            // reset scroll rects
            scrollRects.Where(it => it != null).ForEach(it => it.verticalNormalizedPosition = 0.5f);

            // order slots by distance, favoring big slots
            float openDelay = 0;
            float closeDelay = closeDelayIncrement * records.Length;
            PhasedArtefactPresenter[] slotsInOrder = PhaseGroup.Slots
                .OfType<PhasedArtefactPresenter>()
                .OrderBy(it =>
                {
                    float distance = Vector3.Distance(GetRectCenter(viewport),
                        GetRectCenter(it.transform.GetComponent<RectTransform>()));
                    return it.LayoutType == PhasedArtefactPresenter.Layout.Large
                        ? 0.25f * distance
                        : distance;
                })
                .ToArray();

            // randomize records but keep them ordered by priority
            List<ArtefactsRecord> shuffledRecords = records
                .Shuffled()
                .GroupBy(it => it.Priority)
                .OrderByDescending(it => it.Key)
                .SelectMany(it => it)
                .ToList();

            for (var i = 0; i < slotsInOrder.Length; i++)
            {
                PhasedArtefactPresenter slot = slotsInOrder[i];
                if (i < shuffledRecords.Count)
                {
                    var record = records[i];
                    slot.SetVisible(true);
                    slot.name = $"{slot.LayoutType} -- {record.Id}:{record.Name}";
                    slot.Record = record;
                    slot.SetAdditionalOpenDelay(openDelay);
                    slot.SetAdditionalCloseDelay(closeDelay);
                    slot.Activated += OnActivated;
                    slot.Selected += OnSelected;
                    openDelay += openDelayIncrement;
                    closeDelay -= closeDelayIncrement;
                }
                else
                {
                    slot.name = $"{slot.LayoutType} -- empty";
                    slot.Record = null;
                }
            }

            slotsInOrder.ForEach(it => it.RebuildOpenables());
        }

        protected override void OnClear()
        {
            PhaseGroup.Slots
                .Where(it => it.IsValid)
                .OfType<PhasedArtefactPresenter>()
                .ForEach(it => it.Record = null);
            base.OnClear();
        }

        protected override void OnClose()
        {
            base.OnClose();
            PhaseGroup.Slots
                .Where(it => it.IsValid)
                .ForEach(it => it.Close());
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            if (_isReveal)
            {
                _isReveal = false;
                PhaseGroup.Slots
                    .Where(it => it.IsValid)
                    .OfType<IRevealable>()
                    .ForEach(it => it.Reveal());
            }
            else
            {
                PhaseGroup.Slots.ForEach(it => it.Open());
            }
        }

        public async UniTaskVoid RevealAsync()
        {
            _isReveal = true;
            OpenAsync().Forget(); // this will call OnOpen() where we play the animation based on _isReveal
            await UniTask.CompletedTask;
        }
    }
}