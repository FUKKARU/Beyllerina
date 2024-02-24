using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaGameManager : MonoBehaviour
{
    private static CharaGameManager charaGameManager;
    //�@�Q�[���S�̂ŊǗ�����f�[�^
    [SerializeField]
    private CharacterDate characterDate = null;

    private void Awake()
    {
        
        if (charaGameManager == null)
        {
            charaGameManager = this;
            //���[�h���Ă��ێ�����
            DontDestroyOnLoad(this);
            
        }
        
    }
    //�@MyGameManagerData��Ԃ�
    public CharacterDate GetCharacterDate()
    {
        return characterDate;
    }
}
