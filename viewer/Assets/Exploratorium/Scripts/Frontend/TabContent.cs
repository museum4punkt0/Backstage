using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(LayoutElement))]
    public class TabContent : MonoBehaviour
    {
        [CanBeNull] [ReadOnly] [ShowInInspector] [Tooltip("Canvas group to set interactable when the tab is active")]
        private CanvasGroup _canvasGroup;

        [CanBeNull]
        [ReadOnly]
        [ShowInInspector]
        [Tooltip("Layout element to make this tab invisible to the layout group when inactive")]
        private LayoutElement _layoutElement;

        [SerializeField] [Tooltip("The tab group this tab content belongs to")]
        private TabGroup tabGroup;

        [CanBeNull] [SerializeField] private string displayName;

        [Tooltip("The opacity of this tab content when in disabled state, i.e. when another tab is active")]
        [SerializeField]
        private float disabledAlpha = 0;

        [InfoBox(
            "Activated objects must be children of this gameObject or in a separate branch in the scene hierarchy",
            InfoMessageType.Warning,
            nameof(IsActiveInvalid)
        )]
        [CanBeNull]
        [SerializeField]
        [Tooltip("GameObject to set active when the tab is active")]
        private GameObject[] activate;

        [CanBeNull] [SerializeField] private RectTransform contentRect;

        [SerializeField] private UnityBoolEvent onChanged;

        [ReadOnly] [ShowInInspector] private bool _isOn;

        [ReadOnly] [CanBeNull] [ShowInInspector]
        private TabGroup _tabGroup;

        [CanBeNull] private string _displayName;

        [CanBeNull] public RectTransform ContentRect => contentRect;

        [CanBeNull] public TabGroup TabGroup => _tabGroup;
        public bool IsActive => _isOn;
        private bool IsActiveInvalid => activate.Any(it => it == gameObject && transform.IsChildOf(it.transform));

        public event Action<bool> Changed;

        public void SetGroup(TabGroup value)
        {
            if (_tabGroup != null)
                _tabGroup.UnRegisterTab(this);
            _tabGroup = value;
            if (_tabGroup != null)
                _tabGroup.RegisterTab(this);
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_displayName)) // might still be unset when this is first called
                    _displayName = displayName;
                return _displayName;
            }
            set
            {
                _displayName = value;
                if (tabGroup != null)
                    tabGroup.RefreshTab(this);
            }
        }

        private void Reset() => _canvasGroup = GetComponent<CanvasGroup>();

        private void Awake()
        {
            Debug.Assert(contentRect != null, "contentRect != null", this);
            _canvasGroup = GetComponent<CanvasGroup>();
            _layoutElement = GetComponent<LayoutElement>();
            _tabGroup ??= tabGroup;
            _displayName ??= displayName;
        }

        public void OnEnable()
        {
            if (_tabGroup != null) 
                _tabGroup.RegisterTab(this);
        }

        public void OnDisable()
        {
            if (_tabGroup != null) 
                _tabGroup.UnRegisterTab(this);
        }

        internal void SetActiveWithoutNotify(bool isOn)
        {
            _isOn = isOn;
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = isOn;
                _canvasGroup.alpha = isOn ? 1.0f : disabledAlpha;
                _canvasGroup.blocksRaycasts = isOn;
            }

            if (_layoutElement != null)
                _layoutElement.ignoreLayout = !isOn;

            if (activate != null)
            {
                foreach (var go in activate)
                {
                    if (go == null)
                        continue;
                    go.SetActive(isOn);
                }
            }

            Changed?.Invoke(isOn);
            onChanged.Invoke(isOn);
        }

        public void SetActive(bool isOn)
        {
            if (_tabGroup != null)
                _tabGroup.RequestTabActivation(this);
            else
                SetActiveWithoutNotify(isOn);
        }
    }
}