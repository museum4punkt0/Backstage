using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    [RequireComponent(typeof(Button))]
    public class ScrollbarButton : UIBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Vector2 normalizedIncrement;

        private Button _button;
        private bool _init;

        protected override void Awake() => EnsureInit();

        private void EnsureInit()
        {
            if (_init)
                return;
            _button = GetComponent<Button>();
            _init = true;
        }

        protected override void OnEnable()
        {
            scrollRect.onValueChanged.AddListener(OnChanged);
            _button.onClick.AddListener(OnClicked);
            OnChanged(scrollRect.normalizedPosition);
        }


        protected override void OnDisable()
        {
            scrollRect.onValueChanged.RemoveListener(OnChanged);
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked() => scrollRect.normalizedPosition += normalizedIncrement;

        private void OnChanged(Vector2 pos)
        {
            EnsureInit();
            _button.interactable = normalizedIncrement.x > 0 && scrollRect.normalizedPosition.x < .99f ||
                                   normalizedIncrement.x < 0 && scrollRect.normalizedPosition.x > .01f ||
                                   normalizedIncrement.y > 0 && scrollRect.normalizedPosition.y < .99f ||
                                   normalizedIncrement.y < 0 && scrollRect.normalizedPosition.y > .01f;
        }
    }
}