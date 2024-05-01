using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForQuestionMark : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] GameObject Chikuonki;
    [SerializeField]
    private AudioClip P_Sound;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = Chikuonki.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            audioSource.PlayOneShot(P_Sound);
        }
        
    }
}
