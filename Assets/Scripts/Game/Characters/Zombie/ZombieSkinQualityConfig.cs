using System;
using UnityEngine;

namespace Game.Characters.Zombie
{
    [Serializable]
    public struct ZombieSkinQualityItem
    {
        public int maxAmount;
        public SkinQuality skinQuality;
    }

    [CreateAssetMenu(fileName = "ZombieSkinQualityConfig", menuName = "Configs/ZombieSkinQuality", order = 0)]
    public class ZombieSkinQualityConfig : ScriptableObject
    {
        [SerializeField]
        private ZombieSkinQualityItem[] qualityItems;

        [SerializeField]
        private SkinQuality defaultSkinQuality;

        private int minThreshold = -1;
        private int maxThreshold = -1;
        
        public SkinQuality GlobalSkinQuality { get; private set; }

        public void RecalculateSkinQuality(int numZombies)
        {
            //avoid repetitive cal on small changes
            if (minThreshold <= numZombies && numZombies <= maxThreshold) return;

            foreach (var qualityItem in qualityItems)
            {
                if (numZombies > qualityItem.maxAmount)
                {
                    minThreshold = qualityItem.maxAmount + 1;
                    continue;
                }

                GlobalSkinQuality = qualityItem.skinQuality;
                maxThreshold = qualityItem.maxAmount;
                return;
            }

            GlobalSkinQuality = defaultSkinQuality;
            maxThreshold = int.MaxValue - 1;
        }
    }
}