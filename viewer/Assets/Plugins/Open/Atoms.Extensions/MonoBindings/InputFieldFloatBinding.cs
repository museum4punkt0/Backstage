using System.Globalization;
using TMPro;
using UnityEngine;

namespace UnityAtoms.MonoHooks
{
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class InputFieldFloatBinding : FloatBinding
    {
        private TMP_InputField _inputField;

        [Min(0)]
        [SerializeField] private int precision = 1;

        private void OnEnable()
        {
            if (!TryGetComponent(out _inputField))
                return;

            _inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            _inputField.onValueChanged.AddListener(OnFieldChanged);
            OnVariableChanged(variable.Value);
        }

        private void OnFieldChanged(string value)
        {
            if (variable == null)
                return;
            var isParsed = float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed);
            if (isParsed)
                variable.SetValue(parsed);
        }

        protected override void OnVariableChanged(float value)
        {
            if (!_inputField)
                return;
            _inputField.SetTextWithoutNotify(value.ToString($"F{precision}", CultureInfo.InvariantCulture));
        }

        private void OnDisable()
        {
            if (_inputField != null)
                _inputField.onValueChanged.RemoveListener(OnFieldChanged);
        }
    }
}