using System;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Reference of type `PawnRolePair`. Inherits from `AtomEventReference&lt;PawnRolePair, PawnRoleVariable, PawnRolePairEvent, PawnRoleVariableInstancer, PawnRolePairEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class PawnRolePairEventReference : AtomEventReference<
        PawnRolePair,
        PawnRoleVariable,
        PawnRolePairEvent,
        PawnRoleVariableInstancer,
        PawnRolePairEventInstancer>, IGetEvent 
    { }
}
