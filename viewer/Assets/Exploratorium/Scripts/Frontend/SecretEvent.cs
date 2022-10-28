using Exploratorium.Net.Shared;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    public class SecretEvent : MonoBehaviour
    {
        [BoxGroup(Constants.ObservedEvents), SerializeField, Required]
        private Button button;

        [BoxGroup(Constants.ObservedEvents), SerializeField, Required]
        private PawnRoleVariable role;

        [BoxGroup(Constants.InvokedEvents), SerializeField, Required]
        private VoidEvent upEvent;

        [BoxGroup(Constants.InvokedEvents), SerializeField, Required]
        private VoidEvent downEvent;

        [SerializeField] [Min(1)]
        private int count = 3;

        [SerializeField] [Min(0)]
        private float timeout = 0.5f;

        [SerializeField]
        private PawnRole markRole = PawnRole.Controller;

        [SerializeField, Required] private GameObject marker;

        private float _lastTap;
        private float _count;
        private bool _isUp;

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
            role.Changed.Register(OnRoleChanged);
            OnRoleChanged(role.Value);
        }

        private void OnRoleChanged(PawnRole obj) => marker.SetActive(obj == markRole);

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
            role.Changed.Unregister(OnRoleChanged);
        }

        private void OnButtonClicked()
        {
            if (_lastTap + timeout < Time.time)
                _count = 0;
            _lastTap = Time.time;
            _count++;

            if (_count >= count)
            {
                _count = 0;
                _isUp = !_isUp;
                Activate(_isUp);
            }

            marker.SetActive(_isUp);
        }

        private void Activate(bool isUp)
        {
            if (isUp)
                upEvent.Raise();
            else
                downEvent.Raise();
        }
    }
}