using System;
using System.Linq;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using RenderHeads.Media.AVProVideo;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Exploratorium.Frontend
{
    public class InactivityTimeout : MonoBehaviour
    {
        [SerializeField]
        [InfoBox("The timeout will be suspended while any of the specified media players is playing",
            InfoMessageType.None)]
        private MediaPlayer[] mediaPlayers;

        [BoxGroup(Constants.ObservedEvents)]
        [SerializeField] private VoidEvent resetTimeout;

        [SerializeField] private float defaultDuration;
        private float _duration;

        [SerializeField]
        private UnityEvent onElapsed;

        [SerializeField]
        private bool invokeContinuously;

        private bool _invokeConsumed;

        [ShowInInspector]
        [ReadOnly]
        private float _timeout;

        [SerializeField] private bool debug;

        public event Action Elapsed;

        public float Timeout => _timeout;

        public float Duration => _duration;

        private void Awake()
        {
            Debug.Assert(!debug, "!debug");
            Debug.Assert(resetTimeout != null, "resetTimeout != null");
            Debug.Assert(mediaPlayers.Any(), "mediaPlayers.Any()");
        }

        private void OnEnable()
        {
            Debug.Assert(
                DirectusManager.Instance != null &&
                DirectusManager.Instance.Connector != null &&
                DirectusManager.Instance.Connector.Model is { IsReady: true },
                $"{nameof(InactivityTimeout)} : Directus initialization and model acquisition should have happened long before this object is enabled."
            );

#if UNITY_EDITOR
            // fix exceptions thrown in the editor on domain reload
            if (DirectusManager.Instance == null)
                return;
            if (DirectusManager.Instance.Connector == null)
                return;
#endif

            SettingsRecord settings = DirectusManager.Instance
                .Connector
                .Model
                .GetItemsOfType<SettingsRecord>()
                .FirstOrDefault();
            if (settings == default)
            {
                Debug.LogWarning($"{nameof(InactivityTimeout)} : Invalid settings, using default timeout.", this);
                _duration = defaultDuration;
            }
            else
            {
                _duration = settings.ResetTimer;
                Debug.Log($"{nameof(InactivityTimeout)} : Reset timer is set to {_timeout} seconds.", this);
            }

            if (resetTimeout != null)
                resetTimeout.Register(OnResetTimeout);
            
            PushTimeout();
        }

        private void OnResetTimeout()
        {
            if (debug)
                Debug.Log($"{nameof(InactivityTimeout)} : Timeout reset");
            PushTimeout();
        }

        private void OnDisable()
        {
            if (resetTimeout != null)
                resetTimeout.Unregister(PushTimeout);        }

        private void PushTimeout() => _timeout = Time.time + _duration;

        private void Update()
        {
            bool hasInput = Input.touchCount > 0 ||
                            Input.anyKey ||
                            (Input.mousePresent && Input.GetMouseButton(0));
            bool isPlayingMedia = false;
            if (!hasInput)
            {
                foreach (var mediaPlayer in mediaPlayers)
                {
                    if (mediaPlayer.Control.IsPlaying() &&
                        mediaPlayer.Control.GetCurrentTime() < mediaPlayer.Info.GetDuration() * 0.9f)
                    {
                        isPlayingMedia = true;
                        break;
                    }
                }
            }

            if (hasInput || isPlayingMedia)
            {
                PushTimeout();
                _invokeConsumed = false;
                //Debug.Log($"{nameof(InactivityTimeout)} : Reset timer is set to {_timeout} seconds.", this);
            }

            var shouldInvoke = _timeout < Time.time;
            if (shouldInvoke)
            {
                if (!_invokeConsumed)
                {
                    Elapsed?.Invoke();
                    onElapsed.Invoke();
                    _invokeConsumed = !invokeContinuously;
                }
            }
        }
    }
}