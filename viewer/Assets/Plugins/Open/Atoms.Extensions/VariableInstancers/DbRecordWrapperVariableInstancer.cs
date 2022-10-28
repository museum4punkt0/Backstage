using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Variable Instancer of type `DbRecordWrapper`. Inherits from `AtomVariableInstancer&lt;DbRecordWrapperVariable, DbRecordWrapperPair, DbRecordWrapper, DbRecordWrapperEvent, DbRecordWrapperPairEvent, DbRecordWrapperDbRecordWrapperFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-hotpink")]
    [AddComponentMenu("Unity Atoms/Variable Instancers/DbRecordWrapper Variable Instancer")]
    public class DbRecordWrapperVariableInstancer : AtomVariableInstancer<
        DbRecordWrapperVariable,
        DbRecordWrapperPair,
        DbRecordWrapper,
        DbRecordWrapperEvent,
        DbRecordWrapperPairEvent,
        DbRecordWrapperDbRecordWrapperFunction>
    { }
}
