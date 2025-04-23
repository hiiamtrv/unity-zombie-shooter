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

        private bool isDisposing;
        private float cooldown;

        public void OnBeforeSpawn(bool isReused)
        {
            isDisposing = false;
            OnReused?.Invoke();
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
                    OnPoolReturn?.Invoke(this);
                }
            }
        }
    }
}