using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



[CreateAssetMenu(fileName = "CharacterDate", menuName = "CharacterDate")]
public class CharacterDate : ScriptableObject
{
    //　次のシーン名
    [SerializeField]
    private string nextSceneName;
    //　1P 2Pに代入
    [SerializeField]
    private GameObject Player01, Player02;
    //　データの初期化
    private void OnEnable()
    {
            //　タイトルシーンの時だけリセット
        if (SceneManager.GetActiveScene().name == "CharactorSellect")
        {
            nextSceneName = "";
            Player01 = null;
            Player02 = null; 
        }
    }

    public void SetNextSceneName(string nextSceneName)
    {
        this.nextSceneName = nextSceneName;
        
    }

   
    public void SetPlayer01(GameObject character01)
    {
       this.Player01 = character01;
        DontDestroyOnLoad(Player01);
    }

     public void SetPlayer02(GameObject character02)
    {
        this.Player02 = character02;
        DontDestroyOnLoad(Player02);
    }
    public GameObject GetPlayer01()
    {
        return Player01;
    }

    public GameObject GetPlayer02()
    {
        return Player02;
    }


}

  
    

