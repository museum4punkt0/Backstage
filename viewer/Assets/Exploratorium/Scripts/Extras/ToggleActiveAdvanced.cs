using Sirenix.OdinInspector;
using UnityAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium.Extras
{
    public class ToggleActiveAdvanced : MonoBehaviour
    {
        [SerializeField]
        [HideLabel, TextArea(2, 10), GUIColor(1f, 1f, 0)]
        private string note;

#if ENABLE_INPUT_SYSTEM
    [BoxGroup("Events"), SerializeField]
    private UnityEngine.InputSystem.InputAction toggleKey;
#else
        [BoxGroup("Events"), SerializeField]
        private KeyCode toggleKey;
#endif

        [BoxGroup("Events"), SerializeField]
        private Button toggleButton;

        [BoxGroup("Events"), SerializeField]
        private AtomEventBase toggleEvent;

        [SerializeField] private bool showOnStart = false;
        [SerializeField] private GameObject[] targets;

        [SerializeField] [Min(1)]
        private int count = 1;

        [SerializeField] [Min(0)]
        private float timeout = 0.5f;

        private float _lastTap;
        private float _count;
        private bool _isOn;

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
        if (toggleKey != null)
        {
            toggleKey.Enable();
            toggleKey.performed += OnActionPerformed;
        }
#endif

            if (toggleButton != null)
                toggleButton.onClick.AddListener(OnButtonClicked);

            if (toggleEvent != null)
                toggleEvent.OnEventNoValue += OnEventRaised;
        }


#if ENABLE_INPUT_SYSTEM
    private void OnActionPerformed(InputAction.CallbackContext ctx)
    {
        if (toggleKey.triggered)
        {
            OnTriggered();
        }
    }
#endif

        private void OnButtonClicked() => OnTriggered();

        private void OnEventRaised() => OnTriggered();

        private void OnTriggered()
        {
            RecordTap();
            if (_count >= count)
                Show(!_isOn);
        }

        private void RecordTap()
        {
            if (_isOn)
            {
                _count = count;
            }
            else
            {
                if (_lastTap + timeout < Time.time)
                    _count = 0;
                _lastTap = Time.time;
                _count++;
            }
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
        if (toggleKey != null)
            toggleKey.performed -= OnActionPerformed;
#endif

            if (toggleButton != null)
                toggleButton.onClick.RemoveListener(OnButtonClicked);
            if (toggleEvent != null)
                toggleEvent.OnEventNoValue -= OnEventRaised;
        }

#if !ENABLE_INPUT_SYSTEM
        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                OnTriggered();
        }
#endif

        private void Start() => Show(showOnStart);

        private void Show(bool isOn)
        {
            Debug.Log($"{nameof(ToggleActiveAdvanced)} : {name} isOn = {isOn}");
            _isOn = isOn;
            foreach (var target in targets)
                target.SetActive(isOn);
        }

        public void Off() => Show(false);
        public void On() => Show(true);
    }
}