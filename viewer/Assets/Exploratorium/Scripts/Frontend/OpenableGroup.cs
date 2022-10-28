using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Exploratorium.Frontend
{
    public class OpenableGroup : Openable
    {
        [BoxGroup("Animation"), SerializeField]
        private CanvasGroup fadeGroup;

        [BoxGroup("Animation"), SerializeField]
        private RectTransform moveGroup;

        [BoxGroup("Animation"), SerializeField]
        private CanvasGroup enableGroup;

        [FormerlySerializedAs("moveOnClose")] [BoxGroup("Animation"), SerializeField]
        private Vector3 moveTo = new Vector3(0, 100, 0);

        [FormerlySerializedAs("moveOnOpen")] [BoxGroup("Animation"), SerializeField]
        private Vector3 moveFrom = new Vector3(0, 0, 0);

        [FormerlySerializedAs("scaleOnClose")] [BoxGroup("Animation"), SerializeField]
        private Vector3 scaleTo = new Vector3(1f, 1f, 1f);

        [FormerlySerializedAs("scaleOnOpen")] [BoxGroup("Animation"), SerializeField]
        private Vector3 scaleFrom = new Vector3(1f, 1f, 1f);

        [MinValue(0), MaxValue(1f)] [BoxGroup("Animation"), SerializeField]
        private float alphaClosed = 0f;

        [MinValue(0), MaxValue(1f)] [BoxGroup("Animation"), SerializeField]
        private float alphaOpen = 1f;

        [BoxGroup("Animation"), SerializeField]
        private Ease easeTo = Ease.InQuad;

        [BoxGroup("Animation"), SerializeField]
        private Ease easeFrom = Ease.OutQuad;

        [BoxGroup("State")] [SerializeField]
        private bool interactableWhileOpen = true;

        [BoxGroup("State")] [SerializeField]
        private bool interactableWhileClosed = false;

        [BoxGroup("Fixes")]
        [InfoBox("This should be enabled on components that are not driven by a parent layout component. When " +
                 "true, a closed component will have its position reset to its open position before tweens are rebuilt. " +
                 "This fixes issues where the object position is incremented on consecutive rebuilds.")]
        [SerializeField]
        private bool resetPositionBeforeRebuild = false;

        [SerializeField] private UnityEvent onOpening;

        [SerializeField] private UnityEvent onClosed;


        private Sequence _openSq;
        private Sequence _closeSq;

        private Vector3 _originalPosition;

        private const string CycleTween = "CyleSq";
        // private Vector3 _moveTo;
        // private Vector3 _moveFrom;
        // private Vector3 _scaleFrom;
        // private Vector3 _scaleTo;

        /// <summary>
        /// After setting this, call <see cref="Openable.RebuildTweens"/>
        /// </summary>
        public Vector3 ScaleTo
        {
            get => scaleTo;
            set => scaleTo = value;
        }

        /// <summary>
        /// After setting this, call <see cref="Openable.RebuildTweens"/>
        /// </summary>
        public Vector3 ScaleFrom
        {
            get => scaleFrom;
            set => scaleFrom = value;
        }

        /// <summary>
        /// After setting this, call <see cref="Openable.RebuildTweens"/>
        /// </summary>
        public Vector3 MoveTo
        {
            get => moveTo;
            set => moveTo = value;
        }

        /// <summary>
        /// After setting this, call <see cref="Openable.RebuildTweens"/>
        /// </summary>
        public Vector3 MoveFrom
        {
            get => moveFrom;
            set => moveFrom = value;
        }

        protected override void Awake()
        {
            base.Awake();
            //_moveTo = moveTo;
            //_moveFrom = moveFrom;
            //_scaleTo = scaleTo;
            //_scaleFrom = scaleFrom;
            _originalPosition = transform.localPosition;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            if (fadeGroup == null)
                fadeGroup = GetComponent<CanvasGroup>();
            if (enableGroup == null)
                enableGroup = GetComponent<CanvasGroup>();
            if (moveGroup == null)
                moveGroup = GetComponent<RectTransform>();
        }
#endif

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // prevent playmode exit exceptions
            _openSq.Kill();
            _closeSq.Kill();
            /*
             if (moveGroup != null)
                DOTween.Kill(moveGroup);
            if (enableGroup != null)
                DOTween.Kill(enableGroup);
            if (fadeGroup != null)
                DOTween.Kill(fadeGroup);
            */
        }


        protected override async UniTaskVoid OnCloseAsync()
        {
            // These tweens should be recyclable, why do they killed?
            if (!_openSq.IsActive())
                RebuildTweensImmediately();

            if (!_openSq.IsComplete())
            {
                if (debug)
                    Debug.Log($"{nameof(OpenableGroup)} : Completing open-sequence on {name}", this);
                _openSq.Complete();
            }

            if (debug)
                Debug.Log($"{nameof(OpenableGroup)} : Restarting close-sequence on {name}", this);
            _closeSq.Restart();
            await UniTask.CompletedTask;
        }

        protected override async UniTaskVoid OnOpenAsync()
        {
            // These tweens should be recyclable, why do they killed?
            if (!_closeSq.IsActive())
                RebuildTweensImmediately();

            if (!_closeSq.IsComplete())
            {
                if (debug)
                    Debug.Log($"{nameof(OpenableGroup)} : Completing close-sequence on {name}", this);
                _closeSq.Complete();
            }

            if (debug)
                Debug.Log(
                    $"{nameof(OpenableGroup)} : Restarting open-sequence on {name}, this will be count {OpenCount}",
                    this);
            _openSq.Restart();
            await UniTask.CompletedTask;
        }

        protected override async UniTaskVoid OnCycleAsync()
        {
            if (debug)
                Debug.Log($"{nameof(OpenableGroup)} : Cycling open-sequences on {name}", this);

            // These tweens should be recyclable, why do they get killed?
            if (!_openSq.IsActive() || !_openSq.IsActive())
                OnRebuildTweens();

            if (IsOpen)
            {
                // - using callbacks here because a sequence can't be nested into multiple other sequences
                DOTween.Sequence()
                    // ordering of these two .Restart() calls is important to avoid a "back-frame" glitch caused by
                    // the sequences .From() values overriding each other.
                    .AppendCallback(() => _openSq.SetTarget(this).SetId(CycleTween).Restart())
                    .AppendCallback(() => _closeSq.SetTarget(this).SetId(CycleTween).Restart())
                    ;
            }
            else
            {
                // - we play the regular open sequence when currently in a closed state
                // - using callbacks here because a sequence can't be nested into multiple other sequences
                DOTween.Sequence()
                    .AppendCallback(() => _openSq.SetTarget(this).SetId(CycleTween).Restart());
            }

            await UniTask.CompletedTask;
        }


        protected override void OnRebuildTweens()
        {
            if (!IsOpen && IsReady && resetPositionBeforeRebuild)
                transform.localPosition = _originalPosition;

            if (debug)
            {
                Debug.Log(
                    $"{nameof(OpenableGroup)} : Rebuilding tweens for {name} with open-delay = {OpenDelay:F2}s, close-delay = {CloseDelay:F2}s and duration = {Duration:F2}s",
                    this);
            }

            _closeSq = DOTween.Sequence(this)
                .SetRecyclable(true)
                .SetAutoKill(false)
                .SetTarget(this)
                //.OnKill(() => Debug.Log($"close sq on {name} killed"))
                //.OnComplete(() => Debug.Log($"close sq complete on {name}"))
                .Pause();

            if (debug)
            {
                _closeSq.AppendCallback(() =>
                    Debug.Log($"{nameof(OpenableGroup)} : Closing {name} with a delay of {CloseDelay:F2}s", this));
            }

            _closeSq.AppendInterval(CloseDelay);
            if (debug)
            {
                _closeSq.AppendCallback(() =>
                    Debug.Log($"{nameof(OpenableGroup)} : Closing {name} after delay has elapsed", this));
            }

            _openSq = DOTween.Sequence(this)
                .SetRecyclable(true)
                .SetAutoKill(false)
                .SetTarget(this)
                //.OnKill((() => Debug.Log($"open sq on {name} killed")))
                //.OnComplete((() => Debug.Log($"open sq complete on {name}")))
                .Pause();

            if (debug)
            {
                _openSq.AppendCallback(() =>
                    Debug.Log($"{nameof(OpenableGroup)} : Opening {name} with a delay of {OpenDelay:F2}s", this));
            }

            _openSq.AppendInterval(OpenDelay);
            if (debug)
            {
                _openSq.AppendCallback(() =>
                    Debug.Log($"{nameof(OpenableGroup)} : Opening {name} after delay has elapsed", this));
            }

            _openSq.AppendCallback(onOpening.Invoke);

            if (enableGroup != null)
            {
                _closeSq.Join(DOTween.Sequence(enableGroup)
                    .SetRecyclable(true)
                    //.AppendCallback(() => Debug.Log($"{nameof(OpenableGroup)} : Disabling interactivity of {enableGroup.name}", this))
                    .AppendCallback(() => enableGroup.interactable = interactableWhileClosed)
                    .AppendCallback(() => enableGroup.blocksRaycasts = interactableWhileClosed)
                );

                _openSq.Join(
                    DOTween.Sequence(enableGroup)
                        .SetRecyclable(true)
                        //.AppendCallback(() => Debug.Log($"{nameof(OpenableGroup)} : Enabling interactivity of {enableGroup.name}", this))
                        .AppendCallback(() => enableGroup.interactable = interactableWhileOpen)
                        .AppendCallback(() => enableGroup.blocksRaycasts = interactableWhileOpen)
                );
            }

            if (moveGroup != null)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(OpenableGroup)} : Building a move tween for {moveGroup.name}, current pos is {moveGroup.localPosition} and offset is {moveTo};",
                        this);

                // IMPORTANT: movement has to be non-relative to avoid weird behaviour!
                _closeSq.Join(moveGroup.DOLocalMove(moveGroup.localPosition + moveTo, Duration)
                    .SetEase(easeTo)
                    .SetRelative(false)
                    //.OnComplete(() => Debug.Log($"{nameof(OpenableGroup)} : Moved out {moveGroup.name}", this))
                    .SetRecyclable(true)
                );
                _closeSq.Join(moveGroup.DOScale(scaleTo, Duration)
                    .From(Vector3.one, false, false)
                    .SetEase(easeTo)
                    //.OnComplete(() => Debug.Log($"{nameof(OpenableGroup)} : Scaled out {moveGroup.name}", this))
                    .SetRecyclable(true)
                );

                _openSq.Join(moveGroup.DOLocalMove(moveGroup.localPosition, Duration)
                    .From(moveGroup.localPosition + MoveFrom, false, false)
                    .SetEase(easeFrom)
                    //.OnComplete(() => Debug.Log($"{nameof(OpenableGroup)} : Moved in {moveGroup.name} from {_moveOffset}", this))
                    .SetRecyclable(true)
                );
                _openSq.Join(moveGroup.DOScale(Vector3.one, Duration)
                    .From(scaleFrom, false, false)
                    .SetEase(easeFrom)
                    //.OnComplete(() => Debug.Log($"{nameof(OpenableGroup)} : Scaled in {moveGroup.name}", this))
                    .SetRecyclable(true)
                );
            }

            if (fadeGroup != null)
            {
                _closeSq.Join(fadeGroup
                    .DOFade(alphaClosed, Duration)
                    .From(alphaOpen)
                    .SetEase(easeTo)
                    .OnComplete(() => Debug.Log($"{nameof(OpenableGroup)} : Faded out {fadeGroup.name}", this))
                    .SetRecyclable(true)
                );

                _openSq.Join(fadeGroup
                    .DOFade(alphaOpen, Duration)
                    .From(alphaClosed)
                    .SetEase(easeFrom)
                    .OnComplete(() => Debug.Log($"{nameof(OpenableGroup)} : Faded in {fadeGroup.name}", this))
                    .SetRecyclable(true)
                );
            }

            _closeSq.AppendCallback(onClosed.Invoke);

            if (debug)
                Debug.Log(
                    $"{nameof(OpenableGroup)} : Starting to initialize {name} to {(IsOpen ? "OPEN" : "CLOSED")}-state after rebuild",
                    this);
            if (IsOpen)
                _openSq.Complete(true);
            else
                _closeSq.Complete(true);

            if (debug)
                Debug.Log(
                    $"{nameof(OpenableGroup)} : Finished initializing {name} to {(IsOpen ? "OPEN" : "CLOSED")}-state after rebuild",
                    this);
            // _openCount = 0;  

            if (debug)
                Debug.Log(
                    $"{nameof(OpenableGroup)} : Tweens created for {name}; {DOTween.TotalActiveTweens()}/{DOTween.TotalPlayingTweens()}",
                    this);
        }
    }
}