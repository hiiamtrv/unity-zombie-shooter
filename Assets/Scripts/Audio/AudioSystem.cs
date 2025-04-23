using System;
using System.Collections.Generic;
using Base.Locator;
using Base.Pool;
using UnityEngine;

namespace Audio
{
    public class AudioSystem : MonoBehaviour
    {
        [SerializeField]
        private AudioPoolObject poolObjectPrefab;

        private Dictionary<int, float> lastPlayTime = new Dictionary<int, float>();

        public static void PlaySound(AudioClip clip, float playThreshold = 0.1f)
        {
            if (!clip) return;
            
            var self = Locator<AudioSystem>.Instance;
            if (!self) return;
            
            var now = Time.time;
            if (self.lastPlayTime.TryGetValue(clip.GetInstanceID(), out var prev) && now - prev <= playThreshold) return;

            self.lastPlayTime[clip.GetInstanceID()] = now;
           
            var poolManager = Locator<ObjectPoolManager>.Instance;
            var pool = poolManager.GetOrCreatePool(self.poolObjectPrefab);
            var newSoundObject = (AudioPoolObject)pool.RetrieveFromPool();
            newSoundObject.PlaySound(clip);
        }
    }
}