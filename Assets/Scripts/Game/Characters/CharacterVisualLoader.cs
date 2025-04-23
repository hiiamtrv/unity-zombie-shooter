using System;
using System.Linq;
using Base.Locator;
using Base.Pool;
using Game.Interfaces;
using Game.SkinPool;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Characters
{
    public class CharacterVisualLoader : MonoBehaviour
    {
        [SerializeField]
        private VisualPoolObject prefab;

        [SerializeField]
        private MeshRenderer placeHolderMesh;

        [SerializeField]
        private float initialInactiveCountdown;

        private IVisualPoolObjectConsumer[] consumers;
        private VisualPoolObject currentVisual;
        private bool isUsingVisual;
        private float inactiveCountdown;

        private static Vector3 offscreenVector = new Vector3(9999, 9999, 9999);

        private void Awake()
        {
            isUsingVisual = false;
        }

        private void Start()
        {
            consumers = GetComponents<IVisualPoolObjectConsumer>();
            Debug.Log($"Num consumers {consumers.Length}");
        }

        private void Update()
        {
            if (isUsingVisual)
            {
                if (!currentVisual)
                {
                    UnloadVisual();
                    return;
                }

                if (!currentVisual.skinnedMeshRenderer.isVisible)
                {
                    inactiveCountdown -= Time.deltaTime;
                    if (inactiveCountdown <= 0)
                    {
                        UnloadVisual();
                        return;
                    }
                }
                else
                {
                    inactiveCountdown = initialInactiveCountdown;
                }

                currentVisual.transform.SetPositionAndRotation(
                    transform.position,
                    transform.rotation
                );
            }
            else
            {
                if (placeHolderMesh.isVisible)
                {
                    LoadVisual();
                    inactiveCountdown = initialInactiveCountdown;
                }
            }
        }

        private void UnloadVisual()
        {
            isUsingVisual = false;
            placeHolderMesh.gameObject.SetActive(true);

            if (currentVisual != null)
            {
                currentVisual.ReturnToPool();
                currentVisual.transform.position = offscreenVector;
                currentVisual.runtimeOwner = null;
                currentVisual = null;
                //broadcast for consumer
                foreach (var consumer in consumers)
                {
                    consumer.UnloadVisualPoolObject();
                }
            }
        }

        private void LoadVisual()
        {
            isUsingVisual = true;
            placeHolderMesh.gameObject.SetActive(false);

            var sys = Locator<ObjectPoolManager>.Instance;
            if (!sys)
            {
                UnloadVisual();
                return;
            }

            var pool = sys.GetOrCreatePool(prefab);
            currentVisual = (VisualPoolObject)pool.RetrieveFromPool(out _, false);
            currentVisual.runtimeOwner = gameObject;
            currentVisual.gameObject.SetActive(true); //ensure visual has valid state before shown

            //broadcast for consumers
            foreach (var consumer in consumers)
            {
                consumer.LoadVisualPoolObject(currentVisual);
            }
        }
    }
}