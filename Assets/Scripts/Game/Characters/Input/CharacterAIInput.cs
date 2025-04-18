using Game.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Characters.Input
{
    public class CharacterAIInput : MonoBehaviour, ICharacterInput
    {
        public Vector2? MoveInput => null; //let agent do the movement
        public float? LookAngle => null; //let agent do the rotation
        public bool IsAttacking => false;
        public Vector3 Velocity => agent?.velocity ?? Vector3.zero;
        public event ICharacterInput.SwitchWeaponHandler SwitchWeapon;
        public event ICharacterInput.ReloadWeaponHandler ReloadWeapon;

        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }
}