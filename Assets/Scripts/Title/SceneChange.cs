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
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}