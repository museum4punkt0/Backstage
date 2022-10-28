using System;
using DG.Tweening;
using Directus.Generated;
using Sirenix.OdinInspector;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class PhasedSectionPresenter : SectionPresenter, IPhased
    {
        private const string Phasing = "Phasing";
        private const string SelectAnimation = "Select Animation";

        [BoxGroup(Phasing)] [SerializeField] private PhaseGroup phaseGroup;

        [BoxGroup(SelectAnimation)] [Min(0), SerializeField]
        private float scaleTo = 1.2f;

        [BoxGroup(SelectAnimation)] [Min(0), SerializeField]
        private Vector2 boxSelectedSize = Vector2.one * 40f;

        [BoxGroup(SelectAnimation)] [Min(0), SerializeField]
        private Vector2 boxDeselectedSize = Vector2.one * 0f;

        [BoxGroup(SelectAnimation)] [SerializeField]
        private CanvasGroup plusGroup;

        [BoxGroup(SelectAnimation)] [Min(0), SerializeField]
        private float duration = 1f;

        private IPhaseGroup _phaseGroup;

        private Vector2 _originalSelfSizeDelta;
        private RectTransform _selfRect;
        private RectTransform _labelRect;
        private RectTransform _plusRect;
        private Vector2 _originalPlusDelta;
        private Vector2 _originalLabelDelta;
        private Vector3 _originalSelfScale;
        private bool _isInit;
        private int _phase;

        public event Action<RecordPresenter<SectionsRecord>> Activated;
        
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
        }

        private const string InternalID = nameof(PhasedArtefactPresenter) + "INTERNAL";

        private void Awake() => EnsureInit();

        protected override void OnEnable()
        {
            base.OnEnable();

            if (PhaseGroup != null)
            {
                ResetInternalState();
                PhaseGroup.Register(this);
            }

            plusGroup.alpha = IsSelected ? 1f : 0f;
        }

        protected override void OnDisable()
        {
            base.OnEnable();

            if (PhaseGroup != null)
                PhaseGroup.UnRegister(this);
            DOTween.Kill(this, InternalID);
        }

        private void EnsureInit()
        {
            if (_isInit)
                return;
            _isInit = true;

            if (_phaseGroup == null)
                _phaseGroup = phaseGroup;

            _selfRect = GetComponent<RectTransform>();
            Debug.Assert(_selfRect != null, "_selfRect != null", this);

            _labelRect = Label.GetComponent<RectTransform>();
            Debug.Assert(_labelRect != null, "_labelRect != null", this);

            _plusRect = plusGroup.GetComponent<RectTransform>();
            Debug.Assert(_plusRect != null, "_plusRect != null", this);

            if (selectButton == null)
                selectButton = GetComponent<Button>();
            Debug.Assert(selectButton != null, "selectButton != null", this);

            // store original rects
            _originalSelfSizeDelta = _selfRect.sizeDelta;
            _originalPlusDelta = _plusRect.sizeDelta;
            _originalLabelDelta = _labelRect.sizeDelta;
            _originalSelfScale = _selfRect.localScale;
        }


        protected override void OnDeselect()
        {
            if (!this)
                return;
            base.OnSelect();
            EnsureInit();

            if (debug)
                Debug.Log($"{nameof(PhasedArtefactPresenter)} : Deselected {typeof(PhasedSectionPresenter)} {Record?.Name}",
                    this);

            DeSelectWithoutNotify();
        }


        protected override void OnSelect()
        {
            if (!this)
                return;
            base.OnDeselect();
            EnsureInit();

            if (debug)
                Debug.Log($"{nameof(PhasedArtefactPresenter)} :Selected {typeof(PhasedSectionPresenter)} {Record?.Name}", this);

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
                    PhaseGroup.Select(this);
            }
        }

        protected override void OnSelectedChanged(DbRecordWrapper record)
        {
            if (!this)
                return;
            EnsureInit();
            base.OnSelectedChanged(record);
        }

        protected override void OnRecordChanged()
        {
            if (!this)
                return;
            EnsureInit();
            ResetInternalState();
            base.OnRecordChanged();
        }


        protected override void OnLocaleChanged(Locale locale)
        {
            if (!this)
                return;
            EnsureInit();
            base.OnLocaleChanged(locale);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
        }

        public void SelectWithoutNotify()
        {
            EnsureInit();

            if (!enabled)
                return;

            _phase = 1;
            DOTween.Kill(this, InternalID);
            DOTween.Sequence()
                .Join(_selfRect.DOScale(Vector3.one * scaleTo, duration))
                .Join(_plusRect.DOSizeDelta(boxSelectedSize, duration))
                .Join(plusGroup.DOFade(1f, duration))
                .SetTarget(this)
                .SetId(InternalID)
                .SetAutoKill(true)
                .Restart();
        }

        public void DeSelectWithoutNotify()
        {
            EnsureInit();

            if (!enabled)
            {
                ResetInternalState();
                return;
            }

            _phase = 0;
            DOTween.Kill(this, InternalID);
            DOTween.Sequence()
                .Join(_selfRect.DOScale(_originalSelfScale, duration))
                .Join(_plusRect.DOSizeDelta(boxDeselectedSize, duration))
                .Join(plusGroup.DOFade(0f, duration))
                .SetTarget(this)
                .SetId(InternalID)
                .SetAutoKill(true)
                .Restart();
        }

        private void ResetInternalState()
        {
            EnsureInit();
            _phase = 0;
            _selfRect.sizeDelta = _originalSelfSizeDelta;
            _plusRect.sizeDelta = _originalPlusDelta;
            _labelRect.sizeDelta = _originalLabelDelta;
            plusGroup.alpha = 0;
        }
    }
}