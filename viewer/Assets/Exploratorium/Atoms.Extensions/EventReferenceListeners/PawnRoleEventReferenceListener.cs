using UnityEngine;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Reference Listener of type `PawnRole`. Inherits from `AtomEventReferenceListener&lt;PawnRole, PawnRoleEvent, PawnRoleEventReference, PawnRoleUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/PawnRole Event Reference Listener")]
    public sealed class PawnRoleEventReferenceListener : AtomEventReferenceListener<
        PawnRole,
        PawnRoleEvent,
        PawnRoleEventReference,
        PawnRoleUnityEvent>
    { }
}
