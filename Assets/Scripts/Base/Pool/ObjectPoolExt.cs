using UnityEngine;

namespace Base.Pool
{
    public static class ObjectPoolExt
    {
        public static void ReturnOrDestroy<T>(this T poolable, IPoolable.PoolReturnHandler handler)
            where T : MonoBehaviour, IPoolable
        {
            if (handler != null)
            {
                handler.Invoke(poolable);
            }
            else
            {
                Object.Destroy(poolable.gameObject);
            }
        }
    }
}