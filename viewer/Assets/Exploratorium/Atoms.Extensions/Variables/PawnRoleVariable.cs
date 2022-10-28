using UnityEngine;
using System;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Variable of type `PawnRole`. Inherits from `AtomVariable&lt;PawnRole, PawnRolePair, PawnRoleEvent, PawnRolePairEvent, PawnRolePawnRoleFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/PawnRole", fileName = "PawnRoleVariable")]
    public sealed class PawnRoleVariable : AtomVariable<PawnRole, PawnRolePair, PawnRoleEvent, PawnRolePairEvent, PawnRolePawnRoleFunction>
    {
        protected override bool ValueEquals(PawnRole other) => Value == other;
    }
}
