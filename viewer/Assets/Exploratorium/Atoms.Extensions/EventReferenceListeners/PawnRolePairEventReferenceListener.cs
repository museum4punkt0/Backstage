using UnityEngine;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Reference Listener of type `PawnRolePair`. Inherits from `AtomEventReferenceListener&lt;PawnRolePair, PawnRolePairEvent, PawnRolePairEventReference, PawnRolePairUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/PawnRolePair Event Reference Listener")]
    public sealed class PawnRolePairEventReferenceListener : AtomEventReferenceListener<
        PawnRolePair,
        PawnRolePairEvent,
        PawnRolePairEventReference,
        PawnRolePairUnityEvent>
    { }
}
