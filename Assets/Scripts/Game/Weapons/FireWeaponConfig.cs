using UnityEngine;

namespace Game.Characters
{
    [CreateAssetMenu(fileName = "FireWeaponConfig", menuName = "Configs/FireWeapon", order = 0)]
    public class FireWeaponConfig : ScriptableObject
    {
        [Header("Stats")]
        public int magazineSize;

        public int initialBullets;
        public bool infiniteBullet;

        public float secondsReload;

        [Header("Firing")]
        public float secondsBetweenShots;
    }
}