using System;
using UnityEngine.Events;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// None generic Unity Event of type `PawnRolePair`. Inherits from `UnityEvent&lt;PawnRolePair&gt;`.
    /// </summary>
    [Serializable]
    public sealed class PawnRolePairUnityEvent : UnityEvent<PawnRolePair> { }
}
