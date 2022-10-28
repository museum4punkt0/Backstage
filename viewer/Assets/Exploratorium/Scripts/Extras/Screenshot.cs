using System;
using System.IO;
using UnityEngine;

namespace Exploratorium.Extras
{
    public class Screenshot : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
    [SerializeField]
    private UnityEngine.InputSystem.Key triggerKey = Key.F9;
#else
        [SerializeField]
        private KeyCode triggerKeyCode;
#endif


        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current[triggerKey].wasPressedThisFrame)
#else
            if (Input.GetKeyDown(triggerKeyCode))
#endif
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    $"{Application.productName}_{Application.version}_{DateTimeOffset.UtcNow:yyyy-MM-dd--HH-mm-ssT}.png");
                ScreenCapture.CaptureScreenshot(path);
                Debug.Log($"Screenshot saved to {path}");
            }
        }
    }
}