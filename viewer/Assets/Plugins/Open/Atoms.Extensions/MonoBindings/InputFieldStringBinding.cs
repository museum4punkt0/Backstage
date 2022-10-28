using TMPro;
using UnityEngine;

namespace UnityAtoms.MonoBindings

{
    /// <summary>
    /// Mono Hook for an Input Field
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class InputFieldStringBinding : StringBinding
    {
        private enum Mode
        {
            OnValueChanged,
            OnEndEdit,
            Both
        }

        [SerializeField]
        private Mode mode = Mode.OnValueChanged;

        private TMP_InputField _inputField;

        private void OnEnable()
        {
            if (!TryGetComponent(out _inputField))
                return;
            if (mode == Mode.OnValueChanged || mode == Mode.Both)
                _inputField.onValueChanged.AddListener(OnFieldChanged);
            if (mode == Mode.OnEndEdit || mode == Mode.Both)
                _inputField.onEndEdit.AddListener(OnFieldChanged);
            OnVariableChanged(variable.Value);
        }

        private void OnFieldChanged(string value)
        {
            if (variable != null)
                variable.Value = value;
        }

        protected override void OnVariableChanged(string value)
        {
            _inputField.SetTextWithoutNotify(value);
        }

        private void OnDisable()
        {
            if (!_inputField)
                return;
            if (mode == Mode.OnValueChanged || mode == Mode.Both)
                _inputField.onValueChanged.RemoveListener(OnFieldChanged);
            if (mode == Mode.OnEndEdit || mode == Mode.Both)
                _inputField.onEndEdit.RemoveListener(OnFieldChanged);
        }
    }
}