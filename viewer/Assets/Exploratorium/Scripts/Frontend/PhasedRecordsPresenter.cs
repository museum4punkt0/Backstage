using System;
using System.Linq;
using Directus.Connect.v9;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exploratorium.Frontend
{
    public abstract class PhasedRecordsPresenter<TRecord> : RecordsPresenter<TRecord> where TRecord : DbRecord
    {
        [BoxGroup("Phasing")]
        [SerializeField] private PhaseGroup phaseGroup;

        public event Action<TRecord> Selected;
        public event Action<TRecord> Activated;

        public PhaseGroup PhaseGroup => phaseGroup;

        protected void OnSelected(RecordPresenter<TRecord> presenter) => Selected?.Invoke(presenter.Record);
        protected void OnActivated(RecordPresenter<TRecord> presenter) => Activated?.Invoke(presenter.Record);

        protected override void OnClose()
        {
            phaseGroup.RecenterViews();
        }

        protected override void OnOpen()
        {
            phaseGroup.RecenterViews();
        }

        protected override void OnClear()
        {
            foreach (var presenter in phaseGroup.Slots.OfType<PhasedRecordPresenter<TRecord>>())
            {
                // anytime we clear we assume that the current set of spawned prefabs is no longer supposed to b
                // interactable
                presenter.Activated -= OnActivated;
                presenter.Selected -= OnSelected;
            }
        }
    }
}