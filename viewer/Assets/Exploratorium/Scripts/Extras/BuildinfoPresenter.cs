using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Exploratorium.Extras
{
    public class BuildinfoPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private Text text;

        void Start()
        {
            SetText();
        }

        [Button]
        private void SetText()
        {
            var txt = $"{Application.companyName} {Application.productName}, Version {Application.version}" +
                      (
                          !Application.isEditor
                              ? $", Build {Application.buildGUID}"
                              : ""
                      );
            if (text)
                text.text = txt;
            if (tmpText)
                tmpText.text = txt;
        }
    }
}