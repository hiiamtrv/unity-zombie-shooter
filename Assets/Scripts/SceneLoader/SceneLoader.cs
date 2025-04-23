using System;
using UnityEngine;
using UnityEngine.GameInput;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader instance;
        public static SceneLoader Instance => instance;

        [SerializeField]
        private string loadingScene;

        private bool isLoadingSceneLoaded;

        private string waitingLoadScene;
        private string waitingUnloadScene;
        private AsyncOperation currOperation;
        private SceneLoaderUI sceneLoaderUI;

        private GameInputActions.PlayerActions playerInput;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            playerInput = new GameInputActions().Player;
            playerInput.Enable();
#endif
        }

        private void Update()
        {
            if (isLoadingSceneLoaded)
            {
                if (currOperation?.isDone is false) //null or true are not accepted
                {
                    sceneLoaderUI?.SetProgress(currOperation.progress);
                }
            }

#if UNITY_EDITOR
            if (playerInput.Dev_ReloadScene.WasReleasedThisFrame())
            {
                LoadScene("Playground", "Playground");
            }
#endif
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Scene Loaded: {scene.name}");
            if (mode != LoadSceneMode.Additive) return;
            if (scene.name.Equals(loadingScene))
            {
                sceneLoaderUI = FindAnyObjectByType<SceneLoaderUI>();
                isLoadingSceneLoaded = true;
                PerformIfNeeded();
            }
            else if (scene.name.Equals(waitingLoadScene))
            {
                SceneManager.SetActiveScene(scene);
                waitingLoadScene = string.Empty;
                PerformIfNeeded();
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"Scene Unload: {scene.name}");
            if (scene.name.Equals(loadingScene))
            {
                sceneLoaderUI = null;
                isLoadingSceneLoaded = false;
            }
            else if (scene.name.Equals(waitingUnloadScene))
            {
                waitingUnloadScene = string.Empty;
                PerformIfNeeded();
            }
        }

        public void LoadScene(string toScene)
        {
            if (currOperation != null)
            {
                Debug.Log($"Another operation is in process {currOperation.progress}");
            }

            waitingLoadScene = string.Empty;
            waitingLoadScene = toScene;
            PerformIfNeeded();
        }

        public void LoadScene(string fromScene, string toScene)
        {
            if (currOperation != null)
            {
                Debug.Log($"Another operation is in process {currOperation.progress}");
            }

            waitingUnloadScene = fromScene;
            waitingLoadScene = toScene;
            PerformIfNeeded();
        }

        private void PerformIfNeeded()
        {
            Debug.Log($"Perform if needed: [{waitingUnloadScene}] -> [{waitingLoadScene}] ({isLoadingSceneLoaded})");
            //if no waiting scene, try unloading loading scene
            if (waitingLoadScene == string.Empty && waitingUnloadScene == string.Empty)
            {
                if (isLoadingSceneLoaded)
                {
                    currOperation = SceneManager.UnloadSceneAsync(loadingScene);
                }

                return;
            }

            //there might be scenes waiting to load/unload, check if loading scene is ready
            //Loading scene not ready key
            if (!isLoadingSceneLoaded)
            {
                currOperation = SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
                return;
            }

            //Unloading scene case
            if (waitingUnloadScene != string.Empty)
            {
                sceneLoaderUI?.SetText(waitingUnloadScene, true);
                currOperation = SceneManager.UnloadSceneAsync(waitingUnloadScene);
                return;
            }

            //Loading scene case
            sceneLoaderUI?.SetText(waitingLoadScene, false);
            currOperation = SceneManager.LoadSceneAsync(waitingLoadScene, LoadSceneMode.Additive);
        }
    }
}