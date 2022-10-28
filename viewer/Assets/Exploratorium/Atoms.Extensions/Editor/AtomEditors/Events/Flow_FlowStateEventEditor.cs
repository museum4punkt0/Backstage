#if UNITY_2019_1_OR_NEWER
using Exploratorium.Frontend;
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Event property drawer of type `Flow.FlowState`. Inherits from `AtomEventEditor&lt;Flow.FlowState, Flow_FlowStateEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(FlowStateEvent))]
    public sealed class Flow_FlowStateEventEditor : AtomEventEditor<Flow.FlowState, FlowStateEvent> { }
}
#endif
