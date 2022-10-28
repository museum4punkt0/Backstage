using UnityEngine;
using System;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Variable of type `DbRecordWrapper`. Inherits from `AtomVariable&lt;DbRecordWrapper, DbRecordWrapperPair, DbRecordWrapperEvent, DbRecordWrapperPairEvent, DbRecordWrapperDbRecordWrapperFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/DbRecordWrapper", fileName = "DbRecordWrapperVariable")]
    public sealed class DbRecordWrapperVariable : AtomVariable<DbRecordWrapper, DbRecordWrapperPair, DbRecordWrapperEvent, DbRecordWrapperPairEvent, DbRecordWrapperDbRecordWrapperFunction>
    {
        protected override bool ValueEquals(DbRecordWrapper other) => other?.Record != null && other.Record.Equals(Value?.Record);
    }
}
