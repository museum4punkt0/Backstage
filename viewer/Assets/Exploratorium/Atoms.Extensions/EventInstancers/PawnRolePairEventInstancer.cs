using UnityEngine;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Instancer of type `PawnRolePair`. Inherits from `AtomEventInstancer&lt;PawnRolePair, PawnRolePairEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/PawnRolePair Event Instancer")]
    public class PawnRolePairEventInstancer : AtomEventInstancer<PawnRolePair, PawnRolePairEvent> { }
}
