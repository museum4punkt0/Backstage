using System;
using System.Linq;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAtoms.MonoBindings
{
    /// <summary>
    /// Mono Hook for a Dropdown
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(TMP_Dropdown))]
    public sealed class DropdownStringBinding : StringBinding
    {
        private TMP_Dropdown _dropdown;

        [SerializeField] private StringValueList options;
        private bool _optionsDirty;

        private void OnEnable()
        {
            if (!TryGetComponent(out _dropdown))
                return;

            Debug.Assert(options != null, "options != null");
            Debug.Assert(_dropdown != null, "_dropdown != null");

            options.Cleared.Register(OnOptionsChanged);
            options.Added.Register(OnOptionsChanged);
            options.Removed.Register(OnOptionsChanged);
            _dropdown.onValueChanged.AddListener(OnChangeVariableAdapter);

            OnVariableChanged(variable.Value);
            OnOptionsChanged();
        }

        private void OnOptionsChanged()
        {
            // prevent updating multiple times per frame
            _optionsDirty = true;
        }

        private void Update()
        {
            if (!_optionsDirty)
                return;

            if (_dropdown == null)
                return;

            _dropdown.options.Clear();
            _dropdown.options.AddRange(options.List.Select(it => new TMP_Dropdown.OptionData(it)));

            if (_dropdown.options.Count == 0)
            {
                // invalid state, cant maintain a valid selection, so we keep the previous value
                _dropdown.interactable = false;
            }
            else
            {
                _dropdown.interactable = true;
                // new valid state, we try to keep the old value selected if possible
                bool newContainsOld = _dropdown.options.Any(it => it.text == variable.Value);
                if (newContainsOld)
                    _dropdown.value = _dropdown.options.FindIndex(it => it.text == variable.Value);
                else
                    _dropdown.value = 0;
            }

            _dropdown.RefreshShownValue();
            _optionsDirty = false;
        }

        protected override void OnVariableChanged(string value)
        {
            if (_dropdown == null)
                return;
            _dropdown = GetComponent<TMP_Dropdown>();
            var ndx = _dropdown.options.FindIndex(it => it.text == value);
            _dropdown.SetValueWithoutNotify(ndx);
            _dropdown.RefreshShownValue();
        }

        private void OnDisable()
        {
            if (!_dropdown)
                return;
            options.Cleared.Unregister(OnOptionsChanged);
            options.Added.Unregister(OnOptionsChanged);
            options.Removed.Unregister(OnOptionsChanged);
            _dropdown.onValueChanged.RemoveListener(OnChangeVariableAdapter);
        }

        private void OnChangeVariableAdapter(int ndx)
        {
            // prevent variable change if options.count is zero as this is not a change received from a user event
            if (_dropdown.options.Count != 0)
                variable.Value = _dropdown.options[ndx].text;
        }
    }
}