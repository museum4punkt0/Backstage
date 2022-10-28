using System;
using System.Collections.Generic;
using Directus.Generated;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    public class RelatedArtefactsPresenter : RecordsPresenter<ArtefactsRecord>
    {
        //private ObjectPool<GameObject> _pool;

        [SerializeField] private PhasedArtefactPresenter artefactPrefab;
        [SerializeField] private PhaseGroup phaseGroup;
        [SerializeField] private float openDelayIncrement = 0f;
        [SerializeField] private float closeDelayIncrement = 0f;

        [InfoBox("When adding more than one spawn parent, consecutive calls to Show() will rotate through " +
                 "them to enable cross-blending effects")]
        [SerializeField]
        private Transform[] spawnParents;

        [SerializeField] private LayoutGroup[] rebuildLayoutGroups;

        private readonly Dictionary<int, List<PhasedArtefactPresenter>> _artefacts =
            new Dictionary<int, List<PhasedArtefactPresenter>>();

        private int _rotation;

        public event Action<ArtefactsRecord> Selected;
        public event Action<ArtefactsRecord> Activated;

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < spawnParents.Length; i++)
                _artefacts[i] = new List<PhasedArtefactPresenter>();
        }

        protected override void OnLocaleChanged(Locale locale)
        {
        }

        protected override void OnShow(params ArtefactsRecord[] records)
        {
            _rotation = (_rotation + 1) % spawnParents.Length;
            OnClear();
            if (debug) Debug.Log($"{nameof(RelatedArtefactsPresenter)} : Showing {records.Length} artefacts");
            float openDelay = 0;
            float closeDelay = closeDelayIncrement * records.Length;

            foreach (var record in records)
            {
                PhasedArtefactPresenter presenter = Instantiate(
                    artefactPrefab,
                    spawnParents[_rotation],
                    false
                );
                presenter.SetPhaseGroup(phaseGroup);
                presenter.transform.SetAsLastSibling();
                presenter.transform.localScale = Vector3.one;
                presenter.name = $"{artefactPrefab.name} -- {record.Id}:{record.Name}";
                presenter.Record = record;
                presenter.SetAdditionalOpenDelay(openDelay);
                presenter.SetAdditionalCloseDelay(closeDelay);
                presenter.Activated += OnActivated;
                presenter.Selected += OnSelected;
                _artefacts[_rotation].Add(presenter);
                openDelay += openDelayIncrement;
                closeDelay -= closeDelayIncrement;
                LayoutRebuilder.MarkLayoutForRebuild(presenter.GetComponent<RectTransform>());
            }

            RebuildLayoutGroupsImmediately();
            _artefacts[_rotation].ForEach(it => it.RebuildOpenables(true));
        }

        private void TrackPresenter(PhasedArtefactPresenter presenter)
        {
            if (!_artefacts.ContainsKey(_rotation))
                _artefacts[_rotation] = new List<PhasedArtefactPresenter>();
            _artefacts[_rotation].Add(presenter);
        }

        [Button]
        private void RebuildLayoutGroups() =>
            rebuildLayoutGroups.ForEach(it => LayoutRebuilder.MarkLayoutForRebuild(it.GetComponent<RectTransform>()));

        [Button]
        private void RebuildLayoutGroupsImmediately() =>
            rebuildLayoutGroups.ForEach(it =>
                LayoutRebuilder.ForceRebuildLayoutImmediate(it.GetComponent<RectTransform>()));

        private void OnSelected(RecordPresenter<ArtefactsRecord> presenter) => Selected?.Invoke(presenter.Record);
        private void OnActivated(RecordPresenter<ArtefactsRecord> presenter) => Activated?.Invoke(presenter.Record);

        protected override void OnClear()
        {
            foreach (var pair in _artefacts)
            {
                foreach (var presenter in pair.Value)
                {
                    // anytime we clear we assume that the current set of spawned prefabs is no longer supposed to b
                    // interactable
                    presenter.Activated -= OnActivated;
                    presenter.Selected -= OnSelected;
                }
            }

            // clear the current swap target of all spawned prefabs

            foreach (PhasedArtefactPresenter presenter in _artefacts[_rotation])
            {
                if (presenter.transform.parent == spawnParents[_rotation])
                {
                    presenter.name = $"{artefactPrefab.name} -- POOLED (inactive)";
                    Destroy(presenter.gameObject);
                }
            }

            _artefacts[_rotation].Clear();
        }

        protected override void OnClose()
        {
            _artefacts[_rotation].ForEach(it => it.Close());
        }

        protected override void OnOpen()
        {
            if (spawnParents.Length > 1)
            {
                var previous = (_rotation - 1) % spawnParents.Length;
                _artefacts[previous].ForEach(it => it.Close());
            }

            _artefacts[_rotation].ForEach(it => it.Open());
        }
    }
}