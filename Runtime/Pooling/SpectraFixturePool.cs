using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraFixturePool : UdonSharpBehaviour
    {
        public GameObject[] pooledObjects;
        public bool disableUnusedAtStart = true;

        private void Start()
        {
            if (!disableUnusedAtStart || pooledObjects == null) return;

            for (int i = 0; i < pooledObjects.Length; i++)
            {
                if (pooledObjects[i] != null)
                {
                    pooledObjects[i].SetActive(false);
                }
            }
        }

        public GameObject Acquire()
        {
            if (pooledObjects == null) return null;

            for (int i = 0; i < pooledObjects.Length; i++)
            {
                GameObject candidate = pooledObjects[i];
                if (candidate != null && !candidate.activeSelf)
                {
                    candidate.SetActive(true);
                    return candidate;
                }
            }

            return null;
        }

        public void Release(GameObject target)
        {
            if (target != null)
            {
                target.SetActive(false);
            }
        }

        public void ReleaseAll()
        {
            if (pooledObjects == null) return;

            for (int i = 0; i < pooledObjects.Length; i++)
            {
                if (pooledObjects[i] != null)
                {
                    pooledObjects[i].SetActive(false);
                }
            }
        }
    }
}
