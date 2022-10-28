using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityAtoms.MonoHooks;
using UnityEngine;
using UnityEngine.UI;
namespace UnityAtoms.MonoBindings
{

    /// <summary>
    /// Mono Binding for Selectable components
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class CanvasGroupBoolBinding : BoolBinding
    {
        private CanvasGroup _canvasGroup;

        [SerializeField] private bool invertValue;
        [SerializeField] private BoolUnityEvent onValueChanged;

        private void OnEnable()
        {
            if (!TryGetComponent(out _canvasGroup))
                return;

            OnVariableChanged(variable.Value);
        }

        protected override void OnVariableChanged(bool value)
        {
            if (!_canvasGroup)
                return;
            bool v = invertValue ? !value : value;
            _canvasGroup.interactable = v;
            onValueChanged.Invoke(v);
        }
    }
}