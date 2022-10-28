using System;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Event Reference of type `DbRecordWrapperPair`. Inherits from `AtomEventReference&lt;DbRecordWrapperPair, DbRecordWrapperVariable, DbRecordWrapperPairEvent, DbRecordWrapperVariableInstancer, DbRecordWrapperPairEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class DbRecordWrapperPairEventReference : AtomEventReference<
        DbRecordWrapperPair,
        DbRecordWrapperVariable,
        DbRecordWrapperPairEvent,
        DbRecordWrapperVariableInstancer,
        DbRecordWrapperPairEventInstancer>, IGetEvent 
    { }
}
