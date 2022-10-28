using System;
using Exploratorium.Frontend;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Reference of type `Flow.FlowState`. Inherits from `AtomEventReference&lt;Flow.FlowState, Flow.FlowStateVariable, Flow_FlowStateEvent, Flow.FlowStateVariableInstancer, Flow_FlowStateEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class Flow_FlowStateEventReference : AtomEventReference<
        Flow.FlowState,
        Flow_FlowStateVariable,
        FlowStateEvent,
        Flow_FlowStateVariableInstancer,
        Flow_FlowStateEventInstancer>, IGetEvent 
    { }
}
