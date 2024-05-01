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
        [Header("スキルのクールタイムGauge")] public Image SkillCooltimeGauge;
        [Header("必殺技のクールタイムGauge")] public Image SpecialCooltimeGauge;
        [Header("必殺技のクールタイム表示テキスト")] public TextMeshProUGUI SpecialCooltimeText;
        [Header("必殺技のチャージGauge")] public Image SpecialChargingGauge;
        [Header("必殺技のGauge")] public Image SpecialGauge;
        [Header("ラウンド数（プレイアブル/アンプレイアブル）")] public TextMeshProUGUI[] RoundUI;
        [Header("KOしたUI")] public GameObject KO_UI;
        [Header("KOされたUI")] public GameObject KOed_UI;
        [Header("プレイアブルの名前UI")] public GameObject PlayableNameUI;
        [Header("アンプレイアブルの名前UI")] public GameObject UnPlayableNameUI;
        [Header("ゲームシーンを中継しているカメラ")] public GameObject GameCamera;
        [Header("シャンデリア")] public Light Chanderia;
        [Header("ランタン")] public Light[] Lamps;



        [NonSerialized] public Transform rapier_effect_parent;
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

            rapier_effect_parent = GameObject.FindGameObjectWithTag("rapier_effect_parent").transform;

            ShowRoundNum(GameData.GameData.PlayableRoundNum, GameData.GameData.UnPlayableRoundNum);
        }

        void Update()
        {
            // バーを減少させる。
            ChangeBarsFillAmount();
        }

        void LateUpdate()
        {
            // 名前UIの表示
            NameUIConstrain(GameCamera, P_Pm.gameObject, PlayableNameUI, PlayerSO.Entity.NameUIOffset);
            NameUIConstrain(GameCamera, U_Pm.gameObject, UnPlayableNameUI, PlayerSO.Entity.NameUIOffset);
        }


        void ShowRoundNum(byte pNum, byte uNum)
        {
            RoundUI[0].text = pNum.ToString();
            RoundUI[1].text = uNum.ToString();

            if ((GameData.GameData.PlayableRoundNum >= GameSO.Entity.RoundNum - 1) || (GameData.GameData.UnPlayableRoundNum >= GameSO.Entity.RoundNum - 1))
            {
                RoundUI[0].color = Color.yellow;
                RoundUI[1].color = Color.yellow;
            }
            else
            {
                RoundUI[0].color = Color.white;
                RoundUI[1].color = Color.white;
            }
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

        void NameUIConstrain(GameObject camera, GameObject constraint, GameObject nameUI, float offset = 0)
        {
            // "nameUI"を、"constraint"より、y座標が（ローカル座標で）"offset"大きい位置にする。
            nameUI.transform.position = constraint.transform.position;
            Vector3 nameUILPos = nameUI.transform.localPosition;
            nameUILPos.y += offset;
            nameUI.transform.localPosition = nameUILPos;

            // "camera"の方を向かせる。
            nameUI.transform.LookAt(camera.transform.position);

            // 反対方向を向かせる。
            nameUI.transform.rotation = Quaternion.AngleAxis(180, nameUI.transform.up) * nameUI.transform.rotation;
        }

        #region KO/KOed

        /// <summary>
        /// プレイアブルがKO
        /// </summary>
        public void KO()
        {
            if (!IsGameResultJudged)
            {
                IsGameResultJudged = true;

                StartCoroutine(KOBehaviour(true));
            }
        }

        /// <summary>
        /// プレイアブルがKOed
        /// </summary>
        public void KOed()
        {
            if (!IsGameResultJudged)
            {
                IsGameResultJudged = true;

                StartCoroutine(KOBehaviour(false));
            }

            SoundManager.Instance.PlaySE(10);
        }

        IEnumerator KOBehaviour(bool isKO)
        {
            // KOの演出
            const int CANVAS_WIDTH = 800;
            GameObject koUi = isKO ? KO_UI : KOed_UI;
            RectTransform trans = koUi.GetComponent<RectTransform>();
            float d = PlayerSO.Entity.KODur;
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;

                if (time >= d)
                {
                    Vector3 p1 = trans.localPosition;
                    p1.x = 0;
                    trans.localPosition = p1;
                    break;
                }

                Vector3 p2 = trans.localPosition;
                p2.x = -CANVAS_WIDTH / d * time + CANVAS_WIDTH;

                trans.localPosition = p2;

                yield return null;
            }

            if (isKO)
            {
                // ラウンド数を増やす
                GameData.GameData.PlayableRoundNum += 1;
                ShowRoundNum(GameData.GameData.PlayableRoundNum, GameData.GameData.UnPlayableRoundNum);

                yield return new WaitForSeconds(PlayerSO.Entity.OnKONextDur);

                if (GameData.GameData.PlayableRoundNum < GameSO.Entity.RoundNum)
                {
                    // 次ラウンドのシーンに遷移
                    LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
                }
                else
                {
                    // 勝利シーンに遷移
                    GameData.GameData.IsWin = true;
                    LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.WinLose, true);
                }
            }
            else
            {
                // ラウンド数を増やす
                GameData.GameData.UnPlayableRoundNum += 1;
                ShowRoundNum(GameData.GameData.PlayableRoundNum, GameData.GameData.UnPlayableRoundNum);

                yield return new WaitForSeconds(PlayerSO.Entity.OnKONextDur);

                if (GameData.GameData.UnPlayableRoundNum < GameSO.Entity.RoundNum)
                {
                    // 次ラウンドのシーンに遷移
                    LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.Game, true);
                }
                else
                {
                    // 敗北シーンに遷移
                    GameData.GameData.IsWin = false;
                    LoadSceneAsync.LoadSceneAsync.Load(GameSO.Entity.SceneName.WinLose, true);
                }
            }
        }
        #endregion

        public void OnSpecialLightDir(bool toSpecial /*強化状態になるならtrue、強化状態が終了したならfalse*/)
        {
            if (toSpecial)
            {
                Chanderia.color = PlayerSO.Entity.LightDirectionTable.LightSpecialColor;
                foreach (Light lamp in Lamps)
                {
                    lamp.color = PlayerSO.Entity.LightDirectionTable.LightSpecialColor;
                    lamp.intensity = PlayerSO.Entity.LightDirectionTable.LampSpecialIntensity;
                }
            }
            else
            {
                Chanderia.color = PlayerSO.Entity.LightDirectionTable.LightNormalColor;
                foreach (Light lamp in Lamps)
                {
                    lamp.color = PlayerSO.Entity.LightDirectionTable.LightNormalColor;
                    lamp.intensity = PlayerSO.Entity.LightDirectionTable.LampNormalIntensity;
                }
            }
        }
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
                GameObject nowLoadingText = GameObject.FindGameObjectWithTag("NowLoading");
                nowLoadingText.GetComponent<TextMeshProUGUI>().enabled = true;
            }

            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}