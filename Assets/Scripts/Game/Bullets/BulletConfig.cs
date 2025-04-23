using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Bullet
{
    [Serializable]
    public class BulletConfig: DamageSourceConfig
    {
        [SerializeField]
        private float maxFlyDistance;
        
        public float MaxFlyDistance => maxFlyDistance;
    }
}