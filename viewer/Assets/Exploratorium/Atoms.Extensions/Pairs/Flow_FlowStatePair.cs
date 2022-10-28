using System;
using Exploratorium.Frontend;
using UnityEngine;

namespace UnityAtoms.Extensions
{
    /// <summary>
    /// IPair of type `&lt;Flow.FlowState&gt;`. Inherits from `IPair&lt;Flow.FlowState&gt;`.
    /// </summary>
    [Serializable]
    public struct Flow_FlowStatePair : IPair<Flow.FlowState>
    {
        public Flow.FlowState Item1 { get => _item1; set => _item1 = value; }
        public Flow.FlowState Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private Flow.FlowState _item1;
        [SerializeField]
        private Flow.FlowState _item2;

        public void Deconstruct(out Flow.FlowState item1, out Flow.FlowState item2) { item1 = Item1; item2 = Item2; }
    }
}