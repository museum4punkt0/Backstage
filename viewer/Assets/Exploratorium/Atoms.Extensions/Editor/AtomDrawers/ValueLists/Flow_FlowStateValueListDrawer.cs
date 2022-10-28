#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Value List property drawer of type `Flow.FlowState`. Inherits from `AtomDrawer&lt;Flow_FlowStateValueList&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(Flow_FlowStateValueList))]
    public class Flow_FlowStateValueListDrawer : AtomDrawer<Flow_FlowStateValueList> { }
}
#endif
