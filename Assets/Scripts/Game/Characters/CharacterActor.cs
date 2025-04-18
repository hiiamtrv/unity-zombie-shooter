using System;
using Game.Bullet;
using Game.Characters.Input;
using Game.Interfaces;
using UnityEngine;

namespace Game.Characters
{
    public class CharacterActor : MonoBehaviour, IDamageable
    {
        private static readonly int SpeedRatioZ = Animator.StringToHash("SpeedRatioZ");
        private static readonly int SpeedRatioX = Animator.StringToHash("SpeedRatioX");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

        [SerializeField]
        public CharacterConfig config;

        [SerializeField]
        private Animator animator;

        private ICharacterInput characterInput;
        private Rigidbody rb;
        private float health;

        public float Health => health;

        private void Awake()
        {
            health = config.initialHealth;
            characterInput = GetComponent<ICharacterInput>();
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (characterInput.LookAngle.HasValue)
            {
                var lookAngle = characterInput.LookAngle.Value;
                transform.rotation = Quaternion.AngleAxis(lookAngle, Vector3.up);
            }
        }

        void FixedUpdate()
        {
            var moveInput = characterInput.MoveInput;
            if (moveInput?.magnitude > 0.1f)
            {
                var inputDirection = new Vector3(moveInput.Value.x, 0, moveInput.Value.y);
                if (inputDirection.magnitude > 0.1f) inputDirection.Normalize();
                var targetVel = inputDirection * config.moveSpeed;
                var diffVel = targetVel - rb.velocity;

                rb.AddForce(diffVel / Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }

        private void LateUpdate()
        {
            var localVelocity = transform.InverseTransformVector(characterInput.Velocity);
            var isAttacking = characterInput.IsAttacking;
            animator.SetFloat(SpeedRatioZ, localVelocity.z);
            animator.SetFloat(SpeedRatioX, localVelocity.x);
            animator.SetBool(IsAttacking, isAttacking);
        }

        public bool CanHit(GameObject dmgSource)
        {
            return false;
        }

        public void DoDamage(DamageData damageData, GameObject dmgSource)
        {
            //do nothing
        }
    }
}