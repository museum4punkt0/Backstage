using System;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Event Reference of type `DbRecordWrapper`. Inherits from `AtomEventReference&lt;DbRecordWrapper, DbRecordWrapperVariable, DbRecordWrapperEvent, DbRecordWrapperVariableInstancer, DbRecordWrapperEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class DbRecordWrapperEventReference : AtomEventReference<
        DbRecordWrapper,
        DbRecordWrapperVariable,
        DbRecordWrapperEvent,
        DbRecordWrapperVariableInstancer,
        DbRecordWrapperEventInstancer>, IGetEvent 
    { }
}
