using System;
using System.Collections.Generic;
using Audio;
using Game.Interfaces;
using Game.Utils;
using UnityEngine;

namespace Game.Characters
{
    public class MeleeWeaponActor : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private MeleeWeaponConfig config;

        [SerializeField]
        private GameLayerConfig hitMask;

        [SerializeField]
        private AudioClip hitSound;

        private float attackCooldown = 0;

        public void TryAttack(Vector3 muzzlePosition, Vector3 aimDirection)
        {
            if (attackCooldown > 0) return;

            attackCooldown = config.secondsBetweenAttacks;
            var hits = Physics.OverlapSphere(muzzlePosition, config.damageRadius, hitMask.LayerMask);
            foreach (var hit in hits)
            {
                var angle = Vector3.Angle(hit.bounds.center - muzzlePosition, aimDirection);
                if (angle > config.damageFrontAngle) continue;

                if (!hit.TryGetComponent(out IDamageable damageable) || !damageable.CanHit(gameObject)) continue;

                var damage = config.damageConfig.GetDamage();
                damageable.DoDamage(damage, gameObject);
            }
            
            AudioSystem.PlaySound(hitSound);
        }

        private void Update()
        {
            if (attackCooldown > 0)
            {
                attackCooldown -= Time.deltaTime;
            }
        }

        public void TryReload()
        {
            //Melee do not reload
        }

        public string WeaponName => "Melee";
        public int BulletsInMag => 1; //dont use this 
        public int BulletsLeft => 1; //dont use this
        public bool InfiniteBullet => true; //dont use this

        public void ReturnWeaponTransform()
        {
            //Do nothing, zombie use bare hands
        }

        public void AssignWeaponTransformOnHand(Transform handTransform)
        {
            //Do nothing, zombie use bare hands
        }

        public float GetReloadingPercentage()
        {
            return 100f;
        }
    }
}