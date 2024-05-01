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
        tutorialImages[0].SetActive(true);
        yield return new WaitForSeconds(2);
        tutorialImages[0].SetActive(false);

        tutorialImages[1].SetActive(true);
        yield return new WaitForSeconds(2);
        tutorialImages[1].SetActive(false);

        tutorialImages[2].SetActive(true);
        yield return new WaitForSeconds(2);
        tutorialImages[2].SetActive(false);

        dOF.focalLength.value = 1;
        tutorailFin = true;
        yield break;
    }


}
