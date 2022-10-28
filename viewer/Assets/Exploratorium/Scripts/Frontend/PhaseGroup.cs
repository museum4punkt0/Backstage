using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exploratorium.Frontend
{
    public class PhaseGroup : MonoBehaviour, IPhaseGroup
    {
        private const string DebugButtons = "Debug Buttons";
        private readonly HashSet<IPhased> _slots = new HashSet<IPhased>();

        public ISet<IPhased> Slots => _slots;

        private void Start()
        {
            OpenAll();
        }

        [Button, HorizontalGroup(DebugButtons)]
        private void OpenAll()
        {
            foreach (var box in _slots)
            {
                if (box == null)
                    continue;
                
                box.Open();
            }
        }

        [Button, HorizontalGroup(DebugButtons)]
        private void CloseAll()
        {
            foreach (var box in _slots)
            {
                if (box == null)
                    continue;

                box.Close();
            }
        }

        public void RecenterViews()
        {
            /* TODO: IMPLEMENT THIS
            */
        }

        public void Select([NotNull]IPhased phasedPresenter)
        {
            foreach (var box in _slots)
            {
                if (box == null)
                    continue;
                
                if (box == phasedPresenter)
                    box.SelectWithoutNotify();
                else
                    box.DeSelectWithoutNotify();
            }
        }

        public void Deselect([NotNull] IPhased phasedPresenter)
        {
            phasedPresenter.DeSelectWithoutNotify();
        }

        public void UnRegister([NotNull] IPhased phasedPresenter)
        {
            _slots.Remove(phasedPresenter);
        }

        public void Register([NotNull] IPhased phasedPresenter)
        {
            _slots.Add(phasedPresenter);
        }
    }
}