using System;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


public class BuildToolsSettings : ScriptableObject
{
    private static BuildToolsSettings _instance;

    public static BuildToolsSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                var assetId = AssetDatabase.FindAssets($"t:{nameof(BuildToolsSettings)}").FirstOrDefault();
                if (!string.IsNullOrEmpty(assetId))
                {
                    Debug.Log(
                        $"Using {nameof(BuildToolsSettings)} at '{AssetDatabase.GUIDToAssetPath(assetId)}' ({assetId}) as singleton reference.");
                    _instance = AssetDatabase.LoadAssetAtPath<BuildToolsSettings>(
                        AssetDatabase.GUIDToAssetPath(assetId));
                }
            }

            if (_instance == null)
            {
                Debug.Log($"No instance of {nameof(BuildToolsSettings)} (t:{nameof(BuildToolsSettings)}) was found.");
            }

            return _instance;
        }
    }

    [SerializeField]
    private string deployToFolder;

    [SerializeField]
    private string buildToFolder = "Builds";


    [SerializeField]
    private string isccExePath;

    public string IsccExePath => isccExePath;
    public string BuildToFolder => buildToFolder;

    [ShowInInspector]
    [MultiLineProperty]
    [ReadOnly]
    public string DeployToFolderPath => deployToFolder.TrimEnd('\\', '/');

    [ShowInInspector]
    [MultiLineProperty]
    [ReadOnly]
    public string BuildToFolderPath
    {
        get
        {
            if (Path.IsPathRooted(BuildToFolder))
                return BuildToFolder;
            return new DirectoryInfo(Application.dataPath).Parent + "/" + BuildToFolder.TrimEnd('\\', '/');
        }
    }

    [Button]
    private void RevealBuildDirectory()
    {
        Debug.Log(BuildToFolderPath);
#if UNITY_EDITOR_WIN
        EditorUtility.RevealInFinder(BuildToFolderPath);
#elif UNITY_EDITOR_OSX
        EditorUtility.RevealInFinder(BuildToFolderPath);
#endif
    }

    [Button]
    private void SelectDeployFolder()
    {
        var path = EditorUtility.OpenFolderPanel("Point to deploy folder", Application.dataPath, "STAGING");
        if (!string.IsNullOrWhiteSpace(path))
            deployToFolder = path;
    }

    [Button]
    private void SelectISCCExe()
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Point to Inno Setup's ISCC.exe",
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), new[] {"exe"});
        if (!string.IsNullOrWhiteSpace(path))
            isccExePath = path;
    }

    [Button]
    private void RevealDeploymentDirectory()
    {
        Debug.Log(DeployToFolderPath);
        EditorUtility.RevealInFinder(DeployToFolderPath);
    }

    [MenuItem(BuildTools.MenuPrefix + "/Editor Settings/Create Build Settings")]
    public static void CreateSettings()
    {
        var instance = CreateInstance<BuildToolsSettings>();
        AssetDatabase.CreateAsset(instance, $"Assets/{nameof(BuildToolsSettings)}.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = instance;
    }

    [MenuItem(BuildTools.MenuPrefix + "/Editor Settings/Show Build Settings")]
    public static void ShowSettings()
    {
        var foundGuid = AssetDatabase.FindAssets($"t:{nameof(BuildToolsSettings)}").FirstOrDefault();

        if (foundGuid != null)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject =
                AssetDatabase.LoadAssetAtPath<BuildToolsSettings>(AssetDatabase.GUIDToAssetPath(foundGuid));
        }
        else
        {
            Debug.LogWarning($"'{nameof(BuildToolsSettings)}.asset' was not be found.");
        }
    }
}