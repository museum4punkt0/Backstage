using UnityEngine;
using UnityEngine.UI;

namespace UnityAtoms.MonoBindings
{
    /// <summary>
    /// Mono Hook for On Button Click
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(Toggle))]
    public sealed class ToggleBoolBinding : BoolBinding
    {
        private Toggle _toggle;

        private void OnEnable()
        {
            if (!TryGetComponent(out _toggle))
                return;

            _toggle.onValueChanged.AddListener(OnChangeVariable);
            if (variable != null)
                OnVariableChanged(variable.Value);
        }

        protected override void OnVariableChanged(bool value)
        {
            if (_toggle != null)
                _toggle.SetIsOnWithoutNotify(value);
        }

        private void OnDisable()
        {
            if (_toggle != null)
                _toggle.onValueChanged.RemoveListener(OnChangeVariable);
        }
    }
}