using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    public class MeshTrail : MonoBehaviour
    {
        float meshRecretateRate = 0.025f;
        float meshDestroyTime = 3f;
        public Transform positionToSpawn;
        public bool isTrailActive;
        public MeshRenderer[] usedMeshes;
        public Material effectMat;
        public SwordFadeOut swordFadeOut;

        public IEnumerator TrailCreate()
        {
            while (true)
            {
                GameObject copy = Instantiate(gameObject, transform.position, transform.rotation, GameManager.Instance.rapier_effect_parent);
                copy.GetComponent<MeshRenderer>().material = effectMat;
                copy.GetComponent<SwordFadeOut>().fadeOutRequest = true;
                copy.GetComponent<BoxCollider>().enabled = false;
                Destroy(copy, meshDestroyTime);
                yield return new WaitForSeconds(meshRecretateRate);
            }
        }
    }
}