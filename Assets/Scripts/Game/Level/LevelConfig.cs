using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/Level", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        public string[] LevelSceneNames;
    }
}