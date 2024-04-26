using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BaseSystem
{
    public class PlayerMove : MonoBehaviour
    {
        #region 変数宣言
        // ベイの種類を識別する。インスペクタ上で設定しておくこと！
        enum TYPE { NULL, Ballerina, BreakDancer, Enemy1 }
        [SerializeField] TYPE type = TYPE.NULL;

        // プレイヤーの状態(public)
        public enum PlayerState { IDLE, PUSH, COUNTER, KNOCKBACKED };
        public PlayerState State { get; set; }

        // 物理演算用
        Rigidbody rb;
        GameObject stageCenter;

        //　カメラシェイク
        CameraShake_Battle CameraS_B;

        //エフェクト
        [SerializeField] GameObject hitEffect;
        Transform hit_effect_parent;
        GameObject shield;
        GameObject[] enemyEffect;

        // GMからデータを取得するよう
        GameManager gm;

        // SOからデータを取得する用
        public PlayerSO P_SO { get; set; }
        public BehaviourTable P_SOB { get; set; }
        public DamageTable P_SOD { get; set; }
        public StatusTable S_SO { get; set; }
        public StatusTableName S_SON { get; set; }
        public StatusTablePlayable S_SOP { get; set; }
        public StatusTableUnPlayable S_SOU { get; set; }
        public StatusTableInitStatus S_SOI { get; set; }
        public float Hp { get; set; }
        float weight;
        float knoRes; // 内部的にRigidbody.drag（＝抵抗）を操作している
        float rotSpe;

        // 敵への参照を取得する用
        GameObject opponent; // ゲームオブジェクト
        PlayerMove opponentPm; // PlayerMoveクラス
        Rigidbody opponentRb; // Rigidbody

        // ベイの行動処理用
        bool isDamageManager = false; // ダメージ処理を行うインスタンスであるかどうか
        bool isDamagable = false; // 【一人しか行わない】ダメージを食らえるか（＝無敵時間でないか）
        bool IsSkillDirection { get; set; } = false; // スキルの演出中かどうか
        bool IsSpecialDirection { get; set; } = false; // 必殺技の演出中かどうか
        bool isPushIfUnplayable = false; // アンプレイアブルである時、プッシュを使うフラグ
        bool isCounterIfUnplayable = false; // アンプレイアブルである時、カウンターを使うフラグ
        bool[] isSkillIfUnplayables; // アンプレイアブルである時、スキルを使うフラグのリスト
        bool isSpecialIfUnplayable = false; // アンプレイアブルである時、必殺技を使うフラグ
        bool IsPushBehaviourDone { get; set; } = false; // プッシュの処理を、完了しているか
        bool IsCounterBehaviourDone { get; set; } = false; // カウンターの処理を、完了しているか
        bool isKnockbackedBehaviourDone = false; // ノックバックされた時の処理を、完了しているか
        bool isOnPushCooltime = false; // プッシュのクールタイム中であるかどうか
        bool isOnCounterCooltime = false; // カウンターのクールタイム中であるかどうか
        bool[] isOnSkillCooltimes; // スキルのクールタイム中であるかどうか
        bool isOnSpecialCooltime = false; // 必殺技のクールタイム中であるかどうか
        bool isOnStateChangeCooltime = false; // 時間経過による状態変化中であるかどうか
        bool ifUnplayableOnSkill = false; // アンプレイアブルが、スキルを使って巨大化している最中かどうか
        Vector3 axis; // ベイの回転軸
        float axisTimer = 0; // ベイの回転軸を傾ける時間
        bool isHpLow = false; // HPが一定以下になって、歳差運動をしているかどうか
        Vector3 rePos; // リスポーンポイント
        float genericDamageCoef = 1; // 汎用ダメージ係数
        float rotDir = 1; // 正回転なら1、逆回転なら-1
        float pushPowerCoef = 1; // プッシュ力の係数（調整用）
        int specialPoint = 0; // 必殺技の発動ポイント

        bool antiGravity = false;//重力停止
        #endregion



        #region 【Start】
        void Start()
        {
            // プレイヤーの初期状態を設定
            State = PlayerState.IDLE;

            // 物理演算の準備（Unity側の重力をオフにし、人力で計算する重力の中心を取得）
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            stageCenter = GameObject.FindGameObjectWithTag("Center");

            // GameManagerを取得
            gm = GameManager.Instance;

            // PlayerSOを取得
            P_SO = PlayerSO.Entity;
            P_SOB = P_SO.BehaviourTable;
            P_SOD = P_SO.DamageTable;

            // 対応するStatusTableを取得し、変動する数値を改めて、このクラス内の変数に格納
            switch (type)
            {
                case TYPE.Ballerina:
                    S_SO = BallerinaStatusSO.Entity.StatusTable;
                    break;

                case TYPE.BreakDancer:
                    S_SO = BreakDancerStatusSO.Entity.StatusTable;
                    break;

                case TYPE.Enemy1:
                    S_SO = Enemy1StatusSO.Entity.StatusTable;
                    break;

                case TYPE.NULL:
                    Debug.LogError("<color=red>typeが設定されていません</color>");
                    break;
            }
            S_SON = S_SO.StatusTableName;
            S_SOP = S_SO.StatusTablePlayable;
            S_SOU = S_SO.StatusTableUnPlayable;
            S_SOI = S_SO.StatusTableInitStatus;
            if (S_SO.IsPlayable)
            {
                if (GameData.GameData.RoundNum == 1)
                {
                    Hp = S_SOI.Hp;
                    gm.PlayableBar.fillAmount = 1;
                    gm.PlayableDamagedBar.fillAmount = 1;
                }
                else
                {
                    Hp = GameData.GameData.PlayableHp;
                    gm.PlayableBar.fillAmount = Hp / S_SOI.Hp;
                    gm.PlayableDamagedBar.fillAmount = Hp / S_SOI.Hp;
                }
            }
            else
            {
                Hp = S_SOI.Hp;
                gm.UnPlayableBar.fillAmount = 1;
                gm.UnPlayableDamagedBar.fillAmount = 1;
            }
            
            weight = S_SOI.Weight;
            knoRes = S_SOI.KnockbackResistance;
            rotSpe = S_SOI.RotationSpeed * SelectTeam.RotData.RotateNumber;

            // 敵への参照を取得
            int idx = Array.IndexOf(gm.Beys, gameObject);
            if (idx == 0)
            {
                opponent = gm.Beys[1];
                isDamageManager = true; // 0番目のインスタンスで、ダメージ処理を行う。
            }
            else if (idx == 1)
            {
                opponent = gm.Beys[0];
            }
            else
            {
                Debug.LogError("<color=red>敵オブジェクトの取得に失敗しました</color>");
            }
            opponentPm = opponent.GetComponent<PlayerMove>();
            opponentRb = opponent.GetComponent<Rigidbody>();

            // ベイの行動処理用の配列をインスタンス化して、初期化する。
            if (S_SO.IsPlayable)
            {
                isOnSkillCooltimes = new bool[S_SOP.SkillNum];
                for (int i = 0; i < isOnSkillCooltimes.Length; i++)
                {
                    isOnSkillCooltimes[i] = false;
                }
            }
            else
            {
                isOnSkillCooltimes = new bool[S_SOU.SkillNum];
                for (int i = 0; i < isOnSkillCooltimes.Length; i++)
                {
                    isOnSkillCooltimes[i] = false;
                }

                isSkillIfUnplayables = new bool[S_SOU.SkillNum];
                for (int i = 0; i < isSkillIfUnplayables.Length; i++)
                {
                    isSkillIfUnplayables[i] = false;
                }
            }

            // リスポーンポイントの取得
            string tag = (this == GameManager.Instance.P_Pm) ? P_SOB.P_RePosTag : P_SOB.U_RePosTag;
            rePos = GameObject.FindGameObjectWithTag(tag).transform.position;

            // CameraShake_Battle を取得
            CameraS_B = GameObject.FindGameObjectWithTag("CameraShakeGameObject").GetComponent<CameraShake_Battle>();

            // エフェクトを取得
            hit_effect_parent = GameObject.FindGameObjectWithTag("hit_effect_parent").transform;
            if (S_SO.IsPlayable)
            {
                shield = GameObject.FindGameObjectWithTag("Shield");
                Shield(false);
            }
            enemyEffect = GameObject.FindGameObjectsWithTag("EnemyEffect");

            // ゲーム開始から少しの間は、無敵時間になっている。
            if (gameObject.activeSelf) StartCoroutine(CountDamagableDuration());

            if (!S_SO.IsPlayable)
            {
                // アンプレイアブルなら、プッシュとスキルを使うフラグを、周期的かつ交互にUpdateメソッドに送る
                if (gameObject.activeSelf) StartCoroutine(InputPushAndSkillPeriodically());
            }
            else
            {
                // そうでないなら、毎秒ポイントを貯める
                if (gameObject.activeSelf) StartCoroutine(PointIncrease());
            }
        }

        IEnumerator InputPushAndSkillPeriodically()
        {
            bool isSkill2Push = true; // スキル → プッシュの遷移を行う番であるかどうか

            while (true)
            {
                if (isSkill2Push)
                {
                    isSkill2Push = false;

                    float dTime = S_SOU.Skill2PushInterval;
                    float ofst = S_SOU.Skill2PushIntervalOffset;
                    float time = Random.Range(dTime - ofst, dTime + ofst);
                    yield return new WaitForSeconds(time);
                    isPushIfUnplayable = true;
                }
                else
                {
                    isSkill2Push = true;

                    float dTime = S_SOU.Push2SkillInterval;
                    float ofst = S_SOU.Push2SkillIntervalOffset;
                    float time = Random.Range(dTime - ofst, dTime + ofst);
                    yield return new WaitForSeconds(time);
                    int i = Random.Range(0, S_SOU.SkillNum); // ランダムなスキル
                    isSkillIfUnplayables[i] = true;
                }
            }
        }

        IEnumerator PointIncrease()
        {
            while (true)
            {
                specialPoint += P_SOB.PointAmount;
                PointBonus();
                specialPoint = Mathf.Clamp(specialPoint, 0, S_SOI.SpecialPoint);
                yield return new WaitForSeconds(P_SOB.PointDur);
            }
        }

        // 体力が減るほど、ボーナスポイントがもらえる。
        void PointBonus()
        {
            int[] bonusPoint = P_SOB.BonusPoint;
            int len = bonusPoint.Length;
            float hpRange = S_SOI.Hp / len; // 判定境界間の長さ
            if (Hp == 0)
            {
                return;
            }
            else
            {
                // (0, S_SOI.Hp] まで判定できる。
                for (int i = 0; i < len; i++)
                {
                    if (hpRange * i < Hp && Hp <= hpRange * (i + 1))
                    {
                        specialPoint += bonusPoint[i];
                    }
                }
            }
        }
        #endregion

        #region 【FixedUpdate】まず、ベイの物理状態を更新する。次に、すり鉢状のフィールドにおける、重力による挙動を再現する。
        void FixedUpdate()
        {
            // ベイの物理状態の更新
            ChangeRigidbodyParameters();

            if (!antiGravity)
            {
                // 重力
                rb.AddForce(Vector3.down * 9.81f * P_SO.GravityScale, ForceMode.Force);

                // 常に中心へ移動
                rb.AddForce((stageCenter.transform.position - transform.position).normalized * P_SO.SpeedTowardCenter, ForceMode.Force);
            }


        }

        // ベイの現在のステータスに基づいて、Rigidbodyのパラメーターを更新する。
        void ChangeRigidbodyParameters()
        {
            rb.mass = weight; // mass：重量
            rb.drag = P_SOB.DragCoef * knoRes; // drag：抵抗
        }
        #endregion

        #region 【OnCollision】【一人しか行わない】敵との接触を検知し、無敵時間でないかつ両者がスキル・必殺技の演出中でないならば、ダメージ処理を行いかつ条件によって自分または敵をKNOCKBACKED状態にする。
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(P_SO.BeyTagName) && isDamageManager)
            {
                CameraS_B.ShakeOn();
                Instantiate(hitEffect, (gameObject.transform.position + collision.gameObject.transform.position) / 2, Quaternion.identity, hit_effect_parent);
                if (isDamagable && (!IsSpecialDirection && !opponentPm.IsSpecialDirection) && (!IsSkillDirection && !opponentPm.IsSkillDirection))
                {
                    isDamagable = false;
                    HitBehaviour(); // PlayerStateの遷移など
                    if (gameObject.activeSelf) StartCoroutine(CountDamagableDuration()); // 無敵時間のカウント
                }
            }
        }
        IEnumerator CountDamagableDuration()
        {
            yield return new WaitForSeconds(P_SOD.DamagableDuration);
            isDamagable = true;
        }
        #endregion

        #region 【Update】
        void Update()
        {
            #region PlayerStateの遷移：遷移クールタイム中でないなら、プレイヤーからの入力またはコルーチンからのフラグを取得して、PUSH状態またはCOUNTER状態になる。
            Idle2Push();
            Counter2Push();
            Idle2Counter();
            Push2Counter();
            #endregion

            #region スキル・必殺技：プレイヤーからの入力またはコルーチンからのフラグを取得
            Skill();
            Special();
            ShowSpecialPoint();
            #endregion

            #region PlayerStateに基づくベイの行動処理
            Rotate(); // 最初に回転処理を行う。
            Idle();
            Push();
            Counter();
            Knockbacked();
            #endregion

            #region デス判定
            JudgeFall(); // 場外判定
            JudgeDeath();
            #endregion

            #region PlayerStateの遷移：遷移クールタイム中でないなら、遷移クールタイムを開始し、時間経過でIDLE状態になる。
            Push2Idle();
            Counter2Idle();
            Knockbacked2Idle();
            #endregion
        }
        #endregion



        #region　PlayerStateの遷移の詳細（対応する状態の時、【条件】を満たしたら即座に遷移する。ベイの行動処理に関わる変数のリセットも行う。）
        // 以下のいずれかの遷移を行う。
        // ・【自分】PUSH    => IDLE       【敵】IDLE    => KNOCKBACKED
        // ・【自分】COUNTER => IDLE       【敵】PUSH    => KNOCKBACKED
        // ・【自分】IDLE    => KNOCKBACKED【敵】PUSH    => IDLE
        // ・【自分】PUSH    => KNOCKBACKED【敵】COUNTER => IDLE
        // 【条件】（敵と接触した際に呼ばれる。）
        // 【その他】ダメージ処理も行う。
        void HitBehaviour()
        {
            switch (opponentPm.State)
            {
                case PlayerState.IDLE:
                    if (State == PlayerState.PUSH)
                    {
                        State = PlayerState.IDLE;
                        IsPushBehaviourDone = false;

                        opponentPm.State = PlayerState.KNOCKBACKED;
                        SoundManager.Instance.PlaySE(2);

                        Damage(this, PlayerState.PUSH);
                        Damage(opponentPm, PlayerState.KNOCKBACKED);

                        if (P_SO.Dbg.IsShowNormalLog)
                        {
                            Debug.Log($"<color=#64ff64>{name} が {opponent.name} にプッシュした！</color>");
                        }
                    }
                    else
                    {
                        Damage(this, State);
                        Damage(opponentPm, PlayerState.IDLE);
                    }
                    break;

                case PlayerState.PUSH:
                    if (State == PlayerState.IDLE)
                    {
                        State = PlayerState.KNOCKBACKED;

                        opponentPm.State = PlayerState.IDLE;
                        opponentPm.IsPushBehaviourDone = false;

                        Damage(this, PlayerState.KNOCKBACKED);
                        Damage(opponentPm, PlayerState.PUSH);

                        if (P_SO.Dbg.IsShowNormalLog)
                        {
                            Debug.Log($"<color=#64ff64>{opponent.name} が {name} にプッシュした！</color>");
                        }
                    }
                    else if (State == PlayerState.COUNTER)
                    {
                        State = PlayerState.IDLE;
                        Shield(false);
                        knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;

                        opponentPm.State = PlayerState.KNOCKBACKED;
                        opponentPm.IsPushBehaviourDone = false;
                        SoundManager.Instance.PlaySE(2);

                        Damage(this, PlayerState.COUNTER);
                        Damage(opponentPm, PlayerState.KNOCKBACKED);

                        if (P_SO.Dbg.IsShowNormalLog)
                        {
                            Debug.Log($"<color=#64ff64>{name} が {opponent.name} にカウンターした！</color>");
                        }
                    }
                    else
                    {
                        Damage(this, State);
                        Damage(opponentPm, PlayerState.PUSH);
                    }
                    break;

                case PlayerState.COUNTER:
                    if (State == PlayerState.PUSH)
                    {
                        State = PlayerState.KNOCKBACKED;
                        IsPushBehaviourDone = false;
                        SoundManager.Instance.PlaySE(2);

                        opponentPm.State = PlayerState.IDLE;
                        opponentPm.Shield(false);
                        opponentPm.knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        opponentPm.IsCounterBehaviourDone = false;

                        Damage(this, PlayerState.KNOCKBACKED);
                        Damage(opponentPm, PlayerState.COUNTER);

                        if (P_SO.Dbg.IsShowNormalLog)
                        {
                            Debug.Log($"<color=#64ff64>{opponent.name} が {name} にカウンターした！</color>");
                        }
                    }
                    else
                    {
                        Damage(this, State);
                        Damage(opponentPm, PlayerState.COUNTER);
                    }
                    break;

                case PlayerState.KNOCKBACKED:
                    Damage(this, State);
                    Damage(opponentPm, PlayerState.KNOCKBACKED);
                    break;
            }
        }
        // 与えられたPlayerMoveクラスのインスタンスに、そのインスタンスが与えられた状態である場合のダメージを与える。
        void Damage(PlayerMove pm, PlayerState state)
        {
            // 基礎ダメージ
            float momentumNorm = (rb.mass * rb.velocity).magnitude; // ダメージマネージャー自身の運動量
            float opponentMomentumNorm = (opponentRb.mass * opponentRb.velocity).magnitude; // ダメージマネージャーの相手の運動量
            float baseDamage = momentumNorm + opponentMomentumNorm;

            // ステータス補正値
            float weightAdjustValue = CalcMrkDamage(weight, S_SOI.Weight); // 重量補正値
            float rotSpeAdjustValue = CalcMrkDamage(rotSpe, S_SOI.RotationSpeed); // 回転速度補正値
            float knoResAdjustValue = CalcMrkDamage(knoRes, S_SOI.KnockbackResistance); // ノックバック耐性補正値
            float hpAdjustValue = pm.isHpLow ? P_SOD.HpAdjustValue[0] : P_SOD.HpAdjustValue[1]; // 体力補正値
            float statusAdjustValue = weightAdjustValue * rotSpeAdjustValue * knoResAdjustValue * hpAdjustValue;

            // 状態補正値
            float stateAdjustValue = 0f;
            switch (state)
            {
                case PlayerState.IDLE:
                    stateAdjustValue = P_SOD.StateAdjustValue[0];
                    break;

                case PlayerState.PUSH:
                    stateAdjustValue = P_SOD.StateAdjustValue[1];
                    break;

                case PlayerState.COUNTER:
                    stateAdjustValue = P_SOD.StateAdjustValue[2];
                    break;

                case PlayerState.KNOCKBACKED:
                    stateAdjustValue = P_SOD.StateAdjustValue[3];
                    break;
            }

            // ダメージ係数
            float damageCoef = P_SOD.DamageCoef;

            // ダメージ計算
            float damage = baseDamage * statusAdjustValue * stateAdjustValue * damageCoef;
            damage = Mathf.Clamp(damage, P_SOD.MinDamage, P_SOD.MaxDamage);
            damage *= genericDamageCoef;

            if (pm.S_SO.IsPlayable && pm.P_SO.Dbg.P_DamageMul)
            {
                damage *= 100;
            }
            else if (!pm.S_SO.IsPlayable && pm.P_SO.Dbg.U_DamageMul)
            {
                damage *= 100;
            }
            else if (pm.S_SO.IsPlayable && pm.P_SO.Dbg.P_DamageImmune)
            {
                damage *= 0;
            }
            else if (!pm.S_SO.IsPlayable && pm.P_SO.Dbg.U_DamageImmune)
            {
                damage *= 0;
            }

            pm.Hp -= damage; // 与えられたインスタンスにダメージを与える

            // Barを変化させる
            if (pm.S_SO.IsPlayable)
            {
                pm.gm.PlayableBar.fillAmount = pm.Hp / pm.S_SOI.Hp;
                pm.gm.IsChangePlayableBar = true;
            }
            else
            {
                pm.gm.UnPlayableBar.fillAmount = pm.Hp / pm.S_SOI.Hp;
                pm.gm.IsChangeUnPlayableBar = true;
            }

            if (P_SO.Dbg.IsShowNormalLog) // ログを表示
            {
                Debug.Log($"<color=#64ff64>{pm.gameObject.name} に {damage} ダメージ！</color>");
            }
        }
        // 重量補正値/回転速度補正値/ノックバック耐性補正値 の計算
        float CalcMrkDamage(float x, float d)
        {
            x = Mathf.Abs(x);
            float k = -2 / d * Mathf.Log(2 - P_SOD.MrkAdjustValueY);
            if (x < d)
            {
                return -Mathf.Exp(k * (x - d)) + 2;
            }
            else
            {
                float m = -k * Mathf.Exp(k * (P_SOD.MrkAdjustValueX - d));
                return m * (x - d) + 1;
            }
        }

        // IDLE => PUSH
        // 【条件】スキル・必殺技の演出中でない、かつ、プレイアブルかつプッシュキーが押されたまたはアンプレイアブルかつプッシュのフラグを受け取った。
        // アンプレイアブルの時、プッシュのフラグを受け取る間隔によっては、プッシュがスキップされる。
        void Idle2Push()
        {
            if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
            {
                if (S_SO.IsPlayable)
                {
                    if (!isOnPushCooltime && IA.InputGetter.Instance.IsPush && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.PUSH;
                        isOnPushCooltime = true;
                        if (gameObject.activeSelf) StartCoroutine(CountPushCooltime());
                        SoundManager.Instance.PlaySE(3);
                    }
                }
                else
                {
                    if (isPushIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isPushIfUnplayable = false;

                        StartCoroutine(UnPlayablePushDir(true));

                        SoundManager.Instance.PlaySE(3);
                    }
                }
            }
        }

        // COUNTER => PUSH
        // 【条件】スキル・必殺技の演出中でない、かつ、プレイアブルかつプッシュキーが押されたまたはアンプレイアブルかつプッシュのフラグを受け取った。
        // アンプレイアブルの時、プッシュのフラグを受け取る間隔によっては、プッシュがスキップされる。
        void Counter2Push()
        {
            if (State == PlayerState.COUNTER && !isOnStateChangeCooltime)
            {
                if (S_SO.IsPlayable)
                {
                    if (!isOnPushCooltime && IA.InputGetter.Instance.IsPush && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.PUSH;
                        Shield(false);
                        isOnPushCooltime = true;
                        knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;
                        if (gameObject.activeSelf) StartCoroutine(CountPushCooltime());
                        SoundManager.Instance.PlaySE(3);
                    }
                }
                else
                {
                    if (isPushIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isPushIfUnplayable = false;

                        StartCoroutine(UnPlayablePushDir(false));

                        SoundManager.Instance.PlaySE(3);
                    }
                }
            }
        }

        IEnumerator UnPlayablePushDir(bool isFromIdle)
        {
            GameObject pushEffect = Instantiate(Enemy1StatusSO.Entity.PushEffectObj, transform.position, Quaternion.identity, transform);
            if (ifUnplayableOnSkill)
            {
                pushEffect.transform.localScale *= Enemy1StatusSO.Entity.SkillSizeCoef;
            }

            yield return new WaitForSeconds(Enemy1StatusSO.Entity.PushDirDur);

            State = PlayerState.PUSH;

            if (!isFromIdle)
            {
                knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                IsCounterBehaviourDone = false;
            }

            SoundManager.Instance.PushSE(false);
        }

        IEnumerator CountPushCooltime()
        {
            float ct = S_SOI.PushCoolTime;
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            // 半透明にする。
            Color _col = gm.PushCooltimeGauge.color;
            _col.a = P_SOB.GaugeAOnCooltime / (float)255;
            gm.PushCooltimeGauge.color = _col;

            gm.PushCooltimeGauge.fillAmount = 0f;

            while (time >= 0f)
            {
                gm.PushCooltimeGauge.fillAmount = -time / ct + 1;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.PushCooltimeGauge.fillAmount = 1f;

            // 不透明に戻す。
            Color col = gm.PushCooltimeGauge.color;
            col.a = 255 / (float)255;
            gm.PushCooltimeGauge.color = col;

            isOnPushCooltime = false;
        }

        // IDLE => COUNTER
        // 【条件】スキル・必殺技の演出中でない、かつ、プレイアブルかつカウンターキーが押されたまたはアンプレイアブルかつカウンターのフラグを受け取った。
        // アンプレイアブルの時、カウンターのフラグを受け取る間隔によっては、カウンターがスキップされる。
        void Idle2Counter()
        {
            if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
            {
                if (S_SO.IsPlayable)
                {
                    if (!isOnCounterCooltime && IA.InputGetter.Instance.IsCounter && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.COUNTER;
                        Shield(true);
                        isOnCounterCooltime = true;
                        if (gameObject.activeSelf) StartCoroutine(CountCounterCooltime());

                        SoundManager.Instance.PlaySE(5);
                    }
                }
                else
                {
                    if (isCounterIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isCounterIfUnplayable = false;

                        State = PlayerState.COUNTER;

                        SoundManager.Instance.PlaySE(5);
                    }
                }
            }
        }

        // PUSH => COUNTER
        // 【条件】スキル・必殺技の演出中でない、かつ、プレイアブルかつカウンターキーが押されたまたはアンプレイアブルかつカウンターのフラグを受け取った。
        // アンプレイアブルの時、カウンターのフラグを受け取る間隔によっては、カウンターがスキップされる。
        void Push2Counter()
        {
            if (State == PlayerState.PUSH && !isOnStateChangeCooltime)
            {
                if (S_SO.IsPlayable)
                {
                    if (!isOnCounterCooltime && IA.InputGetter.Instance.IsCounter && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.COUNTER;
                        Shield(true);
                        isOnCounterCooltime = true;
                        IsPushBehaviourDone = false;
                        if (gameObject.activeSelf) StartCoroutine(CountCounterCooltime());

                        SoundManager.Instance.PlaySE(5);
                    }
                }
                else
                {
                    if (isCounterIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isCounterIfUnplayable = false;

                        State = PlayerState.COUNTER;
                        IsPushBehaviourDone = false;

                        SoundManager.Instance.PlaySE(5);
                    }
                }
            }
        }

        IEnumerator CountCounterCooltime()
        {
            float ct = S_SOI.CounterCoolTime;
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            // 半透明にする。
            Color _col = gm.CounterCooltimeGauge.color;
            _col.a = P_SOB.GaugeAOnCooltime / (float)255;
            gm.CounterCooltimeGauge.color = _col;

            gm.CounterCooltimeGauge.fillAmount = 0f;

            while (time >= 0f)
            {
                gm.CounterCooltimeGauge.fillAmount = -time / ct + 1;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.CounterCooltimeGauge.fillAmount = 1f;

            // 不透明に戻す。
            Color col = gm.CounterCooltimeGauge.color;
            col.a = 255 / (float)255;
            gm.CounterCooltimeGauge.color = col;

            isOnCounterCooltime = false;
        }

        // PUSH => IDLE
        // 【条件】一定時間が経過し、それでもなおPUSH状態。
        void Push2Idle()
        {
            if (State == PlayerState.PUSH && !isOnStateChangeCooltime)
            {
                if (gameObject.activeSelf) StartCoroutine(Push2IdleWithCount());
            }
        }
        IEnumerator Push2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(P_SOB.Duration2IdleOnPushFailed);
            if (State == PlayerState.PUSH)
            {
                State = PlayerState.IDLE;
                IsPushBehaviourDone = false;
            }
            isOnStateChangeCooltime = false;
        }

        // COUNTER => IDLE
        // 【条件】一定時間が経過し、それでもなおCOUNTER状態。
        void Counter2Idle()
        {
            if (State == PlayerState.COUNTER && !isOnStateChangeCooltime)
            {
                if (gameObject.activeSelf) StartCoroutine(Counter2IdleWithCount());
            }
        }
        IEnumerator Counter2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(P_SOB.Duration2IdleOnCounterFailed);
            if (State == PlayerState.COUNTER)
            {
                State = PlayerState.IDLE;
                Shield(false);
                knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                IsCounterBehaviourDone = false;
            }
            isOnStateChangeCooltime = false;
        }

        // KNOCKBACKED => IDLE
        // 【条件】一定時間が経過し、それでもなおKNOCKBACKED状態。
        void Knockbacked2Idle()
        {
            if (State == PlayerState.KNOCKBACKED && !isOnStateChangeCooltime)
            {
                if (gameObject.activeSelf) StartCoroutine(Knockbacked2IdleWithCount());
            }
        }
        IEnumerator Knockbacked2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(P_SOB.Duration2IdleWhenKnockbacked);
            if (State == PlayerState.KNOCKBACKED)
            {
                State = PlayerState.IDLE;
                isKnockbackedBehaviourDone = false;
            }
            isOnStateChangeCooltime = false;
        }

        void Shield(bool isActivate)
        {
            if (S_SO.IsPlayable)
            {
                shield.SetActive(isActivate);
            }
        }
        #endregion

        #region スキル・必殺技の詳細
        void Skill()
        {
            if (S_SO.IsPlayable)
            {
                for (int i = 0; i < isOnSkillCooltimes.Length; i++)
                {
                    if (!isOnSkillCooltimes[i] && IA.InputGetter.Instance.IsSkill/*[i]*/)
                    {
                        isOnSkillCooltimes[i] = true;

                        if (gameObject.activeSelf) StartCoroutine(CountSkillCooltime(i));

                        SoundManager.Instance.SkillSE(true);

                        SkillBehaviour(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < isSkillIfUnplayables.Length; i++)
                {
                    if (isSkillIfUnplayables[i])
                    {
                        isSkillIfUnplayables[i] = false;

                        SoundManager.Instance.SkillSE(false);

                        SkillBehaviour(i);
                    }
                }
            }
        }

        // i番目のスキルを使う
        void SkillBehaviour(int i)
        {
            IsSkillDirection = true;

            StartCoroutine(SkillBehaviourDirection());
        }


        Vector3 sp, op;
        bool action = true, _action = true;
        IEnumerator SkillBehaviourDirection()
        {
            switch (type)
            {
                case TYPE.Ballerina:
                    int pow = BallerinaStatusSO.Entity.SkillPow;
                    float _t = 0;
                    float _d = BallerinaStatusSO.Entity.SkillRiseDur;

                    float t = 0;
                    float d = BallerinaStatusSO.Entity.SkillRushDur;
                    sp = transform.position;
                    op = opponent.transform.position;
                    float ax = (Mathf.Pow(d, pow) + sp.x - op.x) / (pow * d);
                    float ay = (Mathf.Pow(d, pow) + sp.y - op.y) / (pow * d);
                    float az = (Mathf.Pow(d, pow) + sp.z - op.z) / (pow * d);
                    Vector3 a = new Vector3(ax, ay, az);
                    float bx = sp.x - Mathf.Pow(a.x, pow);
                    float by = sp.y - Mathf.Pow(a.y, pow);
                    float bz = sp.z - Mathf.Pow(a.z, pow);
                    Vector3 b = new Vector3(bx, by, bz);

                    SoundManager.Instance.PlaySE(8);

                    while (_action)
                    {
                        _t += Time.deltaTime;

                        Vector3 pos = transform.position;
                        pos.y = -_t * (_t - 2 * _d);
                        transform.position = pos;
                        if (_t > _d)
                        {
                            _action = false;
                        }

                        yield return null;
                    }

                    SoundManager.Instance.PlaySE(9);

                    while (action)
                    {
                        t += Time.deltaTime;

                        float yx = Mathf.Pow(t - a.x, pow) + b.x;
                        float yy = Mathf.Pow(t - a.y, pow) + b.y;
                        float yz = Mathf.Pow(t - a.z, pow) + b.z;

                        transform.position = new Vector3(yx, yy, yz);
                        if (t > d) action = false;

                        yield return null;
                    }

                    break;

                case TYPE.BreakDancer:
                    StartCoroutine(RotationChange());
                    break;

                case TYPE.Enemy1:
                    antiGravity = true;
                    while (transform.position.y < Enemy1StatusSO.Entity.SkillHeight)
                    {
                        Vector3 cPos = transform.position;
                        cPos.y += Enemy1StatusSO.Entity.SkillSpeed * Time.deltaTime;
                        transform.position = cPos;

                        yield return null;
                    }
                    ifUnplayableOnSkill = true;
                    transform.localScale *= Enemy1StatusSO.Entity.SkillSizeCoef;
                    foreach (GameObject e in enemyEffect)
                    {
                        e.transform.localScale *= Enemy1StatusSO.Entity.SkillSizeCoef;
                    }
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(2).gameObject.SetActive(true);
                    weight *= Enemy1StatusSO.Entity.SkillWeightCoef;
                    antiGravity = false;
                    SoundManager.Instance.SkillSE(false);
                    break;
            }

            IsSkillDirection = false;

            switch (type)
            {
                case TYPE.Ballerina:
                    opponentRb.AddForce((op - sp).normalized * S_SOI.PushPower * BallerinaStatusSO.Entity.SkillPushPowerCoef, ForceMode.Impulse);
                    Damage(opponentPm, PlayerState.KNOCKBACKED);
                    break;

                case TYPE.BreakDancer:
                    break;

                case TYPE.Enemy1:
                    float time = 0;
                    while (true)
                    {
                        time += Time.deltaTime;
                        if (time >= Enemy1StatusSO.Entity.SkillDuration)
                        {
                            ifUnplayableOnSkill = false;
                            transform.localScale /= Enemy1StatusSO.Entity.SkillSizeCoef;
                            foreach(GameObject e in enemyEffect)
                            {
                                e.transform.localScale /= Enemy1StatusSO.Entity.SkillSizeCoef;
                            }
                            weight /= Enemy1StatusSO.Entity.SkillWeightCoef;
                            transform.GetChild(1).gameObject.SetActive(true);
                            transform.GetChild(2).gameObject.SetActive(false);
                            break;
                        }
                        yield return null;
                    }
                    break;
            }

            yield break;
        }

        IEnumerator RotationChange()
        {
            float t = 0;
            float d = BreakDancerStatusSO.Entity.SkillRotChangeDur;
            while (true)
            {
                t += Time.deltaTime;
                rotDir = -2 * t / d + 1;
                if (t > d)
                {
                    rotDir = -1;
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(BreakDancerStatusSO.Entity.SkillDur);

            t = 0;
            while (true)
            {
                t += Time.deltaTime;
                rotDir = 2 * t / d - 1;
                if (t > d)
                {
                    rotDir = 1;
                    break;
                }

                yield return null;
            }
        }

        IEnumerator CountSkillCooltime(int idx) // idxは0始まりなことに注意！
        {
            float ct = S_SOI.SkillCooltimes[idx];
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            // 半透明にする。
            Color _col = gm.SkillCooltimeGauges[idx].color;
            _col.a = P_SOB.GaugeAOnCooltime / (float)255;
            gm.SkillCooltimeGauges[idx].color = _col;

            gm.SkillCooltimeGauges[0].fillAmount = 0f;

            while (time >= 0f)
            {
                gm.SkillCooltimeGauges[0].fillAmount = -time / ct + 1;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.SkillCooltimeGauges[0].fillAmount = 1f;

            // 不透明に戻す。
            Color col = gm.SkillCooltimeGauges[idx].color;
            col.a = 255 / (float)255;
            gm.SkillCooltimeGauges[idx].color = col;

            isOnSkillCooltimes[idx] = false;

            switch (idx)
            {
                case 0:
                    action = true; _action = true;
                    break;
            }
        }

        void Special()
        {
            if (S_SO.IsPlayable)
            {
                if (specialPoint == S_SOI.SpecialPoint && !isOnSpecialCooltime && IA.InputGetter.Instance.IsSpecial)
                {
                    isOnSpecialCooltime = true;

                    specialPoint = 0;

                    if (gameObject.activeSelf) StartCoroutine(CountSpecialCooltime());

                    SpecialBehaviour();
                }
            }
            else
            {
                if (isSpecialIfUnplayable)
                {
                    isSpecialIfUnplayable = false;

                    SpecialBehaviour();
                }
            }
        }

        // 必殺技を使う
        void SpecialBehaviour()
        {
            IsSpecialDirection = true;

            StartCoroutine(SpecialBehaviourDirection());
        }

        // 必殺技の演出を行う
        IEnumerator SpecialBehaviourDirection()
        {
            /*
            switch (type)
            {
                case TYPE.Ballerina:
                    break;

                case TYPE.BreakDancer:
                    break;

                case TYPE.Enemy1:
                    break;
            }*/

            IsSpecialDirection = false;
            switch (type)
            {
                case TYPE.Ballerina:
                    StartCoroutine(BallerinaCountSpecialTime());
                    break;

                case TYPE.BreakDancer:
                    StartCoroutine(BreakDancerCountSpecialTime());
                    break;

                case TYPE.Enemy1:
                    break;
            }
            yield break;
        }

        IEnumerator BallerinaCountSpecialTime()
        {
            SoundManager.Instance.PlaySE(6);
            SoundManager.Instance.Special(true);

            genericDamageCoef *= BallerinaStatusSO.Entity.GenericDamageCoefCoef;

            GameManager.Instance.OnSpecialLightDir(true);
            GameObject rightWeapon = GameObject.FindGameObjectWithTag("RightWeapon").transform.GetChild(0).gameObject;
            GameObject leftWeapon = GameObject.FindGameObjectWithTag("LeftWeapon").transform.GetChild(0).gameObject;
            rightWeapon.SetActive(true);
            leftWeapon.SetActive(true);
            Coroutine rightCor = StartCoroutine(rightWeapon.GetComponent<MeshTrail>().TrailCreate());
            Coroutine leftCor = StartCoroutine(leftWeapon.GetComponent<MeshTrail>().TrailCreate());

            yield return new WaitForSeconds(BallerinaStatusSO.Entity.SpecialDur);

            genericDamageCoef /= BallerinaStatusSO.Entity.GenericDamageCoefCoef;

            SoundManager.Instance.Special(false);

            genericDamageCoef /= BallerinaStatusSO.Entity.OnWeakGenericDamageCoefCoef;

            GameManager.Instance.OnSpecialLightDir(false);
            StopCoroutine(rightCor);
            rightCor = null;
            StopCoroutine(leftCor);
            leftCor = null;
            rightWeapon.SetActive(false);
            leftWeapon.SetActive(false);

            yield return new WaitForSeconds(BallerinaStatusSO.Entity.WeakDur);

            genericDamageCoef *= BallerinaStatusSO.Entity.OnWeakGenericDamageCoefCoef;

            yield break;
        }

        IEnumerator BreakDancerCountSpecialTime()
        {
            /*光る*/
            pushPowerCoef *= BreakDancerStatusSO.Entity.SpecialPushPowerCoefCoef;
            rotSpe *= BreakDancerStatusSO.Entity.SpecialRotSpeedCoef;

            yield return new WaitForSeconds(BreakDancerStatusSO.Entity.SpecialDur);

            /*光らなくする*/
            pushPowerCoef /= BreakDancerStatusSO.Entity.SpecialPushPowerCoefCoef;
            rotSpe /= BreakDancerStatusSO.Entity.SpecialRotSpeedCoef;

            yield break;
        }

        IEnumerator CountSpecialCooltime()
        {
            float ct = S_SOI.SpecialCooltime;
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            gm.SpecialCooltimeGauge.fillAmount = 1f;

            while (time >= 0f)
            {
                gm.SpecialCooltimeGauge.fillAmount = time / ct;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.SpecialCooltimeGauge.fillAmount = 0f;

            isOnSpecialCooltime = false;
        }

        void ShowSpecialPoint()
        {
            if (S_SO.IsPlayable)
            {
                gm.SpecialGauge.fillAmount = specialPoint / (float)S_SOI.SpecialPoint;
                if (specialPoint == S_SOI.SpecialPoint && !isOnSpecialCooltime)
                {
                    // 必殺技が発動できることを知らせる
                    gm.SpecialGauge.color = Color.yellow;
                }
                else
                {
                    gm.SpecialGauge.color = Color.green;
                }
            }
        }
        #endregion

        #region PlayerStateに基づくベイの行動処理の詳細
        // 1.地面に垂直な姿勢制御をする。
        // 2.自転する。ただし、HPが低くなったら歳差運動に切り替わる。
        void Rotate()
        {
            // HPが一定以下かどうかチェックし、フラグを切り替える。
            isHpLow = Hp < S_SOI.Hp * P_SOB.AxisSlopeStartHpCoef ? true : false;

            // 回転処理を行う前に、ベイのローカルy軸（緑）の方向を地面の法線ベクトルに合わせる。
            Ray shotRay = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(shotRay, out RaycastHit ground))
            {
                Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, P_SOB.PlayerMainAxisChangeSpeed * Time.deltaTime);
            }

            // HPが一定以下になったら、歳差運動をする。
            if (isHpLow)
            {
                // 指定秒数ごとに回転軸の傾きを変化させる。
                axisTimer += Time.deltaTime;
                if (axisTimer > P_SOB.AxisSlopeChangeInterval)
                {
                    axisTimer -= P_SOB.AxisSlopeChangeInterval;
                    float theta = Random.Range(P_SOB.AxisSlopRange.x, P_SOB.AxisSlopRange.y);
                    axis = Quaternion.AngleAxis(theta, transform.forward) * transform.up;
                }

                // 回転軸を中心軸（transform.up）周りに回転させる。
                float axisSpeed = P_SOB.AxisRotateSpeed / S_SOI.Hp * Hp;
                axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * axis;
            }
            // そうでないなら、自転する。
            else
            {
                axis = transform.up;
            }

            // ベイを回転軸周りに回転させる。
            float rotSpeed = rotSpe / S_SOI.Hp * Hp;
            float minRotSpeed = P_SOB.RotationSpeedCoefRange.x * rotSpe;
            float maxRotSpeed = P_SOB.RotationSpeedCoefRange.y * rotSpe;
            rotSpeed = Mathf.Clamp(rotSpeed, minRotSpeed, maxRotSpeed); // 角速度を制限する。
            rotSpeed *= rotDir;
            transform.localRotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, axis) * transform.localRotation;
        }

        // IDLE状態では、現状何もしない。
        void Idle()
        {
            if (State == PlayerState.IDLE)
            {
                return;
            }
        }

        // PUSH状態では、1回だけ敵に向かって瞬間的に力を加える。
        void Push()
        {
            if (State == PlayerState.PUSH)
            {
                if (!IsPushBehaviourDone)
                {
                    IsPushBehaviourDone = true;

                    float power = S_SOI.PushPower * pushPowerCoef;
                    rb.AddForce((opponent.transform.position - transform.position).normalized * power, ForceMode.Impulse);
                }
            }
        }

        // COUNTER状態では、1回だけノックバック耐性を大きく乗算する。
        void Counter()
        {
            if (State == PlayerState.COUNTER)
            {
                if (!IsCounterBehaviourDone)
                {
                    IsCounterBehaviourDone = true;
                    knoRes *= P_SOB.KnockbackResistanceCoefOnCounter;
                }
            }
        }

        // KNOCKBACKED状態では、1回だけ以下の処理を行う。
        // ・自分と敵の運動量の大きさを合計し、その定数倍の瞬間的な力（最低保証あり）を、敵と反対方向に加える。
        void Knockbacked()
        {
            if (State == PlayerState.KNOCKBACKED)
            {
                if (!isKnockbackedBehaviourDone)
                {
                    isKnockbackedBehaviourDone = true;

                    float self = (weight * rb.velocity).magnitude;
                    float oppo = (opponentPm.weight * opponentRb.velocity).magnitude;
                    float power = (self + oppo) * P_SOB.PowerCoefOnKnockbacked;
                    if (power < P_SOB.MinPowerOnKnockbacked)
                    {
                        power = P_SOB.MinPowerOnKnockbacked;
                    }
                    rb.AddForce((transform.position - opponent.transform.position).normalized * power, ForceMode.Impulse);

                    //Vector3 self2Opponent = opponent.transform.position - transform.position;
                    //Vector3 v = rb.velocity;
                    //if (Vector3.Dot(self2Opponent, v) > 0)
                    //{
                    //    rb.velocity *= -1;
                    //}
                }
            }
        }
        #endregion

        #region デス判定の詳細

        void JudgeFall()
        {
            if (transform.position.y < P_SOB.FallJudgeY) transform.position = rePos;
        }

        void JudgeDeath()
        {
            if (Hp < 0)
            {
                StopAllCoroutines(); // コルーチンをすべて停止
                if (S_SO.IsPlayable)
                {
                    gm.PlayableBar.fillAmount = 0f;
                }
                else
                {
                    gm.UnPlayableBar.fillAmount = 0f;
                }

                // 勝利/敗北の処理を発火する
                if (!S_SO.IsPlayable)
                {
                    GameManager.Instance.Win();
                }
                else
                {
                    GameManager.Instance.Lose();
                }

                gameObject.SetActive(false); // 非アクティブにする
            }
        }
        #endregion

        public PlayerState GetState()
        {
            return State;
        }

        public PlayerState GetOpponentState()
        {
            return opponentPm.GetState();
        }
    }
}
