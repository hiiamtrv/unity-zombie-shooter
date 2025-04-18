using Game.Interfaces;
using UnityEngine;
using UnityEngine.GameInput;

namespace Game.Characters.Input
{
    public class CharacterPlayerInput : MonoBehaviour, ICharacterInput
    {
        private GameInputActions.PlayerActions playerInput;

        private Vector2 moveInput;
        private float lookAngle;
        private bool isAttacking;
        private bool isAutoAttack;

        public Vector2? MoveInput => moveInput;
        public float? LookAngle => lookAngle;
        public bool IsAttacking => isAttacking;
        public Vector3 Velocity => rb.velocity;
        public event ICharacterInput.SwitchWeaponHandler SwitchWeapon;
        public event ICharacterInput.ReloadWeaponHandler ReloadWeapon;

        private Rigidbody rb;

        private void Awake()
        {
            playerInput = new GameInputActions().Player;
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            playerInput.Enable();
        }

        private void OnDisable()
        {
            playerInput.Disable();
        }

        private void Update()
        {
            moveInput = playerInput.Move.ReadValue<Vector2>();
            isAttacking = isAutoAttack || playerInput.Attack.IsPressed();

            if (playerInput.Look.IsPressed())
            {
                var lookDirection = playerInput.Look.ReadValue<Vector2>();
                lookAngle = Mathf.Atan2(lookDirection.x, lookDirection.y) * Mathf.Rad2Deg;
            }

            if (playerInput.Reload.WasPressedThisFrame())
            {
                ReloadWeapon?.Invoke();
            }

            if (playerInput.SwitchWeapon.WasPressedThisFrame())
            {
                isAutoAttack = false;
                SwitchWeapon?.Invoke();
            }

            if (playerInput.AutoAttack.WasPressedThisFrame())
            {
                isAutoAttack = !isAutoAttack;
            }
        }
    }
}