#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Variable property drawer of type `Flow.FlowState`. Inherits from `AtomDrawer&lt;Flow_FlowStateVariable&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(Flow_FlowStateVariable))]
    public class Flow_FlowStateVariableDrawer : VariableDrawer<Flow_FlowStateVariable> { }
}
#endif
