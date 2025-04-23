using System;
using Base.Locator;
using Base.Pool;
using Game.Interfaces;
using Game.SkinPool;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Characters.Zombie
{
    public class ZombiePoolObject : MonoBehaviour, IPoolable, IVisualPoolObjectConsumer
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
        private ZombieSkinQualityConfig skinQualityConfig;

        private SkinnedMeshRenderer skinnedMeshRenderer;

        private bool isDisposing;
        private float cooldown;
        private static int numActiveZombies = 0;
        private ZombieSkinQualityItem currentItem;

        public void OnBeforeSpawn(bool isReused, int numActiveObjects)
        {
            isDisposing = false;
            OnReused?.Invoke();

            numActiveZombies++;
            skinQualityConfig.RecalculateSkinQuality(numActiveZombies);
        }

        public event IPoolable.PoolReturnHandler OnPoolReturn;
        public bool DDOL => false;
        public int PoolNumPooledObject => maxPooledObjects;

        private void OnEnable()
        {
            FetchAndApplyQualityItem();
        }

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
                if (currentItem != skinQualityConfig.GlobalSkinQuality)
                {
                    FetchAndApplyQualityItem();
                }
            }
        }

        private void FetchAndApplyQualityItem()
        {
            currentItem = skinQualityConfig.GlobalSkinQuality;
            
            if (skinnedMeshRenderer == null) return;
            skinnedMeshRenderer.quality = currentItem.skinQuality;
            skinnedMeshRenderer.shadowCastingMode = currentItem.shadowCastingMode;
            skinnedMeshRenderer.receiveShadows = currentItem.receiveShadows;
        }

        public void LoadVisualPoolObject(VisualPoolObject visual)
        {
            skinnedMeshRenderer = visual.skinnedMeshRenderer;
        }

        public void UnloadVisualPoolObject()
        {
            skinnedMeshRenderer = null;
        }
    }
}