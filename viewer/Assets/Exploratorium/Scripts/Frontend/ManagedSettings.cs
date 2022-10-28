using System.Linq;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using Directus.Generated;
using JetBrains.Annotations;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Exploratorium.Frontend
{
    public class ManagedSettings : MonoBehaviour
    {
        [SerializeField] private UnityEvent onChanged;
        [SerializeField] private VoidEvent changedEvent;
        //[SerializeField] private FloatVariable resetTimerVariable;
        //[SerializeField] private IntVariable idleIntervalMinVariable;
        //[SerializeField] private IntVariable idleIntervalRandomVariable;
        [SerializeField] private BoolVariable slideshowAutoplayVariable;
        [SerializeField] private BoolVariable videoAutoplayVariable;
        [SerializeField] private BoolVariable modelDefaultShowinfoVariable;
        [SerializeField] private BoolVariable slideshowDefaultShowinfoVariable;
        [SerializeField] private FloatVariable slideshowSlideDurationVariable;

        private SettingsRecord _settings;
        private static ManagedSettings _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"{nameof(ManagedSettings)} : {name} : singleton violation, duplicate destroyed", this);
                Destroy(this);
                return;
            }

            _instance = this;
        }

        private void OnEnable()
        {
            DirectusManager.Instance.ModelChanged += OnModelChanged;
            if (DirectusManager.Instance.Connector != null && DirectusManager.Instance.IsReady)
                OnModelChanged(DirectusManager.Instance.Connector);
        }

        private void OnDisable()
        {
            DirectusManager.Instance.ModelChanged -= OnModelChanged;
        }

        private void OnModelChanged([NotNull] DirectusConnector connector)
        {
            Debug.Log($"{nameof(ManagedSettings)} : Settings changed");
            _settings = connector.Model.GetItemsOfType<SettingsRecord>().First();
            
            /* TODO: convert remaining usages of SettingsRecord to variable references 
            if (resetTimerVariable != null)
                resetTimerVariable.Value = _settings.ResetTimer;
            if (idleIntervalMinVariable != null)
                idleIntervalMinVariable.Value = _settings.IdleIntervalMin;
            if (idleIntervalRandomVariable != null)
                idleIntervalRandomVariable.Value = _settings.IdleIntervalRandom;
            */
            
            if (slideshowAutoplayVariable != null)
                slideshowAutoplayVariable.Value = _settings.SlideshowAutoplay;
            if (videoAutoplayVariable != null)
                videoAutoplayVariable.Value = _settings.VideoAutoplay;
            if (modelDefaultShowinfoVariable != null)
                modelDefaultShowinfoVariable.Value = _settings.ModelDefaultShowinfo;
            if (slideshowDefaultShowinfoVariable != null)
                slideshowDefaultShowinfoVariable.Value = _settings.SlideshowDefaultShowinfo;
            if (slideshowSlideDurationVariable != null)
                slideshowSlideDurationVariable.Value = _settings.SlideshowSlideDuration;
            if (changedEvent != null)
                changedEvent.Raise();
            onChanged.Invoke();
        }
    }
}