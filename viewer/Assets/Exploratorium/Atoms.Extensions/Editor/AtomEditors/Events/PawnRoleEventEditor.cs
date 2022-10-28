#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Event property drawer of type `PawnRole`. Inherits from `AtomEventEditor&lt;PawnRole, PawnRoleEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(PawnRoleEvent))]
    public sealed class PawnRoleEventEditor : AtomEventEditor<PawnRole, PawnRoleEvent> { }
}
#endif
