using UnityEngine;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Event of type `DbRecordWrapperPair`. Inherits from `AtomEvent&lt;DbRecordWrapperPair&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/DbRecordWrapperPair", fileName = "DbRecordWrapperPairEvent")]
    public sealed class DbRecordWrapperPairEvent : AtomEvent<DbRecordWrapperPair>
    {
    }
}
