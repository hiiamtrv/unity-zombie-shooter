using UnityEngine;

namespace Game.Interfaces
{
    public interface ICharacterInput
    {
        public delegate void SwitchWeaponHandler();

        public delegate void ReloadWeaponHandler();

        public Vector2? MoveInput { get; }
        public float? LookAngle { get; }
        public bool IsAttacking { get; }
        public Vector3 Velocity { get; }

        public event SwitchWeaponHandler SwitchWeapon;
        public event ReloadWeaponHandler ReloadWeapon;
    }
}