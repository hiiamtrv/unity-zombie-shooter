using System;
using System.Collections.Generic;
using System.Linq;
using Game.Interfaces;
using UnityEngine;

namespace Game.Characters
{
    public class CharacterWeaponHolder : MonoBehaviour
    {
        private ICharacterInput characterInput;

        [SerializeField]
        private GameObject[] weaponGameObject;

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
        }

        private void OnDisable()
        {
            characterInput.ReloadWeapon -= ReloadWeapon;
            characterInput.SwitchWeapon -= SwitchWeapon;
        }

        private void SwitchWeapon()
        {
            var numWeapons = weapons.Length;
            weaponIndex = (weaponIndex + 1) % numWeapons;
        }

        private void ReloadWeapon()
        {
            CurrentWeapon.TryReload();
        }

        private void Update()
        {
            if (characterInput.IsAttacking)
            {
                CurrentWeapon.TryAttack();
            }
        }
    }
}