using System;
using Base.Pool;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Bullet
{
    public class StraightBullet : MonoBehaviour, IPoolable, IBullet
    {
        [SerializeField]
        private DamageConfig damageConfig;

        [SerializeField]
        private float initialTimeout;

        [SerializeField]
        private float speed;

        private float timeout;

        public void OnBeforeSpawn(bool isReused)
        {
            timeout = initialTimeout;
        }

        public void SetupBullet()
        {
            var rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * speed;
        }

        public event IPoolable.PoolReturnHandler OnPoolReturn;
        public bool DDOL => true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable) && damageable.CanHit(gameObject))
            {
                var dmgData = damageConfig.GetDamage();
                damageable.DoDamage(dmgData, gameObject);

                this.ReturnOrDestroy(OnPoolReturn);
            }
            else if (other.gameObject.layer.Equals("Environment"))
            {
                this.ReturnOrDestroy(OnPoolReturn);
            }
        }

        private void Update()
        {
            timeout -= Time.deltaTime;
            if (timeout <= 0)
            {
                this.ReturnOrDestroy(OnPoolReturn);
            }
        }
    }
}