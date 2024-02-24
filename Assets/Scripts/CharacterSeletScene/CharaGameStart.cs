using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaGameStart : MonoBehaviour
{
    private CharacterDate characterDate;

    [SerializeField]
    private Vector3 Player01position = new Vector3(-961f, 1, 0);
    [SerializeField]
    private Vector3 Player02position = new Vector3(-961f, 0, 0);



    void Start()
    {
      
        characterDate = FindObjectOfType<CharaGameManager>().GetCharacterDate();
        Instantiate(characterDate.GetPlayer01(), Player01position, Quaternion.identity);
        Instantiate(characterDate.GetPlayer02(), Player02position, Quaternion.identity);

    }
}
