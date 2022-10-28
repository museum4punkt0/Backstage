using Exploratorium.Frontend;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Variable Instancer of type `Flow.FlowState`. Inherits from `AtomVariableInstancer&lt;Flow_FlowStateVariable, Flow_FlowStatePair, Flow.FlowState, Flow_FlowStateEvent, Flow_FlowStatePairEvent, Flow_FlowStateFlow_FlowStateFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-hotpink")]
    [AddComponentMenu("Unity Atoms/Variable Instancers/Flow_FlowState Variable Instancer")]
    public class Flow_FlowStateVariableInstancer : AtomVariableInstancer<
        Flow_FlowStateVariable,
        Flow_FlowStatePair,
        Flow.FlowState,
        FlowStateEvent,
        Flow_FlowStatePairEvent,
        Flow_FlowStateFlow_FlowStateFunction>
    { }
}
