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
        [Header("必殺技のクールタイムGauge")] public Image SpecialCooltimeGauge;
        [Header("必殺技のGauge")] public Image SpecialGauge;
        [Header("ラウンドUI")] public GameObject RoundUI;
        [Header("KOしたUI")] public GameObject KO_UI;
        [Header("KOされたUI")] public GameObject KOed_UI;
        [Header("Now Loading のテキスト")] public TextMeshProUGUI NowLoadingText;
        [Header("プレイアブルの名前UI")] public GameObject PlayableNameUI;
        [Header("アンプレイアブルの名前UI")] public GameObject UnPlayableNameUI;
        [Header("ゲームシーンを中継しているカメラ")] public GameObject GameCamera;



        [NonSerialized] public bool IsChangePlayableBar = false; // プレイアブルのバーを変化させるかどうか
        [NonSerialized] public bool IsChangeUnPlayableBar = false; // アンプレイアブルのバーを変化させるかどうか

        // PlayerMoveクラスのインスタンス（PvEかつ1v1の想定）
        [NonSerialized] public PlayerMove P_Pm;
        [NonSerialized] public PlayerMove U_Pm;

        [NonSerialized] public bool IsGameResultJudged = false; // 勝利/敗北の処理を、行っている/行ったかどうか

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
            // バーを減少させる。
            ChangeBarsFillAmount();
        }

        void LateUpdate()
        {
            // 名前UIの表示
            NameUIConstrain(GameCamera, P_Pm.gameObject, PlayableNameUI, PlayerSO.Entity.NameUIOffset);
            NameUIConstrain(GameCamera, U_Pm.gameObject, UnPlayableNameUI, PlayerSO.Entity.NameUIOffset);
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
            // KOの演出
            const int CANVAS_WIDTH = 800;
            RectTransform trans = KO_UI.GetComponent<RectTransform>();
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

            yield return new WaitForSeconds(PlayerSO.Entity.KOFadeDur);
            Vector3 p3 = trans.localPosition;
            p3.x = CANVAS_WIDTH;
            trans.localPosition = p3;

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
        [SerializeField,Range(0,100)] float indexer;
        IEnumerator KOBehaviourIfLose()
        {
            // KOedの演出
            const int CANVAS_WIDTH = 800;
            RectTransform trans = KOed_UI.GetComponent<RectTransform>();
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

            yield return new WaitForSeconds(PlayerSO.Entity.KOFadeDur);
            Vector3 p3 = trans.localPosition;
            p3.x = CANVAS_WIDTH;
            trans.localPosition = p3;

            // 敗北シーンに遷移
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