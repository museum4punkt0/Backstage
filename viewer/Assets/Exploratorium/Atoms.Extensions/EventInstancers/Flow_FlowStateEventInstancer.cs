using Exploratorium.Frontend;
using UnityEngine;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Instancer of type `Flow.FlowState`. Inherits from `AtomEventInstancer&lt;Flow.FlowState, Flow_FlowStateEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/Flow_FlowState Event Instancer")]
    public class Flow_FlowStateEventInstancer : AtomEventInstancer<Flow.FlowState, FlowStateEvent> { }
}
