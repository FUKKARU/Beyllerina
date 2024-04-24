using System;
using System.Collections;
using UnityEngine;

public class SwordFadeOut : MonoBehaviour
{
    [NonSerialized] public bool fadeOutRequest = false;
    bool effectStart = false;
    Material myMaterial;

    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    void Update()
    {
        if (fadeOutRequest && !effectStart)
        {
            myMaterial = GetComponent<MeshRenderer>().material;
            StartCoroutine(FadeOut(myMaterial, 0, shaderVarRate, shaderVarRefreshRate));
            effectStart = true;
        }

    }


    IEnumerator FadeOut(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);
        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }

    }
}
