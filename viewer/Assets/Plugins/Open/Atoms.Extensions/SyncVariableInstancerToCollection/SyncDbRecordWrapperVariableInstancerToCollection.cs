using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Adds Variable Instancer's Variable of type DbRecordWrapper to a Collection or List on OnEnable and removes it on OnDestroy. 
    /// </summary>
    [AddComponentMenu("Unity Atoms/Sync Variable Instancer to Collection/Sync DbRecordWrapper Variable Instancer to Collection")]
    [EditorIcon("atom-icon-delicate")]
    public class SyncDbRecordWrapperVariableInstancerToCollection : SyncVariableInstancerToCollection<DbRecordWrapper, DbRecordWrapperVariable, DbRecordWrapperVariableInstancer> { }
}
