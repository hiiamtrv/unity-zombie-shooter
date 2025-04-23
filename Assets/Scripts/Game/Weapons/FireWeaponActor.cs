using System;
using Audio;
using Base.Locator;
using Base.Pool;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Characters
{
    public class FireWeaponActor : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private string weaponName;

        [SerializeField]
        private FireWeaponConfig config;

        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private Transform weaponTransform;

        [SerializeField]
        private AudioClip fireSound;

        private int bulletsLeft;
        private int bulletsInMag;
        private float fireCooldown;
        private bool isReloading;

        private IPoolable pooledBullet;
        private ObjectPoolManager objectPoolManager;

        public string WeaponName => weaponName;

        public int BulletsInMag => bulletsInMag;
        public int BulletsLeft => bulletsLeft;
        public bool InfiniteBullet => config.infiniteBullet;

        private float spreadAngle;

        private void Start()
        {
            pooledBullet = bulletPrefab.GetComponent<IPoolable>(); //can be null, if null using casual bullet
            if (pooledBullet != null)
            {
                objectPoolManager = Locator<ObjectPoolManager>.Instance;
            }

            bulletsLeft = config.initialBullets;
            FulfillBullets();
        }

        private void Update()
        {
            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
                if (fireCooldown <= 0)
                {
                    if (isReloading) FulfillBullets();
                    isReloading = false;
                }
            }

            var spreadCooldown = config.spreadReductionPerSec * Time.deltaTime;
            if (spreadAngle > spreadCooldown)
            {
                spreadAngle -= spreadCooldown;
            }
            else
            {
                spreadAngle = 0;
            }
        }

        public void TryAttack(Vector3 muzzlePosition, Vector3 aimDirection)
        {
            if (fireCooldown > 0) return;
            if (bulletsInMag <= 0) return;
            Fire(muzzlePosition, aimDirection);
        }

        private void Fire(Vector3 muzzlePosition, Vector3 aimDirection)
        {
            for (int i = 0; i < config.raysPerShot; i++)
            {
                var shotgunSpreadAngle = Random.Range(-config.raySpreadAngle, config.raySpreadAngle) / 2.0f;
                ProduceBullet(muzzlePosition, aimDirection, shotgunSpreadAngle);
            }

            ApplySpread();
            bulletsInMag--;
            if (bulletsInMag <= 0)
            {
                Reload();
            }
            else
            {
                fireCooldown = config.secondsBetweenShots;
            }
            
            AudioSystem.PlaySound(fireSound);
        }

        public void TryReload()
        {
            if (fireCooldown > 0) return;
            if (!InfiniteBullet && bulletsLeft <= 0) return;
            Reload();
        }

        private void Reload()
        {
            fireCooldown = config.secondsReload;
            isReloading = true;
        }

        private void FulfillBullets()
        {
            if (config.infiniteBullet)
            {
                bulletsInMag = config.magazineSize;
            }
            else
            {
                //transfer new bullets amount, do not forget leftover bullets
                bulletsLeft += bulletsInMag;
                bulletsLeft -= config.magazineSize;
                bulletsInMag += config.magazineSize;
            }
        }

        private void ApplySpread(int numShots = 1)
        {
            spreadAngle += config.spreadPerShot * numShots;
            spreadAngle = Mathf.Min(spreadAngle, config.maxSpreadAngle);
        }

        private float GetRandomSpreadAngle(float maxSpreadAngle)
        {
            return Random.Range(-maxSpreadAngle, maxSpreadAngle) / 2f;
        }

        public void ReturnWeaponTransform()
        {
            weaponTransform.SetParent(transform);
            weaponTransform.gameObject.SetActive(false);
        }

        public void AssignWeaponTransformOnHand(Transform handTransform)
        {
            var handleTransform = weaponTransform.Find("Handle");
            var handleLocaLPos = handleTransform != null
                ? handleTransform.localPosition
                : Vector3.zero;

            //cheat: the model has right hand whose weapon Z overlapped with hand Y
            weaponTransform.SetParent(handTransform);
            weaponTransform.localEulerAngles = new Vector3(0f, 90f, 90f);
            weaponTransform.localPosition =
                new Vector3(-handleLocaLPos.z, 0f, handleLocaLPos.y) * weaponTransform.localScale.x;
            weaponTransform.gameObject.SetActive(true);
        }

        private void ProduceBullet(Vector3 muzzlePosition, Vector3 aimDirection, float shotgunSpreadAngle)
        {
            GameObject bulletGo;
            var bulletSpreadAngle = GetRandomSpreadAngle(spreadAngle);
            var bulletRotation = GetErrorQuaternion(bulletSpreadAngle + shotgunSpreadAngle, aimDirection);

            if (pooledBullet != null)
            {
                var pool = objectPoolManager.GetOrCreatePool(pooledBullet);
                bulletGo = pool.RetrieveFromPool().gameObject;
                bulletGo.transform.position = muzzlePosition;
                bulletGo.transform.rotation = bulletRotation;
            }
            else //called if not pooledBullet or failed retrieve
            {
                bulletGo = Instantiate(bulletPrefab, muzzlePosition, bulletRotation);
            }

            if (bulletGo.TryGetComponent(out IBullet bullet))
            {
                bullet.SetupBullet(config.bulletConfig);
            }
        }

        public float GetReloadingPercentage()
        {
            if (!isReloading) return 100f;
            return 100f - Mathf.Round(fireCooldown / config.secondsReload * 100f);
        }

        private Quaternion GetErrorQuaternion(float errorAngle, Vector3 originalDirection)
        {
            var errorVec = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            errorVec.Normalize();

            var errorRight = Quaternion.AngleAxis(errorVec.x * errorAngle, Vector3.up);
            var errorUp = Quaternion.AngleAxis(errorVec.y * errorAngle, Vector3.right);
            return errorRight * errorUp * Quaternion.LookRotation(originalDirection);
        }
    }
}