using UnityEngine;

namespace Game.Characters.Zombie
{
    [CreateAssetMenu(fileName = "ZombieAgentConfig", menuName = "Configs/Agents/Zombie", order = 0)]
    public class ZombieAgentConfig : ScriptableObject
    {
        public Vector2 minMaxWanderDistance;
    }
}