using Base.Pool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.SkinPool
{
    public class VisualPoolObject : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private int numPooledObject;

        public GameObject runtimeOwner;
        
        public Animator Animator;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        
        public void OnBeforeSpawn(bool isReused, int numActiveObjects)
        {
            Animator.Rebind();
        }

        public event IPoolable.PoolReturnHandler OnPoolReturn;
        public bool DDOL => true;
        public int PoolNumPooledObject => numPooledObject;

        public void ReturnToPool()
        {
            OnPoolReturn?.Invoke(this);
        }
    }
}