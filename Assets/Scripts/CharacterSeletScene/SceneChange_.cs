using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CharacterSelect
{
    public class SceneChange_ : MonoBehaviour
    {
        [SerializeField] string SceneName;

        public void GameStart()
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}