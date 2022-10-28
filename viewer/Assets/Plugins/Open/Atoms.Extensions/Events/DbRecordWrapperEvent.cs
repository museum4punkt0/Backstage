using UnityEngine;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Event of type `DbRecordWrapper`. Inherits from `AtomEvent&lt;DbRecordWrapper&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/DbRecordWrapper", fileName = "DbRecordWrapperEvent")]
    public sealed class DbRecordWrapperEvent : AtomEvent<DbRecordWrapper>
    {
    }
}
