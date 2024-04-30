using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeSub : MonoBehaviour
{
    Transform cursor;
    [SerializeField] GameObject quitChooseUI;
    const float cursorSpeed = 6.5f;

    [SerializeField] Sprite playButtonInitial;
    [SerializeField] Sprite playButtonHover;
    [SerializeField] GameObject playButton;
    bool hover_playButton;
    void PlayButton()
    {
        LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.CharacterSelect, true);
    }

    [SerializeField] Sprite zeroOneButtonInitial;
    [SerializeField] Sprite zeroOneButtonHover;
    [SerializeField] GameObject zeroOneButton;
    bool hover_zeroOneButton;
    void ZeroOneButton()
    {
        playButton.GetComponent<BoxCollider2D>().enabled = false;
        zeroOneButton.GetComponent<CircleCollider2D>().enabled = false;
        eXIT_Button_Check_Banner.SetActive(true);
        eXIT_Button_Check_YesButton.SetActive(true);
        eXIT_Button_Check_CancelButton.SetActive(true);
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
    }

    [SerializeField] GameObject eXIT_Button_Check_Banner;

    void Input()
    {
        if (IA.InputGetter.Instance.IsSelect)
        {
            print(1);
            if (hover_playButton) PlayButton();
            else if (hover_zeroOneButton) ZeroOneButton();
            else if (hover_EXIT_Button_Check_YesButton) EXIT_Button_Check_YesButton();
            else if (hover_EXIT_Button_Check_CancelButton) EXIT_Button_Check_CancelButton();
        }
    }
    void Awake()
    {
        cursor = GetComponent<Transform>();
        eXIT_Button_Check_Banner.SetActive(false);
        eXIT_Button_Check_YesButton.SetActive(false);
        eXIT_Button_Check_CancelButton.SetActive(false);
    }

    void Update()
    {
        Vector2 val = IA.InputGetter.Instance.ValueRotate;
        Vector2 cPos = cursor.position;
        cPos.x = Mathf.Clamp(cPos.x, -9.7f, 9.7f);
        cPos.y = Mathf.Clamp(cPos.y, -6.5f, 6.5f);
        cursor.position = cPos;
        cursor.position += new Vector3(val.x, val.y, 0) * cursorSpeed * Time.deltaTime;
        print(IA.InputGetter.Instance.IsSelect);
        Input();

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

        if (obj == playButton)
        {
            playButton.GetComponent<SpriteRenderer>().sprite = playButtonHover;
            hover_playButton = true;
        }
        else if (obj == zeroOneButton)
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
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        GameObject obj = collider.gameObject;

        if (obj == playButton)
        {
            playButton.GetComponent<SpriteRenderer>().sprite = playButtonInitial;
            hover_playButton = false;
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
    }
}
