using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Volume volume;
    DepthOfField dOF;

    [SerializeField] List<GameObject> tutorialImages = new List<GameObject>();

    public bool tutorailFin { get; private set; }
    IEnumerator Start()
    {
        tutorailFin = false;
        volume.profile.TryGet(out dOF);
        dOF.focalLength.value = 300;

        foreach (GameObject img in tutorialImages)
        {
            img.SetActive(true);
            yield return new WaitForSeconds(2);
            img.SetActive(false);
        }

        dOF.focalLength.value = 1;
        tutorailFin = true;
        yield break;
    }


}
