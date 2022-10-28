using Exploratorium.Frontend;
using UnityEngine;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Reference Listener of type `Flow.FlowState`. Inherits from `AtomEventReferenceListener&lt;Flow.FlowState, Flow_FlowStateEvent, Flow_FlowStateEventReference, Flow_FlowStateUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/Flow_FlowState Event Reference Listener")]
    public sealed class Flow_FlowStateEventReferenceListener : AtomEventReferenceListener<
        Flow.FlowState,
        FlowStateEvent,
        Flow_FlowStateEventReference,
        Flow_FlowStateUnityEvent>
    { }
}
