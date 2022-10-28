using System;
using System.Linq;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using JetBrains.Annotations;
using Sirenix.Utilities;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    public abstract class RecordPresenter<T> : MonoBehaviour, IOpenable where T : DbRecord
    {
        [CanBeNull] [SerializeField]
        protected Button selectButton;

        [CanBeNull] [SerializeField]
        private DbRecordWrapperVariable selected;

        [FormerlySerializedAs("canvasGroup")] [CanBeNull] [SerializeField]
        protected CanvasGroup selfGroup;

        [SerializeField] protected bool debug = false;
        [SerializeField] private Openable[] openables;

        private T _record;
        private bool _rebuildRequested;

        public event Action<RecordPresenter<T>> Selected;
        public event Action<RecordPresenter<T>> Deselected;
        protected const string TranslationMissing = "";

        public T Record
        {
            get => _record;
            set
            {
                _record = value;
                if (debug)
                {
                    if (_record != null)
                        Debug.Log($"{nameof(RecordPresenter<T>)} : Record {_record.__Primary} assigned to {name}",
                            this);
                    else
                        Debug.Log($"{nameof(RecordPresenter<T>)} : Record cleared from {name}", this);
                }

                Deselect();
                OnRecordChanged();
            }
        }

        private void Awake()
        {
            Debug.Assert(!debug, "!debug", this);
            Debug.Assert(openables.All(it => it != null), "openables.All(it => it != null)", this);
        }


        protected virtual void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(Select);
            }

            if (selected != null && selected.Changed != null)
            {
                selected.Changed.Register(OnSelectedChanged);
                OnSelectedChanged(selected.Value);
            }
        }

        protected virtual void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            OnDeselect();
            if (selectButton != null)
                selectButton.onClick.RemoveListener(Select);
            if (selected != null && selected.Changed != null)
                selected.Changed.Unregister(OnSelectedChanged);
        }

        public void Deselect()
        {
            if (debug)
                Debug.Log($"{nameof(RecordPresenter<T>)} : Deselected <{typeof(T).GetNiceName()}>[{Record?.__Primary}]",
                    this);

            OnDeselect();
            Deselected?.Invoke(this);
            if (selected != null && selected.Value != null && selected.Value.Record == Record)
                selected.Value = null;
        }

        protected abstract void OnDeselect();

        protected abstract void OnSelect();


        protected abstract void OnSelectedChanged(DbRecordWrapper record);

        protected abstract void OnRecordChanged();

        protected abstract void OnLocaleChanged(Locale locale);

        public void Select()
        {
            if (debug)
                Debug.Log($"{nameof(RecordPresenter<T>)} : Selected <{typeof(T).GetNiceName()}>[{Record?.__Primary}]",
                    this);

            OnSelect();
            Selected?.Invoke(this);
            if (selected != null)
            {
                if (debug)
                    Debug.Log(
                        $"{nameof(RecordPresenter<T>)} : {selected.name} := <{typeof(T).GetNiceName()}>[{Record?.__Primary}]",
                        this);
                selected.Value = new DbRecordWrapper(Record);
            }
        }

        public void SetVisible(bool isVisible)
        {
            if (selfGroup != null)
            {
                selfGroup.interactable = isVisible;
                selfGroup.blocksRaycasts = isVisible;
                selfGroup.alpha = isVisible ? 1f : 0;
            }
        }

        public bool IsSelected => selected != null && selected.Value.Record == Record;


        protected static Texture2D GetPreviewTx([NotNull] DirectusFile file)
        {
            if (file == null)
                return null;
            return ResizedImageAssets.GetTx(DirectusManager.Instance.Connector.GetLocalFilePath(file));
        }

        protected static Sprite GetPreviewSprite([NotNull] DirectusFile file)
        {
            if (file == null)
                return null;
            return ResizedImageAssets.GetSprite(DirectusManager.Instance.Connector.GetLocalFilePath(file));
        }


        /// <summary>
        /// Call <see cref="RebuildOpenables"/> after calling this to apply the delay.
        /// </summary>
        public void SetAdditionalOpenDelay(float delay) => openables.ForEach(it => it.SetAdditionalOpenDelay(delay));

        /// <summary>
        /// Call <see cref="RebuildOpenables"/> after calling this to apply the delay.
        /// </summary>
        public void SetAdditionalCloseDelay(float delay) => openables.ForEach(it => it.SetAdditionalCloseDelay(delay));

        /// <summary>
        /// Rebuild tweens based on current settings and context. Required for adaptation to layout changes and settings.
        /// </summary>
        public void RebuildOpenables(bool force = false)
        {
            _rebuildRequested = true;
            if (force)
                openables.ForEach(it =>
                {
                    if (it != null)
                        it.SetDirty();
                    else
                        Debug.LogWarning(
                            $"{GetType().GetNiceName()} : Null reference found in {nameof(openables)} on {name}", this);
                });
        }

        private void LateUpdate()
        {
            if (_rebuildRequested)
            {
                openables.ForEach(it =>
                {
                    if (it != null)
                        it.RebuildTweens(false);
                    else
                        Debug.LogWarning(
                            $"{GetType().GetNiceName()} : Null reference found in {nameof(openables)} on {name}", this);
                });
                _rebuildRequested = false;
            }
        }

        public void Close()
        {
            // fix a NRE thrown on network client sync
            if (!this)
                return;

            openables.ForEach(it =>
            {
                if (it != null)
                    it.CloseAsync().Forget();
                else
                {
                    Debug.LogWarning(
                        $"{GetType().GetNiceName()} : Null reference found in {nameof(openables)} on {name}", this);
                }
            });
            OnClose();
        }

        protected abstract void OnClose();

        public void Open()
        {
            Deselect();
            openables.ForEach(it =>
            {
                if (it != null)
                    it.OpenAsync().Forget();
                else
                    Debug.LogWarning(
                        $"{GetType().GetNiceName()} : Null reference found in {nameof(openables)} on {name}", this);
            });
            OnOpen();
        }

        protected abstract void OnOpen();

        protected string PreprocessText(string text) => string.IsNullOrWhiteSpace(text) ? "" : text;
    }
}