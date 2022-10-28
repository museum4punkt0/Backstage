using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exploratorium
{
    [InfoBox(
        "Allows rotation of the screen resolution between portrait and landscape mode to test and debug other responsive components",
        InfoMessageType.None)]
    public class ResponsiveScreenControl : MonoBehaviour
    {
        private static ResponsiveScreenControl _singleton;

        [FormerlySerializedAs("fullscreen")] [SerializeField]
        private VoidEvent resetToNative;

        [FormerlySerializedAs("toggleOrientation")] [SerializeField]
        private VoidEvent rotateScreen;

        [SerializeField] private FullScreenMode fullscreenMode = FullScreenMode.FullScreenWindow;

        private int _count;

        [ShowInInspector]
        private float aspect = 0.5625f;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void Init()
        {
            _singleton = null;
        }
#endif

        private void Awake()
        {
            if (_singleton == null)
            {
                _singleton = this;
            }
            else
            {
                Debug.Log($"{nameof(ResponsiveScreenControl)} : Destroying duplicate instance");
                Destroy(this);
                return;
            }

            Debug.Assert(resetToNative != null, "resetToNative != null");
            Debug.Assert(rotateScreen != null, "rotateScreen != null");
        }

        private void OnEnable()
        {
            resetToNative.Register(OnResetToNative);
            rotateScreen.Register(OnRotate);
        }

        private void OnDisable()
        {
            resetToNative.Unregister(OnResetToNative);
            rotateScreen.Unregister(OnRotate);
        }

        private void OnDestroy()
        {
            if (_singleton == this)
                _singleton = null;
        }

        [Button("Rotate Screen")]
        private void OnRotate()
        {
            _count++;
            if (_count % 2 == 1)
            {
                // portrait
                Screen.SetResolution(
                    width: (int)(Display.main.systemHeight * aspect),
                    height: Display.main.systemHeight,
                    fullscreenMode: fullscreenMode
                );
            }
            else
            {
                // landscape
                Screen.SetResolution(
                    width: Display.main.systemWidth,
                    height: (int)(Display.main.systemWidth * aspect),
                    fullscreenMode: fullscreenMode);
            }

            Debug.Log(
                $"{nameof(ResponsiveScreenControl)} : Rotated to {Screen.currentResolution.width}x{Screen.currentResolution.height}");
        }

        [Button("Reset to Native Resolution")]
        private void OnResetToNative()
        {
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight,
                FullScreenMode.FullScreenWindow);
        }
    }
}