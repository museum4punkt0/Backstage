using Exploratorium.Frontend;
using UnityEngine;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Value List of type `Flow.FlowState`. Inherits from `AtomValueList&lt;Flow.FlowState, Flow_FlowStateEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-piglet")]
    [CreateAssetMenu(menuName = "Unity Atoms/Value Lists/Flow_FlowState", fileName = "Flow_FlowStateValueList")]
    public sealed class Flow_FlowStateValueList : AtomValueList<Flow.FlowState, FlowStateEvent> { }
}
