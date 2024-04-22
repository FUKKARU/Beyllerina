using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    public class CameraShake_Battle : MonoBehaviour
    {
        [SerializeField] AnimationCurve curve;
        float duration = 0.3f;

        RectTransform rectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

        }

        public void ShakeOn()
        {
            StartCoroutine(Shake());
        }

        IEnumerator Shake()
        {

            Vector3 startPosition = rectTransform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float strength = curve.Evaluate(elapsedTime / duration);
                rectTransform.position = startPosition + UnityEngine.Random.insideUnitSphere * strength;
                yield return null;
            }

            rectTransform.position = startPosition;
        }
    }
}