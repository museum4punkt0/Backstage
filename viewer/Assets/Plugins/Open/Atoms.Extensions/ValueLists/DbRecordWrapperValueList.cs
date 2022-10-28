using UnityEngine;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Value List of type `DbRecordWrapper`. Inherits from `AtomValueList&lt;DbRecordWrapper, DbRecordWrapperEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-piglet")]
    [CreateAssetMenu(menuName = "Unity Atoms/Value Lists/DbRecordWrapper", fileName = "DbRecordWrapperValueList")]
    public sealed class DbRecordWrapperValueList : AtomValueList<DbRecordWrapper, DbRecordWrapperEvent> { }
}
