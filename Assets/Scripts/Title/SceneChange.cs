using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{


    public class SceneChange : MonoBehaviour
    {

        [SerializeField] GameObject quitChooseUI;
        void Awake()
        {
            IA.InputGetter.Instance.IsSelect = false;
            IA.InputGetter.Instance.IsSelect = false;
            quitChooseUI.SetActive(false);
        }

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

        public void QuitGame()
        {
            quitChooseUI.SetActive(true);
            
        }

        public void ApplicationShutDown()
        {
            IA.InputGetter.Instance.IsQuit = true;
        }

        public void BackToGame()
        {
            quitChooseUI.SetActive(false);
        }

        public void StartGame()
        {
            IA.InputGetter.Instance.IsSelect = true;
        }
    }


}