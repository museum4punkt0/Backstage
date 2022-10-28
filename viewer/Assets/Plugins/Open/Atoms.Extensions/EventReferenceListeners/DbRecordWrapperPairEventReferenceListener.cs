using UnityEngine;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Event Reference Listener of type `DbRecordWrapperPair`. Inherits from `AtomEventReferenceListener&lt;DbRecordWrapperPair, DbRecordWrapperPairEvent, DbRecordWrapperPairEventReference, DbRecordWrapperPairUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/DbRecordWrapperPair Event Reference Listener")]
    public sealed class DbRecordWrapperPairEventReferenceListener : AtomEventReferenceListener<
        DbRecordWrapperPair,
        DbRecordWrapperPairEvent,
        DbRecordWrapperPairEventReference,
        DbRecordWrapperPairUnityEvent>
    { }
}
