using TMPro;
using UnityAtoms.MonoHooks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAtoms.MonoBindings
{
    /// <summary>
    /// Mono Hook for a Slider
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(Slider))]
    public sealed class SliderFloatBinding : FloatBinding
    {
        [SerializeField] private TMP_Text readout;
        [SerializeField] private int precision = 1;
        [SerializeField] private string unit = "";
        
        private Slider _slider;

        private void OnEnable()
        {
            if (!TryGetComponent(out _slider))
                return;
            
            Debug.Assert(_slider != null, "_slider != null");

            _slider.onValueChanged.AddListener(OnChangeVariable);
            OnVariableChanged(variable.Value);
        }


        protected override void OnVariableChanged(float value)
        {
            if (_slider != null)
                _slider.SetValueWithoutNotify(value);
            if (readout != null)
                readout.text = $"{value.ToString($"F{precision}")}\u0020{unit}";
        }

        private void OnDisable()
        {
            if (!_slider)
                _slider.onValueChanged.RemoveListener(OnChangeVariable);
        }
    }
}