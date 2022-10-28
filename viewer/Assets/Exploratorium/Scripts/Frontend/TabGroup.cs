using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Exploratorium.Frontend
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private TabContent defaultTab;
        [SerializeField] private TabToggle togglePrefab;
        [SerializeField] private RectTransform toggleContainer;

        // TODO: we should probably not set colors here and let theming be handled in a completely separate system
        [Obsolete] [HideInInspector] [SerializeField]
        private Color onColor;
        [Obsolete] [HideInInspector] [SerializeField]
        private Color offColor;

        [Obsolete]
        public Color OffColor
        {
            get => offColor;
            set => offColor = value;
        }

        [Obsolete]
        public Color OnColor
        {
            get => onColor;
            set => onColor = value;
        }

        private readonly List<TabContent> _tabs = new List<TabContent>();
        private readonly Dictionary<TabContent, TabToggle> _toggles = new Dictionary<TabContent, TabToggle>();
        private TabContent _defaultTab;
        public int Count => _tabs.Count;

        private void Awake()
        {
            if (_defaultTab == null)
                _defaultTab = defaultTab;
            if (_defaultTab != null)
                RegisterTab(_defaultTab);
        }

        public TabContent DefaultTab
        {
            get => _defaultTab;
            set
            {
                _defaultTab = value;
                if (!_tabs.Contains(_defaultTab))
                    _tabs.Add(_defaultTab);
            }
        }
        /*

        public void SetOffColor(Color value)
        {
            offColor = value;
            _toggles.ForEach(it => it.Value.Extension.OffColor = value);
        }

*/
        /*
        public void SetOnColor(Color value)
        {
            onColor = value;
            _toggles.ForEach(it => it.Value.Extension.OnColor = value);
        }
        */

        internal void RegisterTab([NotNull] TabContent tab)
        {
            Debug.Assert(tab != null, $"{nameof(tab)} != null");
            
            if (_tabs.Contains(tab))
            {
                Debug.LogWarning($"{nameof(TabGroup)} : {tab.name} is already registered");
                return;
            }

            TabToggle toggle = Instantiate(togglePrefab, toggleContainer);
            
            /*
            // TODO: we should probably not set colors here and let theming be handled in a completely separate system
            toggle.Extension.OnColor = onColor;
            toggle.Extension.OffColor = offColor;
            */
            
            toggle.Toggle.onValueChanged.AddListener(isOn => OnStateChangeRequested(tab, isOn));
            if (toggle.Extension.Label != null)
                toggle.Extension.Label.text = tab.DisplayName;

            _tabs.Add(tab);
            _toggles.Add(tab, toggle);

            if (_defaultTab == null)
            {
                _defaultTab = tab;
                OnStateChangeRequested(tab, true);
            }
            else
            {
                OnStateChangeRequested(tab, false);
            }
        }

        internal void UnRegisterTab([NotNull] TabContent tab)
        {
            Debug.Assert(tab != null, $"{nameof(tab)} != null");

            if (_tabs.Count == 1 && _tabs.Contains(tab))
            {
                Debug.LogWarning($"{nameof(TabGroup)} : Can't remove the last tab of a group");
                return;
            }

            if (!_tabs.Remove(tab))
            {
                Debug.LogWarning($"{tab.name} is not registered as a tab in {name}");
                return;
            }

            if (tab.IsActive)
                ActivateTab(_defaultTab);
            TabToggle toggle = _toggles[tab];
            _toggles.Remove(tab);
            Destroy(toggle.gameObject);

        }

        private void OnStateChangeRequested([NotNull] TabContent tab, bool active)
        {
            // we ignore the active parameter and just activate based on whichever tab was touched last
            
            ActivateTab(tab);
        }

        public void RequestTabActivation([NotNull] TabContent tab) => ActivateTab(tab);

        private void ActivateTab([NotNull] TabContent tab)
        {
            Debug.Assert(tab != null, $"{nameof(tab)} != null");
            foreach (var other in _tabs)
            {
                other.SetActiveWithoutNotify(other == tab);
                _toggles[other].SetIsOnWithoutNotify(other == tab);
            }
        }

        private void ActivateTab(int ndx)
        {
            Debug.Assert(ndx > 0 && ndx < _tabs.Count - 1,
                "siblingIndex > 0 && siblingIndex < _tabs.Count - 1");
            ActivateTab(_tabs[ndx]);
        }

        public void RefreshTab(TabContent tabContent)
        {
            if (_toggles[tabContent].Extension.Label != null)
                _toggles[tabContent].Extension.Label.text = tabContent.DisplayName;
            
            /*
            // TODO: we should probably not set colors here and let theming be handled in a completely separate system
            _toggles[tabContent].Extension.OnColor = onColor;
            _toggles[tabContent].Extension.OffColor = offColor;
            */
        }
    }
}