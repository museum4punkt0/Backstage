using Exploratorium.Frontend;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Set variable value Action of type `Flow.FlowState`. Inherits from `SetVariableValue&lt;Flow.FlowState, Flow_FlowStatePair, Flow_FlowStateVariable, Flow_FlowStateConstant, Flow_FlowStateReference, Flow_FlowStateEvent, Flow_FlowStatePairEvent, Flow_FlowStateVariableInstancer&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-purple")]
    [CreateAssetMenu(menuName = "Unity Atoms/Actions/Set Variable Value/Flow_FlowState", fileName = "SetFlow_FlowStateVariableValue")]
    public sealed class SetFlow_FlowStateVariableValue : SetVariableValue<
        Flow.FlowState,
        Flow_FlowStatePair,
        Flow_FlowStateVariable,
        Flow_FlowStateConstant,
        Flow_FlowStateReference,
        FlowStateEvent,
        Flow_FlowStatePairEvent,
        Flow_FlowStateFlow_FlowStateFunction,
        Flow_FlowStateVariableInstancer>
    { }
}
