using System;
using UnityEngine;

namespace Game.Utils
{
    [RequireComponent(typeof(Collider))]
    public class TriggerDelegator : MonoBehaviour
    {
        public Action<Collider> OnDelegatedTriggerEnter;
        public Action<Collider> OnDelegatedTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            OnDelegatedTriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnDelegatedTriggerExit?.Invoke(other);
        }
    }
}