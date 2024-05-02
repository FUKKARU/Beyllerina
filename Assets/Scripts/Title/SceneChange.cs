using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Title
{
    public class SceneChange : MonoBehaviour
    {
        Transform cursor;
        [SerializeField] GameObject quitChooseUI;
        const float cursorSpeedRaw = 6.5f;
        float cursorSpeed;
        const float cursorSpeedStep = 0.2f;
        [SerializeField] TextMeshProUGUI cursorSpeedText;
        float cursorSpeedTextShowTime = 0f; // Œ¸‚Á‚Ä‚¢‚­

        [SerializeField] Sprite playButtonInitial;
        [SerializeField] Sprite playButtonHover;
        [SerializeField] GameObject playButton;
        bool hover_playButton;

        [SerializeField] GameObject keyHelp;

        void PlayButton()
        {
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.CharacterSelect, true);

            BaseSystem.SoundManager.Instance.PlaySE(0);
        }

        [SerializeField] Sprite zeroOneButtonInitial;
        [SerializeField] Sprite zeroOneButtonHover;
        [SerializeField] GameObject zeroOneButton;
        bool hover_zeroOneButton;
        void ZeroOneButton()
        {
            zeroOneButton.GetComponent<SpriteRenderer>().sprite = zeroOneButtonInitial;
            playButton.GetComponent<BoxCollider2D>().enabled = false;
            zeroOneButton.GetComponent<CircleCollider2D>().enabled = false;

            eXIT_Button_Check_Banner.SetActive(true);
            eXIT_Button_Check_YesButton.SetActive(true);
            eXIT_Button_Check_CancelButton.SetActive(true);

            BaseSystem.SoundManager.Instance.PlaySE(0);
        }

        [SerializeField] Sprite eXIT_Button_Check_YesButton_Initial;
        [SerializeField] Sprite eXIT_Button_Check_YesButton_Hover;
        [SerializeField] GameObject eXIT_Button_Check_YesButton;
        bool hover_EXIT_Button_Check_YesButton;
        void EXIT_Button_Check_YesButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

            BaseSystem.SoundManager.Instance.PlaySE(0);
        }

        [SerializeField] Sprite eXIT_Button_Check_cancelButton_Initial;
        [SerializeField] Sprite eXIT_Button_Check_cancelButton_Hover;
        [SerializeField] GameObject eXIT_Button_Check_CancelButton;
        bool hover_EXIT_Button_Check_CancelButton;
        void EXIT_Button_Check_CancelButton()
        {
            playButton.GetComponent<BoxCollider2D>().enabled = true;
            zeroOneButton.GetComponent<CircleCollider2D>().enabled = true;
            eXIT_Button_Check_Banner.SetActive(false);
            eXIT_Button_Check_YesButton.SetActive(false);
            eXIT_Button_Check_CancelButton.SetActive(false);

            BaseSystem.SoundManager.Instance.PlaySE(0);
        }

        [SerializeField] GameObject eXIT_Button_Check_Banner;

        void InputMethod()
        {
            if (IA.InputGetter.Instance.IsSelect)
            {
                if (hover_playButton) PlayButton();
                else if(hover_zeroOneButton) ZeroOneButton();
                else if(hover_EXIT_Button_Check_YesButton) EXIT_Button_Check_YesButton();
                else if(hover_EXIT_Button_Check_CancelButton) EXIT_Button_Check_CancelButton();
            }
        }
        void Awake()
        {

            cursor = GetComponent<Transform>();
            playButton.GetComponent<BoxCollider2D>().enabled = true;
            zeroOneButton.GetComponent<CircleCollider2D>().enabled = true;
            eXIT_Button_Check_Banner.SetActive(false);
            eXIT_Button_Check_YesButton.SetActive(false);
            eXIT_Button_Check_CancelButton.SetActive(false);

        }

        void Start()
        {
            cursorSpeed = cursorSpeedRaw * BaseSystem.GameData.GameData.DirectionMoveSpeedCoef;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Alpha4))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    BaseSystem.GameData.GameData.DirectionMoveSpeedCoef += cursorSpeedStep;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    BaseSystem.GameData.GameData.DirectionMoveSpeedCoef -= cursorSpeedStep;
                }
                else return;

                cursorSpeed = cursorSpeedRaw * BaseSystem.GameData.GameData.DirectionMoveSpeedCoef;
                cursorSpeedTextShowTime = 1;
                cursorSpeedText.text = "~ " + BaseSystem.GameData.GameData.DirectionMoveSpeedCoef.ToString("F1"); // ¬”‘æˆêˆÊ‚Ü‚Å
                cursorSpeedText.enabled = true;
            }

            Vector2 val = IA.InputGetter.Instance.ValueDirection;
            Vector2 cPos = cursor.position;
            cPos.x = Mathf.Clamp(cPos.x, -10f, 10f);
            cPos.y = Mathf.Clamp(cPos.y, -4.5f, 6.5f);
            cursor.position = cPos; 
            cursor.position += new Vector3(val.x, val.y, 0) * cursorSpeed * Time.deltaTime;
            InputMethod();

            if (cursorSpeedTextShowTime > 0f)
            {
                cursorSpeedTextShowTime -= Time.deltaTime;

                if (cursorSpeedTextShowTime <= 0f)
                {
                    cursorSpeedTextShowTime = 0f;
                    cursorSpeedText.enabled = false;
                }
            }
        }

        public void QuitGame()
        {
            quitChooseUI.SetActive(true);
            
        }

        public void BackToGame()
        {
            quitChooseUI.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            GameObject obj = collider.gameObject;

            if(obj == playButton)
            {
                playButton.GetComponent<SpriteRenderer>().sprite = playButtonHover;
                hover_playButton = true;
                if (keyHelp != null) keyHelp.SetActive(true);
            }
            else if(obj == zeroOneButton)
            {
                zeroOneButton.GetComponent<SpriteRenderer>().sprite = zeroOneButtonHover;
                hover_zeroOneButton = true;
            }
            else if (obj == eXIT_Button_Check_CancelButton)
            {
                eXIT_Button_Check_CancelButton.GetComponent<SpriteRenderer>().sprite = eXIT_Button_Check_cancelButton_Hover;
                hover_EXIT_Button_Check_CancelButton = true;
            }
            else if (obj == eXIT_Button_Check_YesButton)
            {
                obj.GetComponent<SpriteRenderer>().sprite = eXIT_Button_Check_YesButton_Hover;
                hover_EXIT_Button_Check_YesButton = true;
            }

            BaseSystem.SoundManager.Instance.PlaySE(2);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            GameObject obj = collider.gameObject;

            if (obj == playButton)
            {
                playButton.GetComponent<SpriteRenderer>().sprite = playButtonInitial;
                hover_playButton = false;
                if (keyHelp != null) keyHelp.SetActive(false);
            }
            else if (obj == zeroOneButton)
            {
                zeroOneButton.GetComponent<SpriteRenderer>().sprite = zeroOneButtonInitial;
                hover_zeroOneButton = false;
            }
            else if (obj == eXIT_Button_Check_CancelButton)
            {
                eXIT_Button_Check_CancelButton.GetComponent<SpriteRenderer>().sprite = eXIT_Button_Check_cancelButton_Initial;
                hover_EXIT_Button_Check_CancelButton = false;
            }
            else if (obj == eXIT_Button_Check_YesButton)
            {
                eXIT_Button_Check_YesButton.GetComponent<SpriteRenderer>().sprite = eXIT_Button_Check_YesButton_Initial;
                hover_EXIT_Button_Check_YesButton = false;
            }

            //BaseSystem.SoundManager.Instance.PlaySE(1);
        }
    }
}