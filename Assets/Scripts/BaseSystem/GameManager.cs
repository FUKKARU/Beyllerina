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
        [Header("Now Loading のテキスト")] public TextMeshProUGUI NowLoadingText;



        [NonSerialized] public bool IsChangePlayableBar = false; // プレイアブルのバーを変化させるかどうか
        [NonSerialized] public bool IsChangeUnPlayableBar = false; // アンプレイアブルのバーを変化させるかどうか

        // PlayerMoveクラスのインスタンス（PvEかつ1v1の想定）
        [NonSerialized] public PlayerMove P_Pm;
        [NonSerialized] public PlayerMove U_Pm;

        [NonSerialized] public bool IsGameResultJudged = false; // 勝利/敗北の処理を、行っている/行ったかどうか

        void Start()
        {
            P_Pm = Beys[0].GetComponent<PlayerMove>();
            U_Pm = Beys[1].GetComponent<PlayerMove>();
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
                    PlayableDamagedBar.fillAmount -= P_Pm.S_SOP.HpBarChangeSpeed * Time.deltaTime;
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
                    UnPlayableDamagedBar.fillAmount -= U_Pm.S_SOU.HpBarChangeSpeed * Time.deltaTime;
                }
                else
                {
                    IsChangeUnPlayableBar = false;
                }
            }
        }

        #region 勝利/敗北

        /// <summary>
        /// プレイアブルが勝利
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
        /// プレイアブルが敗北
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
            //KOの演出
            while (true)
            {
                if (1 == 1) break;
                yield return null;
            }

            if (GameData.GameData.RoundNum < GameSO.Entity.RoundNum)
            {
                // ラウンド数を増やす
                GameData.GameData.RoundNum += 1;

                // HPを戻し、次ラウンドのシーンに遷移する
                StartCoroutine(WinBehaviour());
            }
            else
            {
                // 勝利シーンに遷移
                LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Win, true);
            }
        }

        IEnumerator WinBehaviour()
        {
            // HPを戻す演出
            yield return new WaitForSeconds(PlayerSO.Entity.UntilHpRecoverDur);
            P_Pm.Hp += (P_Pm.S_SOI.Hp - P_Pm.Hp) * PlayerSO.Entity.HpRecoverRatio / 100;
            PlayableBar.fillAmount = P_Pm.Hp / P_Pm.S_SOI.Hp;
            GameData.GameData.PlayableHp = P_Pm.Hp; // シーン遷移前のHpを記録しておく
            yield return new WaitForSeconds(PlayerSO.Entity.FromHpRecoverDur);

            // 次ラウンドのシーンに遷移
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
        }

        IEnumerator KOBehaviourIfLose()
        {
            //KOの演出
            while (true)
            {
                if (1==1) break;
                yield return null;
            }

            // 敗北シーンに遷移
            LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Lose, true);
        }
        #endregion
    }

    namespace LoadSceneAsync
    {
        public static class LoadSceneAsync
        {
            public static void Load(string sceneName, bool isShowMessage = false)
            {
                if (isShowMessage)
                {
                    GameManager.Instance.NowLoadingText.enabled = true;
                }
                
                SceneManager.LoadSceneAsync(sceneName);
            }
        }
    }
}
