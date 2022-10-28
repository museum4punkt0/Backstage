#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Event property drawer of type `PawnRolePair`. Inherits from `AtomEventEditor&lt;PawnRolePair, PawnRolePairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(PawnRolePairEvent))]
    public sealed class PawnRolePairEventEditor : AtomEventEditor<PawnRolePair, PawnRolePairEvent> { }
}
#endif
