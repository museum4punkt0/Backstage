using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Exploratorium.Frontend
{
    public class OpenableText : Openable
    {
        [BoxGroup("Graphics")] [SerializeField]
        private TMP_Text[] texts;

        [SerializeField, BoxGroup("Animation"), Min(1f)]
        private float fadeCharactersPerSecond = 500f;

        [SerializeField, BoxGroup("Animation"), Min(0)]
        private float durationPerCharacter = .5f;

        [SerializeField, BoxGroup("Animation"), Min(0)]
        private float incrementalDelay = 0f;

        [SerializeField, BoxGroup("Animation")]
        private bool useCloseSequence;

        [SerializeField, BoxGroup("Animation"), ShowIf(nameof(useCloseSequence))]
        private bool reverseOutSequence = true;

        [SerializeField] private UnityEvent onOpening;

        [SerializeField] private UnityEvent onClosed;

        //private DOTweenTMPAnimator[] _animators;


        private Sequence _openSq;
        private Sequence _closeSq;
        private float _durationPerCharacter;
        private float _incrementalDelay;

        /// <summary>
        /// After setting this, call <see cref="Openable.RebuildTweens"/>
        /// </summary>
        public float DurationPerCharacter
        {
            get => _durationPerCharacter;
            set => _durationPerCharacter = Mathf.Max(0, value);
        }

        /// <summary>
        /// After setting this, call <see cref="Openable.RebuildTweens"/>
        /// </summary>
        public float IncrementalDelay
        {
            get => _incrementalDelay;
            set => _incrementalDelay = Mathf.Max(0, value);
        }

        protected override void Awake()
        {
            _durationPerCharacter = durationPerCharacter;
            _incrementalDelay = incrementalDelay;
            //_animators = new DOTweenTMPAnimator[texts.Length];
            /*for (int i = 0; i < texts.Length; i++)
                _animators[i] = new DOTweenTMPAnimator(texts[i]);*/
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DOTween.Kill(this);
            foreach (var text in texts)
                DOTween.Kill(text);
        }


        protected override async UniTaskVoid OnCloseAsync()
        {
            if (!_openSq.IsComplete())
            {
                if (debug)
                    Debug.Log($"{nameof(OpenableText)} : Completing open-sequence on {name}", this);
                _openSq.Complete();
            }

            if (debug)
                Debug.Log($"{nameof(OpenableText)} : Restarting close-sequence on {name}", this);
            _closeSq.Restart();
            await UniTask.CompletedTask;
        }

        protected override async UniTaskVoid OnCycleAsync()
        {
            if (!_closeSq.IsComplete())
            {
                if (debug)
                    Debug.Log($"{nameof(OpenableGroup)} : Completing close-sequence on {name}", this);
                _closeSq.Complete();
            }

            if (!_openSq.IsComplete())
            {
                if (debug)
                    Debug.Log($"{nameof(OpenableGroup)} : Completing open-sequence on {name}", this);
                _closeSq.Complete();
            }

            if (debug)
                Debug.Log($"{nameof(OpenableGroup)} : Cycling open-sequence on {name}", this);

            if (IsOpen)
            {
                DOTween.Sequence()
                    .Append(_closeSq)
                    .Append(_openSq)
                    .Restart();
            }
            else
            {
                // we only play the open sequence when currently in a closed state
                DOTween.Sequence()
                    .Append(_openSq)
                    .Restart();
            }

            await UniTask.CompletedTask;
        }

        protected override async UniTaskVoid OnOpenAsync()
        {
            if (!_closeSq.IsComplete())
            {
                if (debug)
                    Debug.Log($"{nameof(OpenableText)} : Completing close-sequence on {name}", this);
                _closeSq.Complete();
            }

            if (debug)
                Debug.Log($"{nameof(OpenableText)} : Restarting open-sequence on {name}", this);
            _openSq.Restart();
            await UniTask.CompletedTask;
        }

        protected override void OnRebuildTweens()
        {
            _closeSq = DOTween.Sequence(this).SetRecyclable(true).SetAutoKill(false).Pause();
            _closeSq
                //.AppendCallback(() => Debug.Log($"{nameof(OpenableText)} : Closing {name} (delayed by {CloseDelay}s)", this))
                .AppendInterval(CloseDelay)
                //.AppendCallback(() => Debug.Log($"{nameof(OpenableGroup)} : Closing {name} (delay elapsed)", this))
                ;
            _openSq = DOTween.Sequence(this).SetRecyclable(true).SetAutoKill(false).Pause();
            _openSq
                //.AppendCallback(() => Debug.Log($"{nameof(OpenableText)} : Opening {name} (delayed by {CloseDelay}s)", this))
                .AppendInterval(OpenDelay)
                //.AppendCallback(() => Debug.Log($"{nameof(OpenableText)} : Opening {name} (delay elapsed)", this))
                .AppendCallback(onOpening.Invoke);


            for (var i = 0; i < texts.Length; i++)
            {
                DOTween.Kill(texts[i], true);
                _openSq.Join(
                    DOTween.Sequence(texts[i])
                        .AppendInterval(IncrementalDelay * i)
                        .Append(texts[i].DOFade(1f, durationPerCharacter * texts[i].text.Length))
                );
                if (useCloseSequence)
                {
                    var delay = reverseOutSequence
                        ? IncrementalDelay * (texts.Length - i - 1)
                        : IncrementalDelay * i;
                    _closeSq.Join(
                        DOTween.Sequence(texts[i])
                            .AppendInterval(delay)
                            .Append(texts[i].DOFade(0f, durationPerCharacter * texts[i].text.Length))
                    );
                }
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

            Debug.Log(
                $"{nameof(OpenableGroup)} : Finished initializing {name} to {(IsOpen ? "OPEN" : "CLOSED")}-state after rebuild",
                this);

            Debug.Log(
                $"{nameof(OpenableText)} : Tweens created for {name}; {DOTween.TotalActiveTweens()}/{DOTween.TotalPlayingTweens()}",
                this);
        }

        /*protected override void OnRebuildTweens()
        {
            
            _closeSq = DOTween.Sequence(this).SetRecyclable(true).SetAutoKill(false).Pause();
            _closeSq
                .AppendCallback(
                    () => Debug.Log($"{nameof(OpenableText)} : Closing {name} (delayed by {CloseDelay}s)", this))
                .AppendInterval(CloseDelay)
                .AppendCallback(() => Debug.Log($"{nameof(OpenableGroup)} : Closing {name} (delay elapsed)", this));
            _openSq = DOTween.Sequence(this).SetRecyclable(true).SetAutoKill(false).Pause();
            _openSq
                .AppendCallback(
                    () => Debug.Log($"{nameof(OpenableText)} : Opening {name} (delayed by {CloseDelay}s)", this))
                .AppendInterval(OpenDelay)
                .AppendCallback(() => Debug.Log($"{nameof(OpenableText)} : Opening {name} (delay elapsed)", this))
                .AppendCallback(onOpening.Invoke);

            for (var i = 0; i < _animators.Length; i++)
            {
                // we run the fade sequences as callback so the fade-sequences do not get created immediately based on incorrect texts
                DOTween.Kill(_animators[i], true);
                DOTween.Kill(texts[i], true);
                _openSq.Join(
                    DOTween.Sequence(texts[i])
                        .AppendInterval(IncrementalDelay * i)
                        .Append(FadeInText(ref _animators[i]))
                );
                if (useCloseSequence)
                {
                    var delay = reverseOutSequence
                        ? IncrementalDelay * (_animators.Length - i - 1)
                        : IncrementalDelay * i;
                    _closeSq.Join(
                        DOTween.Sequence(texts[i])
                            .AppendInterval(delay)
                            .Append(FadeOutText(ref _animators[i]))
                    );
                }
            }

            _closeSq.AppendCallback(onClosed.Invoke);

            Debug.Log($"{nameof(OpenableGroup)} : Starting to initialize {name} to {(IsOpen ? "OPEN" : "CLOSED")}-state after rebuild", this);
            if (IsOpen)
                _openSq.Complete(true);
            else
                _closeSq.Complete(true);
            Debug.Log($"{nameof(OpenableGroup)} : Finished initializing {name} to {(IsOpen ? "OPEN" : "CLOSED")}-state after rebuild", this);

            Debug.Log(
                $"{nameof(OpenableText)} : Tweens created for {name}; {DOTween.TotalActiveTweens()}/{DOTween.TotalPlayingTweens()}",
                this);
        }*/
        [NotNull]
        private Sequence FadeInText([NotNull] ref DOTweenTMPAnimator animator)
        {
            animator.Refresh();
            float interval = 1.0f / fadeCharactersPerSecond;
            Sequence sq = DOTween.Sequence().SetTarget(animator).SetAutoKill(true).SetRecyclable(false);
            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                if (!animator.textInfo.characterInfo[i].isVisible) continue;
                sq.Insert(i * interval, animator.DOFadeChar(i, 1f, _durationPerCharacter).From(0));
            }

            return sq;
        }

        [NotNull]
        private Sequence FadeOutText([NotNull] ref DOTweenTMPAnimator animator)
        {
            animator.Refresh(); // update the animator in case the text has changed for some reason
            float interval = 1.0f / fadeCharactersPerSecond;
            var sq = DOTween.Sequence().SetTarget(animator).SetAutoKill(true).SetRecyclable(false);
            var totalDuration = animator.textInfo.characterCount * interval;
            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                if (!animator.textInfo.characterInfo[i].isVisible) continue;
                sq.Insert(totalDuration - i * interval, animator.DOFadeChar(i, 0, _durationPerCharacter).From(1f));
            }

            return sq;
        }
    }
}