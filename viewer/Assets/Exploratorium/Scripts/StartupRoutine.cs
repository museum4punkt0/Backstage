using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Directus.Connect.v9.Unity.Runtime;
using JetBrains.Annotations;
using Exploratorium.Net.Shared;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Netcode;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ToggleActiveAdvanced = Exploratorium.Extras.ToggleActiveAdvanced;

#pragma warning disable CS0168
namespace Exploratorium
{
    [DefaultExecutionOrder(-2000)]
    public class StartupRoutine : MonoBehaviour
    {
        [SerializeField] private float delayAutostart = 15f;
        /*
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
    
    
            var copies = FindObjectsOfType<Startup>();
            if (copies.Length == 0)
                Debug.LogError($"{nameof(Startup)} : Cannot find instance");
        }*/

        [FormerlySerializedAs("startScene")] [CanBeNull] [SerializeField]
        private string startSceneName = "Start";

        [CanBeNull] [SerializeField]
        private DirectusPresenter directusPresenter;

        [CanBeNull] [SerializeField]
        private NetworkManager multiplayer;

        [CanBeNull] [SerializeField]
        private CanvasGroup loadingScreen;

        [CanBeNull] [SerializeField]
        private CanvasGroup[] disableControlsDuringAutostart;

        [SerializeField] private ToggleActiveAdvanced helpScreen;
        [SerializeField] private ToggleActiveAdvanced settingsScreen;

        [SerializeField] private VoidEvent cancelAutostart;

        [BoxGroup("Config Variables")] [SerializeField]
        private BoolVariable autostartClient;

        [BoxGroup("Config Variables")] [SerializeField]
        private BoolVariable autostartServer;

        [BoxGroup("Config Variables")] [SerializeField]
        private IntVariable autostartRole;

        //[SerializeField] private Locale startLocale;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private VoidEvent startServer;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private VoidEvent startClient;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private VoidEvent startHost;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private VoidEvent reqSolo;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private VoidEvent reqControl;

        [BoxGroup(Constants.InvokedEvents)] [SerializeField]
        private VoidEvent reqObserver;


        [SerializeField] private GameObjectValueList showLoadingScreen;

        [SerializeField] private Image autostartPendingProgress;

        [SerializeField] private GameObject[] whileAutostartPending;

        [SerializeField] private GameObject[] whileAutostartInProgress;

        [SerializeField] [Min(5)]
        private int networkRetryInterval = 5;

        private readonly CancellationTokenSource _autostartCts = new CancellationTokenSource();
        private static int _runCount;

        //private ProgressInfo _progress;
        //private IObserver<ProgressInfo> _progressObserver;

        private void Awake()
        {
            _runCount++;
            if (_runCount > 1)
            {
                Debug.LogError(
                    $"{nameof(StartupRoutine)} : Start can run only once per app launch; Disabling the instance and aborting routine; This should never happen");
                enabled = false;
                return;
            }

            Debug.Log($"{nameof(StartupRoutine)} : Awake", this);
            Debug.Assert(!string.IsNullOrWhiteSpace(startSceneName),
                $"{nameof(startSceneName)} property is unassigned");
            Debug.Assert(directusPresenter != null, $"{nameof(directusPresenter)} property is unassigned");
            Debug.Assert(multiplayer != null, $"{nameof(multiplayer)} property is unassigned");
            Debug.Assert(loadingScreen != null, $"{nameof(loadingScreen)} property is unassigned");
            Debug.Assert(reqSolo != null, $"{nameof(reqSolo)} property is unassigned");
            Debug.Assert(reqControl != null, $"{nameof(reqControl)} property is unassigned");
            Debug.Assert(reqObserver != null, $"{nameof(reqObserver)} property is unassigned");
            Debug.Assert(startServer != null, $"{nameof(startServer)} property is unassigned");
            Debug.Assert(startHost != null, $"{nameof(startHost)} property is unassigned");
            Debug.Assert(startClient != null, $"{nameof(startClient)} property is unassigned");
            Debug.Assert(autostartRole != null, $"{nameof(autostartRole)} property is unassigned");
            Debug.Assert(autostartClient != null, $"{nameof(autostartClient)} property is unassigned");
            Debug.Assert(autostartServer != null, $"{nameof(autostartServer)} property is unassigned");
            Debug.Assert(cancelAutostart != null, $"{nameof(cancelAutostart)} property is unassigned");
            Debug.Assert(multiplayer != null, $"{nameof(multiplayer)} property is unassigned");

            Debug.Assert(showLoadingScreen != null, $"{nameof(showLoadingScreen)} property is unassigned");
            Debug.Assert(autostartPendingProgress != null,
                $"{nameof(autostartPendingProgress)} property is unassigned");

            Debug.Log($"{nameof(StartupRoutine)} : Awake complete", this);
        }

        private void OnEnable()
        {
            if (showLoadingScreen && loadingScreen)
            {
                showLoadingScreen.Added.Register(UpdateLoadingScreen);
                showLoadingScreen.Removed.Register(UpdateLoadingScreen);
            }

            if (cancelAutostart != null)
                cancelAutostart.Register(CancelAutostart);
        }

        private void OnDisable()
        {
            if (showLoadingScreen && loadingScreen)
            {
                showLoadingScreen.Added.Unregister(UpdateLoadingScreen);
                showLoadingScreen.Removed.Unregister(UpdateLoadingScreen);
            }

            if (cancelAutostart != null)
                cancelAutostart.Unregister(CancelAutostart);
        }

        public void CancelAutostart() => _autostartCts.Cancel();


        private void UpdateLoadingScreen()
        {
            if (loadingScreen)
                loadingScreen.alpha = showLoadingScreen.Count > 0 ? 1.0f : 0f;
        }

        private void Start()
        {
            if (_runCount > 1)
            {
                Debug.LogError(
                    $"{nameof(StartupRoutine)} : Start can run only once per app launch; Disabling the instance and aborting routine; This should never happen");
                enabled = false;
                return;
            }

            StartAsync().Forget();
        }

        private async UniTask WatchAsync(CancellationToken ct)
        {
            Debug.Log($"{nameof(StartupRoutine)} : WATCHDOG : Starting");
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(60f), cancellationToken: ct);
                Debug.Log($"{nameof(StartupRoutine)} : WATCHDOG : Validating network state");
                if (multiplayer.IsConnectedClient || multiplayer.IsServer)
                    continue;
                Debug.LogWarning($"{nameof(StartupRoutine)} : WATCHDOG : Cluster appears to be in an undefined state");
                CancelAutostart();
                await StartRecoveryAsync();
            }
        }

        private async UniTask StartRecoveryAsync()
        {
            try
            {
                Debug.LogWarning($"{nameof(StartupRoutine)} :  RECOVERY : Starting recovery for this node");
                whileAutostartInProgress.ForEach(it => it.SetActive(true));
                Debug.Assert(multiplayer != null, "multiplayer != null");
                Debug.LogWarning($"{nameof(StartupRoutine)} : RECOVERY : Shutting down network");
                multiplayer.Shutdown();
                await UniTask.Delay(TimeSpan.FromSeconds(5f));
                Debug.LogWarning($"{nameof(StartupRoutine)} : RECOVERY : Starting host");
                startHost.Raise();
                await UniTask.Delay(TimeSpan.FromSeconds(10f));
                switch (autostartRole.Value)
                {
                    case (int)PawnRole.Observer:
                        Debug.LogWarning($"{nameof(StartupRoutine)} : RECOVERY : Requesting {PawnRole.Observer} mode");
                        reqObserver.Raise();
                        break;
                    case (int)PawnRole.Controller:
                        Debug.LogWarning(
                            $"{nameof(StartupRoutine)} : RECOVERY : Requesting {PawnRole.Controller} mode");
                        reqControl.Raise();
                        break;
                    case (int)PawnRole.None:
                    case (int)PawnRole.Solo:
                        Debug.LogWarning($"{nameof(StartupRoutine)} : RECOVERY : Requesting {PawnRole.Solo} mode");
                        reqSolo.Raise();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(10f));
            }
            finally
            {
                whileAutostartInProgress.ForEach(it => it.SetActive(false));
            }
        }

        private async UniTask<bool> StartAsync()
        {
            Debug.Log($"{nameof(StartupRoutine)} : Start");
            DOTween.Init().SetCapacity(1000, 500);
            Debug.Log($"{nameof(StartupRoutine)} : DOTween capacity set to 1000/500", this);

            if (showLoadingScreen)
                showLoadingScreen.Add(gameObject);

            if (gameObject.scene.name != startSceneName && gameObject.scene.name != "DontDestroyOnLoad")
            {
                gameObject.SetActive(false);
                Debug.LogError(
                    $"{nameof(StartupRoutine)} : Must be placed in a scene called '{startSceneName}' but is currently in '{gameObject.scene.name}'",
                    this);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                if (Application.isPlaying)
                    Application.Quit();
            }

            // activate startup scene
            Scene startScene = SceneManager.GetSceneByName(startSceneName);
            SceneManager.SetActiveScene(startScene);

#if UNITY_EDITOR
            if (startScene.isLoaded)
            {
                // unload other scenes
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene otherScene = SceneManager.GetSceneAt(i);
                    if (otherScene.buildIndex != startScene.buildIndex)
                    {
                        await SceneManager.UnloadSceneAsync(otherScene);
                        Debug.Log($"{nameof(StartupRoutine)} : Scene {otherScene.name} unloaded", this);
                    }
                }
            }
#endif


            // ensure correct render settings
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
            //Screen.SetResolution((int)(Display.main.renderingHeight * 0.5625f), Display.main.renderingHeight, FullScreenMode.FullScreenWindow);
            Debug.Log($"{nameof(StartupRoutine)} : Fullscreen mode is {Screen.fullScreenMode}", this);
            QualitySettings.vSyncCount = 1;
            Debug.Log($"{nameof(StartupRoutine)} : VSync count is {QualitySettings.vSyncCount}", this);
            QualitySettings.antiAliasing = 0;
            Debug.Log($"{nameof(StartupRoutine)} : MSAA is {QualitySettings.antiAliasing}", this);
            Debug.Log($"{nameof(StartupRoutine)} : Shadow quality is {QualitySettings.shadows}", this);
            Debug.Log($"{nameof(StartupRoutine)} : Shadow resolution is {QualitySettings.shadowResolution}", this);
            Application.targetFrameRate = 60;
            Debug.Log($"{nameof(StartupRoutine)} : Target frame rate is {Application.targetFrameRate}", this);

            Debug.Log(
                $"{nameof(StartupRoutine)} : Selected locale is {LocalizationSettings.SelectedLocale.Identifier.Code}",
                this);

            if (showLoadingScreen)
                showLoadingScreen.Remove(gameObject);
            Debug.Log($"{nameof(StartupRoutine)} : Start complete", this);

            try
            {
                Debug.Log($"{nameof(StartupRoutine)} : Pending autostart", this);
                settingsScreen.Off();

                // delay autostart and show timer progress
                whileAutostartPending.ForEach(it => it.SetActive(true));
                whileAutostartInProgress.ForEach(it => it.SetActive(false));
                float autostartAt = Time.time + delayAutostart;
                while (autostartAt > Time.time)
                {
                    if (_autostartCts.IsCancellationRequested)
                        _autostartCts.Token.ThrowIfCancellationRequested();

                    autostartPendingProgress.fillAmount = (autostartAt - Time.time) / delayAutostart;
                    await UniTask.NextFrame(_autostartCts.Token);
                }

                whileAutostartPending.ForEach(it => it.SetActive(false));

                Debug.Log($"{nameof(StartupRoutine)} : Beginning autostart", this);

                whileAutostartInProgress.ForEach(it => it.SetActive(true));

                disableControlsDuringAutostart?.ForEach(it => it.interactable = false);

                directusPresenter.CancelModelBuilder();

                await directusPresenter.BuildModelAsync(_autostartCts.Token);
                if (DirectusManager.Instance.Connector.Model == null ||
                    !DirectusManager.Instance.Connector.Model.IsReady)
                {
                    Debug.LogError($"{nameof(StartupRoutine)} Unrecoverable startup error");
                    return false;
                }

                /*
                float waitForServerTimeout = Time.time + 300f;
                while (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer && Time.time < waitForServerTimeout)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: _autostartCts.Token);
                }*/

                await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _autostartCts.Token);

                if (autostartClient.Value && autostartServer.Value)
                    startHost.Raise();
                else if (autostartClient.Value)
                    startClient.Raise();
                else if (autostartServer.Value)
                    startServer.Raise();

                whileAutostartInProgress.ForEach(it => it.SetActive(false));
                WatchAsync(_autostartCts.Token).Forget();
                settingsScreen.Off();
                helpScreen.Off();
                return true;
            }
            catch (OperationCanceledException e)
            {
                Debug.Log($"{nameof(StartupRoutine)} : Autostart canceled", this);
                disableControlsDuringAutostart?.ForEach(it => it.interactable = true);
                DirectusManager.Instance.Cancel();
                //settingsScreen.On();
                //helpScreen.On();
                return false;
            }
            finally
            {
                if (disableControlsDuringAutostart != null)
                    disableControlsDuringAutostart.ForEach(it => it.interactable = true);
                whileAutostartInProgress.ForEach(it => it.SetActive(false));
                whileAutostartPending.ForEach(it => it.SetActive(false));
                Debug.Log($"{nameof(StartupRoutine)} : Autostart complete", this);
            }
        }
    }
}