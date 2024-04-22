using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    float trailActiveTime = 2.0f;
    float meshRecretateRate = 0.1f;
    float meshDestroyTime = 3f;
    public Transform positionToSpawn;
    bool isTrailActive;
    public MeshRenderer[] usedMeshes;
    public Material mat;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(TrailCreate(trailActiveTime));
        }
    }

    IEnumerator TrailCreate(float timer)
    {
        while(timer > 0)
        {
            timer -= meshRecretateRate;
            /*
            for(int i = 0; i < usedMeshes.Length; i++) 
            {
                GameObject gO = new GameObject();
                gO.transform.SetPositionAndRotation(positionToSpawn.position,positionToSpawn.rotation);
                MeshRenderer mR =  gO.AddComponent<MeshRenderer>();
                MeshFilter mF = gO.AddComponent<MeshFilter>();

                


                mF.mesh = mesh;
                mR.material = mat;

                
            }
            */
            GameObject copy = Instantiate(gameObject);
            Destroy(copy, meshDestroyTime);
            yield return new WaitForSeconds(meshRecretateRate);
        }

        isTrailActive = false;
    }
}
