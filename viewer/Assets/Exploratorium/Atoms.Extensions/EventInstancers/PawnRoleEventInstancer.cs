using UnityEngine;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Event Instancer of type `PawnRole`. Inherits from `AtomEventInstancer&lt;PawnRole, PawnRoleEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/PawnRole Event Instancer")]
    public class PawnRoleEventInstancer : AtomEventInstancer<PawnRole, PawnRoleEvent> { }
}
