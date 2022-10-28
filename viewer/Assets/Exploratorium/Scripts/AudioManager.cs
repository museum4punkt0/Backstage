using System;
using Exploratorium.Net.Shared;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Extensions;
using UnityEngine;

namespace Exploratorium
{
    public class AudioManager : MonoBehaviour
    {
        [InfoBox(
            "The value range is expected to be either [0, 1] or [0, 100], values outside these ranges will be clamped.")]
        [SerializeField, BoxGroup(Constants.ReadVariables)]
        private FloatVariable volumeVariable;

        [SerializeField, BoxGroup(Constants.ReadVariables)]
        private PawnRoleVariable roleVariable;

        [SerializeField, BoxGroup(Constants.ObservedEvents)]
        private VoidEvent resetEvent;

        [SerializeField]
        [Min(0)]
        private int defaultVolume = 30;

        private void Awake()
        {
            Debug.Assert(volumeVariable != null, "volumeVariable != null", this);
            Debug.Assert(roleVariable != null, "roleVariable != null", this);
            Debug.Assert(resetEvent != null, "resetEvent != null", this);
        }

        private void OnEnable()
        {
            if (volumeVariable != null && volumeVariable.Changed != null)
            {
                volumeVariable.Changed.Register(OnVolumeChanged);
                OnVolumeChanged(volumeVariable.Value);
            }

            if (roleVariable != null && roleVariable.Changed != null)
            {
                roleVariable.Changed.Register(OnRoleChanged);
                OnRoleChanged(roleVariable.Value);
            }

            if (roleVariable != null)
                resetEvent.Register(OnReset);
            OnVolumeChanged(volumeVariable.Value);
        }

        private void OnReset()
        {
            if (volumeVariable != null)
                volumeVariable.Value = defaultVolume;
        }

        private void OnDisable()
        {
            if (volumeVariable != null && volumeVariable.Changed != null)
                volumeVariable.Changed.Unregister(OnVolumeChanged);
            if (roleVariable != null && roleVariable.Changed != null)
                roleVariable.Changed.Unregister(OnRoleChanged);
            if (roleVariable != null)
                resetEvent.Unregister(OnReset);
        }

        private void OnRoleChanged(PawnRole role)
        {
            if (volumeVariable != null)
                OnVolumeChanged(volumeVariable.Value);
        }

        private void OnVolumeChanged(float value)
        {
            switch (roleVariable.Value)
            {
                case PawnRole.None:
                case PawnRole.Observer:
                case PawnRole.Solo:
                    // OBSERVER volume will be driven by the controller (net sync)
                    // SOLO volume is controlled locally
                    AudioListener.volume = Mathf.Clamp01(value > 1.0f ? value / 100f : value);
                    break;
                case PawnRole.Controller:
                    // the CONTROLLER is always silent and drives the volume of all observers (net sync)
                    AudioListener.volume = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.Log($"{nameof(AudioListener)} : Volume is set to {AudioListener.volume * 100f:F2}%");
        }
    }
}