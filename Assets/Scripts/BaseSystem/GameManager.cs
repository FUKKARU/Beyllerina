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
        [Header("�v���C�A�u����Bar")] public Image P_Bar;
        [Header("�v���C�A�u����DamagedBar")] public Image P_DBar;
        [Header("�A���v���C�A�u����Bar")] public Image U_Bar;
        [Header("�A���v���C�A�u����DamagedBar")] public Image U_DBar;

        [NonSerialized] public bool IsChangePlayableBar = false; // �v���C�A�u���̃o�[��ω������邩�ǂ���
        [NonSerialized] public bool IsChangeUnPlayableBar = false; // �A���v���C�A�u���̃o�[��ω������邩�ǂ���

        // PlayerMove�N���X�̃C���X�^���X
        PlayerMove pPm;
        PlayerMove uPm;

        void Start()
        {
            pPm = Beys[0].GetComponent<PlayerMove>();
            uPm = Beys[1].GetComponent<PlayerMove>();
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
                float targetValue = P_Bar.fillAmount; // �����܂Ńo�[�����炷
                float nowValue = P_DBar.fillAmount; // ���݂̃o�[�̐i��

                if (nowValue > targetValue) // �ڕW�l���z���Ă��Ȃ��Ƃ�
                {
                    P_DBar.fillAmount -= pPm.S_SOP.HpBarChangeSpeed * Time.deltaTime;
                }
                else
                {
                    IsChangePlayableBar = false;
                }
            }

            if (IsChangeUnPlayableBar)
            {
                float targetValue = U_Bar.fillAmount; // �����܂Ńo�[�����炷
                float nowValue = U_DBar.fillAmount; // ���݂̃o�[�̐i��

                if (nowValue > targetValue) // �ڕW�l���z���Ă��Ȃ��Ƃ�
                {
                    U_DBar.fillAmount -= uPm.S_SOU.HpBarChangeSpeed * Time.deltaTime;
                }
                else
                {
                    IsChangeUnPlayableBar = false;
                }
            }
        }
    }
}
