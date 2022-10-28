using System;
using DG.Tweening;
using Directus.Connect.v9;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium
{
    public class ProgressBar : MonoBehaviour
    {
        [CanBeNull] [SerializeField]
        private Image componentProgress;

        [CanBeNull] [SerializeField]
        private TMP_Text componentMessage;

        [CanBeNull] [SerializeField]
        private TMP_Text componentBytes;

        [CanBeNull] [SerializeField]
        private Image overallProgress;

        [CanBeNull] [SerializeField]
        private TMP_Text overallMessage;

        [CanBeNull] [SerializeField]
        private TMP_Text overallBytes;

        [CanBeNull] [SerializeField]
        private CanvasGroup canvasGroup;

        [CanBeNull] [SerializeField]
        private Image working;

        private Tween _isWorkingAnimation;

        private void Awake()
        {
            Debug.Assert(componentProgress != null, $"{nameof(componentProgress)} property is unassigned");
            Debug.Assert(componentMessage != null, $"{nameof(componentMessage)} property is unassigned");
            Debug.Assert(componentBytes != null, $"{nameof(componentBytes)} property is unassigned");
            Debug.Assert(overallProgress != null, $"{nameof(overallProgress)} property is unassigned");
            Debug.Assert(overallMessage != null, $"{nameof(overallMessage)} property is unassigned");
            Debug.Assert(overallBytes != null, $"{nameof(overallBytes)} property is unassigned");
            Debug.Assert(working != null, $"{nameof(working)} property is unassigned");
        }

        private void Start()
        {
            ClearProgressBar();
        }

        public void OnProgressChangedAsync(ProgressInfo e)
        {
            switch (e.Scope)
            {
                case ProgressScope.Overall:
                    if (overallProgress != null)
                        overallProgress.fillAmount = e.Total > 0 ? e.Completed / (float)e.Total : 0.0f;
                    if (overallMessage != null)
                        overallMessage.text = $"{e.Message}";
                    if (overallBytes != null)
                        overallBytes.SetText("{0:0}/{1:0}", e.Completed, e.Total);
                    break;
                case ProgressScope.Undefined:
                case ProgressScope.Component:
                    if (componentProgress != null)
                        componentProgress.fillAmount = e.Total > 0 ? e.Completed / (float)e.Total : 0.0f;
                    if (componentMessage != null)
                        componentMessage.text = $"{e.Message}";
                    if (componentBytes != null)
                        componentBytes.SetText("{0:0}/{1:0}", e.Completed, e.Total);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InitProgressBar()
        {
            ClearProgressBar();
            if (canvasGroup)
                canvasGroup.alpha = 1.0f;
            if (working)
            {
                _isWorkingAnimation = DOTween.Sequence()
                        .Insert(atPosition: .5f, t: DOTween.To(
                            getter: () => working.rectTransform.anchorMin.x,
                            setter: x =>
                                working.rectTransform.anchorMin = new Vector2(x, working.rectTransform.anchorMin.y),
                            endValue: 1f,
                            duration: 2f).SetEase(Ease.Linear).ChangeStartValue(0))
                        .Insert(atPosition: 0, t: DOTween.To(
                            getter: () => working.rectTransform.anchorMax.x,
                            setter: x =>
                                working.rectTransform.anchorMax = new Vector2(x, working.rectTransform.anchorMax.y),
                            endValue: 1f,
                            duration: 2f).SetEase(Ease.Linear).ChangeStartValue(0))
                        .SetLoops(-1, LoopType.Restart)
                        .Play()
                    ;
            }
        }

        public void ClearProgressBar()
        {
            if (canvasGroup)
                canvasGroup.alpha = 0;
            _isWorkingAnimation?.Kill();
            OnProgressChangedAsync(new ProgressInfo
            {
                Message = "", Total = 0, Completed = 0, Scope = ProgressScope.Component
            });
            OnProgressChangedAsync(new ProgressInfo
            {
                Message = "", Total = 0, Completed = 0, Scope = ProgressScope.Overall
            });
        }
    }
}