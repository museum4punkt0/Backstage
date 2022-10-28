using UnityEngine;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Value List of type `PawnRole`. Inherits from `AtomValueList&lt;PawnRole, PawnRoleEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-piglet")]
    [CreateAssetMenu(menuName = "Unity Atoms/Value Lists/PawnRole", fileName = "PawnRoleValueList")]
    public sealed class PawnRoleValueList : AtomValueList<PawnRole, PawnRoleEvent> { }
}
