using UnityEngine;
using UnityAtoms.BaseAtoms;
using Exploratorium.Net.Shared;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// Adds Variable Instancer's Variable of type PawnRole to a Collection or List on OnEnable and removes it on OnDestroy. 
    /// </summary>
    [AddComponentMenu("Unity Atoms/Sync Variable Instancer to Collection/Sync PawnRole Variable Instancer to Collection")]
    [EditorIcon("atom-icon-delicate")]
    public class SyncPawnRoleVariableInstancerToCollection : SyncVariableInstancerToCollection<PawnRole, PawnRoleVariable, PawnRoleVariableInstancer> { }
}
