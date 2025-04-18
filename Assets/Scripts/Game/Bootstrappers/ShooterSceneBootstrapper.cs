using System;
using Base.Locator;
using Base.Pool;
using UnityEngine;

namespace Game.Bootstrapper
{
    public class ShooterSceneBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private ObjectPoolManager objectPoolManager;

        private void Awake()
        {
            Locator<ObjectPoolManager>.SetInstance(objectPoolManager);
        }
    }
}