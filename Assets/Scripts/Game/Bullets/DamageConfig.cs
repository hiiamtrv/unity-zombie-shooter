using System.Collections.Generic;
using Base.Pool;
using UnityEngine;

namespace Game.Bullet
{
    [CreateAssetMenu(fileName = "DamageConfig", menuName = "Configs/Damage", order = 0)]
    public class DamageConfig : ScriptableObject
    {
        [SerializeField]
        private float damage;

        [SerializeField]
        private float critRate;

        [SerializeField]
        private float critMultiplier;

        public DamageData GetDamage()
        {
            var isCrit = Random.value < critRate;
            var dmgAmount = damage * (isCrit ? critMultiplier : 1);
            return new DamageData
            {
                damage = dmgAmount,
                isCrit = isCrit,
            };
        }
    }
}