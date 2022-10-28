using System;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

namespace UnityAtoms
{
    /// <summary>
    /// Reference of type `DbRecordWrapper`. Inherits from `AtomReference&lt;DbRecordWrapper, DbRecordWrapperPair, DbRecordWrapperConstant, DbRecordWrapperVariable, DbRecordWrapperEvent, DbRecordWrapperPairEvent, DbRecordWrapperDbRecordWrapperFunction, DbRecordWrapperVariableInstancer, AtomCollection, AtomList&gt;`.
    /// </summary>
    [Serializable]
    public sealed class DbRecordWrapperReference : AtomReference<
        DbRecordWrapper,
        DbRecordWrapperPair,
        DbRecordWrapperConstant,
        DbRecordWrapperVariable,
        DbRecordWrapperEvent,
        DbRecordWrapperPairEvent,
        DbRecordWrapperDbRecordWrapperFunction,
        DbRecordWrapperVariableInstancer>, IEquatable<DbRecordWrapperReference>
    {
        public DbRecordWrapperReference() : base() { }
        public DbRecordWrapperReference(DbRecordWrapper value) : base(value) { }
        public bool Equals(DbRecordWrapperReference other) { return base.Equals(other); }
        protected override bool ValueEquals(DbRecordWrapper other)
        {
            return other.Record.Equals(Value?.Record);
        }
    }
}
