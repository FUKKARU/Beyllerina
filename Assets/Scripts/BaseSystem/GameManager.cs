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
        [Header("プレイアブルのBar")] public Image P_Bar;
        [Header("プレイアブルのDamagedBar")] public Image P_DBar;
        [Header("アンプレイアブルのBar")] public Image U_Bar;
        [Header("アンプレイアブルのDamagedBar")] public Image U_DBar;

        [NonSerialized] public bool IsChangePlayableBar = false; // プレイアブルのバーを変化させるかどうか
        [NonSerialized] public bool IsChangeUnPlayableBar = false; // アンプレイアブルのバーを変化させるかどうか

        // PlayerMoveクラスのインスタンス
        PlayerMove pPm;
        PlayerMove uPm;

        void Start()
        {
            pPm = Beys[0].GetComponent<PlayerMove>();
            uPm = Beys[1].GetComponent<PlayerMove>();
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
                float targetValue = P_Bar.fillAmount; // ここまでバーを減らす
                float nowValue = P_DBar.fillAmount; // 現在のバーの進捗

                if (nowValue > targetValue) // 目標値を越えていないとき
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
                float targetValue = U_Bar.fillAmount; // ここまでバーを減らす
                float nowValue = U_DBar.fillAmount; // 現在のバーの進捗

                if (nowValue > targetValue) // 目標値を越えていないとき
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
