using System;
using Audio;
using Base.Locator;
using Base.Pool;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Bullet
{
    public class StraightBullet : MonoBehaviour, IPoolable, IBullet
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private int maxPooledObjects;

        [SerializeField]
        private GameObject hitPrefabFx;

        [SerializeField]
        private AudioClip explodeSound;

        private BulletConfig bulletConfig;
        private float maxSqrDistance;
        private Vector3 originalPos;

        public void OnBeforeSpawn(bool isReused, int numActiveObjects)
        {
            if (isReused) GetComponentInChildren<TrailRenderer>().Clear();
        }

        public void SetupBullet(BulletConfig bulletCfg)
        {
            var rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * speed;

            bulletConfig = bulletCfg;
            maxSqrDistance = bulletCfg.MaxFlyDistance;
            originalPos = transform.position;
        }

        public event IPoolable.PoolReturnHandler OnPoolReturn;
        public bool DDOL => true;
        public int PoolNumPooledObject => maxPooledObjects;

        private void OnTriggerEnter(Collider other)
        {
            bool isHit = false;
            if (other.TryGetComponent(out IDamageable damageable) && damageable.CanHit(gameObject))
            {
                var dmgData = bulletConfig.GetDamage();
                damageable.DoDamage(dmgData, gameObject);
                
                isHit = true;
            }
            else if (other.gameObject.layer.Equals("Environment"))
            {
                isHit = true;
            }

            if (isHit)
            {
                CreateHitFx(other);
                AudioSystem.PlaySound(explodeSound);
                OnPoolReturn?.Invoke(this);
            }
        }

        private void Update()
        {
            var sqrDist = Vector3.SqrMagnitude(transform.position - originalPos);
            if (sqrDist >= maxSqrDistance)
            {
                OnPoolReturn?.Invoke(this);
            }
        }

        private void CreateHitFx(Collider other)
        {
            var rb = GetComponent<Rigidbody>();
            var rollbackBulletPos = transform.position - rb.velocity * 100f; //cast based on bullet velocity to get the actual hit point
            var hitPos = other.ClosestPoint(rollbackBulletPos);
            Instantiate(hitPrefabFx, hitPos, Quaternion.identity);
        }
    }
}