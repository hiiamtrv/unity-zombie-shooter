using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Characters
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Configs/Character", order = 0)]
    public class CharacterConfig : ScriptableObject
    {
        public float moveSpeed;
        public float initialHealth;
    }
}