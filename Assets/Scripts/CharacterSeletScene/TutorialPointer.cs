using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

namespace CharacterSelect
{
    public class TutorialPointer : MonoBehaviour
    {
        [SerializeField] GameObject PageText, AllPageText, AllAboutTutorial;
        [SerializeField] GameObject cursor, NoForwardButton, NoBehindButton, Chikuonki, Barellia01, CameraRotateScri, NewCursor;
        [SerializeField] GameObject[] TutorialPage;
        [SerializeField] int Page = 0;
        const float cursorSpeed = 6.5f;
        [SerializeField] AudioClip ClickSound;
        AudioSource audioSource;
        [SerializeField] GameObject[] keyHelps;
        [SerializeField] CameraShake_CS cameraShakeScript;
        [SerializeField] Image crosshair;

        bool EveryFlag;

        [SerializeField] GameObject ForwardButton;
        bool hover_ForwardButton;


        [SerializeField] GameObject ToBehindButton;
        bool hover_ToBehindButton;

        private void Start()
        {
            audioSource = Chikuonki.GetComponent<AudioSource>();
        }
        private void Update()
        {
            int PageForText = Page + 1;
            TextMeshProUGUI text = PageText.GetComponent<TextMeshProUGUI>();
            Vector3 val = IA.InputGetter.Instance.ValueDirection;


            cursor.transform.position += new Vector3(0, val.y, -val.x) * cursorSpeed * Time.deltaTime;
            Vector3 cPos = cursor.transform.position;
            cPos.y = Mathf.Clamp(cPos.y, -10f, 10f);
            cPos.z = Mathf.Clamp(cPos.z, -4.5f, 6.5f);
            //Debug.Log(cPos);
            cursor.transform.position = cPos;
            InputMethod();
            text.text = PageForText.ToString("0");
            //Debug.Log(Page.ToString("0"));

        }


        void InputMethod()
        {


            if (EveryFlag == true && IA.InputGetter.Instance.IsSelect || IA.InputGetter.Instance.IsSelect)
            {
                Page++;
                Page = Mathf.Clamp(Page, 0, TutorialPage.Length);

                if (hover_ForwardButton == true) P_ForwardButton();
                else if (hover_ToBehindButton == true)
                {
                    P_ToBehindButton();

                }


                for (int i = 0; i < TutorialPage.Length; i++)
                {

                    TutorialPage[i].gameObject.SetActive(false);

                    if (Page < TutorialPage.Length)
                    {
                        TutorialPage[Page].gameObject.SetActive(true);
                    }


                }


            }

            if (IA.InputGetter.Instance.IsCancel)
            {
                Page--;
                Page = Mathf.Clamp(Page, 0, TutorialPage.Length);

                for (int i = 0; i < TutorialPage.Length; i++)
                {
                    TutorialPage[i].gameObject.SetActive(false);

                    if (Page < TutorialPage.Length)
                    {
                        TutorialPage[Page].gameObject.SetActive(true);

                    }


                }

            }

            if (Page == 3)
            {

                PageText.SetActive(false);
                AllPageText.SetActive(false);
                Barellia01.SetActive(true);
                AllAboutTutorial.SetActive(false);
                cursor.SetActive(false);
                NewCursor.SetActive(true);
                CameraRotateScri.GetComponent<CameraRotate>().enabled = true;
                foreach (GameObject e in keyHelps) e.SetActive(false);
                cameraShakeScript.enabled = true;
                crosshair.enabled = true;

                this.enabled = false;
            }

            if (Page >= 2)
            {
                NoForwardButton.SetActive(true);
                ForwardButton.SetActive(false);
            }
            else
            {
                NoForwardButton.SetActive(false);
                ForwardButton.SetActive(true);

            }

            if (Page <= 0)
            {
                NoBehindButton.SetActive(true);
                ToBehindButton.SetActive(false);

            }
            else
            {
                NoBehindButton.SetActive(false);
                ToBehindButton.SetActive(true);

            }



        }

        private void OnCollisionEnter(Collision collision)
        {



            if (collision.gameObject.name == "ForwardButton")
            {
                audioSource.PlayOneShot(ClickSound);
                /*
                Debug.Log("ìñÇΩÇ¡ÇƒÇÈÇÊÅI");
                ForwardButton.GetComponent<SpriteRenderer>().sprite = ForwardButtonHover;
                */
                EveryFlag = true;
                hover_ForwardButton = true;

            }
            else if (collision.gameObject.name == "ToBehindButton")
            {
                audioSource.PlayOneShot(ClickSound);
                /*
                Debug.Log("ìñÇΩÇ¡ÇƒÇÈÇÊÅI");
                ToBehindButton.GetComponent<SpriteRenderer>().sprite = ToBehindButtonHover;
                */
                EveryFlag = true;
                hover_ToBehindButton = true;

            }

            if (collision.gameObject.name == "NoForwardButton")
            {

            }
            else if (collision.gameObject.name == "NoBehindButton")
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

                if (Page < TutorialPage.Length)
                {
                    TutorialPage[Page].gameObject.SetActive(true);
                }


            }

            //Debug.Log(Page);


        }

        void P_ToBehindButton()
        {
            Page -= 2;

            for (int i = 0; i < TutorialPage.Length; i++)
            {
                TutorialPage[i].gameObject.SetActive(false);

                if (Page < TutorialPage.Length)
                {
                    TutorialPage[Page].gameObject.SetActive(true);
                }


            }
            //Debug.Log(Page);


        }
    }
}