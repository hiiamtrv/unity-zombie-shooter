using System;
using Audio;
using Base.Locator;
using Base.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Bootstrapper
{
    [Serializable]
    public struct GeneratePreservedAction
    {
        public GameObject prefab;
        public int amount;
    }
    
    public class ShooterSceneBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private ObjectPoolManager objectPoolManager;
        
        [SerializeField]
        private AudioSystem audioSystem;

        [SerializeField]
        private GeneratePreservedAction[] generatePreservedActions;
        
        private void Awake()
        {
            Locator<ObjectPoolManager>.SetInstance(objectPoolManager);
            Locator<AudioSystem>.SetInstance(audioSystem);

            foreach (var action in generatePreservedActions)
            {
                GenerateReservedObjects(action.prefab, action.amount);
            }
        }

        public void GenerateReservedObjects(GameObject prefab, int amount)
        {
            if (!prefab.TryGetComponent(out IPoolable poolable)) return;
            var pool = objectPoolManager.GetOrCreatePool(poolable);
            pool.GenerateReserved(amount);
        }
    }
}