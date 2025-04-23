using System;
using Base.Locator;
using Base.Pool;
using UnityEngine;

namespace Game.Spawners
{
    public class PoolableObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        private float interval;

        [SerializeField]
        private GameObject prefab;

        private float spawnCountdown;
        private IPoolable pooledPrefab;
        private ObjectPoolManager mgr;

        private void Start()
        {
            if (!prefab.TryGetComponent(out pooledPrefab))
            {
                Debug.Log("PoolableObjectSpawner: Prefab is not a pooled gameObject. Nothing will be spawned");
                return;
            }

            mgr = Locator<ObjectPoolManager>.Instance;
        }

        private void Update()
        {
            if (spawnCountdown > 0) spawnCountdown -= Time.deltaTime;

            if (spawnCountdown <= 0)
            {
                SpawnPooledObject();
                spawnCountdown = interval;
            }
        }

        private void SpawnPooledObject()
        {
            if (pooledPrefab == null) return;
            var pool = mgr.GetOrCreatePool(pooledPrefab);

            var newObject = pool.RetrieveFromPool();
            newObject.gameObject.transform.position = transform.position;
        }
    }
}