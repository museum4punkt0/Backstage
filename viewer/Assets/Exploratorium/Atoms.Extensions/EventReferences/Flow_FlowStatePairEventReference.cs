using System;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Reference of type `Flow_FlowStatePair`. Inherits from `AtomEventReference&lt;Flow_FlowStatePair, Flow.FlowStateVariable, Flow_FlowStatePairEvent, Flow.FlowStateVariableInstancer, Flow_FlowStatePairEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class Flow_FlowStatePairEventReference : AtomEventReference<
        Flow_FlowStatePair,
        Flow_FlowStateVariable,
        Flow_FlowStatePairEvent,
        Flow_FlowStateVariableInstancer,
        Flow_FlowStatePairEventInstancer>, IGetEvent 
    { }
}
