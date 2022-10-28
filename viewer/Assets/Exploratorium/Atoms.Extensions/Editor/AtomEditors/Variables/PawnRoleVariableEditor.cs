using UnityEditor;
using UnityAtoms.Editor;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions.Editor
{
    /// <summary>
    /// Variable Inspector of type `PawnRole`. Inherits from `AtomVariableEditor`
    /// </summary>
    [CustomEditor(typeof(PawnRoleVariable))]
    public sealed class PawnRoleVariableEditor : AtomVariableEditor<PawnRole, PawnRolePair> { }
}
