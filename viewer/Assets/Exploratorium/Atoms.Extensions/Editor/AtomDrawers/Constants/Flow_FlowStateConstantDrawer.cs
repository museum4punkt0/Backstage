#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Constant property drawer of type `Flow.FlowState`. Inherits from `AtomDrawer&lt;Flow_FlowStateConstant&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(Flow_FlowStateConstant))]
    public class Flow_FlowStateConstantDrawer : VariableDrawer<Flow_FlowStateConstant> { }
}
#endif
