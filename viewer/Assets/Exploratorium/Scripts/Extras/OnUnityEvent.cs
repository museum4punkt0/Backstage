using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Exploratorium.Extras
{
    public class OnUnityEvent : MonoBehaviour
    {
        public UnityEvent onEnable;
        public UnityEvent onStart;
        public UnityEvent onUpdate;
        public UnityEvent onDisable;
    
        [FoldoutGroup("Description", true)]
        [TextArea(2, 10)]
        [SerializeField]
        [HideLabel]
        private string description;
    
        void Start()
        {
            onStart.Invoke();
        }

        void Update()
        {
            onUpdate.Invoke();
        }

        void OnEnable()
        {
            onEnable.Invoke();
        }

        void OnDisable()
        {
            onDisable.Invoke();
        }
    }
}