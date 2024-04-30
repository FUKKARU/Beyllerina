using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSelect
{
    public class SceneChange : MonoBehaviour
    {
        
        public void Onclick()
        {
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
        }

    }

}