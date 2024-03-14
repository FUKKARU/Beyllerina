using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaseSystem
{
    public class GameManager : MonoBehaviour
    {
        #region staticかつシングルトンにする
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

        [Header("ベイ（プレイアブル/アンプレイアブル）")] public GameObject[] Beys;
        [Header("プレイアブルのBar")] public Image PlayableBar;
        [Header("プレイアブルのDamagedBar")] public Image PlayableDamagedBar;
        [Header("アンプレイアブルのBar")] public Image UnPlayableBar;
        [Header("アンプレイアブルのDamagedBar")] public Image UnPlayableDamagedBar;
        [Header("プッシュのクールタイムGauge")] public Image PushCooltimeGauge;
        [Header("カウンターのクールタイムGauge")] public Image CounterCooltimeGauge;
        [Header("スキルのクールタイムGauge")] public Image[] SkillCooltimeGauges;

        [NonSerialized] public bool IsChangePlayableBar = false; // プレイアブルのバーを変化させるかどうか
        [NonSerialized] public bool IsChangeUnPlayableBar = false; // アンプレイアブルのバーを変化させるかどうか

        // PlayerMoveクラスのインスタンス（PvEかつ1v1の想定）
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
            // バーを減少させる。
            ChangeBarsFillAmount();
        }

        void ChangeBarsFillAmount()
        {
            if (IsChangePlayableBar)
            {
                float targetValue = PlayableBar.fillAmount; // ここまでバーを減らす
                float nowValue = PlayableDamagedBar.fillAmount; // 現在のバーの進捗

                if (nowValue > targetValue) // 目標値を越えていないとき
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
                float targetValue = UnPlayableBar.fillAmount; // ここまでバーを減らす
                float nowValue = UnPlayableDamagedBar.fillAmount; // 現在のバーの進捗

                if (nowValue > targetValue) // 目標値を越えていないとき
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
