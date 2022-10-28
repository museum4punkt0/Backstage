using UnityEngine;
using UnityAtoms.BaseAtoms;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Set variable value Action of type `PawnRole`. Inherits from `SetVariableValue&lt;PawnRole, PawnRolePair, PawnRoleVariable, PawnRoleConstant, PawnRoleReference, PawnRoleEvent, PawnRolePairEvent, PawnRoleVariableInstancer&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-purple")]
    [CreateAssetMenu(menuName = "Unity Atoms/Actions/Set Variable Value/PawnRole", fileName = "SetPawnRoleVariableValue")]
    public sealed class SetPawnRoleVariableValue : SetVariableValue<
        PawnRole,
        PawnRolePair,
        PawnRoleVariable,
        PawnRoleConstant,
        PawnRoleReference,
        PawnRoleEvent,
        PawnRolePairEvent,
        PawnRolePawnRoleFunction,
        PawnRoleVariableInstancer>
    { }
}
