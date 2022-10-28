#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Editor
{
    /// <summary>
    /// Constant property drawer of type `DbRecordWrapper`. Inherits from `AtomDrawer&lt;DbRecordWrapperConstant&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(DbRecordWrapperConstant))]
    public class DbRecordWrapperConstantDrawer : VariableDrawer<DbRecordWrapperConstant> { }
}
#endif
