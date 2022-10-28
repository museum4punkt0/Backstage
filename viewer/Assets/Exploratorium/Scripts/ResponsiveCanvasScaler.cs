using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(RectTransform))]
    [InfoBox("Adjusts a CanvasScaler based on screen orientation", InfoMessageType.None)]
    public class ResponsiveCanvasScaler : MonoBehaviour
    {
        [Min(0)]
        [SerializeField] private float scale = 1.0f;

        [SerializeField]
        private Vector2 referenceResolutionPortrait = new Vector2(1080, 1920);

        [SerializeField]
        private Vector2 referenceResolutionLandscape = new Vector2(1920, 1080);

        private CanvasScaler _scaler;

        private void Awake() => _scaler = GetComponent<CanvasScaler>();
        private void Reset() => _scaler = GetComponent<CanvasScaler>();
        private void OnValidate() => _scaler = GetComponent<CanvasScaler>();

        private void Update()
        {
            if (_scaler == null)
                return;

            bool isPortrait = Screen.width < Screen.height;
            _scaler.referenceResolution = isPortrait ? referenceResolutionPortrait : referenceResolutionLandscape;
            _scaler.scaleFactor = scale;
        }
    }
}