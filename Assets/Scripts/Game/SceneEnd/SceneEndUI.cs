using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SceneEnd
{
    public class SceneEndUI : MonoBehaviour
    {
        public static string BackSceneName;
        public static bool IsWin;

        [SerializeField]
        private TMP_Text txtResult;

        [SerializeField]
        private Button btnEnterScene;

        [SerializeField]
        private Button btnQuit;

        private void Start()
        {
            txtResult.text = IsWin ? "You WIN" : "You lose";
            btnEnterScene.GetComponentInChildren<TMP_Text>().text = IsWin ? "Restart" : "Try Again";
            btnEnterScene.onClick.AddListener(() =>
            {
                var currSceneName = gameObject.scene.name;
                SceneLoader.SceneLoader.Instance.LoadScene(currSceneName, BackSceneName);
            });
            btnQuit.onClick.AddListener(Application.Quit);
        }
    }
}