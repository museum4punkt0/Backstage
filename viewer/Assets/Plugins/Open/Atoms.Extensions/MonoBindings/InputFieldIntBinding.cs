using System.Globalization;
using TMPro;
using UnityAtoms.MonoBindings;
using UnityEngine;

namespace UnityAtoms.MonoHooks
{
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class InputFieldIntBinding : IntBinding
    {
        private TMP_InputField _inputField;

        private void OnEnable()
        {
            if (!TryGetComponent(out _inputField))
                return;

            _inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            _inputField.onValueChanged.AddListener(OnFieldChanged);
            OnVariableChanged(variable.Value);
        }

        private void OnFieldChanged(string value)
        {
            if (variable == null)
                return;
            var isParsed = int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed);
            if (isParsed)
                variable.SetValue(parsed);
        }

        protected override void OnVariableChanged(int value)
        {
            if (!_inputField)
                return;
            _inputField.SetTextWithoutNotify(value.ToString("D", CultureInfo.InvariantCulture));
        }

        private void OnDisable()
        {
            if (_inputField != null)
                _inputField.onValueChanged.RemoveListener(OnFieldChanged);
        }
    }
}