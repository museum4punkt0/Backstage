using Exploratorium.Frontend;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Adds Variable Instancer's Variable of type Flow.FlowState to a Collection or List on OnEnable and removes it on OnDestroy. 
    /// </summary>
    [AddComponentMenu("Unity Atoms/Sync Variable Instancer to Collection/Sync Flow_FlowState Variable Instancer to Collection")]
    [EditorIcon("atom-icon-delicate")]
    public class SyncFlow_FlowStateVariableInstancerToCollection : SyncVariableInstancerToCollection<Flow.FlowState, Flow_FlowStateVariable, Flow_FlowStateVariableInstancer> { }
}
