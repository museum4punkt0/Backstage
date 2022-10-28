using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    [RequireComponent(typeof(Toggle))]
    [RequireComponent(typeof(ToggleExtension))]
    public class TabToggle : UIBehaviour
    {
        private Toggle _toggle;
        private ToggleExtension _extension;
        private bool _init;

        public ToggleExtension Extension
        {
            get
            {
                EnsureInit();
                return _extension;
            }
        }

        public Toggle Toggle
        {
            get
            {
                EnsureInit();
                return _toggle;
            }
        }

        protected override void Awake() => EnsureInit();

        private void EnsureInit()
        {
            if (_init)
                return;
            _toggle = GetComponent<Toggle>();
            _extension = GetComponent<ToggleExtension>();
            _init = true;
        }

        public void SetIsOnWithoutNotify(bool isOn)
        {
            EnsureInit();
            _toggle.SetIsOnWithoutNotify(isOn);
            _extension.Refresh();
        }
    }
}