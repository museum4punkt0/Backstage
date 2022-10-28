using System;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Reference of type `PawnRole`. Inherits from `AtomEventReference&lt;PawnRole, PawnRoleVariable, PawnRoleEvent, PawnRoleVariableInstancer, PawnRoleEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class PawnRoleEventReference : AtomEventReference<
        PawnRole,
        PawnRoleVariable,
        PawnRoleEvent,
        PawnRoleVariableInstancer,
        PawnRoleEventInstancer>, IGetEvent 
    { }
}
