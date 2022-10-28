using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAtoms.MonoBindings
{
    /// <summary>
    /// Mono Hook for a Slider
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(Slider))]
    public sealed class SliderIntBinding : IntBinding
    {
        [SerializeField] private TMP_Text readout;
        [SerializeField] private string unit;

        private Slider _slider;

        private void OnEnable()
        {
            if (!TryGetComponent(out _slider))
                return;

            _slider.wholeNumbers = true;
            _slider.onValueChanged.AddListener(OnFieldChanged);
            if (variable != null)
                OnVariableChanged(variable.Value);
        }

        private void OnFieldChanged(float value)
        {
            if (variable != null)
                variable.SetValue((int)value);
        }

        protected override void OnVariableChanged(int value)
        {
            if (_slider != null)
                _slider.SetValueWithoutNotify(value);
            if (readout != null)
                readout.text = $"{value}\u0020{unit}";
        }

        private void OnDisable()
        {
            if (_slider != null)
                _slider.onValueChanged.RemoveListener(OnFieldChanged);
        }
    }
}