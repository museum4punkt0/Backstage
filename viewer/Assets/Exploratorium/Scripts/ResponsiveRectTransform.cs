using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Exploratorium
{
    [Serializable]
    public struct ResponsiveRectConfig
    {
        [TextArea(2, 10), GUIColor(1f, 1f, 0f), HideLabel] [SerializeField]
        public string description;

        [SerializeField] public Vector2 localPosition;
        [SerializeField] public Vector2 anchorMin;
        [SerializeField] public Vector2 anchorMax;
        [SerializeField] public Vector2 anchoredPosition;
        [SerializeField] public Vector2 sizeDelta;
        [SerializeField] public Vector2 pivot;
        [SerializeField] public Vector3 localScale;


        [SerializeField] public GameObject whileActive;
    }

    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [InfoBox("Adjusts a RectTransform based on screen orientation", InfoMessageType.None)]
    public class ResponsiveRectTransform : MonoBehaviour
    {
        [SerializeField] private bool isResponsive = false;

        [ShowInInspector] [ReadOnly]
        private bool _isPortrait;

        [SerializeField] private ResponsiveRectConfig portraitConfig;
        [SerializeField] private ResponsiveRectConfig landscapeConfig;
        [SerializeField] private List<ResponsiveRectConfig> copiedConfigs;

        private RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            Debug.Assert(_rt != null);
        }


        private void Reset()
        {
            _rt = GetComponent<RectTransform>();
            CopyValues();
        }

        private void Start()
        {
            _isPortrait = Screen.width < Screen.height;
            OnUpdate();
        }


        [Button]
        private void CopyValues()
        {
            copiedConfigs.Add(new ResponsiveRectConfig
            {
                localPosition = _rt.localPosition,
                anchorMin = _rt.anchorMin,
                anchorMax = _rt.anchorMax,
                anchoredPosition = _rt.anchoredPosition,
                sizeDelta = _rt.sizeDelta,
                pivot = _rt.pivot,
                description = $"{DateTime.Now:s}"
            });
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            _rt = GetComponent<RectTransform>();
            Debug.Assert(portraitConfig.localScale.sqrMagnitude > 0.01f,
                "portraitConfig.localScale.sqrMagnitude > 0.01f",
                this);
            Debug.Assert(landscapeConfig.localScale.sqrMagnitude > 0.01f,
                "landscapeConfig.localScale.sqrMagnitude > 0.01f",
                this);
            _isPortrait = Screen.width < Screen.height;
            OnUpdate();
        }

        private void Update()
        {
            if (!isResponsive)
                return;

            bool isPortrait = Screen.width < Screen.height;
            //if (Application.isPlaying)
            if (_isPortrait == isPortrait)
                return;

            _isPortrait = isPortrait;
            OnUpdate();
        }

        private void OnUpdate()
        {
            if (!isResponsive)
                return;

            if (_isPortrait)
            {
                _rt.localPosition = portraitConfig.localPosition;
                _rt.anchorMin = portraitConfig.anchorMin;
                _rt.anchorMax = portraitConfig.anchorMax;
                _rt.anchoredPosition = portraitConfig.anchoredPosition;
                _rt.sizeDelta = portraitConfig.sizeDelta;
                _rt.pivot = portraitConfig.pivot;
                _rt.localScale = portraitConfig.localScale;
                if (landscapeConfig.whileActive)
                    landscapeConfig.whileActive.SetActive(false);
                if (portraitConfig.whileActive)
                    portraitConfig.whileActive.SetActive(true);
            }
            else
            {
                _rt.localPosition = landscapeConfig.localPosition;
                _rt.anchorMin = landscapeConfig.anchorMin;
                _rt.anchorMax = landscapeConfig.anchorMax;
                _rt.anchoredPosition = landscapeConfig.anchoredPosition;
                _rt.sizeDelta = landscapeConfig.sizeDelta;
                _rt.pivot = landscapeConfig.pivot;
                _rt.localScale = landscapeConfig.localScale;
                if (portraitConfig.whileActive)
                    portraitConfig.whileActive.SetActive(false);
                if (landscapeConfig.whileActive)
                    landscapeConfig.whileActive.SetActive(true);
            }
        }
    }
}