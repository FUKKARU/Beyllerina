using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake_Battle : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;
    float duration = 0.3f;

    public void ShakeOn()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + UnityEngine.Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }
}

