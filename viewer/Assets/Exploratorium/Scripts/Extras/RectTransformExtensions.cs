using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium.Extras
{
    public static class RectTransformExtensions
    {
        public static RectTransform CycleLayoutGroup(this RectTransform rectTransform)
        {
            CycleLayoutGroup(rectTransform.GetComponent<LayoutGroup>());
            return rectTransform;
        }

        /// <summary>
        /// This fixes a UI Layout glitch where some items would not be positioned correctly due to outdated layout
        /// data while procedurally assembling ui layouts. So far no other method has been found that fixes these
        /// glitches where LayoutRebuilder methods fail.
        /// </summary>
        public static LayoutGroup CycleLayoutGroup(LayoutGroup layoutGroup)
        {
            if (layoutGroup != null)
            {
                layoutGroup.enabled = !layoutGroup.enabled;
                layoutGroup.enabled = !layoutGroup.enabled;
            }

            return layoutGroup;
        }

        public static void CycleContentSizeFitter(this RectTransform rt)
        {
            CycleContentSizeFitter(rt.GetComponent<ContentSizeFitter>());
        }

        public static void CycleContentSizeFitter(this ContentSizeFitter fitter)
        {
            if (fitter != null)
            {
                fitter.enabled = !fitter.enabled;
                fitter.enabled = !fitter.enabled;
            }
        }
    }
}