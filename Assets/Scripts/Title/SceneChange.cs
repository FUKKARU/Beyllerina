using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public class SceneChange : MonoBehaviour
    {
        void Update()
        {
            if (IA.InputGetter.Instance.IsSelect)
            {
                LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.CharacterSelect, true);
            }
            else if (IA.InputGetter.Instance.IsQuit)
            {

            }
        }
    }
}