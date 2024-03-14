using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [NonSerialized] public bool IsChangePlayableBar = false; // �v���C�A�u���̃o�[��ω������邩�ǂ���
        [NonSerialized] public bool IsChangeUnPlayableBar = false; // �A���v���C�A�u���̃o�[��ω������邩�ǂ���

        // PlayerMove�N���X�̃C���X�^���X�iPvE����1v1�̑z��j
        PlayerMove pPm;
        PlayerMove uPm;

        void Start()
        {
            pPm = Beys[0].GetComponent<PlayerMove>();
            uPm = Beys[1].GetComponent<PlayerMove>();

            PlayableDamagedBar.fillAmount = 1f;
            UnPlayableDamagedBar.fillAmount = 1f;
        }

        void Update()
        {
            // �o�[������������B
            ChangeBarsFillAmount();
        }

        void ChangeBarsFillAmount()
        {
            if (IsChangePlayableBar)
            {
                float targetValue = PlayableBar.fillAmount; // �����܂Ńo�[�����炷
                float nowValue = PlayableDamagedBar.fillAmount; // ���݂̃o�[�̐i��

                if (nowValue > targetValue) // �ڕW�l���z���Ă��Ȃ��Ƃ�
                {
                    PlayableDamagedBar.fillAmount -= pPm.S_SOP.HpBarChangeSpeed * Time.deltaTime;
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
                    UnPlayableDamagedBar.fillAmount -= uPm.S_SOU.HpBarChangeSpeed * Time.deltaTime;
                }
                else
                {
                    IsChangeUnPlayableBar = false;
                }
            }
        }
    }
}
