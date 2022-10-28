using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium
{
    public class ToggleActive : MonoBehaviour
    {
        [SerializeField] private Button toggleButton;
        [SerializeField] private GameObject[] targets;

        private bool _isOn;

        private void OnEnable() => toggleButton.onClick.AddListener(OnClick);

        private void OnDisable() => toggleButton.onClick.RemoveListener(OnClick);

        private void OnClick()
        {
            _isOn = !_isOn;
            foreach (var target in targets)
            {
                if (target != null)
                    target.SetActive(_isOn);
            }
        }
    }
}