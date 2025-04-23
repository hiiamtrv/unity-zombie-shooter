using UnityEngine;

namespace Game.Interfaces
{
    public interface IWeapon
    {
        public void TryAttack(Vector3 muzzlePosition, Vector3 aimDirection);
        public void TryReload();

        public string WeaponName { get; }
        public int BulletsInMag { get; }
        public int BulletsLeft { get; }
        public bool InfiniteBullet { get; }

        public void ReturnWeaponTransform();
        public void AssignWeaponTransformOnHand(Transform handTransform);
        public float GetReloadingPercentage();
    }
}