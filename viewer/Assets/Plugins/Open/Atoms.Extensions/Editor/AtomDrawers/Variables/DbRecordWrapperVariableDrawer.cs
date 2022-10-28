#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Editor
{
    /// <summary>
    /// Variable property drawer of type `DbRecordWrapper`. Inherits from `AtomDrawer&lt;DbRecordWrapperVariable&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(DbRecordWrapperVariable))]
    public class DbRecordWrapperVariableDrawer : VariableDrawer<DbRecordWrapperVariable> { }
}
#endif
