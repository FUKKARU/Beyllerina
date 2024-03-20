using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterSelect
{
    public class CameraShake_CS : MonoBehaviour
    {
        [SerializeField] AnimationCurve curve;
        float duration = 0.3f;

        void Start () 
        {
            StartCoroutine(ShakeRoutine());
        }


        IEnumerator ShakeRoutine()
        {
            yield return new WaitForSeconds(5);
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
            StartCoroutine(ShakeRoutine());
        }
    }
}


