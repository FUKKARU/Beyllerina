using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Win
{
    public class SceneChange : MonoBehaviour
    {
        void Update()
        {
            if (IA.InputGetter.Instance.IsSelect)
            {
                LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Title, true);
            }
            else if (IA.InputGetter.Instance.IsQuit)
            {
                LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Title, true);
            }
        }
    }
}