using System;
using UnityEngine.Events;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// None generic Unity Event of type `PawnRole`. Inherits from `UnityEvent&lt;PawnRole&gt;`.
    /// </summary>
    [Serializable]
    public sealed class PawnRoleUnityEvent : UnityEvent<PawnRole> { }
}
