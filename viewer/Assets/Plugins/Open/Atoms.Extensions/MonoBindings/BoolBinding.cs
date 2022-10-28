using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace UnityAtoms.MonoBindings
{
    /// <summary>
    /// Base class for all `MonoBinding`s of type `BoolVariable`.
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    public abstract class BoolBinding : MonoBehaviour
    {
        /// <summary>
        /// The Event
        /// </summary>
        [SerializeField]
        protected BoolVariable variable;

        private void Awake()
        {
            Debug.Assert(variable != null, "variable != null");
            if (variable != null)
            {
                if(variable.Changed != null)
                    variable.Changed.OnEventNoValue += OnChangedAdapter;
                else
                    Debug.LogWarning($"{nameof(BoolBinding)} : A variable changed event is required to make variable bindings work.");
            }
        }

        protected abstract void OnVariableChanged(bool value);
        

        protected void OnChangeVariable(bool value)
        {
            if (variable != null) 
                variable.Value = value;
        }
        
        private void OnChangedAdapter() => OnVariableChanged(variable.Value);

        private void OnDestroy()
        {
            if (variable != null && variable.Changed != null)
                variable.Changed.OnEventNoValue -= OnChangedAdapter;
        }
    }
}