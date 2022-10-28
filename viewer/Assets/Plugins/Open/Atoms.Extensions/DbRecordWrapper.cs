using System;
using Directus.Connect.v9;
using TMPro;

namespace UnityAtoms
{
    [Serializable]
    public class DbRecordWrapper
    {
        [NonSerialized]
        private DbRecord _record;
        public DbRecordWrapper(DbRecord record) => _record = record;
        
        public override string ToString() => (_record != null ? Record?.ToString() : "null") ?? string.Empty;

        public DbRecord Record => _record;
    }
}