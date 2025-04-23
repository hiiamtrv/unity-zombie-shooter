using System;
using System.Collections;
using Base.Pool;
using UnityEngine;

namespace Audio
{
    public class AudioPoolObject : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private int poolSize;

        [SerializeField]
        private AudioSource source;

        public void OnBeforeSpawn(bool isReused, int numActiveObjects)
        {
        }

        public event IPoolable.PoolReturnHandler OnPoolReturn;
        public bool DDOL => true;
        public int PoolCapacity => poolSize;

        private void OnEnable()
        {
            StopAllCoroutines();
        }

        public void StopPlaying()
        {
           source.Stop();
           OnPoolReturn?.Invoke(this);
        }

        public void PlaySound(AudioClip clip)
        {
            source.clip = clip;
            source.Play();
            StartCoroutine(IeDisableWhenDonePlaying());
        }

        IEnumerator IeDisableWhenDonePlaying()
        {
            yield return new WaitUntil(() => !source.isPlaying);
            OnPoolReturn?.Invoke(this);
        }
    }
}