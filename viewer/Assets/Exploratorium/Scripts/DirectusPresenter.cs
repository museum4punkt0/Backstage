using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading;
using Cysharp.Threading.Tasks;
using Exploratorium;
using Directus.Connect.v9;
using Directus.Connect.v9.Unity.Runtime;
using JetBrains.Annotations;
using Juniper;
using Sirenix.OdinInspector;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DirectusPresenter : MonoBehaviour
{
    [CanBeNull] [SerializeField]
    private string contentReadyScene = "ContentReady";

    [CanBeNull] [SerializeField]
    private RawImage preview;

    [SerializeField] private ProgressBar progressBar;

    [BoxGroup(Constants.ObservedEvents)]
    [SerializeField] private VoidEvent buildEvent;

    [BoxGroup(Constants.ObservedEvents)]
    [SerializeField] private VoidEvent cancelEvent;

    [BoxGroup(Constants.WriteVariables)]
    [SerializeField] private BoolVariable isReady;

    [SerializeField] private DirectusManager directusManager;
    [SerializeField] private GameObjectValueList showLoadingScreen;
    [SerializeField] private bool buildModelOnStart;
    [SerializeField] private Selectable cancelButton;
    [SerializeField] private Selectable buildButton;
    [SerializeField] private Selectable clearButton;

    private CancellationTokenSource _cts = new CancellationTokenSource();

    [SerializeField] private GameObject tagError;
    [SerializeField] private GameObject tagReady;
    [SerializeField] private GameObject tagBuilding;
    [SerializeField] private GameObject tagCache;
    [SerializeField] private GameObject tagOffline;
    [SerializeField] private GameObject tagOnline;
    [SerializeField] private TMP_Text diagnostics;


    private bool _isBuildRunning;

    private void Awake()
    {
        Debug.Assert(preview != null, $"{nameof(preview)} property is unassigned");
        Debug.Assert(!string.IsNullOrWhiteSpace(contentReadyScene),
            $"{nameof(contentReadyScene)} property is unassigned");
    }


    private void OnEnable()
    {
        if (progressBar)
            progressBar.ClearProgressBar();
        buildEvent.Register(OnBuild);
        cancelEvent.Register(OnCancel);

        if (tagError)
            tagError.SetActive(false);
        if (tagOnline)
            tagOnline.SetActive(false);
        if (tagOffline)
            tagOffline.SetActive(false);
        if (tagOffline)
            tagReady.SetActive(false);
        UpdateDiagnosticsView();
    }

    private async void Start()
    {
        await UniTask.Delay(500);
        if (buildModelOnStart)
        {
            CancelModelBuilder();
            BuildModelAsync(_cts.Token).Forget();
        }
    }

    private void OnDisable()
    {
        buildEvent.Unregister(OnBuild);
        cancelEvent.Unregister(OnCancel);
    }

    private void OnBuild()
    {
        CancelModelBuilder();
        BuildModelAsync(_cts.Token).Forget();
    }

    public void OnCancel()
    {
        CancelModelBuilder();
    }

    public async UniTask BuildModelAsync(CancellationToken ct)
    {
        try
        {
            Debug.Log("BuildModel");
            _isBuildRunning = true;
            if (isReady != null)
                isReady.Value = false;
            ClearDiagnosticsView();

            await UniTask.Delay(500, cancellationToken: ct);
            if (SceneManager.GetSceneByName(contentReadyScene).isLoaded)
                await SceneManager.UnloadSceneAsync(contentReadyScene);
            await UniTask.Delay(500, cancellationToken: ct);

            if (progressBar)
                progressBar.InitProgressBar();
            await RetrieveContentAsync(_cts.Token);
            if (progressBar)
                progressBar.ClearProgressBar();

            if (!directusManager.Connector.Model.IsReady)
            {
                Debug.LogError("Failed to build model");
                if (progressBar)
                    progressBar.ClearProgressBar();
                if (buildButton != null)
                    buildButton.interactable = true;
                UpdateDiagnosticsView();
                return;
            }

            if (progressBar)
                progressBar.ClearProgressBar();

            await UniTask.Delay(100, cancellationToken: ct);
            await SceneManager.LoadSceneAsync(contentReadyScene, LoadSceneMode.Additive);
            UpdateDiagnosticsView();
            _isBuildRunning = false;
            if (isReady != null)
                isReady.Value = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _isBuildRunning = false;
            if (isReady != null)
                isReady.Value = false;
            if (progressBar != null)
                progressBar.ClearProgressBar();
            if (cancelButton != null)
                cancelButton.interactable = false;
            if (buildButton != null)
                buildButton.interactable = true;
            UpdateDiagnosticsView();
        }
    }

    private void ClearDiagnosticsView()
    {
        if (diagnostics != null)
            diagnostics.text = "";
    }

    private void UpdateDiagnosticsView()
    {
        if (diagnostics != null && directusManager.Connector?.Diagnostics != null)
            diagnostics.text =
                $"{directusManager.Connector?.Diagnostics?.ErrorCount} Errors{(directusManager.Connector.IsAuthenticated ? ", Authenticated" : "")}{(directusManager.Connector.IsCacheUpdateBlocked ? ", Offline" : ", Live")}";
        else
            diagnostics.text = "";
    }

    private void Update()
    {
        if (tagError != null)
            tagError.SetActive(directusManager.Connector.IsInit &&
                               !(directusManager.Connector.Model is { IsReady: true }));
        if (tagOffline != null)
            tagOffline.SetActive(directusManager.IsOffline);
        if (tagOnline != null)
            tagOnline.SetActive(!directusManager.IsOffline);
        if (tagCache != null)
            tagCache.SetActive(directusManager.IsCached);
        if (tagReady != null)
            tagReady.SetActive(directusManager.IsReady);
        if (tagBuilding != null)
            tagBuilding.SetActive(directusManager.IsBuilding);
        if (cancelButton)
            cancelButton.interactable = directusManager.IsBuilding ||
                                        _isBuildRunning;
        if (buildButton)
            buildButton.interactable = !directusManager.IsBuilding &&
                                       !_isBuildRunning;
        if (clearButton)
            clearButton.interactable = directusManager.Connector.HasCachePersistence &&
                                       !_isBuildRunning &&
                                       !directusManager.IsBuilding;
    }

    public void CancelModelBuilder()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        if (progressBar != null)
            progressBar.ClearProgressBar();
    }

    int _completed;

    public async UniTask RetrieveContentAsync(CancellationToken ct)
    {
        showLoadingScreen.Add(gameObject);

        IObserver<ProgressInfo> progressObserver = null;
        if (progressBar != null)
        {
            progressObserver = Observer.Create<ProgressInfo>(
                nx => UniTask.Post(() => { progressBar.OnProgressChangedAsync(nx); })
            );
        }

        await directusManager.BuildModelAsync(progressObserver, ct);
        if (preview != null)
        {
            preview.color = Color.clear;
            preview.enabled = false;
        }

        var files = directusManager.Connector.Model.Files.ToList();

        ProgressInfo overallProgress = new ProgressInfo
        {
            Message = "Conforming assets...",
            Total = files.Count,
            Completed = 0,
            Scope = ProgressScope.Overall
        };
        progressObserver?.OnNext(overallProgress);
        List<UniTask> tasks = new List<UniTask>();
        for (var i = 0; i < files.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var file = files[i];
            var path = directusManager.Connector.GetLocalFilePath(file);

            progressObserver?.OnNext(new ProgressInfo
            {
                Message = Path.GetFileName(file.FilenameDisk),
                Total = files.Count,
                Completed = i,
                Scope = ProgressScope.Component
            });

            if (file.Type == MimeTypes.Image.Png || file.Type == MimeTypes.Image.Jpeg)
                tasks.Add(ResizedImageAssets.AddAsync(path));

            if (i % 32 == 0)
            {
                await UniTask.WhenAll(tasks);
                await UniTask.NextFrame(ct);
                tasks.Clear();
                /*if (preview != null)
                {
                    preview.enabled = true;
                    preview.texture = ResizedPictures.GetTx(path);
                    preview.color = Color.white;
                }*/

                overallProgress.Completed += 32;
                progressObserver?.OnNext(overallProgress);
            }
        }

        await UniTask.WhenAll(tasks);
        overallProgress.Completed += files.Count / 32;
        progressObserver?.OnNext(overallProgress);


        Debug.Log($"Preview image cache size is now {ResizedImageAssets.CacheSize / 1048576:F1} MB");

        if (preview != null)
        {
            preview.color = Color.clear;
            preview.enabled = false;
        }

        GC.Collect();
        if (progressBar)
            progressBar.ClearProgressBar();
        showLoadingScreen.Remove(gameObject);
    }
}