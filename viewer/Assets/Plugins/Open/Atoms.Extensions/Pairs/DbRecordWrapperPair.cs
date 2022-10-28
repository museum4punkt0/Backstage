using System;
using UnityEngine;
using UnityAtoms;
namespace UnityAtoms
{
    /// <summary>
    /// IPair of type `&lt;DbRecordWrapper&gt;`. Inherits from `IPair&lt;DbRecordWrapper&gt;`.
    /// </summary>
    [Serializable]
    public struct DbRecordWrapperPair : IPair<DbRecordWrapper>
    {
        public DbRecordWrapper Item1 { get => _item1; set => _item1 = value; }
        public DbRecordWrapper Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private DbRecordWrapper _item1;
        [SerializeField]
        private DbRecordWrapper _item2;

        public void Deconstruct(out DbRecordWrapper item1, out DbRecordWrapper item2) { item1 = Item1; item2 = Item2; }
    }
}