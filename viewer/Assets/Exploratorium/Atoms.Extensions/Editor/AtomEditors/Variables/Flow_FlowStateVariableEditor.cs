using Exploratorium.Frontend;
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Variable Inspector of type `Flow.FlowState`. Inherits from `AtomVariableEditor`
    /// </summary>
    [CustomEditor(typeof(Flow_FlowStateVariable))]
    public sealed class Flow_FlowStateVariableEditor : AtomVariableEditor<Flow.FlowState, Flow_FlowStatePair> { }
}
