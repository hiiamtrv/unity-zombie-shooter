using System;
using UnityEngine;

namespace Base.Pool
{
    public interface IPoolable
    {
        public delegate void PoolReturnHandler(IPoolable poolable);

        public void OnBeforeSpawn(bool isReused);
        public event PoolReturnHandler OnPoolReturn;

        public GameObject gameObject { get; }
        public bool DDOL { get; }
    }
}