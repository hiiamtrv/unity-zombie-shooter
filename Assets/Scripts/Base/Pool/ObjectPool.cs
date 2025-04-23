using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base.Pool
{
    public class ObjectPool
    {
        private int iterator;
        private IPoolable[] idleObjects;
        private HashSet<IPoolable> activeObjects;

        private readonly IPoolable prefab;
        private readonly bool ddol;
        private GameObject root;

        public ObjectPool(IPoolable samplePrefab, bool dontDestroyOnLoad, int maxPooledObjects, GameObject root = null)
        {
            prefab = samplePrefab;
            ddol = dontDestroyOnLoad;
            idleObjects = new IPoolable[maxPooledObjects];
            activeObjects = new HashSet<IPoolable>();
            iterator = -1;
        }

        public IPoolable RetrieveFromPool()
        {
            return RetrieveFromPool(out _);
        }

        public IPoolable RetrieveFromPool(out bool createdNewObject, bool autoActive = true)
        {
            createdNewObject = false;
            IPoolable retrievePoolable;
            if (iterator < 0)
            {
                retrievePoolable = CreateNewOne();
                retrievePoolable.OnBeforeSpawn(false, activeObjects.Count + 1);

                if (ddol)
                {
                    Object.DontDestroyOnLoad(retrievePoolable.gameObject);
                }

                createdNewObject = true;
            }
            else
            {
                retrievePoolable = idleObjects[iterator--];
                retrievePoolable.OnBeforeSpawn(true, activeObjects.Count + 1);
                AssignReuse(retrievePoolable, autoActive);
            }

            activeObjects.Add(retrievePoolable);
            return retrievePoolable;
        }

        public void ReturnAll()
        {
            foreach (var obj in activeObjects)
            {
                AssignIdle(obj);
            }

            activeObjects.Clear();
        }

        public void ReturnToPool(IPoolable obj)
        {
            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
                if (iterator >= idleObjects.Length)
                {
                    Object.Destroy(obj.gameObject);
                }
                else
                {
                    AssignIdle(obj);
                }
            }
        }

        public void Dispose()
        {
            //kill all objects even ddol
            var allObjects = activeObjects.ToList();
            allObjects.AddRange(idleObjects);

            foreach (var poolable in allObjects)
            {
                if (!poolable?.gameObject) continue;
                Object.Destroy(poolable.gameObject);
            }

            iterator = -1;
        }

        private void AssignIdle(IPoolable obj)
        {
            idleObjects[++iterator] = obj;
            obj.gameObject.SetActive(false);
        }

        private void AssignReuse(IPoolable obj, bool autoActive)
        {
            if (autoActive) obj.gameObject.SetActive(true);
        }

        private IPoolable CreateNewOne()
        {
            var poolable = Object.Instantiate(prefab.gameObject, Vector3.zero, Quaternion.identity)
                .GetComponent<IPoolable>();
            if (root != null)
            {
                SceneManager.MoveGameObjectToScene(poolable.gameObject, root.scene);
            }

            poolable.OnPoolReturn += ReturnToPool;
            return poolable;
        }

        public void GenerateReserved(int num)
        {
            for (var i = 0; i < num; i++)
            {
                if (iterator >= idleObjects.Length) return;
                var poolable = CreateNewOne();
                AssignIdle(poolable);
            }
        }
    }
}