using UnityEngine;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Constant of type `PawnRole`. Inherits from `AtomBaseVariable&lt;PawnRole&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-teal")]
    [CreateAssetMenu(menuName = "Unity Atoms/Constants/PawnRole", fileName = "PawnRoleConstant")]
    public sealed class PawnRoleConstant : AtomBaseVariable<PawnRole> { }
}
