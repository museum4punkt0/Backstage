#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Event property drawer of type `Flow.FlowState`. Inherits from `AtomDrawer&lt;Flow_FlowStateEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(FlowStateEvent))]
    public class Flow_FlowStateEventDrawer : AtomDrawer<FlowStateEvent> { }
}
#endif
