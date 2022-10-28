using System;
using UnityAtoms.BaseAtoms;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Reference of type `PawnRole`. Inherits from `AtomReference&lt;PawnRole, PawnRolePair, PawnRoleConstant, PawnRoleVariable, PawnRoleEvent, PawnRolePairEvent, PawnRolePawnRoleFunction, PawnRoleVariableInstancer, AtomCollection, AtomList&gt;`.
    /// </summary>
    [Serializable]
    public sealed class PawnRoleReference : AtomReference<
        PawnRole,
        PawnRolePair,
        PawnRoleConstant,
        PawnRoleVariable,
        PawnRoleEvent,
        PawnRolePairEvent,
        PawnRolePawnRoleFunction,
        PawnRoleVariableInstancer>, IEquatable<PawnRoleReference>
    {
        public PawnRoleReference() : base() { }
        public PawnRoleReference(PawnRole value) : base(value) { }
        public bool Equals(PawnRoleReference other) { return base.Equals(other); }
        protected override bool ValueEquals(PawnRole other)
        {
            throw new NotImplementedException();
        }
    }
}
