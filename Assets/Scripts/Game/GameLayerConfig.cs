using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameLayerConfig", menuName = "Configs/GameLayer", order = 0)]
    public class GameLayerConfig : ScriptableObject
    {
        [SerializeField]
        private LayerMask layerMask;
        
        public LayerMask LayerMask => layerMask;

        public static int ComposeLayers(IEnumerable<GameLayerConfig> configs)
        {
            return configs.Aggregate(0, (current, config) => current | config.LayerMask);
        }
    }
}