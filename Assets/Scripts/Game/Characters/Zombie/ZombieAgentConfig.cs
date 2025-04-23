using UnityEngine;

namespace Game.Characters.Zombie
{
    [CreateAssetMenu(fileName = "ZombieAgentConfig", menuName = "Configs/Agents/Zombie", order = 0)]
    public class ZombieAgentConfig : ScriptableObject
    {
        [Tooltip("Check navigation board")]
        public int agentType;
        public Vector2 minMaxWanderDistance;
        public LayerMask sightBlockerMask;
        public float maxSight;
        public float attackRange;
    }
}