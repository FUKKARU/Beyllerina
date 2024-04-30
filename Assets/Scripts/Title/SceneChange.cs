using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public class SceneChange : MonoBehaviour
    {
        RectTransform cursor;
        [SerializeField] GameObject quitChooseUI;
        const float cursorSpeed = 200f;

        void Awake()
        {
            cursor = GetComponent<RectTransform>();
            quitChooseUI.SetActive(false);
        }

        void Update()
        {
            Vector2 val = IA.InputGetter.Instance.ValueRotate;
            val.x = Mathf.Clamp(val.x, -400, 400);
            val.y = Mathf.Clamp(val.y, -300, 300);
            cursor.localPosition += new Vector3(val.x, val.y, 0) * cursorSpeed * Time.deltaTime;

            // 上に載っているか判定、フラグを変える

            // フラグによってUIの見た目を変えたり

            if (true/*押せる状態になっている*/)
            {
                // ↓↓↓こんな感じに処理する
                if (IA.InputGetter.Instance.IsSelect)
                {
                    LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.CharacterSelect, true);
                }

                if (IA.InputGetter.Instance.IsCancel)
                {
                    LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.CharacterSelect, true);
                }
            }
            else if (true/*べつのUIのフラグ*/)
            {

            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void QuitGame()
        {
            quitChooseUI.SetActive(true);
            
        }

        public void BackToGame()
        {
            quitChooseUI.SetActive(false);
        }
    }
}