using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Exploratorium.Frontend
{
    public class InactivityTimeoutPresenter : MonoBehaviour
    {
        [SerializeField] private InactivityTimeout inactivityTimeout;

        [SerializeField] private Image progressFill;

        [SerializeField] private TMP_Text progressText;

        [SerializeField] private CanvasGroup group;


        private void Awake()
        {
            Debug.Assert(inactivityTimeout != null, "inactivityTimeout != null", this);
            Debug.Assert(progressFill != null, "progressFill != null", this);
            Debug.Assert(progressText != null, "progressText != null", this);
            Debug.Assert(group != null, "group != null", this);
        }

        private void Update()
        {
            if (inactivityTimeout.enabled)
            {
                if (progressFill != null)
                {
                    progressFill.fillAmount =
                        Mathf.Clamp01((inactivityTimeout.Timeout - Time.time) / inactivityTimeout.Duration);
                }

                if (progressText != null)
                {
                    progressText.text = $"{Mathf.Max(0, inactivityTimeout.Timeout - Time.time):F0}";
                }

                if (@group != null)
                {
                    group.alpha = 1f;
                }
            }
            else
            {
                if (@group != null)
                {
                    group.alpha = 0f;
                }
            }
        }
    }
}