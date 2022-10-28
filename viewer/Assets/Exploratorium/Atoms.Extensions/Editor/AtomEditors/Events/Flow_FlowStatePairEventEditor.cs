#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Event property drawer of type `Flow_FlowStatePair`. Inherits from `AtomEventEditor&lt;Flow_FlowStatePair, Flow_FlowStatePairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(Flow_FlowStatePairEvent))]
    public sealed class Flow_FlowStatePairEventEditor : AtomEventEditor<Flow_FlowStatePair, Flow_FlowStatePairEvent> { }
}
#endif
