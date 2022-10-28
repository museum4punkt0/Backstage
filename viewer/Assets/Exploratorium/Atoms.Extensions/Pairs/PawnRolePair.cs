using System;
using UnityEngine;
using Exploratorium.Net.Shared;
namespace UnityAtoms.Extensions
{
    /// <summary>
    /// IPair of type `&lt;PawnRole&gt;`. Inherits from `IPair&lt;PawnRole&gt;`.
    /// </summary>
    [Serializable]
    public struct PawnRolePair : IPair<PawnRole>
    {
        public PawnRole Item1 { get => _item1; set => _item1 = value; }
        public PawnRole Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private PawnRole _item1;
        [SerializeField]
        private PawnRole _item2;

        public void Deconstruct(out PawnRole item1, out PawnRole item2) { item1 = Item1; item2 = Item2; }
    }
}