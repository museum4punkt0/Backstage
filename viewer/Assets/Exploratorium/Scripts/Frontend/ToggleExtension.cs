using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleExtension : UIBehaviour
    {

        [SerializeField] private TMP_Text label;
        [SerializeField] private Image marker;
        [SerializeField] private Color onColor;
        [SerializeField] private Color offColor;

        private Toggle _toggle;
        private bool _init;

        public Color OffColor
        {
            get => offColor;
            set
            {
                offColor = value;
                Refresh();
            }
        }

        public Color OnColor
        {
            get => onColor;
            set
            {
                onColor = value; 
                Refresh();
            }
        
        }

        public Image Marker => marker;

        public TMP_Text Label => label;

        protected override void Awake()
        {
            EnsureInit();
        }

        private void EnsureInit()
        {
            if (_init)
                return;
            _toggle = GetComponent<Toggle>();
            _init = true;
        }

        protected override void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnChanged);
            OnChanged(_toggle.isOn);
        }

        protected override void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnChanged);
        }

        private void OnChanged(bool isOn)
        {
            if (Marker != null)
            {
                Marker.gameObject.SetActive(isOn);
                Marker.color = isOn ? OnColor : OffColor;
            }

            if (Label != null)
            {
                Label.color = isOn ? OnColor : OffColor;
            }
        }

        public void Refresh() => OnChanged(_toggle.isOn);
    }
}