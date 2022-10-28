using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    /// <remarks>
    /// - This class assumes that all buttons are driven by a layout groups such that deactivated buttons will be collapsed towards the active button
    /// - mode of operation:
    ///    - By default only the active-button is shown
    ///    - click on the active-button hides it and shows a set of volume-level-buttons
    ///    - when a volume has been selected the volume-level-buttons are hidden and the active-button is shown again
    /// - The active-button changes its icon based on the current volume state
    /// - The volume-level-buttons contain a marker which indicates the current volume
    /// </remarks>
    public class VolumeControl : MonoBehaviour
    {
        private const int MediumVolume = 60;
        private const int LowVolume = 30;
        private const int HighVolume = 90;

        [FormerlySerializedAs("activate")] [SerializeField]
        private Button buttonActivate;

        [SerializeField] private GameObject indicateMuted;

        [FormerlySerializedAs("indicateMin")] [SerializeField]
        private GameObject indicateLow;

        [SerializeField] private GameObject indicateMedium;

        [FormerlySerializedAs("indicateMax")] [SerializeField]
        private GameObject indicateHigh;

        [SerializeField] private GameObject markMuted;
        [SerializeField] private GameObject markLow;
        [SerializeField] private GameObject markMedium;

        [FormerlySerializedAs("markMax")] [SerializeField]
        private GameObject markHigh;

        [FormerlySerializedAs("buttonMuted")] [SerializeField]
        private Button buttonMute;

        [FormerlySerializedAs("buttonMin")] [SerializeField]
        private Button buttonLow;

        [SerializeField] private Button buttonMedium;

        [FormerlySerializedAs("buttonMax")] [SerializeField]
        private Button buttonHigh;

        [SerializeField] private FloatVariable volume;
        private bool _active;

        private void Awake()
        {
            Debug.Assert(buttonActivate != null, "activate != null");
            Debug.Assert(indicateMuted != null, "indicateMuted != null");
            Debug.Assert(indicateLow != null, "indicateMin != null");
            Debug.Assert(indicateMedium != null, "indicateMedium != null");
            Debug.Assert(indicateHigh != null, "indicateMax != null");
            Debug.Assert(buttonMute != null, "buttonMuted != null");
            Debug.Assert(buttonLow != null, "buttonMin != null");
            Debug.Assert(buttonMedium != null, "buttonMedium != null");
            Debug.Assert(buttonHigh != null, "buttonMax != null");
            Debug.Assert(volume != null, "volume != null");
            Debug.Assert(volume.Changed != null, "volume.Changed != null");
        }

        private void OnEnable()
        {
            buttonMute.onClick.AddListener(OnMute);
            buttonLow.onClick.AddListener(OnLow);
            buttonMedium.onClick.AddListener(OnMedium);
            buttonHigh.onClick.AddListener(OnHigh);
            buttonActivate.onClick.AddListener(OnActivate);
            volume.Changed.Register(OnVolumeChanged);
            _active = false;
            UpdateIndicators();
            UpdateButtonVisibility();
        }

        private void OnVolumeChanged() => UpdateIndicators();

        private void OnDisable()
        {
            buttonMute.onClick.RemoveListener(OnMute);
            buttonLow.onClick.RemoveListener(OnLow);
            buttonMedium.onClick.RemoveListener(OnMedium);
            buttonHigh.onClick.RemoveListener(OnHigh);
            buttonActivate.onClick.RemoveListener(OnActivate);
            volume.Changed.Unregister(OnVolumeChanged);
            _active = false;
            UpdateIndicators();
            UpdateButtonVisibility();
        }

        private void OnActivate()
        {
            _active = !_active;
            UpdateButtonVisibility();
        }

        private void UpdateButtonVisibility()
        {
            buttonMute.gameObject.SetActive(_active);
            buttonLow.gameObject.SetActive(_active);
            buttonMedium.gameObject.SetActive(_active);
            buttonHigh.gameObject.SetActive(_active);
            buttonActivate.gameObject.SetActive(!_active);
        }

        private void OnMute()
        {
            volume.Value = 0;
            UpdateIndicators();
            OnActivate();
        }

        private void OnLow()
        {
            volume.Value = LowVolume;
            UpdateIndicators();
            OnActivate();
        }

        private void OnMedium()
        {
            volume.Value = MediumVolume;
            UpdateIndicators();
            OnActivate();
        }

        private void OnHigh()
        {
            volume.Value = HighVolume;
            UpdateIndicators();
            OnActivate();
        }

        private void UpdateIndicators()
        {
            bool isMuted = volume.Value < LowVolume;
            indicateMuted.SetActive(isMuted);
            markMuted.SetActive(isMuted);

            bool isLow = volume.Value >= LowVolume && volume.Value < MediumVolume;
            indicateLow.SetActive(isLow);
            markLow.SetActive(isLow);

            bool isMedium = volume.Value >= MediumVolume && volume.Value < HighVolume;
            indicateMedium.SetActive(isMedium);
            markMedium.SetActive(isMedium);

            bool isHigh = volume.Value >= HighVolume;
            indicateHigh.SetActive(isHigh);
            markHigh.SetActive(isHigh);
        }
    }
}