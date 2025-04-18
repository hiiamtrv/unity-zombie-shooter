using System.Text;
using Game.Characters;
using Game.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private CharacterActor watchPlayer;

        [SerializeField]
        private CharacterWeaponHolder watchWeaponHolder;

        [SerializeField]
        private TMP_Text playerText;

        private float health;
        private IWeapon currentWeapon;
        private int bulletsInMag;
        private int bulletsLeft;
        private bool infiniteBullet;

        StringBuilder sb = new StringBuilder();

        private void LateUpdate()
        {
            var changed = false;
            changed |= SetAndMarkChanged(ref health, watchPlayer.Health);
            changed |= SetAndMarkChanged(ref currentWeapon, watchWeaponHolder.CurrentWeapon);
            changed |= SetAndMarkChanged(ref bulletsInMag, currentWeapon.BulletsInMag);
            changed |= SetAndMarkChanged(ref bulletsLeft, currentWeapon.BulletsLeft);
            changed |= SetAndMarkChanged(ref infiniteBullet, currentWeapon.InfiniteBullet);

            if (changed)
            {
                playerText.text = RebuildSb();
            }
        }

        private string RebuildSb()
        {
            sb.Clear()
                .AppendJoin(" | ", health, currentWeapon.WeaponName)
                .Append(" ");
            
            if (infiniteBullet)
            {
                sb.Append(bulletsInMag);
            }
            else
            {
                sb.AppendJoin('/', bulletsInMag, bulletsLeft);
            }

            return sb.ToString();
        }

        private static bool SetAndMarkChanged<T>(ref T source, T target)
        {
            if (target.Equals(source)) return false;
            source = target;
            return true;
        }
    }
}