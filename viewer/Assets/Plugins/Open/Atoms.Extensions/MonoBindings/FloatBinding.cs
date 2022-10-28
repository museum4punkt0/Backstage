using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace UnityAtoms.MonoHooks
{
    /// <summary>
    /// Base class for all `MonoBinding`s of type `FloatVariable`.
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    public abstract class FloatBinding : MonoBehaviour
    {
        /// <summary>
        /// The Event
        /// </summary>
        [SerializeField]
        protected FloatVariable variable;

        private void Awake()
        {
            Debug.Assert(variable != null, "variable != null");
            if (variable != null)
            {
                if(variable.Changed != null)
                    variable.Changed.OnEventNoValue += OnChangedAdapter;
                else
                    Debug.LogWarning($"{nameof(FloatBinding)} : A variable changed event is required to make variable bindings work.");
            }
        }


        protected abstract void OnVariableChanged(float value);
        

        protected void OnChangeVariable(float value)
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