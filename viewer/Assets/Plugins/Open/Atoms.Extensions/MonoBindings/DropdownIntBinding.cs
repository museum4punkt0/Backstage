using TMPro;
using UnityEngine;

namespace UnityAtoms.MonoBindings
{
    /// <summary>
    /// Mono Hook for a Dropdown
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(TMP_Dropdown))]
    public sealed class DropdownIntBinding : IntBinding
    {
        private TMP_Dropdown _dropdown;

        private void OnEnable()
        {
            if (TryGetComponent(out _dropdown) && variable != null)
            {
                _dropdown.onValueChanged.AddListener(OnDropdownChanged);
                OnVariableChanged(variable.Value);
            }
        }

        private void OnDropdownChanged(int arg0)
        {
            variable.Value = _dropdown.value;
        }

        protected override void OnVariableChanged(int value)
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.SetValueWithoutNotify(value);
        }

        private void OnDisable()
        {
            if (_dropdown != null)
                _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        }
    }
}