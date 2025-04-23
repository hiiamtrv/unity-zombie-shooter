using Game.Interfaces;
using UnityEngine;
using UnityEngine.GameInput;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Game.Characters.Input
{
    public class CharacterPlayerInput : MonoBehaviour, ICharacterInput
    {
        [SerializeField]
        private GameLayerConfig aimHitLayer;

        [SerializeField]
        private GameLayerConfig aimableLayer;

        [SerializeField]
        private Transform aimingMark;

        [SerializeField]
        private Transform muzzleTransform;

        [SerializeField]
        private float autoAimRadius;

        private GameInputActions.PlayerActions playerInput;

        private Vector2 moveInput;
        private Vector3 aimDirection;
        private bool isAttacking;
        private bool isAutoAttack;

        public Vector2? MoveInput => moveInput;
        public bool IsAttacking => isAttacking;
        public Vector3 Velocity => rb.velocity;
        public Vector3? AimDirection => aimDirection;
        public event ICharacterInput.SwitchWeaponHandler SwitchWeapon;
        public event ICharacterInput.ReloadWeaponHandler ReloadWeapon;

        private Rigidbody rb;
        private Collider lockedTarget;

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
            if (CanCancelLockTarget())
            {
                lockedTarget = null;
            }

            moveInput = playerInput.Move.ReadValue<Vector2>();
            isAttacking = isAutoAttack || playerInput.Attack.IsPressed();
            var lookDirection = playerInput.Look.ReadValue<Vector2>();

            if (lockedTarget)
            {
                AimAt(lockedTarget);
            }
            else if (lookDirection.SqrMagnitude() >= 0.01f)
            {
                //transfer from [-1, 1] to [0, 1] to match viewport 
                lookDirection = lookDirection * 0.5f + 0.5f * Vector2.one;
                var lookRay = Camera.main.ViewportPointToRay(lookDirection);
                UpdateAimDirection(lookRay);
            }
            else if (Mouse.current?.delta.magnitude > 0.1f)
            {
                var mousePos = Mouse.current.position.ReadValue();
                //cheat to cast from player head
                var mouseRay = Camera.main.ScreenPointToRay(mousePos);
                UpdateAimDirection(mouseRay);
            }
            else
            {
                AimNearestEnemy(out bool hasEnemy);
                if (!hasEnemy)
                {
                    aimingMark.gameObject.SetActive(false);
                }
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

            if (lockedTarget != null)
            {
                Debug.DrawLine(transform.position, lockedTarget.bounds.center, Color.red);
            }
        }

        private void UpdateAimDirection(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimHitLayer.LayerMask))
            {
                var enemyCheck = (1 << hit.transform.gameObject.layer) & aimableLayer.LayerMask.value;
                if (enemyCheck != 0)
                {
                    lockedTarget = hit.collider;
                    AimAt(lockedTarget);
                }
                else
                {
                    lockedTarget = null;
                    AimAt(hit.point);
                }
            }
        }

        private void AimNearestEnemy(out bool hasEnemy)
        {
            var enemies = Physics.OverlapSphere(muzzleTransform.position, autoAimRadius, aimableLayer.LayerMask);
            var bestSqrDist = -1f;
            hasEnemy = enemies.Length > 0;
            foreach (var enemy in enemies)
            {
                var direction = enemy.transform.position - muzzleTransform.position;
                var sqrDist = Vector3.SqrMagnitude(direction);
                if (bestSqrDist > 0 && bestSqrDist <= sqrDist) continue;

                bestSqrDist = sqrDist;
                AimAt(enemy);
            }
        }

        private void AimAt(Collider target)
        {
            var aimPoint = target.bounds.center;
            AimAt(aimPoint);
        }

        private void AimAt(Vector3 targetPos)
        {
            aimDirection = targetPos - muzzleTransform.position;
            aimingMark.transform.position = targetPos;
            aimingMark.gameObject.SetActive(true);
        }

        private bool CanCancelLockTarget()
        {
            return lockedTarget == null || !lockedTarget.enabled ||
                   playerInput.Look.IsPressed() || //request aim by gamepad
                   Mouse.current?.delta.magnitude > 0.1f; //request aim by mouse
        }
    }
}