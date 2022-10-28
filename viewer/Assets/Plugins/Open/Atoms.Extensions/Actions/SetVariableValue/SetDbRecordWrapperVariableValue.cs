using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Set variable value Action of type `DbRecordWrapper`. Inherits from `SetVariableValue&lt;DbRecordWrapper, DbRecordWrapperPair, DbRecordWrapperVariable, DbRecordWrapperConstant, DbRecordWrapperReference, DbRecordWrapperEvent, DbRecordWrapperPairEvent, DbRecordWrapperVariableInstancer&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-purple")]
    [CreateAssetMenu(menuName = "Unity Atoms/Actions/Set Variable Value/DbRecordWrapper", fileName = "SetDbRecordWrapperVariableValue")]
    public sealed class SetDbRecordWrapperVariableValue : SetVariableValue<
        DbRecordWrapper,
        DbRecordWrapperPair,
        DbRecordWrapperVariable,
        DbRecordWrapperConstant,
        DbRecordWrapperReference,
        DbRecordWrapperEvent,
        DbRecordWrapperPairEvent,
        DbRecordWrapperDbRecordWrapperFunction,
        DbRecordWrapperVariableInstancer>
    { }
}
