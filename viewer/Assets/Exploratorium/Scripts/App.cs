using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exploratorium
{
    public class App : MonoBehaviour
    {
        static void Init()
        {
            var app = new GameObject("[App]", typeof(App));
            DontDestroyOnLoad(app);
            SceneManager.LoadScene("Start", LoadSceneMode.Single);
        }

        private void Awake()
        {
            // continue from here
        }
    }
}