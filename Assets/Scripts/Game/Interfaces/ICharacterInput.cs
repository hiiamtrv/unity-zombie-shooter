using UnityEngine;

namespace Game.Interfaces
{
    public interface ICharacterInput
    {
        public delegate void SwitchWeaponHandler();

        public delegate void ReloadWeaponHandler();

        public Vector2? MoveInput { get; }
        public bool IsAttacking { get; }
        public Vector3 Velocity { get; }
        public Vector3? AimDirection { get; }

        public event SwitchWeaponHandler SwitchWeapon;
        public event ReloadWeaponHandler ReloadWeapon;
    }
}