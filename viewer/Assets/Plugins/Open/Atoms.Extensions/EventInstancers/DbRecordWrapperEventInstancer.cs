using UnityEngine;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Event Instancer of type `DbRecordWrapper`. Inherits from `AtomEventInstancer&lt;DbRecordWrapper, DbRecordWrapperEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/DbRecordWrapper Event Instancer")]
    public class DbRecordWrapperEventInstancer : AtomEventInstancer<DbRecordWrapper, DbRecordWrapperEvent> { }
}
