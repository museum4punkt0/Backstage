using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exploratorium
{
    [Obsolete]
    public class LoadStart : MonoBehaviour
    {
#if UNITY_EDITOR
    private const string StartSceneName = "Start";

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        Scene start = SceneManager.GetSceneByName(StartSceneName);
        if (!start.isLoaded)
        {
            SceneManager.LoadScene(StartSceneName, LoadSceneMode.Single);
            start = SceneManager.GetSceneByName(StartSceneName);
            SceneManager.SetActiveScene(start);
        }
    }
#endif
    }
}