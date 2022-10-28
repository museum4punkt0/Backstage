using UnityEngine;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Reference Listener of type `Flow_FlowStatePair`. Inherits from `AtomEventReferenceListener&lt;Flow_FlowStatePair, Flow_FlowStatePairEvent, Flow_FlowStatePairEventReference, Flow_FlowStatePairUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/Flow_FlowStatePair Event Reference Listener")]
    public sealed class Flow_FlowStatePairEventReferenceListener : AtomEventReferenceListener<
        Flow_FlowStatePair,
        Flow_FlowStatePairEvent,
        Flow_FlowStatePairEventReference,
        Flow_FlowStatePairUnityEvent>
    { }
}
