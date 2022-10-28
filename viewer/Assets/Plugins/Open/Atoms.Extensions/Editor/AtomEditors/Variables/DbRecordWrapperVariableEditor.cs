using UnityEditor;
using UnityAtoms.Editor;
using UnityAtoms;

namespace UnityAtoms.Editor
{
    /// <summary>
    /// Variable Inspector of type `DbRecordWrapper`. Inherits from `AtomVariableEditor`
    /// </summary>
    [CustomEditor(typeof(DbRecordWrapperVariable))]
    public sealed class DbRecordWrapperVariableEditor : AtomVariableEditor<DbRecordWrapper, DbRecordWrapperPair> { }
}
