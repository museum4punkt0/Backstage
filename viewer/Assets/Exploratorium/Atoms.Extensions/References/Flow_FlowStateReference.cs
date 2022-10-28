using System;
using Exploratorium.Frontend;
using UnityAtoms.BaseAtoms;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Reference of type `Flow.FlowState`. Inherits from `EquatableAtomReference&lt;Flow.FlowState, Flow_FlowStatePair, Flow_FlowStateConstant, Flow_FlowStateVariable, Flow_FlowStateEvent, Flow_FlowStatePairEvent, Flow_FlowStateFlow_FlowStateFunction, Flow_FlowStateVariableInstancer, AtomCollection, AtomList&gt;`.
    /// </summary>
    [Serializable]
    public sealed class Flow_FlowStateReference : EquatableAtomReference<
        Flow.FlowState,
        Flow_FlowStatePair,
        Flow_FlowStateConstant,
        Flow_FlowStateVariable,
        FlowStateEvent,
        Flow_FlowStatePairEvent,
        Flow_FlowStateFlow_FlowStateFunction,
        Flow_FlowStateVariableInstancer>, IEquatable<Flow_FlowStateReference>
    {
        public Flow_FlowStateReference() : base() { }
        public Flow_FlowStateReference(Flow.FlowState value) : base(value) { }
        public bool Equals(Flow_FlowStateReference other) { return base.Equals(other); }
    }
}
