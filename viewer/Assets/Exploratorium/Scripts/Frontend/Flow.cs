using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using Exploratorium.Utility;
using Exploratorium.Net.Shared;
using Sirenix.OdinInspector;
using Stateless;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    public class Flow : MonoBehaviour
    {
        [Serializable]
        public struct FlowState : IEquatable<FlowState>
        {
            private const int GoodPrimeForHashing = 486187739;
            private const int HashStartPrime = 12289;
            public State Source;
            public State Destination;
            public Trigger Trigger;
            public int Section;
            public int Artefact;

            public bool Equals(FlowState other)
            {
                return Source == other.Source && Destination == other.Destination && Trigger == other.Trigger &&
                       Section == other.Section && Artefact == other.Artefact;
            }

            public override bool Equals(object obj)
            {
                return obj is FlowState other && Equals(other);
            }

            public override int GetHashCode()
            {
                // https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-overriding-gethashcode/263416#263416
                int hashcode = HashStartPrime;
                hashcode = hashcode * GoodPrimeForHashing ^ Source.GetHashCode();
                hashcode = hashcode * GoodPrimeForHashing ^ Destination.GetHashCode();
                hashcode = hashcode * GoodPrimeForHashing ^ Trigger.GetHashCode();
                hashcode = hashcode * GoodPrimeForHashing ^ Section.GetHashCode();
                hashcode = hashcode * GoodPrimeForHashing ^ Artefact.GetHashCode();
                return hashcode;
            }
        }

        private readonly StateMachine<State, Trigger> _fsm = new StateMachine<State, Trigger>(State.Initial);
        private readonly HashSet<object> _visited = new HashSet<object>();
        private readonly List<object> _history = new List<object>();
        private StateMachine<State, Trigger>.TriggerWithParameters<FlowState> SyncTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<SectionsRecord> SectionSelectedTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<ArtefactsRecord> ArtefactSelectedTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<ArtefactsRecord> ArtefactActivatedTrigger;

        [BoxGroup(Constants.ReadVariables)] [SerializeField]
        private PawnRoleVariable roleVariable;


        [BoxGroup(Constants.ObservedEvents)] [SerializeField]
        private FlowStateEvent flowSyncEvent;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private VoidEvent resetAudio;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private FlowStateEvent flowReportEvent;

        [SerializeField] private Openable sectionOverallOpenable;
        [SerializeField] private Openable artefactsOverallOpenable;
        [SerializeField] private SectionPresenter sectionPresenter;
        [SerializeField] private CloudArtefactsPresenter cloudArtefactsPresenter;
        [SerializeField] private VideoPresenter introPresenter;
        [SerializeField] private HomePresenter homePresenter;


        //[SerializeField] private ShortcutsPresenter artefactShortcutsPresenter;
        [SerializeField] private ShortcutsPresenter sectionShortcutsPresenter;
        [SerializeField] private IdlePresenter idlePresenter;
        [SerializeField] private ArtefactViewerPresenter[] artefactPresenters;
        [SerializeField] private RelatedPresenter[] relatedPresenters;
        [SerializeField] private InactivityTimeout inactivityTimer;
        [SerializeField] private Button close;
        [SerializeField] private CanvasGroup closeGroup;
        [SerializeField] private CanvasGroup flowRoot;

        [BoxGroup("Default Color Theme")] [SerializeField]
        private ColorBlock defaultColors;

        [SerializeField] private Locale defaultLocale;

        private int _viewerIndex;
        private SectionsRecord _currentSection;
        private ArtefactsRecord _currentArtefact;

        public event Action<FlowState> Transitioned;

        private int PreviousViewerIndex => (_viewerIndex - 1) % artefactPresenters.Length;
        private int CurrentViewerIndex => _viewerIndex;

        private void RotateViewerPresenter()
        {
            artefactPresenters[CurrentViewerIndex].MarkAsBackground();
            _viewerIndex = (_viewerIndex + 1) % artefactPresenters.Length;
            artefactPresenters[CurrentViewerIndex].MarkAsForeground();
        }

        private void Awake()
        {
            Debug.Assert(_fsm != null, "_fsm != null");
            SyncTrigger = _fsm.SetTriggerParameters<FlowState>(Trigger.Sync);
            SectionSelectedTrigger = _fsm.SetTriggerParameters<SectionsRecord>(Trigger.SectionSelected);
            ArtefactSelectedTrigger = _fsm.SetTriggerParameters<ArtefactsRecord>(Trigger.ArtefactSelected);
            ArtefactActivatedTrigger = _fsm.SetTriggerParameters<ArtefactsRecord>(Trigger.ArtefactActivated);
            Debug.Assert(SyncTrigger != null, "SyncTrigger != null");
            Debug.Assert(SectionSelectedTrigger != null, "SectionSelectedTrigger != null");
            Debug.Assert(ArtefactSelectedTrigger != null, "ArtefactSelectedTrigger != null");
            Debug.Assert(ArtefactActivatedTrigger != null, "ArtefactActivatedTrigger != null");
            Debug.Assert(roleVariable != null, "roleVariable != null");
            Debug.Assert(flowReportEvent != null, "flowReportEvent != null");
            Debug.Assert(flowSyncEvent != null, "flowSyncEvent != null");
            Debug.Assert(defaultLocale != null, "defaultLocale != null");

            _fsm.OnTransitioned((transition) =>
            {
                Debug.Log(
                    $"{nameof(Flow)} : Transitioned on trigger '{transition.Trigger}' from state '{transition.Source}' to state '{transition.Destination}' with {transition.Parameters.Length} parameters");
            });

            _fsm.Configure(State.Initial)
                .Permit(Trigger.Continue, State.Idle)
                .Permit(Trigger.WakeUp, State.Active)
                .PermitDynamic(SyncTrigger, arg => arg.Destination)
                .Ignore(Trigger.ModelChanged) // do we use this?
                .Ignore(Trigger.UserTimeout)
                .OnExit(Init)

                // IDLE
                .Machine
                .Configure(State.Idle)
                .Permit(Trigger.UserDetected, State.Home)
                .Permit(Trigger.Continue, State.Home)
                .Permit(Trigger.ModelChanged, State.Initial) // do we use this?
                .PermitDynamic(SyncTrigger, arg => arg.Destination)
                .Ignore(Trigger.UserTimeout)
                .OnEntry(OpenIdle)
                .OnEntry(OpenHome)
                .OnEntry(DisableInactivityTimer)
                .OnEntry(CloseSectionOverall)
                .OnEntry(ReportTransition)
                .OnEntry(SetInteractive)
                .OnEntry(ResetAudio)
                .OnEntry(ResetLocale)
                .OnExit(CloseHomeIf)
                .OnExit(CloseIdle)

                // ACTIVE
                .Machine
                .Configure(State.Active)
                .Permit(Trigger.Reset, State.Idle)
                .Permit(Trigger.ModelChanged, State.Initial) // do we use this?
                .PermitIf(Trigger.UserTimeout, State.Idle, () => !IsTimeoutBlocked, "Timeout blocked")
                .IgnoreIf(Trigger.UserTimeout, () => IsTimeoutBlocked, "Timeout blocked")
                .PermitDynamic(SyncTrigger, arg => arg.Destination)
                //.PermitReentryIf(SyncTrigger)
                .OnEntry(EnableInactivityTimer)
                .OnEntry(ReportTransition)
                .OnExit(ClearVisited)

                // ACTIVE -> HOME
                .Machine
                .Configure(State.Home)
                .SubstateOf(State.Active)
                .OnEntry(OpenHome)
                .OnEntry(CloseSectionOverall)
                .OnEntry(ClearCurrentSection)
                .OnEntry(ReportTransition)
                .OnExit(CloseHomeIf)
                .PermitDynamic(SectionSelectedTrigger,
                    record => BypassVisited(record, State.SectionIntro, State.Section))

                // ACTIVE -> SECTION INTRO
                .Machine
                .Configure(State.SectionIntro)
                .SubstateOf(State.Active)
                .OnEntry(CloseSectionOverall)
                .OnEntryFrom(SectionSelectedTrigger, MarkAsVisited)
                .OnEntryFrom(SectionSelectedTrigger, MarkAsCurrentSection)
                .OnEntryFrom(SectionSelectedTrigger, OpenSectionIntro)
                .OnEntryFrom(SyncTrigger, OpenSectionIntro)
                .OnEntryFrom(SyncTrigger, MarkAsCurrentSection)
                .OnEntry(ReportTransition)
                .OnExit(CloseSectionIntro)
                .Permit(Trigger.Continue, State.Section)
                .Permit(Trigger.Close, State.Home)
                .PermitIf(Trigger.UserTimeout, State.Idle, () => !IsTimeoutBlocked)

                // ACTIVE -> SECTION
                .Machine
                .Configure(State.Section)
                .SubstateOf(State.Active)
                .OnEntry(ClearCurrentArtefact)
                .OnEntry(OpenSectionOverall)
                .OnEntry(CloseAllArtefactRelated)
                .OnEntry(() => ShowCloseButton(true))
                .OnEntryFrom(SectionSelectedTrigger, MarkAsVisited)
                .OnEntryFrom(SectionSelectedTrigger, OpenSection)
                .OnEntryFrom(SectionSelectedTrigger, RevealArtefacts)
                .OnEntryFrom(SectionSelectedTrigger, MarkAsCurrentSection)
                .OnEntryFrom(SectionSelectedTrigger, OpenSectionShortcuts)
                .OnEntryFrom(SectionSelectedTrigger, CycleOrOpenSectionOverall)
                .OnEntryFrom(SyncTrigger, OpenSection)
                .OnEntryFrom(SyncTrigger, RevealArtefacts)
                .OnEntryFrom(SyncTrigger, MarkAsCurrentSection)
                .OnEntryFrom(SyncTrigger, OpenSectionShortcuts)
                .OnEntryFrom(SyncTrigger, CycleOrOpenSectionOverall)
                .OnEntryFrom(Trigger.Continue, () => RevealArtefacts(_history.OfType<SectionsRecord>().Last()))
                .OnEntryFrom(Trigger.Continue, () => OpenSection(_history.OfType<SectionsRecord>().Last()))
                .OnEntryFrom(Trigger.Continue, () => OpenSectionShortcuts(_history.OfType<SectionsRecord>().Last()))
                .OnEntryFrom(Trigger.Close, () => OpenArtefacts(_history.OfType<SectionsRecord>().Last()))
                .OnEntryFrom(Trigger.Close, () => OpenSection(_history.OfType<SectionsRecord>().Last()))
                .OnEntryFrom(Trigger.Close, () => OpenSectionShortcuts(_history.OfType<SectionsRecord>().Last()))
                .OnEntry(ReportTransition)
                .OnExit(() => ShowCloseButton(false))
                .PermitReentryIf(SectionSelectedTrigger,
                    record => BypassVisited(record, State.SectionIntro, State.Section) ==
                              State.Section) // reenter if the intro was shown before
                .PermitIf(SectionSelectedTrigger, State.SectionIntro,
                    record => BypassVisited(record, State.SectionIntro, State.Section) ==
                              State.SectionIntro) // show the intro
                .PermitIf(Trigger.ArtefactActivated, State.ViewArtefact)
                .Permit(Trigger.Close, State.Home)
                .Ignore(Trigger.ArtefactSelected)

                // ACTIVE -> VIEW ARTEFACT
                .Machine
                .Configure(State.ViewArtefact)
                .SubstateOf(State.Active)
                .OnEntryFrom(ArtefactActivatedTrigger, MarkAsVisited)
                .OnEntryFrom(ArtefactActivatedTrigger, MarkAsCurrentArtefact)
                .OnEntryFrom(ArtefactActivatedTrigger, RotateViewerToArtefact)
                .OnEntryFrom(SyncTrigger, MarkAsCurrentArtefact)
                .OnEntryFrom(SyncTrigger, RotateViewerToArtefact)
                // .OnEntryFrom(ArtefactActivatedTrigger, OpenArtefactShortcuts) 
                .OnEntry(() => ShowCloseButton(true))
                .OnEntry(OpenSectionOverall)
                .OnEntry(CloseSection)
                .OnEntry(CloseArtefacts)
                .OnEntry(CloseSectionShortcuts)
                .OnEntry(ReportTransition)
                .OnExit(() => ShowCloseButton(false))
                .OnExit(CloseArtefactViewer)
                .OnExit(ClearCurrentArtefact)
                // .OnExit(CloseArtefactShortcuts)
                .PermitReentryIf(ArtefactActivatedTrigger)
                .PermitDynamic(SectionSelectedTrigger,
                    record => BypassVisited(record, State.SectionIntro, State.Section))
                .Permit(Trigger.Close, State.Section)
                .Ignore(Trigger.ArtefactSelected)
                .Ignore(Trigger.Continue) // fired by end of video signal
                ;

            homePresenter.SectionActivated += record => _fsm.Fire(SectionSelectedTrigger, record);
            cloudArtefactsPresenter.Selected += record => _fsm.Fire(ArtefactSelectedTrigger, record);
            cloudArtefactsPresenter.Activated += record => _fsm.Fire(ArtefactActivatedTrigger, record);
            sectionShortcutsPresenter.ShortcutActivated += record => _fsm.Fire(SectionSelectedTrigger, record);
            introPresenter.FinishedPlaying += () => _fsm.Fire(Trigger.Continue);
            introPresenter.Next += () => _fsm.Fire(Trigger.Continue);
            idlePresenter.UserDetected += () => _fsm.Fire(Trigger.UserDetected);
            inactivityTimer.Elapsed += () => _fsm.Fire(Trigger.UserTimeout);
            close.onClick.AddListener(() => _fsm.Fire(Trigger.Close));
            flowSyncEvent.Register(OnFlowSync);

            foreach (var relatedPresenter in relatedPresenters)
                relatedPresenter.Activated += record => _fsm.Fire(ArtefactActivatedTrigger, record);
        }

        private void ResetAudio()
        {
            if (resetAudio != null)
                resetAudio.Raise();
        }

        private void ResetLocale()
        {
            LocalizationSettings.SelectedLocale = defaultLocale != null
                ? defaultLocale
                : LocalizationSettings.AvailableLocales.Locales.First();
        }

        private void SetInteractive() =>
            flowRoot.interactable = roleVariable == null || roleVariable.Value != PawnRole.Observer;

        public bool IsTimeoutBlocked => roleVariable != null &&
                                        roleVariable.Value == PawnRole.Controller
        //(roleVariable.Value == PawnRole.Observer ||
        ;

        private void OnFlowSync(FlowState args)
        {
            // for some reason this gets called on disconnect
            if (!this) return;

            Debug.Log($"{nameof(Flow)} : SyncEvent received with destination {args.Destination}");
            _fsm.Fire(SyncTrigger, args);
        }

        private void ReportTransition(StateMachine<State, Trigger>.Transition transition)
        {
            FlowState args = new FlowState
            {
                Source = transition.Source,
                Destination = transition.Destination,
                Trigger = transition.Trigger,
                Section = GetCurrentSection()?.Id ?? default,
                Artefact = GetCurrentArtefact()?.Id ?? default
            };
            flowReportEvent.Raise(args);
            Transitioned?.Invoke(args);
        }

        private void CycleOrOpenSectionOverall(FlowState args, StateMachine<State, Trigger>.Transition t)
            => CycleOrOpenSectionOverall(GetRecordOfType<SectionsRecord>(args.Section.ToString()), t);

        private void CycleOrOpenSectionOverall(SectionsRecord record, StateMachine<State, Trigger>.Transition t)
        {
            // when entering with a new section selected, we always want the close animation to play
            if (t.Source == State.Home)
                OpenSectionOverall();
            else
                CycleSectionOverall();
        }

        private T GetRecordOfType<T>(string id) where T : DbRecord
        {
            return DirectusManager.Instance.Connector.Model.GetItemsOfType<T>().First(it => it.__Primary == id);
        }

        private void RevealArtefacts() => cloudArtefactsPresenter.OpenAsync().Forget();

        private void HideArtefacts() => cloudArtefactsPresenter.CloseAsync().Forget();

        private IEnumerator Start()
        {
            yield return null; // allow ui components one NOP frame in their Start() coroutines
            yield return null; // wait for all ui Start() coroutines to do their thing

            DirectusManager.Instance.ModelChanged += OnModelChanged;
            if (DirectusManager.Instance.IsReady)
                OnModelChanged(DirectusManager.Instance.Connector);

            void OnModelChanged(DirectusConnector connector)
            {
                _fsm.Fire(Trigger.ModelChanged);
                _fsm.Fire(Trigger.Continue);
            }
        }

        private void MarkAsCurrentSection(FlowState args) =>
            MarkAsCurrentSection(GetRecordOfType<SectionsRecord>(args.Section.ToString()));

        private void MarkAsCurrentSection(SectionsRecord record) => _currentSection = record;
        private void ClearCurrentSection() => _currentSection = default;
        private SectionsRecord GetCurrentSection() => _currentSection;

        private void MarkAsCurrentArtefact(FlowState args) =>
            MarkAsCurrentArtefact(GetRecordOfType<ArtefactsRecord>(args.Artefact.ToString()));

        private void MarkAsCurrentArtefact(ArtefactsRecord record) => _currentArtefact = record;
        private void ClearCurrentArtefact() => _currentArtefact = default;
        private ArtefactsRecord GetCurrentArtefact() => _currentArtefact;

        private void Init()
        {
            Debug.Log($"{nameof(Flow)} : Initializing");
            ClearVisited();
            ClearCurrentArtefact();
            ClearCurrentSection();
            CloseIdle();
            CloseHome();
            CloseArtefacts();
            CloseAllArtefactViewers();
            CloseAllArtefactRelated();
            CloseSection();
            CloseSectionIntro();
            CloseSectionShortcuts();
            // CloseArtefactShortcuts();
            ShowCloseButton(false);
            _viewerIndex = 0;

            ColorTheme.SetColor(new ColorBlock
                {
                    normalColor = defaultColors.normalColor,
                    selectedColor = defaultColors.selectedColor,
                    pressedColor = defaultColors.pressedColor,
                    disabledColor = defaultColors.disabledColor,
                    highlightedColor = defaultColors.highlightedColor,
                    colorMultiplier = defaultColors.colorMultiplier,
                    fadeDuration = defaultColors.fadeDuration
                }
            );

            Debug.Log($"{nameof(Flow)} : Initialized");
        }

        private void OpenSectionShortcuts(FlowState args) =>
            OpenSectionShortcuts(GetRecordOfType<SectionsRecord>(args.Section.ToString()));

        private void OpenSectionShortcuts(SectionsRecord record)
        {
            sectionShortcutsPresenter.Record = record;
            sectionShortcutsPresenter.Open();
            Canvas.ForceUpdateCanvases();
        }

        private void CloseSectionShortcuts()
            => sectionShortcutsPresenter.Close();

        /*
        private void OpenArtefactShortcuts(ArtefactsRecord record)
        {
            artefactShortcutsPresenter.Record = record.Topic;
            artefactShortcutsPresenter.Open();
            Canvas.ForceUpdateCanvases();
        }

        private void CloseArtefactShortcuts()
            => artefactShortcutsPresenter.Close();
        */

        private void RotateViewerToArtefact(FlowState args) =>
            RotateViewerToArtefact(GetRecordOfType<ArtefactsRecord>(args.Artefact.ToString()));

        private void RotateViewerToArtefact(ArtefactsRecord record)
        {
            // each time we open an artefact we rotate the viewer so we can have nice blended transitions, before
            // we do so, the current one will be closed
            CloseArtefactViewer();
            CloseArtefactRelated();
            RotateViewerPresenter();
            OpenArtefactViewer(record);
            OpenArtefactRelated(record);
        }

        private void CloseArtefactViewer()
        {
            artefactPresenters[CurrentViewerIndex].Close();
        }

        private void OpenArtefactViewer(FlowState args) =>
            OpenArtefactViewer(GetRecordOfType<ArtefactsRecord>(args.Artefact.ToString()));

        private void OpenArtefactViewer(ArtefactsRecord @record)
        {
            artefactPresenters[CurrentViewerIndex].Record = record;
            artefactPresenters[CurrentViewerIndex].Open();
        }

        private void CloseAllArtefactViewers()
        {
            foreach (var presenter in artefactPresenters)
                presenter.Close();
        }

        private void CloseAllArtefactRelated()
        {
            foreach (var presenter in relatedPresenters)
                presenter.CloseAsync().Forget();
        }

        private void CloseArtefactRelated()
        {
            relatedPresenters[CurrentViewerIndex].CloseAsync().Forget();
        }

        private void OpenArtefactRelated(FlowState args) =>
            OpenArtefactRelated(GetRecordOfType<ArtefactsRecord>(args.Artefact.ToString()));

        private void OpenArtefactRelated(ArtefactsRecord record)
        {
            ArtefactsRecord[] allArtefactsInSameSection = record.Section.Artefacts;

            relatedPresenters[CurrentViewerIndex].Show(allArtefactsInSameSection);
            relatedPresenters[CurrentViewerIndex].OpenAsync().Forget();
        }

        private void CloseSection()
            => sectionPresenter.Close();

        private void OpenSectionOverall()
        {
            sectionOverallOpenable.OpenAsync().Forget();
            artefactsOverallOpenable.OpenAsync().Forget();
        }

        private void CloseSectionOverall()
        {
            sectionOverallOpenable.CloseAsync().Forget();
            artefactsOverallOpenable.CloseAsync().Forget();
        }

        private void CycleSectionOverall()
        {
            sectionOverallOpenable.CycleAsync().Forget();
            artefactsOverallOpenable.CycleAsync().Forget();
        }

        private void OpenSection(FlowState args) =>
            OpenSection(GetRecordOfType<SectionsRecord>(args.Section.ToString()));

        private void OpenSection(SectionsRecord record)
        {
            sectionPresenter.Record = record;
            sectionPresenter.Open();
            if (sectionPresenter.Record != null)
            {
                if (ColorUtility.TryParseHtmlString(sectionPresenter.Record.Color, out var color))
                {
                    ColorTheme.SetColor(new ColorBlock
                        {
                            normalColor = defaultColors.normalColor,
                            selectedColor = defaultColors.selectedColor,
                            pressedColor = color,
                            disabledColor = defaultColors.disabledColor,
                            highlightedColor = defaultColors.highlightedColor,
                            colorMultiplier = defaultColors.colorMultiplier,
                            fadeDuration = defaultColors.fadeDuration
                        }
                    );
                }
            }
        }

        private void CloseArtefacts()
            => cloudArtefactsPresenter.CloseAsync().Forget();

        private void RevealArtefacts(FlowState args) =>
            RevealArtefacts(GetRecordOfType<SectionsRecord>(args.Section.ToString()));

        private void RevealArtefacts(SectionsRecord record)
        {
            //
            DirectusModel model = DirectusManager.Instance.Connector.Model;

            ImmutableHashSet<SectionsRecord> sections = record
                .Descendants(it => it.Children)
                .Where(it => it.Status == SectionsRecord.StatusChoices.Published)
                .ToImmutableHashSet();

            IEnumerable<ArtefactsRecord> artefacts = model
                .GetItemsOfType<ArtefactsRecord>()
                .Where(it => it.Status == ArtefactsRecord.StatusChoices.Published)
                .Where(it => sections.Contains(it.Topic));

            cloudArtefactsPresenter.Show(artefacts.ToArray());
            cloudArtefactsPresenter.RevealAsync().Forget();
        }

        private void OpenArtefacts(FlowState args) =>
            OpenArtefacts(GetRecordOfType<SectionsRecord>(args.Section.ToString()));

        private void OpenArtefacts(SectionsRecord record)
        {
            DirectusModel model = DirectusManager.Instance.Connector.Model;
            ImmutableHashSet<SectionsRecord> sections = record
                .Descendants(it => it.Children)
                .Where(it => it.Status == SectionsRecord.StatusChoices.Published)
                .ToImmutableHashSet();
            IEnumerable<ArtefactsRecord> artefacts = model
                .GetItemsOfType<ArtefactsRecord>()
                .Where(it => it.Status == ArtefactsRecord.StatusChoices.Published)
                .Where(it => sections.Contains(it.Topic));
            cloudArtefactsPresenter.Show(artefacts.ToArray());
            cloudArtefactsPresenter.OpenAsync().Forget();
        }

        private void CloseSectionIntro()
            => introPresenter.CloseAsync().Forget();

        private void OpenSectionIntro(FlowState args) =>
            OpenSectionIntro(GetRecordOfType<SectionsRecord>(args.Section.ToString()));

        private void OpenSectionIntro(SectionsRecord record)
        {
            bool hasTranslation = DirectusExtensions.TryGetTranslation(
                LocalizationSettings.SelectedLocale,
                record.Translations,
                out var recordTranslation);
            if (hasTranslation && recordTranslation.Intro != null)
            {
                introPresenter.Show(recordTranslation.Intro);
                introPresenter.OpenAsync().Forget();
            }
            else
            {
                Debug.LogError($"{nameof(Flow)} : Section {record.Name} is missing an intro video reference.");
                introPresenter.Show(null);
                introPresenter.OpenAsync().Forget();
            }
        }


        private void CloseHome() => homePresenter.Close();

        private void CloseHomeIf(StateMachine<State, Trigger>.Transition t)
        {
            switch (t.Destination)
            {
                case State.Home:
                case State.Idle:
                    // keep home open while idle
                    break;
                case State.Initial:
                case State.Active:
                case State.SectionIntro:
                case State.ViewArtefact:
                case State.Section:
                    CloseHome();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OpenHome()
            => homePresenter.Open();

        private void CloseIdle()
            => idlePresenter.Close();

        private void OpenIdle()
            => idlePresenter.Open();

        private State BypassVisited(SectionsRecord section, State visit, State bypass)
        {
            return _visited.Contains(section) ? bypass : visit;
        }

        private void MarkAsVisited(DbRecord obj)
        {
            _visited.Add(obj);
            _history.Add(obj);
        }

        private void ClearVisited()
        {
            _visited.Clear();
            _history.Clear();
        }

        private void EnableInactivityTimer() => inactivityTimer.enabled = true;

        private void DisableInactivityTimer() => inactivityTimer.enabled = false;

        private object GetLastVisited()
        {
            return _history.LastOrDefault();
        }

        [Button]
        public void InactivityTimeout() => _fsm.Fire(Trigger.UserTimeout);

        private void ShowCloseButton(bool isVisible)
        {
            if (closeGroup != null)
            {
                Debug.Log($"{nameof(Flow)} : Close is visible {isVisible}");
                closeGroup.alpha = isVisible ? 1.0f : 0;
                closeGroup.interactable = isVisible;
                closeGroup.blocksRaycasts = isVisible;
            }
        }

        [Serializable]
        public enum Trigger
        {
            Continue,
            WakeUp,
            UserDetected,
            UserTimeout,
            SectionSelected,
            ArtefactSelected,
            Close,
            ArtefactActivated,
            ModelChanged,
            Sync,
            Reset
        }

        [Serializable]
        public enum State
        {
            Initial,
            Idle,
            Active,
            SectionIntro,
            ViewArtefact,
            Section,
            Home
        }
    }
}