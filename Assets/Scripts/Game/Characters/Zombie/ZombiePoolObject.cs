using System;
using Base.Locator;
using Base.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Characters.Zombie
{
    public class ZombiePoolObject : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private UnityEvent OnReused;

        [SerializeField]
        private float disposeStartDelay;

        [SerializeField]
        private float disposeTime;
        
        [SerializeField]
        private int maxPooledObjects;
        
        [SerializeField]
        private SkinnedMeshRenderer skinnedMeshRenderer;
        
        [SerializeField]
        private ZombieSkinQualityConfig skinQualityConfig;

        private bool isDisposing;
        private float cooldown;
        private static int numActiveZombies = 0;
        
        public void OnBeforeSpawn(bool isReused, int numActiveObjects)
        {
            isDisposing = false;
            OnReused?.Invoke();
            
            numActiveZombies++;
            skinQualityConfig.RecalculateSkinQuality(numActiveZombies);
            Debug.Log($"Update skin quality {numActiveObjects} {skinQualityConfig.GlobalSkinQuality}");
        }

        public event IPoolable.PoolReturnHandler OnPoolReturn;
        public bool DDOL => false;
        public int PoolCapacity => maxPooledObjects;

        public void StartDisposeZombie()
        {
            isDisposing = true;
            cooldown = disposeStartDelay + disposeTime;
        }

        private void Update()
        {
            if (isDisposing)
            {
                if (cooldown > 0f) cooldown -= Time.deltaTime;
                if (cooldown <= 0f)
                {
                    numActiveZombies--;
                    skinQualityConfig.RecalculateSkinQuality(numActiveZombies);
                    
                    OnPoolReturn?.Invoke(this);
                }
            }
            else
            {
                if (skinnedMeshRenderer.quality != skinQualityConfig.GlobalSkinQuality)
                {
                    skinnedMeshRenderer.quality = skinQualityConfig.GlobalSkinQuality;
                }
            }
        }
    }
}