using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialPointer : MonoBehaviour
{
    [SerializeField] GameObject PageText;
    [SerializeField] GameObject cursor;
    [SerializeField] GameObject[] TutorialPage;
    [SerializeField] int Page = 0;
    const float cursorSpeed = 6.5f;
    bool EveryFlag;
    
    [SerializeField] GameObject ForwardButton;
    bool hover_ForwardButton;

   
    [SerializeField] GameObject ToBehindButton;
    bool hover_ToBehindButton;

    private void Update()
    {
        Text text = PageText.GetComponent<Text>();
        Vector3 val = IA.InputGetter.Instance.ValueDirection;
        
        
        cursor.transform.position += new Vector3(0, val.y, -val.x) * cursorSpeed * Time.deltaTime;
        Vector3 cPos = cursor.transform.position;
        cPos.y = Mathf.Clamp(cPos.y, -10f, 10f);
        cPos.z = Mathf.Clamp(cPos.z, -4.5f, 6.5f);
        //Debug.Log(cPos);
        cursor.transform.position = cPos;
        InputMethod();
        text.text = Page.ToString("0");
        Debug.Log(Page.ToString("0"));

    }
    

    void InputMethod()
    {
        

        if (EveryFlag == true && IA.InputGetter.Instance.IsSelect || IA.InputGetter.Instance.IsSelect)
        {
            Page ++;

            if (hover_ForwardButton == true) P_ForwardButton();
            else if (hover_ToBehindButton == true)
            {
                P_ToBehindButton();

            }

        }

        if (IA.InputGetter.Instance.IsCancel)
        {
            Page --;
            
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        

        
        if (collision.gameObject.name == "ForwardButton")
        {

            /*
            Debug.Log("ìñÇΩÇ¡ÇƒÇÈÇÊÅI");
            ForwardButton.GetComponent<SpriteRenderer>().sprite = ForwardButtonHover;
            */
            EveryFlag = true;
            hover_ForwardButton = true;
            
        }
        else if (collision.gameObject.name == "ToBehindButton")
        {
            /*
            Debug.Log("ìñÇΩÇ¡ÇƒÇÈÇÊÅI");
            ToBehindButton.GetComponent<SpriteRenderer>().sprite = ToBehindButtonHover;
            */
            EveryFlag = true;
            hover_ToBehindButton = true;
            
        }
        
        if (collision.gameObject.name ==  "NoForwardButton")
        {
            
        }
        else if (collision.gameObject.name == "NoForwardButton")
        {
            
        }
    }

    private void OnCollisionExit(Collision collision)
    {

       
        
        if (collision.gameObject.name != "ForwardButton")
        {
            EveryFlag = false;
            hover_ForwardButton = false;
        }
        //{
        //    ForwardButton.GetComponent<SpriteRenderer>().sprite = ForwardButtonInitial;
        //    hover_ForwardButton = false;
        //}
        else if (collision.gameObject.name != "ToBehindButton")
        {
            EveryFlag = false;
            hover_ToBehindButton = false;
        }
       // {
        //    ToBehindButton.GetComponent<SpriteRenderer>().sprite = ToBehindButtonInitial;
       //     hover_ToBehindButton = false;
       // }
        

        

    }

    void P_ForwardButton()
    {
        

        for (int i = 0; i < TutorialPage.Length; i++)
        {
            TutorialPage[i].gameObject.SetActive(false);
            TutorialPage[Page].gameObject.SetActive(true);

            
        }

        Debug.Log(Page);
        BaseSystem.SoundManager.Instance.PlaySE(0);

    }

    void P_ToBehindButton()
    {
        Page -= 2;

        for (int i = 0; i < TutorialPage.Length; i++)
        {
            TutorialPage[i].gameObject.SetActive(false);
            TutorialPage[Page].gameObject.SetActive(true);

            
        }
        Debug.Log(Page);
        BaseSystem.SoundManager.Instance.PlaySE(0);

    }
}
