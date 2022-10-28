using System;
using Directus.Connect.v9;
using Directus.Generated;
using Sirenix.OdinInspector;
using UnityAtoms;
using UnityEngine;

namespace Exploratorium.Frontend
{
    public abstract class PhasedRecordPresenter<TRecord> : RecordPresenter<TRecord>, IPhased where TRecord : DbRecord
    {
        public event Action<RecordPresenter<TRecord>> Activated;
        private const string Phasing = "Phasing";

        [BoxGroup(Phasing)] [SerializeField] private PhaseGroup phaseGroup;
        private IPhaseGroup _phaseGroup;
        private int _phase;
        
        [NonSerialized]
        private bool _isInit;

        public IPhaseGroup PhaseGroup
        {
            get
            {
                if (_phaseGroup == null)
                    SetPhaseGroup(phaseGroup);
                return _phaseGroup;
            }
        }

        public bool IsValid => Record != null;


        public void SetPhaseGroup(IPhaseGroup group)
        {
            if (_phaseGroup != null)
                _phaseGroup.UnRegister(this);
            _phaseGroup = group;
            if (_phaseGroup != null)
                _phaseGroup.Register(this);
            //PhaseGroup.Deselect(this); // Stack Overflow
        }

        private void EnsureInit()
        {
            if (_isInit)
                return;
            _isInit = true;
            _phase = 0;

            if (_phaseGroup == null)
                _phaseGroup = phaseGroup;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (PhaseGroup != null)
            {
                ResetInternalState();
                PhaseGroup.Register(this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (PhaseGroup != null)
                PhaseGroup.UnRegister(this);
        }

        private void ResetInternalState()
        {
            _phase = 0;
        }


        protected override void OnDeselect()
        {
            EnsureInit();

            if (debug)
                Debug.Log($"{nameof(PhasedArtefactPresenter)} : Deselected {typeof(TRecord)} {Record.__Primary}",
                    this);

            DeSelectWithoutNotify();
        }


        protected override void OnSelect()
        {
            EnsureInit();

            if (debug)
                Debug.Log($"{nameof(PhasedArtefactPresenter)} :Selected {typeof(TRecord)} {Record.__Primary}", this);

            if (_phase == 1)
            {
                Debug.Log("ACTIVATED");
                Activated?.Invoke(this);
                DeSelectWithoutNotify();
            }
            else
            {
                _phase = 1;
                if (PhaseGroup != null)
                    PhaseGroup.Select(this); // this will call SelectWithoutNotify
            }
        }

        protected override void OnSelectedChanged(DbRecordWrapper record)
        {
            EnsureInit();

            var selectedArtefact = record.Record as ArtefactsRecord;
            if (selectedArtefact == null || Record == null || Record != selectedArtefact)
            {
                if (debug) Debug.Log($"{nameof(PhasedArtefactPresenter)} : Reset selected count on {name}", this);
                _phase = 0;
                DeSelectWithoutNotify();
            }
        }

        public void DeSelectWithoutNotify()
        {
            if (!enabled)
                return;

            _phase = 0;
            
            OnDeSelectWithoutNotify();
        }
        
        protected abstract void OnDeSelectWithoutNotify();

        public void SelectWithoutNotify()
        {
            if (!enabled)
                return;

            _phase = 1;

            OnSelectWithoutNotify();
        }

        protected abstract void OnSelectWithoutNotify();
    }
}