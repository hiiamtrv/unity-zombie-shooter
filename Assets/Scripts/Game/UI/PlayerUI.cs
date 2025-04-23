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
        private LevelController levelController;

        [SerializeField]
        private TMP_Text playerText;
        
        [SerializeField]
        private TMP_Text levelText;

        private float health;
        private IWeapon currentWeapon;
        private int bulletsInMag;
        private int bulletsLeft;
        private bool infiniteBullet;
        private float reloadingPercentage;
        private int secondsLeft;

        StringBuilder sb = new StringBuilder();

        private void LateUpdate()
        {
            var changed = false;
            changed |= SetAndMarkChanged(ref health, watchPlayer.Health);
            changed |= SetAndMarkChanged(ref currentWeapon, watchWeaponHolder.CurrentWeapon);
            changed |= SetAndMarkChanged(ref bulletsInMag, currentWeapon.BulletsInMag);
            changed |= SetAndMarkChanged(ref bulletsLeft, currentWeapon.BulletsLeft);
            changed |= SetAndMarkChanged(ref infiniteBullet, currentWeapon.InfiniteBullet);
            changed |= SetAndMarkChanged(ref reloadingPercentage, currentWeapon.GetReloadingPercentage());

            if (changed)
            {
                playerText.text = RebuildSb();
            }
            
            changed = SetAndMarkChanged(ref secondsLeft, Mathf.CeilToInt(levelController.LevelSeconds));
            if (changed)
            {
                levelText.text = $"Stay alive in {secondsLeft}s";
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

            if (reloadingPercentage < 100f)
            {
                sb.Append($" (Reloading {reloadingPercentage}%)");
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