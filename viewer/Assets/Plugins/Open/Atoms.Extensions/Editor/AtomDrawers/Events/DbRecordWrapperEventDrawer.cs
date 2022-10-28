#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `DbRecordWrapper`. Inherits from `AtomDrawer&lt;DbRecordWrapperEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(DbRecordWrapperEvent))]
    public class DbRecordWrapperEventDrawer : AtomDrawer<DbRecordWrapperEvent> { }
}
#endif
