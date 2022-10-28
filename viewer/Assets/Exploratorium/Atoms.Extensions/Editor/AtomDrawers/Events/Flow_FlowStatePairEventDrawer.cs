#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Event property drawer of type `Flow_FlowStatePair`. Inherits from `AtomDrawer&lt;Flow_FlowStatePairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(Flow_FlowStatePairEvent))]
    public class Flow_FlowStatePairEventDrawer : AtomDrawer<Flow_FlowStatePairEvent> { }
}
#endif
