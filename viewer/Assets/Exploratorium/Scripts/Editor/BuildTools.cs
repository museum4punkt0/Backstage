using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Codice.CM.Common;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlasticGui;
using Stubble.Core.Builders;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using CompressionLevel = System.IO.Compression.CompressionLevel;

#pragma warning disable CS0168


public static class BuildTools
{
    public const string MenuPrefix = "Project";

    static BuildPlayerOptions GetCurrentBuildOptions(BuildPlayerOptions defaultOptions = new BuildPlayerOptions())
    {
        return BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(defaultOptions);
    }

    [MenuItem(MenuPrefix + "/Build Win64 Release")]
    public static void BuildWin64Release()
    {
        PrepareReleaseBuild(out var buildPlayerOptions, out var target);
        try
        {
            if (!DoBuild(buildPlayerOptions))
                CleanupFailedBuild(target);
        }
        catch (Exception)
        {
            CleanupFailedBuild(target);
        }
    }

    [MenuItem(MenuPrefix + "/Build Win64 Release and deploy to P:")]
    public static void BuildWin64ReleaseAndDeploy()
    {
        PrepareReleaseBuild(out var buildPlayerOptions, out var target);
        try
        {
            if (!DoBuild(buildPlayerOptions))
                CleanupFailedBuild(target);
            else
                DeployLatestBuild();
        }
        catch (Exception)
        {
            CleanupFailedBuild(target);
        }
    }

    private static void PrepareReleaseBuild(out BuildPlayerOptions buildPlayerOptions, out DirectoryInfo target)
    {
        target = PrepareBuild("R");
        buildPlayerOptions = GetCurrentBuildOptions();
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer;
    }


    [MenuItem(MenuPrefix + "/Build Win64 Debug")]
    public static void BuildWin64Debug()
    {
        var target = PrepareBuild("D");
        BuildPlayerOptions buildPlayerOptions = GetCurrentBuildOptions();
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options =
            BuildOptions.AllowDebugging | BuildOptions.ShowBuiltPlayer | BuildOptions.Development;
        try
        {
            if (!DoBuild(buildPlayerOptions))
                CleanupFailedBuild(target);
        }
        catch (Exception e)
        {
            CleanupFailedBuild(target);
        }
    }

    private static bool DoBuild(BuildPlayerOptions buildPlayerOptions)
    {
        AddressableAssetProfileSettings profileSettings = AddressableAssetSettingsDefaultObject.Settings.profileSettings;
        string profileId = profileSettings.GetProfileId("Default");
        AddressableAssetSettingsDefaultObject.Settings.activeProfileId = profileId;
        AddressableAssetSettings.BuildPlayerContent();

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded; {summary.totalTime}; {(summary.totalSize / 1024.0f / 1024.0f):F2} MB");
            return true;
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.LogError($"Build failed.");
            return false;
        }

        return false;
    }

    private static string GetPlasticSignature()
    {
        try
        {
            var projectPath = new DirectoryInfo(Application.dataPath).Parent.FullName;
            var plasticPath = Path.Combine(projectPath, ".plastic");
            FileInfo plasticWorkspace = new DirectoryInfo(plasticPath)
                .GetFiles()
                .First(it => it.Name == "plastic.workspace");
            var txt = File.ReadAllLines(plasticWorkspace.FullName);
            Debug.Log($"plastic.workspace file found with name {txt[0]} and ID {txt[1]}");
            Guid wkSpaceId = Guid.Parse(txt[1].Trim());
            WorkspaceInfo wkSpace = Plastic.API.GetWorkspaceFromId(wkSpaceId);
           
            if (wkSpace == null)
            {
                Debug.LogWarning($"No PlasticSCM workspace was found for ID {txt[1]}");
                return "NOVCS";
            }
            else
            {
                Debug.Log($"Found workspace ID {wkSpace.Id}");
                Debug.Log($"Found workspace name {wkSpace.Name}");
                Debug.Log($"Found workspace path {wkSpace.ClientPath}");
                var workingBranch = Plastic.API.GetWorkingBranch(wkSpace);
                Debug.Log($"Found branch {workingBranch.BranchName}");
                Debug.Log($"Found changeset {workingBranch.Changeset}");
                var branchNicename = Regex.Replace(workingBranch.BranchName, "[^a-zA-Z0-9]", "");
                return branchNicename + "-" + workingBranch.Changeset;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return "X";
        }
    }

    [MenuItem(MenuPrefix + "/Latest Build/Create Installer")]
    private static void CreateInstallerForLatestBuild()
    {
        // this must be placed inside a "Resources" folder under Assets. 
        const string installScriptTemplate = "BuildTools_InstallScript.mustache";
        var latest = BuildDir
            .GetDirectories()
            .OrderByDescending(it => it.CreationTimeUtc)
            .FirstOrDefault(it => it.EnumerateFiles("UnityPlayer.dll").Any());
        if (latest == null)
            return;

        Dictionary<string, string> templateData = new Dictionary<string, string>()
        {
            { "AppName", Application.productName },
            { "AppVersion", Application.version },
            { "AppPublisher", Application.companyName },
            { "AppURL", "" },
            { "AppExeName", Application.productName + ".exe" },
            { "SourceDir", latest.FullName },
            { "OutputDir", BuildDir.FullName },
        };
        var stubble = new StubbleBuilder().Build();
        var template = Resources.Load<TextAsset>(installScriptTemplate);
        if (template == null)
        {
            Debug.LogError($"Could not find {installScriptTemplate}. Does it exist inside a Resources folder?");
            return;
        }

        var installScript = stubble.Render(template.text, templateData);
        var savePath = $"{latest.Parent.FullName}{Path.DirectorySeparatorChar}InstallerFor_{latest.Name}.iss";
        try
        {
            File.WriteAllText(savePath, installScript);
            Debug.Log(
                $"Inno Setup Installer script created at {savePath}. Compile with (CLI):\niscc {savePath}\n"
            );
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        var cmdArgs = $"/K \"\"{BuildToolsSettings.Instance.IsccExePath}\" \"{savePath}\" \"";
        Debug.Log($"Running: CMD.exe {cmdArgs}");
        System.Diagnostics.Process.Start("CMD.exe", cmdArgs);
    }


    [MenuItem(BuildTools.MenuPrefix + "/Latest Build/Create Archive")]
    private static void PackLatestBuild()
    {
        var latest = BuildDir
            .GetDirectories()
            .OrderByDescending(it => it.CreationTimeUtc)
            .FirstOrDefault(it => it.EnumerateFiles("UnityPlayer.dll").Any());
        if (latest != null)
        {
            Task.Run(
                () =>
                {
                    try
                    {
                        ZipBuild(latest);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            );
        }
    }

    [MenuItem(BuildTools.MenuPrefix + "/Show Builds")]
    private static void ShowBuilds()
    {
        Debug.Log(BuildDir.FullName);
        EditorUtility.RevealInFinder(BuildDir.FullName);
    }

    private static DirectoryInfo BuildDir
    {
        get
        {
            Debug.Assert(BuildToolsSettings.Instance);
            return new DirectoryInfo(BuildToolsSettings.Instance.BuildToFolderPath);
        }
    }

    [MenuItem(BuildTools.MenuPrefix + "/Latest Build/Deploy to P:")]
    private static void DeployLatestBuild()
    {
        var latest = BuildDir
            .GetDirectories()
            .OrderByDescending(it => it.CreationTimeUtc)
            .FirstOrDefault(it => it.EnumerateFiles("UnityPlayer.dll").Any());
        if (latest != null)
        {
            Task.Run(
                () =>
                {
                    try
                    {
                        Deploy(ZipBuild(latest));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            );
        }
    }

    private static string ZipBuild(DirectoryInfo dir)
    {
        if (!dir.Exists)
            throw new ArgumentException("Path invalid", nameof(dir));
        var archive = $"{dir.Parent.FullName}{Path.DirectorySeparatorChar}{dir.Name}.zip";
        if (!File.Exists(archive))
        {
            Debug.Log($"Creating archive {archive} ... (this is a background task and will take a while)");
            ZipFile.CreateFromDirectory(dir.FullName, archive, CompressionLevel.Fastest, true);
            Debug.Log($"{archive} ({new FileInfo(archive).Length / 1024} kb) created.");
        }
        else
        {
            Debug.Log($"Archive {archive} already exists.");
        }

        return archive;
    }

    private static string Deploy(string filePath)
    {
        var fi = new FileInfo(filePath);
        var dest = BuildToolsSettings.Instance.DeployToFolderPath + Path.DirectorySeparatorChar + fi.Name;
        Debug.Log($"Deploying {filePath} ...");
        fi.CopyTo(dest);
        Debug.Log($"Deployed to {dest}.");
        return dest;
    }

    private static  DirectoryInfo PrepareBuild(string suffix)
    {
        var versionString = GenerateVersionString();
        var vcsSignature = GetPlasticSignature();
        PlayerSettings.bundleVersion = versionString + suffix;
        var folderName = $"{DateTime.Now:yyMMdd}_{Application.productName}_{Application.version}___{vcsSignature}";
        BuildDir.Create();
        DirectoryInfo buildDir = BuildDir.CreateSubdirectory(folderName);
        Debug.Log($"{buildDir.FullName} created");
        return buildDir;
    }

    private static void CleanupFailedBuild(DirectoryInfo target)
    {
        if (target.Exists && target.FullName.Contains($"{Application.productName}_{Application.version}"))
        {
            target.Delete(true);
            Debug.Log($"Build failure cleanup deleted {target.FullName}");
        }
    }

    /// <summary>
    /// Takes the current major and minor version and appends revision-id/build-id that is generated from the
    /// current time. MAJOR.MINOR.DAYS.BEATS, where DAYS are days since 1.1.2020 and BEATS are seconds since
    /// 0:00h on the current day divided by 86.4
    /// </summary>
    /// <returns></returns>
    public static string GenerateVersionString()
    {
        long days = (long)(DateTime.UtcNow - new DateTime(2020, 1, 1)).TotalDays;
        long beats = (long)((DateTime.UtcNow - DateTime.UtcNow.Date).TotalSeconds / 86.4f);
        var isKnownFormat = Regex.IsMatch(PlayerSettings.bundleVersion, @"^[0-9]+\.[0-9]+\.*");
        if (!isKnownFormat)
            return $"0.0.{days}.{beats}";

        var current = PlayerSettings.bundleVersion.Split('.');
        return $"{current[0]}.{current[1]}.{days}.{beats}";
    }
}