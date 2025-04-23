using System.Linq;
using Game.Interfaces;
using Game.SkinPool;
using UnityEngine;

namespace Game.Characters
{
    public class CharacterWeaponHolder : MonoBehaviour, IVisualPoolObjectConsumer
    {
        private ICharacterInput characterInput;

        [SerializeField]
        private GameObject[] weaponGameObject;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Transform muzzleTransform;

        private IWeapon[] weapons;
        private int weaponIndex;

        public IWeapon CurrentWeapon => weapons[weaponIndex];

        private void OnEnable()
        {
            weapons = weaponGameObject
                .Select(go => go.GetComponent<IWeapon>())
                .Where(w => w != null)
                .ToArray();
            weaponIndex = 0;

            characterInput = GetComponent<ICharacterInput>();
            characterInput.ReloadWeapon += ReloadWeapon;
            characterInput.SwitchWeapon += SwitchWeapon;

            foreach (var w in weapons)
            {
                w.ReturnWeaponTransform();
            }

            EquipCurrentWeaponModel();
        }

        private void OnDisable()
        {
            characterInput.ReloadWeapon -= ReloadWeapon;
            characterInput.SwitchWeapon -= SwitchWeapon;

            foreach (var w in weapons)
            {
                w.ReturnWeaponTransform();
            }
        }

        private void SwitchWeapon()
        {
            var numWeapons = weapons.Length;
            if (numWeapons <= 1) return;

            weapons[weaponIndex].ReturnWeaponTransform();
            weaponIndex = (weaponIndex + 1) % numWeapons;

            EquipCurrentWeaponModel();
        }

        private void ReloadWeapon()
        {
            CurrentWeapon.TryReload();
        }

        private void Update()
        {
            if (characterInput.IsAttacking)
            {
                var lookDir = characterInput.AimDirection.GetValueOrDefault(transform.forward);
                CurrentWeapon.TryAttack(muzzleTransform.position, lookDir);
            }
        }

        private void EquipCurrentWeaponModel()
        {
            if (animator == null) return;
            
            var handTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
            CurrentWeapon.AssignWeaponTransformOnHand(handTransform);
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