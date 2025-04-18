using System;
using Base.Pool;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Bullet
{
    public class ProjectileBullet : MonoBehaviour, IPoolable, IBullet
    {
        [SerializeField]
        private DamageConfig damageConfig;

        [SerializeField]
        private Vector3 fireVelocity;

        public void OnBeforeSpawn(bool isReused)
        {
        }

        public void SetupBullet()
        {
            var rb = GetComponent<Rigidbody>();
            rb.velocity = transform.TransformVector(fireVelocity);
        }

        public event IPoolable.PoolReturnHandler OnPoolReturn;
        public bool DDOL => false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable) && damageable.CanHit(gameObject))
            {
                var dmgData = damageConfig.GetDamage();
                damageable.DoDamage(dmgData, gameObject);

                this.ReturnOrDestroy(OnPoolReturn);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                this.ReturnOrDestroy(OnPoolReturn);
            }
        }
    }
}