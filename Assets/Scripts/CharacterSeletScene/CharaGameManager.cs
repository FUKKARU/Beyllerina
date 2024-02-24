using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaGameManager : MonoBehaviour
{
    private static CharaGameManager charaGameManager;
    //　ゲーム全体で管理するデータ
    [SerializeField]
    private CharacterDate characterDate = null;

    private void Awake()
    {
        
        if (charaGameManager == null)
        {
            charaGameManager = this;
            //ロードしても保持する
            DontDestroyOnLoad(this);
            
        }
        
    }
    //　MyGameManagerDataを返す
    public CharacterDate GetCharacterDate()
    {
        return characterDate;
    }
}
