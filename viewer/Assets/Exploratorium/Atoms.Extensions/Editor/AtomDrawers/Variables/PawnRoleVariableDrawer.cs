#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Variable property drawer of type `PawnRole`. Inherits from `AtomDrawer&lt;PawnRoleVariable&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(PawnRoleVariable))]
    public class PawnRoleVariableDrawer : VariableDrawer<PawnRoleVariable> { }
}
#endif
