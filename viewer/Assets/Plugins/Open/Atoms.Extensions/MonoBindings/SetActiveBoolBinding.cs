using UnityEngine;

namespace UnityAtoms.MonoBindings
{
    /// <summary>
    /// Mono Hook for On Button Click
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    public sealed class SetActiveBoolBinding : BoolBinding
    {
        [SerializeField] private bool invertBehaviour;
        
        private void Awake() => OnVariableChanged(variable.Value);

        protected override void OnVariableChanged(bool value) => gameObject.SetActive(invertBehaviour ? !value : value);
    }
}