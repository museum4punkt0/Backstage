#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Event property drawer of type `PawnRolePair`. Inherits from `AtomDrawer&lt;PawnRolePairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(PawnRolePairEvent))]
    public class PawnRolePairEventDrawer : AtomDrawer<PawnRolePairEvent> { }
}
#endif
