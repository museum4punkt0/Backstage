using Exploratorium.Net.Shared;
using Sirenix.OdinInspector;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Exploratorium
{
    public class LocalizationPresenter : MonoBehaviour
    {
        [SerializeField]
        [BoxGroup("Observed Variables")]
        private PawnRoleVariable roleVariable;

        [SerializeField]
        private CanvasGroup group;

        [SerializeField] private Button de;
        [SerializeField] private Button en;
        [SerializeField] private Button pl;
        [SerializeField] private Locale deLocale;
        [SerializeField] private Locale enLocale;
        [SerializeField] private Locale plLocale;

        [SerializeField] private Graphic deMarker;
        [SerializeField] private Graphic enMarker;
        [SerializeField] private Graphic plMarker;
        private float _nextSetAllowed;

        public void SetLocale(Locale locale)
        {
            if (Time.time < _nextSetAllowed)
            {
                Debug.Log($"{nameof(LocalizationPresenter)} : Language switch is temporarily blocked");
                return;
            }

            _nextSetAllowed = Time.time + 5f; // block for a bit, this is a relatively long timer that gets reset when
            // the SelectedLocaleChanged event fires but makes sure that when the event
            // doesn't happen we don't end up in a permanently blocked state
            LocalizationSettings.SelectedLocale = locale;
            MarkActiveLocale();
        }

        private void OnEnable()
        {
            if (roleVariable != null && roleVariable.Changed != null)
                roleVariable.Changed.Register(OnRoleChanged);

            if (de)
                de.onClick.AddListener(() => SetLocale(deLocale));
            if (en)
                en.onClick.AddListener(() => SetLocale(enLocale));
            if (pl)
                pl.onClick.AddListener(() => SetLocale(plLocale));

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            OnLocaleChanged(LocalizationSettings.SelectedLocale);
        }

        private void OnLocaleChanged(Locale obj)
        {
            // cancel the blocking but keep the rate limited
            _nextSetAllowed = Time.time + 0.5f;
            MarkActiveLocale();
        }

        private void OnDisable()
        {
            if (roleVariable != null && roleVariable.Changed != null)
                roleVariable.Changed.Unregister(OnRoleChanged);

            if (de)
                de.onClick.RemoveAllListeners();
            if (en)
                en.onClick.RemoveAllListeners();
            if (pl)
                pl.onClick.RemoveAllListeners();
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        private void MarkActiveLocale()
        {
            if (deMarker)
                deMarker.gameObject.SetActive(LocalizationSettings.Instance.GetSelectedLocale() == deLocale);
            if (enMarker)
                enMarker.gameObject.SetActive(LocalizationSettings.Instance.GetSelectedLocale() == enLocale);
            if (plMarker)
                plMarker.gameObject.SetActive(LocalizationSettings.Instance.GetSelectedLocale() == plLocale);
        }

        private void OnRoleChanged(PawnRole role)
        {
            if (@group != null)
            {
                @group.alpha = role == PawnRole.Observer ? 0f : 1f;
                @group.interactable = role != PawnRole.Observer;
            }
        }
    }
}