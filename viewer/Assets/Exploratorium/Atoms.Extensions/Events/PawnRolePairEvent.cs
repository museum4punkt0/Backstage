using UnityEngine;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event of type `PawnRolePair`. Inherits from `AtomEvent&lt;PawnRolePair&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/PawnRolePair", fileName = "PawnRolePairEvent")]
    public sealed class PawnRolePairEvent : AtomEvent<PawnRolePair>
    {
    }
}
