#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Constant property drawer of type `PawnRole`. Inherits from `AtomDrawer&lt;PawnRoleConstant&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(PawnRoleConstant))]
    public class PawnRoleConstantDrawer : VariableDrawer<PawnRoleConstant> { }
}
#endif
