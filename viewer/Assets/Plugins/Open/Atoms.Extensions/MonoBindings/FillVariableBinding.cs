using UnityAtoms;
using UnityAtoms.MonoHooks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAtoms.MonoBindings
{
    /// <summary>
    /// Mono Hook for On Button Click
    /// </summary>
    [EditorIcon("atom-icon-delicate")]
    [RequireComponent(typeof(Image))]
    public sealed class FillVariableBinding : FloatBinding
    {
        private Image _image;

        private void OnEnable()
        {
            if (!TryGetComponent(out _image))
                return;

            _image.type = Image.Type.Filled;
            OnVariableChanged(variable.Value);
        }

        protected override void OnVariableChanged(float value)
        {
            if (!_image)
                return;
            _image.fillAmount = Mathf.Clamp01(value);
        }
    }
}