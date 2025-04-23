using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneLoader
{
    public class SceneLoaderUI : MonoBehaviour
    {
        [SerializeField]
        private Slider slider;

        [SerializeField]
        private TMP_Text text;

        private float percent;
        private bool isUnloading;

        public void SetProgress(float progress)
        {
            percent = progress;
        }

        public void SetText(string sceneName, bool unloading)
        {
            isUnloading = unloading;
            text.text = unloading ? "Unloading" : "Loading" + " " + sceneName;
        }

        private void Update()
        {
            if (isUnloading)
            {
                slider.value = percent;
            }
            else
            {
                slider.value = 1f - percent;
            }
        }
    }
}