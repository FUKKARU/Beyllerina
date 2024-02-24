using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private CharacterDate characterDate;

   [SerializeField]
    private string SceneName;
  

    
       
    

    public void GameStart()
    {
  
        SceneManager.LoadScene(SceneName);
        
    }

    
}
