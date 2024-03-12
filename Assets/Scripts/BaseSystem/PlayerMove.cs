using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BaseSystem
{
    public class PlayerMove : MonoBehaviour
    {
        #region 変数宣言
        // ベイの種類を識別する。インスペクタ上で設定しておくこと！
        enum TYPE { NULL, Ballerina, BreakDancer }
        [SerializeField] TYPE type = TYPE.NULL;

        // プレイヤーの状態(public)
        public enum PlayerState { IDLE, PUSH, COUNTER, KNOCKBACKED };
        public PlayerState State { get; set; }

        // 物理演算用
        Rigidbody rb;
        GameObject stageCenter;

        // SOからデータを取得する用
        PlayerSO pSO;
        BehaviourTable pSOB;
        DamageTable pSOD;
        StatusTable initStatus;
        float Hp { get; set; }
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
        bool IsPushBehaviourDone { get; set; } = false; // プッシュの処理を、完了しているか
        bool IsCounterBehaviourDone { get; set; } = false; // カウンターの処理を、完了しているか
        bool isKnockbackedBehaviourDone = false; // ノックバックされた時の処理を、完了しているか
        bool isOnStateChangeCooltime = false; // 時間経過による状態変化中であるかどうか
        Vector3 axis; // ベイの回転軸
        float axisTimer = 0; // ベイの回転軸を傾ける時間
        bool isHpLow = false; // HPが一定以下になって、歳差運動をしているかどうか

        // このベイに対応するテキストを取得する用
        TextMeshProUGUI text;
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

            // PlayerSOを取得
            pSO = PlayerSO.Entity;
            pSOB = pSO.BehaviourTable;
            pSOD = pSO.DamageTable;

            // 対応するStatusTableを取得し、変動する数値を改めて、このクラス内の変数に格納
            switch (type)
            {
                case TYPE.Ballerina:
                    initStatus = BallerinaStatusSO.Entity.StatusTable;
                    break;
                case TYPE.BreakDancer:
                    initStatus = BreakDancerStatusSO.Entity.StatusTable;
                    break;
                case TYPE.NULL:
                    Debug.LogError("<color=red>typeが設定されていません</color>");
                    break;
            }
            Hp = initStatus.Hp;
            weight = initStatus.Weight;
            knoRes = initStatus.KnockbackResistance;
            rotSpe = initStatus.RotationSpeed + SelectTeam.SceneChange.RotateNumber;

            // 敵への参照を取得
            int idx = Array.IndexOf(GameManager.Instance.Beys, gameObject);
            if (idx == 0)
            {
                opponent = GameManager.Instance.Beys[1];
                isDamageManager = true; // 0番目のインスタンスで、ダメージ処理を行う。
            }
            else if (idx == 1)
            {
                opponent = GameManager.Instance.Beys[0];
            }
            else
            {
                Debug.LogError("<color=red>敵オブジェクトの取得に失敗しました</color>");
            }
            opponentPm = opponent.GetComponent<PlayerMove>();
            opponentRb = opponent.GetComponent<Rigidbody>();

            // このベイに対応するテキストを取得して、HP表示
            text = GameManager.Instance.Texts[Array.IndexOf(GameManager.Instance.Beys, gameObject)];

            // ゲーム開始から少しの間は、無敵時間になっている。
            StartCoroutine(CountDamagableDuration());
        }
        #endregion

        #region 【FixedUpdate】まず、ベイの物理状態を更新する。次に、すり鉢状のフィールドにおける、重力による挙動を再現する。
        void FixedUpdate()
        {
            // ベイの物理状態の更新
            ChangeRigidbodyParameters();

            // 重力
            rb.AddForce(Vector3.down * 9.81f * pSO.GravityScale, ForceMode.Force);

            // 常に中心へ移動
            rb.AddForce((stageCenter.transform.position - transform.position).normalized * pSO.SpeedTowardCenter, ForceMode.Force);

        }

        // ベイの現在のステータスに基づいて、Rigidbodyのパラメーターを更新する。
        void ChangeRigidbodyParameters()
        {
            rb.mass = weight; // mass：重量
            rb.drag = pSOB.DragCoef * knoRes; // drag：抵抗
        }
        #endregion

        #region 【OnCollision】【一人しか行わない】敵との接触を検知し、無敵時間でないならば、ダメージ処理を行いかつ条件によって自分または敵をKNOCKBACKED状態にする。
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(pSO.BeyTagName) && isDamageManager)
            {
                if (isDamagable)
                {
                    isDamagable = false;
                    HitBehaviour(); // PlayerStateの遷移など
                    StartCoroutine(CountDamagableDuration()); // 無敵時間のカウント
                }
            }
        }
        IEnumerator CountDamagableDuration()
        {
            yield return new WaitForSeconds(pSOD.DamagableDuration);
            isDamagable = true;
        }
        #endregion

        #region 【Update】
        void Update()
        {
            #region PlayerStateの遷移：遷移クールタイム中でないなら、プレイヤーの入力を取得して、PUSH状態またはCOUNTER状態になる。
            Idle2Push();
            Counter2Push();
            Idle2Counter();
            Push2Counter();
            #endregion

            #region PlayerStateに基づくベイの行動処理
            Rotate(); // 最初に回転処理を行う。
            Idle();
            Push();
            Counter();
            Knockbacked();
            #endregion

            #region ベイのスクリプトの、各種変数の表示
            ShowHP(); // デス判定も行う。
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

                        Damage(this, PlayerState.PUSH);
                        Damage(opponentPm, PlayerState.KNOCKBACKED);

                        if (pSO.IsShowNormalLog)
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

                        if (pSO.IsShowNormalLog)
                        {
                            Debug.Log($"<color=#64ff64>{opponent.name} が {name} にプッシュした！</color>");
                        }
                    }
                    else if (State == PlayerState.COUNTER)
                    {
                        State = PlayerState.IDLE;
                        knoRes /= pSOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;

                        opponentPm.State = PlayerState.KNOCKBACKED;
                        opponentPm.IsPushBehaviourDone = false;

                        Damage(this, PlayerState.COUNTER);
                        Damage(opponentPm, PlayerState.KNOCKBACKED);

                        if (pSO.IsShowNormalLog)
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

                        opponentPm.State = PlayerState.IDLE;
                        opponentPm.knoRes /= pSOB.KnockbackResistanceCoefOnCounter;
                        opponentPm.IsCounterBehaviourDone = false;

                        Damage(this, PlayerState.KNOCKBACKED);
                        Damage(opponentPm, PlayerState.COUNTER);

                        if (pSO.IsShowNormalLog)
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
            float weightAdjustValue = CalcMrkDamage(weight, initStatus.Weight); // 重量補正値
            float rotSpeAdjustValue = CalcMrkDamage(rotSpe, initStatus.RotationSpeed); // 回転速度補正値
            float knoResAdjustValue = CalcMrkDamage(knoRes, initStatus.KnockbackResistance); // ノックバック耐性補正値
            float hpAdjustValue = pm.isHpLow ? pSOD.HpAdjustValue[0] : pSOD.HpAdjustValue[1]; // 体力補正値
            float statusAdjustValue = weightAdjustValue * rotSpeAdjustValue * knoResAdjustValue * hpAdjustValue;

            // 状態補正値
            float stateAdjustValue = 0f;
            switch (state)
            {
                case PlayerState.IDLE:
                    stateAdjustValue = pSOD.StateAdjustValue[0];
                    break;

                case PlayerState.PUSH:
                    stateAdjustValue = pSOD.StateAdjustValue[1];
                    break;

                case PlayerState.COUNTER:
                    stateAdjustValue = pSOD.StateAdjustValue[2];
                    break;

                case PlayerState.KNOCKBACKED:
                    stateAdjustValue = pSOD.StateAdjustValue[3];
                    break;
            }

            // ダメージ係数
            float damageCoef = pSOD.DamageCoef;

            float damage = baseDamage * statusAdjustValue * stateAdjustValue * damageCoef; // ダメージ計算
            pm.Hp -= damage; // 与えられたインスタンスにダメージを与える
            if (pSO.IsShowNormalLog) // ログを表示
            {
                Debug.Log($"<color=#64ff64>{pm.gameObject.name} に {damage} ダメージ！</color>");
            }
        }
        // 重量補正値/回転速度補正値/ノックバック耐性補正値 の計算
        float CalcMrkDamage(float x, float d)
        {
            float k = -2 / d * Mathf.Log(2 - pSOD.MrkAdjustValueY);
            if (x < d)
            {
                return -Mathf.Exp(k * (x - d)) + 2;
            }
            else
            {
                float m = -k * Mathf.Exp(k * (pSOD.MrkAdjustValueX - d));
                return m * (x - d) + 1;
            }
        }

        // IDLE => PUSH
        // 【条件】プッシュキーが押された。
        void Idle2Push()
        {
            if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
            {
                if (Input.GetKeyDown(initStatus.PushKey))
                {
                    State = PlayerState.PUSH;
                }
            }
        }

        // COUNTER => PUSH
        // 【条件】プッシュキーが押された。
        void Counter2Push()
        {
            if (State == PlayerState.COUNTER && !isOnStateChangeCooltime)
            {
                if (Input.GetKeyDown(initStatus.PushKey))
                {
                    State = PlayerState.PUSH;
                    knoRes /= pSOB.KnockbackResistanceCoefOnCounter;
                    IsCounterBehaviourDone = false;
                }
            }
        }

        // IDLE => COUNTER
        // 【条件】カウンターキーが押された。
        void Idle2Counter()
        {
            if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
            {
                if (Input.GetKeyDown(initStatus.CounterKey))
                {
                    State = PlayerState.COUNTER;
                }
            }
        }

        // PUSH => COUNTER
        // カウンターキーが押された。
        void Push2Counter()
        {
            if (State == PlayerState.PUSH && !isOnStateChangeCooltime)
            {
                if (Input.GetKeyDown(initStatus.CounterKey))
                {
                    State = PlayerState.COUNTER;
                    IsPushBehaviourDone = false;
                }
            }
        }

        // PUSH => IDLE
        // 【条件】一定時間が経過し、それでもなおPUSH状態。
        void Push2Idle()
        {
            if (State == PlayerState.PUSH && !isOnStateChangeCooltime)
            {
                StartCoroutine(Push2IdleWithCount());
            }
        }
        IEnumerator Push2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(pSOB.Duration2IdleOnPushFailed);
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
                StartCoroutine(Counter2IdleWithCount());
            }
        }
        IEnumerator Counter2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(pSOB.Duration2IdleOnCounterFailed);
            if (State == PlayerState.COUNTER)
            {
                State = PlayerState.IDLE;
                knoRes /= pSOB.KnockbackResistanceCoefOnCounter;
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
                StartCoroutine(Knockbacked2IdleWithCount());
            }
        }
        IEnumerator Knockbacked2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(pSOB.Duration2IdleWhenKnockbacked);
            if (State == PlayerState.KNOCKBACKED)
            {
                State = PlayerState.IDLE;
                isKnockbackedBehaviourDone = false;
            }
            isOnStateChangeCooltime = false;
        }
        #endregion

        #region PlayerStateに基づくベイの行動処理の詳細
        // 1.地面に垂直な姿勢制御をする。
        // 2.自転する。ただし、HPが低くなったら歳差運動に切り替わる。
        void Rotate()
        {
            // HPが一定以下かどうかチェックし、フラグを切り替える。
            isHpLow = Hp < initStatus.Hp * pSOB.AxisSlopeStartHpCoef ? true : false;

            // 回転処理を行う前に、ベイのローカルy軸（緑）の方向を地面の法線ベクトルに合わせる。
            Ray shotRay = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(shotRay, out RaycastHit ground))
            {
                Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, pSOB.PlayerMainAxisChangeSpeed * Time.deltaTime);
            }

            // HPが一定以下になったら、歳差運動をする。
            if (isHpLow)
            {
                // 指定秒数ごとに回転軸の傾きを変化させる。
                axisTimer += Time.deltaTime;
                if (axisTimer > pSOB.AxisSlopeChangeInterval)
                {
                    axisTimer -= pSOB.AxisSlopeChangeInterval;
                    float theta = UnityEngine.Random.Range(pSOB.AxisSlopRange.x, pSOB.AxisSlopRange.y);
                    axis = Quaternion.AngleAxis(theta, transform.forward) * transform.up;
                }

                // 回転軸を中心軸（transform.up）周りに回転させる。
                float axisSpeed = pSOB.AxisRotateSpeed / initStatus.Hp * Hp;
                axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * axis;
            }
            // そうでないなら、自転する。
            else
            {
                axis = transform.up;
            }

            // ベイを回転軸周りに回転させる。
            float rotSpeed = rotSpe / initStatus.Hp * Hp;
            float minRotSpeed = pSOB.RotationSpeedCoefRange.x * rotSpe;
            float maxRotSpeed = pSOB.RotationSpeedCoefRange.y * rotSpe;
            rotSpeed = Mathf.Clamp(rotSpeed, minRotSpeed, maxRotSpeed); // 角速度を制限する。
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
                    rb.AddForce((opponent.transform.position - transform.position).normalized * initStatus.PushPower, ForceMode.Impulse);
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
                    knoRes *= pSOB.KnockbackResistanceCoefOnCounter;
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
                    float power = (self + oppo) * pSOB.PowerCoefOnKnockbacked;
                    if (power < pSOB.MinPowerOnKnockbacked)
                    {
                        power = pSOB.MinPowerOnKnockbacked;
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

        #region ベイのスクリプトの、各種変数の表示の詳細
        void ShowHP()
        {
            if (Hp < 0) // HPが0を切ったら、このゲームオブジェクトを非アクティブにする。
            {
                StopAllCoroutines();
                text.text = "0";

                gameObject.SetActive(false);
            }
            else
            {
                text.text = $"{Hp}\n{State}";
            }
        }
        #endregion
    }
}
