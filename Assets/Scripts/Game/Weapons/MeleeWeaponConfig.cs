using Game.Bullet;
using UnityEngine;

namespace Game.Characters
{
    [CreateAssetMenu(fileName = "MeleeWeaponConfig", menuName = "Configs/MeleeWeapon", order = 0)]
    public class MeleeWeaponConfig : ScriptableObject
    {
        [Header("Attacks")]
        public float secondsBetweenAttacks;
        
        [Header("Damage")]
        public float damageRadius;
        public float damageFrontAngle;
        public DamageSourceConfig damageConfig;
    }
}