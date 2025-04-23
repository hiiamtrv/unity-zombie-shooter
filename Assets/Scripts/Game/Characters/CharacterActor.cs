using Audio;
using Base.Locator;
using Game.Bullet;
using Game.Interfaces;
using Game.SkinPool;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Characters
{
    public class CharacterActor : MonoBehaviour, IDamageable, IVisualPoolObjectConsumer
    {
        private static readonly int AnimSpeedRatioZ = Animator.StringToHash("SpeedRatioZ");
        private static readonly int AnimSpeedRatioX = Animator.StringToHash("SpeedRatioX");
        private static readonly int AnimIsAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int AnimIsDead = Animator.StringToHash("IsDead");
        private static readonly int AnimHit = Animator.StringToHash("Hit");

        [SerializeField]
        public CharacterConfig config;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private float paralyzedTimeOnHit;

        [SerializeField]
        private AudioClip hitSound;

        [SerializeField]
        private AudioClip dieSound;

        [SerializeField]
        private UnityEvent onDeath;

        private ICharacterInput characterInput;
        private Rigidbody rb;
        private Collider collider;
        private float health;

        public float Health => health;

        private float paralyzedCountdown;

        private void Awake()
        {
            ResetStats();
            characterInput = GetComponent<ICharacterInput>();
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (paralyzedCountdown > 0)
            {
                paralyzedCountdown -= Time.deltaTime;
            }

            if (paralyzedCountdown <= 0 && !IsDead() && characterInput.AimDirection.HasValue)
            {
                var lookDir = characterInput.AimDirection.Value;
                var lookAngle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(lookAngle, Vector3.up);
            }
        }

        private void FixedUpdate()
        {
            var moveInput = characterInput.MoveInput;
            if (!IsDead() && moveInput?.magnitude > 0.1f)
            {
                var inputDirection = new Vector3(moveInput.Value.x, 0, moveInput.Value.y);
                if (inputDirection.magnitude > 0.1f) inputDirection.Normalize();
                var targetVel = inputDirection * GetMoveSpeed();
                var diffVel = targetVel - rb.velocity + rb.velocity.y * Vector3.up;

                rb.AddForce(diffVel / Time.fixedDeltaTime, ForceMode.Force);
            }

            collider.enabled = !IsDead();
            rb.isKinematic = IsDead();
        }

        private void LateUpdate()
        {
            if (animator != null)
            {
                var localVelocity = transform.InverseTransformVector(characterInput.Velocity);
                var isAttacking = characterInput.IsAttacking;
                animator.SetFloat(AnimSpeedRatioZ, localVelocity.z);
                animator.SetFloat(AnimSpeedRatioX, localVelocity.x);
                animator.SetBool(AnimIsAttacking, isAttacking);
                animator.SetBool(AnimIsDead, IsDead());
            }
        }

        public bool CanHit(GameObject dmgSource)
        {
            return !IsDead();
        }

        public void DoDamage(DamageData damageData, GameObject dmgSource)
        {
            var isAliveBeforeHit = !IsDead();
            health -= damageData.damage;
            animator?.SetTrigger(AnimHit);

            if (isAliveBeforeHit)
            {
                Debug.Log($"Damaged {name} {health}");
                if (health <= 0f) //dead after this hit
                {
                    AudioSystem.PlaySound(dieSound);
                    onDeath?.Invoke();
                }
                else
                {
                    AudioSystem.PlaySound(hitSound);
                    SetParalyzedCountdown(paralyzedTimeOnHit);
                }
            }

            //should not go outside if clause because dead player must not receive damage
        }

        public bool IsDead()
        {
            return health <= 0f;
        }

        public void SetParalyzedCountdown(float seconds, bool isAdditive = false)
        {
            if (isAdditive)
            {
                paralyzedCountdown += seconds;
            }
            else
            {
                paralyzedCountdown = Mathf.Max(paralyzedCountdown, seconds);
            }
        }

        public void ResetStats()
        {
            health = config.initialHealth;
        }

        public float GetMoveSpeed()
        {
            return IsDead() || paralyzedCountdown > 0 ? 0f : config.moveSpeed;
        }

        public void LoadVisualPoolObject(VisualPoolObject visual)
        {
            animator = visual.Animator;
        }

        public void UnloadVisualPoolObject()
        {
            animator = null;
        }
    }
}