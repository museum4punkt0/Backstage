#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Editor
{
    /// <summary>
    /// Value List property drawer of type `DbRecordWrapper`. Inherits from `AtomDrawer&lt;DbRecordWrapperValueList&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(DbRecordWrapperValueList))]
    public class DbRecordWrapperValueListDrawer : AtomDrawer<DbRecordWrapperValueList> { }
}
#endif
