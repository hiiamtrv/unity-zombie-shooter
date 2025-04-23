using System;
using System.Linq;
using Audio;
using Base.Locator;
using Base.Pool;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Bullet
{
    public class ProjectileBullet : MonoBehaviour, IPoolable, IBullet
    {
        [SerializeField]
        private Vector3 fireVelocity;

        [SerializeField]
        private int maxPooledObjects;

        [SerializeField]
        private SphereCollider damageZone; //for get radius only, do not enable this

        [SerializeField]
        private GameLayerConfig environmentLayer;

        [SerializeField]
        private GameLayerConfig enemyLayer;
        
        [SerializeField]
        private GameObject hitAirborneFx;

        [SerializeField]
        private GameObject hitGroundFx;
        
        [SerializeField]
        private AudioClip explodeSound;

        private BulletConfig bulletConfig;
        private Rigidbody rb;

        public void OnBeforeSpawn(bool isReused)
        {
            if (isReused) GetComponentInChildren<TrailRenderer>().Clear();
        }

        public void SetupBullet(BulletConfig bulletCfg)
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = transform.TransformVector(fireVelocity);

            bulletConfig = bulletCfg;
        }

        public event IPoolable.PoolReturnHandler OnPoolReturn;
        public bool DDOL => false;
        public int PoolCapacity => maxPooledObjects;

        private void OnTriggerEnter(Collider other)
        {
            CastZoneDamage(damageZone);

            var fxPrefab = other.gameObject.layer == LayerMask.NameToLayer("Environment")
                ? hitGroundFx
                : hitAirborneFx;
            SpawnHitFx(fxPrefab, rb.velocity);
            AudioSystem.PlaySound(explodeSound);
            OnPoolReturn?.Invoke(this);
        }

        private void SpawnHitFx(GameObject prefab, Vector3 direction)
        {
            var newObj = Instantiate(prefab, transform.position, Quaternion.identity);
            newObj.transform.rotation = Quaternion.LookRotation(-direction);
        }

        private void CastZoneDamage(SphereCollider damageZone)
        {
            var radius = damageZone.radius;
            var targets = Physics.OverlapSphere(transform.position, radius, enemyLayer.LayerMask);

            foreach (var target in targets)
            {
                if (Physics.Linecast(transform.position, target.transform.position, environmentLayer.LayerMask)) continue;
                if (target.TryGetComponent(out IDamageable damageable) && damageable.CanHit(gameObject))
                {
                    var dmg = bulletConfig.GetDamage();
                    damageable.DoDamage(dmg, gameObject);
                }
            }
        }
    }
}