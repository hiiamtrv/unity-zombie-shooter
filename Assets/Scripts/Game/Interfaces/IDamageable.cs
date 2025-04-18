using Game.Bullet;
using UnityEngine;

namespace Game.Interfaces
{
    public interface IDamageable
    {
        public bool CanHit(GameObject dmgSource);
        public void DoDamage(DamageData damageData, GameObject dmgSource);
    }
}