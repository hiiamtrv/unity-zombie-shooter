using System;
using Audio;
using Base.Locator;
using Base.Pool;
using UnityEngine;

namespace Game.Bootstrapper
{
    public class ShooterSceneBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private ObjectPoolManager objectPoolManager;
        
        [SerializeField]
        private AudioSystem audioSystem;

        private void Awake()
        {
            Locator<ObjectPoolManager>.SetInstance(objectPoolManager);
            Locator<AudioSystem>.SetInstance(audioSystem);
        }
    }
}