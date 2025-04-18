using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base.Pool
{
    public class ObjectPool
    {
        private List<IPoolable> idleObjects = new List<IPoolable>();
        private HashSet<IPoolable> activeObjects = new HashSet<IPoolable>();

        private readonly IPoolable prefab;
        private readonly bool ddol;

        public ObjectPool(IPoolable samplePrefab, bool dontDestroyOnLoad)
        {
            prefab = samplePrefab;
            ddol = dontDestroyOnLoad;
        }

        public IPoolable RetrieveFromPool()
        {
            return RetrieveFromPool(out _);
        }

        public IPoolable RetrieveFromPool(out bool createdNewObject, bool autoActive = true)
        {
            createdNewObject = false;
            IPoolable retrievePoolable;
            if (idleObjects.Count <= 0)
            {
                retrievePoolable = Object.Instantiate(prefab.gameObject, Vector3.zero, Quaternion.identity)
                    .GetComponent<IPoolable>();
                retrievePoolable.OnPoolReturn += ReturnToPool;
                retrievePoolable.OnBeforeSpawn(false);

                if (ddol)
                {
                    Object.DontDestroyOnLoad(retrievePoolable.gameObject);
                }

                createdNewObject = true;
            }
            else
            {
                retrievePoolable = idleObjects[idleObjects.Count - 1];
                retrievePoolable.OnBeforeSpawn(true);
                AssignReuse(retrievePoolable, autoActive);
                idleObjects.RemoveAt(idleObjects.Count - 1);
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
                AssignIdle(obj);
                activeObjects.Remove(obj);
            }
        }

        public void Dispose()
        {
            //kill all objects even ddol
            foreach (var obj in activeObjects.Where(obj => obj?.gameObject != null))
            {
                Object.Destroy(obj.gameObject);
            }

            foreach (var obj in idleObjects.Where(obj => obj?.gameObject != null))
            {
                Object.Destroy(obj.gameObject);
            }
        }

        private void AssignIdle(IPoolable obj)
        {
            idleObjects.Add(obj);
            obj.gameObject.SetActive(false);
        }

        private void AssignReuse(IPoolable obj, bool autoActive)
        {
            if (autoActive) obj.gameObject.SetActive(true);
        }
    }
}