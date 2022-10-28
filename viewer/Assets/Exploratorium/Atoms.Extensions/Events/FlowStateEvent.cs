using Exploratorium.Frontend;
using UnityEngine;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event of type `Flow.FlowState`. Inherits from `AtomEvent&lt;Flow.FlowState&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/Flow_FlowState", fileName = "Flow_FlowStateEvent")]
    public sealed class FlowStateEvent : AtomEvent<Flow.FlowState>
    {
    }
}
