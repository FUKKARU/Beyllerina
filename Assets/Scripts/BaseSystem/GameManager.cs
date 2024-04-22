using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace BaseSystem
{
    public class GameManager : MonoBehaviour
    {
        #region static���V���O���g���ɂ���
        public static GameManager Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        [Header("�x�C�i�v���C�A�u��/�A���v���C�A�u���j")] public GameObject[] Beys;
        [Header("�v���C�A�u����Bar")] public Image PlayableBar;
        [Header("�v���C�A�u����DamagedBar")] public Image PlayableDamagedBar;
        [Header("�A���v���C�A�u����Bar")] public Image UnPlayableBar;
        [Header("�A���v���C�A�u����DamagedBar")] public Image UnPlayableDamagedBar;
        [Header("�v�b�V���̃N�[���^�C��Gauge")] public Image PushCooltimeGauge;
        [Header("�J�E���^�[�̃N�[���^�C��Gauge")] public Image CounterCooltimeGauge;
        [Header("�X�L���̃N�[���^�C��Gauge")] public Image[] SkillCooltimeGauges;
        [Header("���E���hUI")] public GameObject RoundUI;
        [Header("KO����UI")] public GameObject KO_UI;
        [Header("KO���ꂽUI")] public GameObject KOed_UI;
        [Header("Now Loading �̃e�L�X�g")] public TextMeshProUGUI NowLoadingText;



        [NonSerialized] public bool IsChangePlayableBar = false; // �v���C�A�u���̃o�[��ω������邩�ǂ���
        [NonSerialized] public bool IsChangeUnPlayableBar = false; // �A���v���C�A�u���̃o�[��ω������邩�ǂ���

        // PlayerMove�N���X�̃C���X�^���X�iPvE����1v1�̑z��j
        [NonSerialized] public PlayerMove P_Pm;
        [NonSerialized] public PlayerMove U_Pm;

        [NonSerialized] public bool IsGameResultJudged = false; // ����/�s�k�̏������A�s���Ă���/�s�������ǂ���

        Image[] roundUIs = new Image[3];

        void Start()
        {
            P_Pm = Beys[0].GetComponent<PlayerMove>();
            U_Pm = Beys[1].GetComponent<PlayerMove>();

            for (int i = 0; i < 3; i++)
            {
                roundUIs[i] = RoundUI.transform.GetChild(i).GetComponent<Image>();
            }

            for (int i = 0; i < GameData.GameData.RoundNum; i++)
            {
                roundUIs[i].enabled = false;
            }
        }

        void Update()
        {
            // �o�[������������B
            ChangeBarsFillAmount();
            /*
            Transform trans = KOed_UI.transform;
            trans.GetComponent<RectTransform>().localPosition = new Vector3 (0f, indexer, 0f);
            */
        }

        void ChangeBarsFillAmount()
        {
            if (IsChangePlayableBar)
            {
                float targetValue = PlayableBar.fillAmount; // �����܂Ńo�[�����炷
                float nowValue = PlayableDamagedBar.fillAmount; // ���݂̃o�[�̐i��

                if (nowValue > targetValue) // �ڕW�l���z���Ă��Ȃ��Ƃ�
                {
                    PlayableDamagedBar.fillAmount -= P_Pm.S_SOP.HpBarChangeSpeed * Time.deltaTime;
                }
                else
                {
                    IsChangePlayableBar = false;
                }
            }

            if (IsChangeUnPlayableBar)
            {
                float targetValue = UnPlayableBar.fillAmount; // �����܂Ńo�[�����炷
                float nowValue = UnPlayableDamagedBar.fillAmount; // ���݂̃o�[�̐i��

                if (nowValue > targetValue) // �ڕW�l���z���Ă��Ȃ��Ƃ�
                {
                    UnPlayableDamagedBar.fillAmount -= U_Pm.S_SOU.HpBarChangeSpeed * Time.deltaTime;
                }
                else
                {
                    IsChangeUnPlayableBar = false;
                }
            }
        }

        #region ����/�s�k

        /// <summary>
        /// �v���C�A�u��������
        /// </summary>
        public void Win()
        {
            if (!IsGameResultJudged)
            {
                IsGameResultJudged = true;

                StartCoroutine(KOBehaviourIfWin());
            }
        }

        /// <summary>
        /// �v���C�A�u�����s�k
        /// </summary>
        public void Lose()
        {
            if (!IsGameResultJudged)
            {
                IsGameResultJudged = true;

                StartCoroutine(KOBehaviourIfLose());
            }
        }

        IEnumerator KOBehaviourIfWin()
        {
            // KO�̉��o
            const int CANVAS_WIDTH = 800;
            RectTransform trans = KO_UI.GetComponent<RectTransform>().transform as RectTransform;
            float d = PlayerSO.Entity.KODur;
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;

                if (time >= d)
                {
                    Vector3 p = trans.GetComponent<RectTransform>().localPosition;
                    p.x = 0;
                    trans.GetComponent<RectTransform>().localPosition = p;
                    break;
                }

                Vector3 pos = trans.GetComponent<RectTransform>().localPosition;
                pos.x = -CANVAS_WIDTH / d * time + CANVAS_WIDTH;

                trans.GetComponent<RectTransform>().localPosition = pos;

                yield return null;
            }

            if (GameData.GameData.RoundNum < GameSO.Entity.RoundNum)
            {
                // ���E���h���𑝂₷
                GameData.GameData.RoundNum += 1;

                // HP��߂��A�����E���h�̃V�[���ɑJ�ڂ���
                StartCoroutine(WinBehaviour());
            }
            else
            {
                // �����V�[���ɑJ��
                LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Win, true);
            }
        }

        IEnumerator WinBehaviour()
        {
            // HP��߂����o
            yield return new WaitForSeconds(PlayerSO.Entity.UntilHpRecoverDur);
            P_Pm.Hp += (P_Pm.S_SOI.Hp - P_Pm.Hp) * PlayerSO.Entity.HpRecoverRatio / 100;
            PlayableBar.fillAmount = P_Pm.Hp / P_Pm.S_SOI.Hp;
            GameData.GameData.PlayableHp = P_Pm.Hp; // �V�[���J�ڑO��Hp���L�^���Ă���
            yield return new WaitForSeconds(PlayerSO.Entity.FromHpRecoverDur);

            // �����E���h�̃V�[���ɑJ��
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
        }
        [SerializeField,Range(0,100)] float indexer;
        IEnumerator KOBehaviourIfLose()
        {
            // KOed�̉��o
            const int CANVAS_WIDTH = 800;
            RectTransform trans = KOed_UI.GetComponent<RectTransform>().transform as RectTransform;
            float d = PlayerSO.Entity.KODur;
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;

                if (time >= d)
                {
                    Vector3 p = trans.GetComponent<RectTransform>().localPosition;
                    p.x = 0;
                    trans.GetComponent<RectTransform>().localPosition = p;
                    break;
                }

                Vector3 pos = trans.GetComponent<RectTransform>().localPosition;
                pos.x = -CANVAS_WIDTH / d * time + CANVAS_WIDTH;

                trans.GetComponent<RectTransform>().localPosition = pos;

                yield return null;
            }
          

            // �s�k�V�[���ɑJ��
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Lose, true);
        }
        #endregion
    }
}

namespace LoadSceneAsync
{
    public static class LoadSceneAsync
    {
        public static void Load(string sceneName, bool isShowMessage = false)
        {
            if (isShowMessage)
            {
                BaseSystem.GameManager.Instance.NowLoadingText.enabled = true;
            }

            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}