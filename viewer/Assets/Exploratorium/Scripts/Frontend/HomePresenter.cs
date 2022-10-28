using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Exploratorium.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Exploratorium.Frontend
{
    public class HomePresenter : MonoBehaviour
    {
        /*    [Tooltip("The prefab for section items")]
            [SerializeField]
            private PhasedSectionPresenter sectionPrefab;*/


        [BoxGroup("Phasing")] [SerializeField] private PhaseGroup phaseGroup;
        
        [Tooltip("The delay in seconds added to each sections base delay (useful to create a staggered animation)")]
        [SerializeField]
        private float delayIncrement = 0.2f;

        [Tooltip("Print debug messages")] [SerializeField]
        private bool debug;

        [Tooltip("The openable objects that will be triggered on open/close signals to this component")]
        [SerializeField]
        private OpenableGroup[] openables;


        private DirectusModel _model;
        public event Action<SectionsRecord> SectionActivated;
        public event Action<SectionsRecord> SectionSelected;
        private readonly SemaphoreSlim _builderLock = new SemaphoreSlim(1, 1);
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly List<PhasedSectionPresenter> _sections = new List<PhasedSectionPresenter>();

        private void Awake()
        {
            Debug.Assert(phaseGroup != null, "phaseGroup != null");
        }

        private void Start()
        {
            DirectusManager.Instance.ModelChanged += Init;
            if (DirectusManager.Instance.IsReady)
                Init(DirectusManager.Instance.Connector);
        }

        private void Init(DirectusConnector directusConnector)
        {
            _model = directusConnector.Model;
            CancelAsyncOperations();
            UniTask.Void(BuildAsync, _cts.Token);
        }

        private void OnEnable()
        {
            UniTask.Void(EnableAsync, _cts.Token);
        }

        private async UniTaskVoid EnableAsync(CancellationToken ct)
        {
            await _builderLock.WaitAsync(ct);
            try
            {
                LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
                OnLocaleChanged(LocalizationSettings.SelectedLocale);
            }
            finally
            {
                _builderLock.Release();
            }
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            CancelAsyncOperations();
        }

        private async UniTaskVoid BuildAsync(CancellationToken ct)
        {
            await _builderLock.WaitAsync(ct);
            try
            {
                Clear();

                SettingsRecord settings = _model.GetItemsOfType<SettingsRecord>().First();
                SectionsRecord[] sections = settings.TopicRoot
                    .Closure(it => it.Children)
                    .Skip(1) // skip the root element
                    .Where(it => it.Status == SectionsRecord.StatusChoices.Published)
                    .OrderBy(it => it.Sort)
                    .ToArray();
                PhasedSectionPresenter[] slots = phaseGroup.Slots
                    .OfType<PhasedSectionPresenter>()
                    .OrderBy(it => it.transform.localPosition.y)
                    .ToArray();
                
                await UniTask.NextFrame(ct);
                float addedOpenDelay = 0;
                float addedCloseDelay = sections.Length * delayIncrement;
                for (var i = 0; i < slots.Length; i++)
                {
                    PhasedSectionPresenter slot = slots[i];
                    if (sections.Length <= i)
                    {
                        slot.name = $"SLOT -- empty";
                        slot.Record = null;
                        slot.SetVisible(false); // not necessary but more explicit this way
                    }
                    else
                    {
                        slot.gameObject.SetActive(true);
                        slot.SetVisible(true);
                        SectionsRecord section = sections[i];
                        slot.name = $"SLOT -- {section.Id}:{section.Name}";
                        slot.Selected += OnSelected;
                        slot.Activated += OnSectionActivated;
                        slot.SetAdditionalOpenDelay(addedOpenDelay);
                        slot.SetAdditionalCloseDelay(addedCloseDelay);
                        addedOpenDelay += delayIncrement;
                        addedCloseDelay -= delayIncrement;
                        _sections.Add(slot);
                        slot.Record = section;
                        slot.Open();
                        
                    }
                }

                slots.ForEach(it => it.RebuildOpenables());
            }
            finally
            {
                _builderLock.Release();
            }
        }

        private void OnSectionActivated(RecordPresenter<SectionsRecord> source)
        {
            if (debug)
                Debug.Log($"{nameof(HomePresenter)} : Section {source.Record.Id} {source.Record.Name} activated");
            SectionActivated?.Invoke(source.Record);
        }

        private void OnLocaleChanged(Locale locale)
        {
            if (debug) 
                Debug.Log($"{nameof(HomePresenter)} : Locale changed to {locale.LocaleName}");
        }

        private void CancelAsyncOperations()
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }

        public void Close()
        {
            if (!this)
                return;

            phaseGroup.Slots
                .OfType<PhasedSectionPresenter>()
                .Where(it => it.Record != null)
                .ForEach(it => it.Close());
            openables.ForEach(it => it.CloseAsync().Forget());
        }

        public void Open()
        {
            openables.ForEach(it => it.OpenAsync().Forget());
            phaseGroup.Slots
                .OfType<PhasedSectionPresenter>()
                .Where(it => it.Record != null)
                .ForEach(it => it.Open());
        }

        private void Clear()
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
        
        private void OnSelected(RecordPresenter<SectionsRecord> source)
        {
            if (debug) Debug.Log($"{nameof(HomePresenter)} : Section {source.Record.Id} {source.Record.Name} selected", this);
            SectionSelected?.Invoke(source.Record);
        }

        private void OnActivated(RecordPresenter<SectionsRecord> source)
        {
            if (debug)
                Debug.Log($"{nameof(HomePresenter)} : Section {source.Record.Id} {source.Record.Name} activated", this);
            SectionActivated?.Invoke(source.Record);
        }
    }
}