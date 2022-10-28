#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Value List property drawer of type `PawnRole`. Inherits from `AtomDrawer&lt;PawnRoleValueList&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(PawnRoleValueList))]
    public class PawnRoleValueListDrawer : AtomDrawer<PawnRoleValueList> { }
}
#endif
