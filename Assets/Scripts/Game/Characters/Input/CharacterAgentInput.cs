using Game.Characters.Zombie;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Characters.Input
{
    public class CharacterAgentInput : MonoBehaviour, ICharacterInput
    {
        private bool isAttacking;
        public Vector2? MoveInput => null; //let agent do the movement
        public Vector3? AimDirection => null; //let agent do the rotation
        public bool IsAttacking => isAttacking;
        public Vector3 Velocity => agent?.velocity ?? Vector3.zero;
        public event ICharacterInput.SwitchWeaponHandler SwitchWeapon;
        public event ICharacterInput.ReloadWeaponHandler ReloadWeapon;

        private NavMeshAgent agent;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        public void SetAttacking(bool attacking)
        {
            isAttacking = attacking;
        }
    }
}