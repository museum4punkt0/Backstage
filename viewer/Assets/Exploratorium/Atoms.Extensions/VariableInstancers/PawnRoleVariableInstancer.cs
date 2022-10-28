using UnityEngine;
using UnityAtoms.BaseAtoms;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Variable Instancer of type `PawnRole`. Inherits from `AtomVariableInstancer&lt;PawnRoleVariable, PawnRolePair, PawnRole, PawnRoleEvent, PawnRolePairEvent, PawnRolePawnRoleFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-hotpink")]
    [AddComponentMenu("Unity Atoms/Variable Instancers/PawnRole Variable Instancer")]
    public class PawnRoleVariableInstancer : AtomVariableInstancer<
        PawnRoleVariable,
        PawnRolePair,
        PawnRole,
        PawnRoleEvent,
        PawnRolePairEvent,
        PawnRolePawnRoleFunction>
    { }
}
