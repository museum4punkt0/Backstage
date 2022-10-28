using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium.Extras
{
    public class ContrastColor : MonoBehaviour
    {
        [SerializeField]
        [TextArea(1,10)]
        private string description;
        
        [SerializeField]
        private Graphic source;

        [SerializeField]
        private Graphic target;

        private bool _dirty;

        private void OnEnable()
        {
            _dirty = true;
        }

        private void LateUpdate()
        {
            if (_dirty && source) 
                target.color = source.color.ContrastColor();
            _dirty = false;
        }
    }
}