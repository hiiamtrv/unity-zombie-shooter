using System.Collections.Generic;
using UnityEngine;

namespace Base.Pool
{
    public class ObjectPoolManager : MonoBehaviour
    {
        private Dictionary<IPoolable, ObjectPool> localPool = new Dictionary<IPoolable, ObjectPool>();
        private static Dictionary<IPoolable, ObjectPool> globalPool = new Dictionary<IPoolable, ObjectPool>();

        public ObjectPool GetOrCreatePool(IPoolable sample, bool? forceCreateDDOl = null)
        {
            ObjectPool pool;
            if (globalPool.TryGetValue(sample, out pool) || localPool.TryGetValue(sample, out pool)) return pool;

            var ddol = forceCreateDDOl ?? sample.DDOL;
            pool = new ObjectPool(sample, ddol);
            if (ddol)
            {
                globalPool.Add(sample, pool);
            }
            else
            {
                localPool.Add(sample, pool);
            }

            return pool;
        }

        public void FlushLocalPools()
        {
            foreach (var pool in localPool.Values)
            {
                pool.Dispose();
            }
        }

        public void FlushGlobalPools()
        {
            foreach (var pool in globalPool.Values)
            {
                pool.Dispose();
            }
        }
    }
}