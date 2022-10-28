using UnityEngine;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Event Reference Listener of type `DbRecordWrapper`. Inherits from `AtomEventReferenceListener&lt;DbRecordWrapper, DbRecordWrapperEvent, DbRecordWrapperEventReference, DbRecordWrapperUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/DbRecordWrapper Event Reference Listener")]
    public sealed class DbRecordWrapperEventReferenceListener : AtomEventReferenceListener<
        DbRecordWrapper,
        DbRecordWrapperEvent,
        DbRecordWrapperEventReference,
        DbRecordWrapperUnityEvent>
    { }
}
