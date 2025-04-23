using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Game.Characters.Zombie
{
    [Serializable]
    public class ZombieSkinQualityItem
    {
        public int maxAmount;
        public SkinQuality skinQuality;
        public ShadowCastingMode shadowCastingMode;
        public bool receiveShadows;
    }

    [CreateAssetMenu(fileName = "ZombieSkinQualityConfig", menuName = "Configs/ZombieSkinQuality", order = 0)]
    public class ZombieSkinQualityConfig : ScriptableObject
    {
        [SerializeField]
        private ZombieSkinQualityItem[] qualityItems;

        [SerializeField]
        private ZombieSkinQualityItem defaultSkinQuality;

        private int minThreshold = -1;
        private int maxThreshold = -1;
        
        public ZombieSkinQualityItem GlobalSkinQuality { get; private set; }

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

                GlobalSkinQuality = qualityItem;
                maxThreshold = qualityItem.maxAmount;
                return;
            }

            GlobalSkinQuality = defaultSkinQuality;
            maxThreshold = int.MaxValue - 1;
        }
    }
}