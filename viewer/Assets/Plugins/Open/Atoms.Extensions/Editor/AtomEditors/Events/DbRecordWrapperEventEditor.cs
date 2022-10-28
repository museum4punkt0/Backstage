#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using UnityAtoms;

namespace UnityAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `DbRecordWrapper`. Inherits from `AtomEventEditor&lt;DbRecordWrapper, DbRecordWrapperEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(DbRecordWrapperEvent))]
    public sealed class DbRecordWrapperEventEditor : AtomEventEditor<DbRecordWrapper, DbRecordWrapperEvent> { }
}
#endif
