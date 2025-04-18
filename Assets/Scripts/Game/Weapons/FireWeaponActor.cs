using Base.Locator;
using Base.Pool;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

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
        private Transform muzzleTransform;

        private int bulletsLeft;
        private int bulletsInMag;
        private float cooldown;
        private bool isReloading;

        private IPoolable pooledBullet;
        private ObjectPoolManager objectPoolManager;

        public string WeaponName => weaponName;

        public int BulletsInMag => bulletsInMag;
        public int BulletsLeft => bulletsLeft;
        public bool InfiniteBullet => config.infiniteBullet;

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
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                {
                    if (isReloading) FulfillBullets();
                    isReloading = false;
                }
            }

            Debug.DrawRay(muzzleTransform.position, muzzleTransform.forward, Color.red, Time.deltaTime);
        }

        public void TryAttack()
        {
            if (cooldown > 0) return;
            if (bulletsInMag <= 0) return;
            Fire();
        }

        private void Fire()
        {
            GameObject bulletGo;
            if (pooledBullet != null)
            {
                var pool = objectPoolManager.GetOrCreatePool(pooledBullet);
                bulletGo = pool.RetrieveFromPool().gameObject;
                bulletGo.transform.position = muzzleTransform.position;
                bulletGo.transform.rotation = muzzleTransform.rotation;
            }
            else //called if not pooledBullet or failed retrieve
            {
                bulletGo = Instantiate(bulletPrefab, muzzleTransform.position, muzzleTransform.rotation);
            }

            if (bulletGo.TryGetComponent(out IBullet bullet))
            {
                bullet.SetupBullet();
            }

            bulletsInMag--;
            if (bulletsInMag <= 0)
            {
                Reload();
            }
            else
            {
                cooldown = config.secondsBetweenShots;
            }
        }

        public void TryReload()
        {
            if (cooldown > 0) return;
            if (bulletsLeft <= 0) return;
            Reload();
        }

        private void Reload()
        {
            cooldown = config.secondsReload;
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
    }
}