using System;
using System.Collections;
using System.Linq;
using Base.Locator;
using Base.Pool;
using Game.SceneEnd;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [Serializable]
    public struct ElementChoice
    {
        public GameObject gameObject;
        public int weight;
    }

    [Serializable]
    public struct SpawnTimer
    {
        public float initialSpawnsPerSecond;
        public float spawnIncreaseInterval;
        public float spawnIncreaseStep;
    }

    public class LevelController : MonoBehaviour
    {
        [SerializeField]
        private LevelConfig config;

        [SerializeField]
        private float levelSeconds;

        [SerializeField]
        private SpawnTimer spawnTimer;

        [SerializeField]
        private ElementChoice[] spawnPointChoices;

        [SerializeField]
        private ElementChoice[] zombieSpawnChoices;

        [SerializeField]
        private string nextScene;

        private float currSpawnsPerSecond;
        private float levelCountdown;
        private float spawnCountdown;
        private float spawnIncreaseCountdown;

        private ObjectPoolManager objectPoolManager;
        public float LevelSeconds => levelCountdown;

        private void Start()
        {
            objectPoolManager = Locator<ObjectPoolManager>.Instance;

            currSpawnsPerSecond = spawnTimer.initialSpawnsPerSecond;
            spawnIncreaseCountdown = spawnTimer.spawnIncreaseInterval;
            spawnCountdown = 1 / currSpawnsPerSecond;
            levelCountdown = levelSeconds;
        }

        private void Update()
        {
            if (Countdown(ref spawnCountdown))
            {
                SpawnAZombie();
                spawnCountdown = 1 / currSpawnsPerSecond;
            }

            if (Countdown(ref spawnIncreaseCountdown))
            {
                currSpawnsPerSecond += spawnTimer.spawnIncreaseStep;
                spawnIncreaseCountdown = spawnTimer.spawnIncreaseInterval;
            }
        }

        private void LateUpdate()
        {
            if (Countdown(ref levelCountdown))
            {
                WinGame();
            }
        }

        private bool Countdown(ref float countdown)
        {
            var isPositive = countdown > 0;
            countdown -= Time.deltaTime;
            return isPositive && countdown <= 0;
        }

        private void SpawnAZombie()
        {
            var zombiePrefab = GetRandomGameObject(zombieSpawnChoices);
            var spawnPoint = GetRandomGameObject(spawnPointChoices);

            var iPoolable = zombiePrefab.GetComponent<IPoolable>();
            if (iPoolable != null && objectPoolManager != null)
            {
                var pool = objectPoolManager.GetOrCreatePool(iPoolable);
                var newZombie = pool.RetrieveFromPool().gameObject;
                newZombie.transform.position = spawnPoint.transform.position;
            }
            else
            {
                Instantiate(zombiePrefab, spawnPoint.transform.position, Quaternion.identity);
            }
        }

        public static GameObject GetRandomGameObject(ElementChoice[] choices)
        {
            var totalWeight = choices.Sum(choice => choice.weight);

            var rand = Random.Range(0, totalWeight);
            foreach (var choice in choices)
            {
                if (rand < choice.weight) return choice.gameObject;
                rand -= choice.weight;
            }

            Debug.Log("[LevelController] GetRandomGameObject could not pick a random one, code may have error");
            return choices[0].gameObject;
        }

        public void LoseGame()
        {
            var currSceneName = gameObject.scene.name;
            SceneEndUI.IsWin = false;
            SceneEndUI.BackSceneName = currSceneName;
            SceneLoader.SceneLoader.Instance.LoadScene(currSceneName, "SceneEnd");
            
            Locator<ObjectPoolManager>.Instance.ReturnGlobalPools();
        }

        private void WinGame()
        {
            var currSceneName = gameObject.scene.name;
            var loadNextScene = false;

            foreach (var levelName in config.LevelSceneNames)
            {
                if (loadNextScene)
                {
                    Locator<ObjectPoolManager>.Instance.ReturnGlobalPools();
                    SceneLoader.SceneLoader.Instance.LoadScene(currSceneName, levelName);
                    return;
                }

                loadNextScene = levelName.Equals(currSceneName);
            }

            //remove all pool objects, we dont need it anymore
            Locator<ObjectPoolManager>.Instance.FlushGlobalPools();
            Locator<ObjectPoolManager>.Instance.FlushLocalPools();

            SceneEndUI.IsWin = true;
            SceneEndUI.BackSceneName = config.LevelSceneNames[0];
            SceneLoader.SceneLoader.Instance.LoadScene(currSceneName, "SceneEnd");
        }
    }
}