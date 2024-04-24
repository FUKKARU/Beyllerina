using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rotate
{
    public class SceneChange : MonoBehaviour
    {
        void Update()
        {
            if (IA.InputGetter.Instance.IsQuit)
            {
                LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Title, true);
            }
        }
    }
}