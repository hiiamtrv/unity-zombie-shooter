using Audio;
using Game.Bullet;
using Game.Interfaces;
using UnityEngine;

namespace Game.Environments
{
    public class ExplodeBarrel : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private DamageSourceConfig damageConfig;

        [SerializeField]
        private SphereCollider explodeCollider; //for radius extraction only, do not active this

        [SerializeField]
        private float health;

        [SerializeField]
        private GameLayerConfig[] ExplodeLayers;

        [SerializeField]
        private GameObject hitFx;

        [SerializeField]
        private AudioClip explodeSound;

        public bool CanHit(GameObject dmgSource)
        {
            return health >= 0;
        }

        public void DoDamage(DamageData damageData, GameObject dmgSource)
        {
            health -= damageData.damage;
            if (health <= 0) Explode();
        }

        private void Explode()
        {
            var hitLayer = GameLayerConfig.ComposeLayers(ExplodeLayers);
            var radius = explodeCollider.radius;

            var colliders = Physics.OverlapSphere(explodeCollider.bounds.center, radius, hitLayer);
            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out IDamageable damageable) && damageable.CanHit(gameObject))
                {
                    var dmg = damageConfig.GetDamage();
                    damageable.DoDamage(dmg, gameObject);
                }
            }

            Instantiate(hitFx, transform.position, Quaternion.identity);
            AudioSystem.PlaySound(explodeSound);
            Destroy(gameObject); //no need to pool ?
        }
    }
}