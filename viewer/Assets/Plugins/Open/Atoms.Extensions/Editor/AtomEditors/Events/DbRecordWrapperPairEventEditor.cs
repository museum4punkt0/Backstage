#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using UnityAtoms;

namespace UnityAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `DbRecordWrapperPair`. Inherits from `AtomEventEditor&lt;DbRecordWrapperPair, DbRecordWrapperPairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(DbRecordWrapperPairEvent))]
    public sealed class DbRecordWrapperPairEventEditor : AtomEventEditor<DbRecordWrapperPair, DbRecordWrapperPairEvent> { }
}
#endif
