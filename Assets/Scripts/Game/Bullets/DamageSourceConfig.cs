using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Bullet
{
    [Serializable]
    public class DamageSourceConfig
    {
        [SerializeField]
        protected float damage;

        [SerializeField]
        protected float critRate;

        [SerializeField]
        protected float critMultiplier;

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