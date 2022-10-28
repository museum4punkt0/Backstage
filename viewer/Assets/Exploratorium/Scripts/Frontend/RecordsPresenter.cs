using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Directus.Connect.v9;
using Exploratorium.Extras;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Exploratorium.Frontend
{
    public abstract class RecordsPresenter<T> : MonoBehaviour where T : DbRecord
    {
        [SerializeField] private GameObject viewerRoot;

        [SerializeField] protected bool debug;

        private T[] _records;

        public T[] Records => _records;

        protected virtual void Awake()
        {
            Debug.Assert(!debug, "!debug", this);
            Debug.Assert(viewerRoot != null, "viewerRoot != null", this);
        }

        protected virtual void OnEnable() => LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        protected virtual void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;


        public void Show(params T[] records)
        {
            if (debug) Debug.Log($"{GetType().GetNiceName()} Show {records.Length} records", this);
            _records = records.Where(it => it != null).ToArray();
            OnShow(_records);
            if (viewerRoot != null)
                viewerRoot.gameObject.SetActive(true);
            Changed?.Invoke(this);
            onChanged?.Invoke();
        }

        public void Clear()
        {
            if (debug)
                if (_records != null)
                    Debug.Log($"{GetType().GetNiceName()} Clear {_records.Length} records", this);
            OnClear();
            if (viewerRoot != null)
                viewerRoot.gameObject.SetActive(false);
            _records = Array.Empty<T>();
            Changed?.Invoke(this);
            onChanged?.Invoke();
        }

        protected abstract void OnLocaleChanged(Locale locale);
        protected abstract void OnShow(params T[] records);
        protected abstract void OnClear();

        public UnityEvent onChanged;
        public event Action<RecordsPresenter<T>> Changed;


        [SerializeField] private OpenableGroup[] openables;


        public async UniTaskVoid CloseAsync()
        {
            if (!this)
                return;

            await UniTask.CompletedTask;
            if (debug)
                Debug.Log($"{GetType().GetNiceName()} : {name} closing", this);
            openables.ForEach(it => it.CloseAsync().Forget());
            OnClose();
            await UniTask.CompletedTask;
            //await UniTask.NextFrame();
        }

        protected abstract void OnClose();

        public async UniTaskVoid OpenAsync()
        {
            if (!this)
                return;

            if (debug)
                Debug.Log($"{GetType().GetNiceName()} : {name} opening", this);
            openables.ForEach(it => it.OpenAsync().Forget());
            OnOpen();
            await UniTask.CompletedTask;
            //await UniTask.NextFrame();
        }
    
        protected abstract void OnOpen();
    
        public static int Mod(int x, int mod) => (x % mod + mod) % mod;
    }
}