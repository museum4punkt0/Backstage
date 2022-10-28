using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium.Extras
{
    public class MatchColor : MonoBehaviour
    {
        [SerializeField]
        [TextArea(1,10)]
        private string description;
        
        [SerializeField]
        private Graphic source;

        [SerializeField]
        private Graphic target;

        private bool _dirty;

        private void OnEnable() => _dirty = true;

        private void LateUpdate()
        {
            if (_dirty)
                target.color = source.color;
            _dirty = false;
        }
    }
}