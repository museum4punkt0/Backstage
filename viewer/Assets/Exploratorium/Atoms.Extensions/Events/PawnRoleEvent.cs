using UnityEngine;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event of type `PawnRole`. Inherits from `AtomEvent&lt;PawnRole&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/PawnRole", fileName = "PawnRoleEvent")]
    public sealed class PawnRoleEvent : AtomEvent<PawnRole>
    {
    }
}
