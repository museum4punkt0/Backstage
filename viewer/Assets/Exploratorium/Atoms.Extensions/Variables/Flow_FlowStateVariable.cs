using Exploratorium.Frontend;
using UnityEngine;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Variable of type `Flow.FlowState`. Inherits from `EquatableAtomVariable&lt;Flow.FlowState, Flow_FlowStatePair, Flow_FlowStateEvent, Flow_FlowStatePairEvent, Flow_FlowStateFlow_FlowStateFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/Flow_FlowState", fileName = "Flow_FlowStateVariable")]
    public sealed class Flow_FlowStateVariable : EquatableAtomVariable<Flow.FlowState, Flow_FlowStatePair, FlowStateEvent, Flow_FlowStatePairEvent, Flow_FlowStateFlow_FlowStateFunction>
    {
    }
}
