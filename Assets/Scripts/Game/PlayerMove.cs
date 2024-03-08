using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    StatusTable status;
    float Hp { get; set; }
    float weight;
    float knockbackResistance; // 内部的にRigidbody.drag（＝抵抗）を操作している

    // 敵への参照を取得する用
    GameObject opponent; // ゲームオブジェクト
    PlayerMove opponentPlayerMove; // PlayerMoveクラス
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

        // 対応するStatusSOを取得し、変動する数値を改めて、このクラス内の変数に格納
        switch (type)
        {
            case TYPE.Ballerina:
                status = BallerinaStatusSO.Entity.Status;
                break;
            case TYPE.BreakDancer:
                status = BreakDancerStatusSO.Entity.Status;
                break;
            case TYPE.NULL:
                Debug.LogError("<color=red>typeが設定されていません</color>");
                break;
        }
        Hp = status.Hp;
        weight = status.Weight;
        knockbackResistance = status.KnockbackResistance;

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
        opponentPlayerMove = opponent.GetComponent<PlayerMove>();
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
        rb.drag = pSO.DragCoef * knockbackResistance; // drag：抵抗
    }
    #endregion

    #region 【OnCollision】敵との接触を検知し、無敵時間でないならば、ダメージ処理を行いかつ条件によって自分または敵をKNOCKBACKED状態にする。
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isDamageManager)
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
        yield return new WaitForSeconds(pSO.DamagableDuration);
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
    // 【条件】（敵と接触した際に呼ばれる。）自分がPUSHで敵がIDLE、または、自分がCOUNTERで敵がPUSH
    // 【その他】ダメージ処理も行う。
    void HitBehaviour()
    {
        switch (opponentPlayerMove.State)
        {
            case PlayerState.IDLE:
                if (State == PlayerState.PUSH)
                {
                    State = PlayerState.IDLE;
                    IsPushBehaviourDone = false;

                    opponentPlayerMove.State = PlayerState.KNOCKBACKED;

                    Damage(this, PlayerState.PUSH);
                    Damage(opponentPlayerMove, PlayerState.KNOCKBACKED);

                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{name} が {opponent.name} にプッシュした！</color>");
                    }
                }
                else
                {
                    Damage(this, State);
                    Damage(opponentPlayerMove, PlayerState.IDLE);
                }
                break;

            case PlayerState.PUSH:
                if (State == PlayerState.IDLE)
                {
                    State = PlayerState.KNOCKBACKED;

                    opponentPlayerMove.State = PlayerState.IDLE;
                    opponentPlayerMove.IsPushBehaviourDone = false;

                    Damage(this, PlayerState.KNOCKBACKED);
                    Damage(opponentPlayerMove, PlayerState.PUSH);

                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{opponent.name} が {name} にプッシュした！</color>");
                    }
                }
                else if (State == PlayerState.COUNTER)
                {
                    State = PlayerState.IDLE;
                    knockbackResistance /= pSO.KnockbackResistanceCoefOnCounter;
                    IsCounterBehaviourDone = false;

                    opponentPlayerMove.State = PlayerState.KNOCKBACKED;
                    opponentPlayerMove.IsPushBehaviourDone = false;

                    Damage(this, PlayerState.COUNTER);
                    Damage(opponentPlayerMove, PlayerState.KNOCKBACKED);

                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{name} が {opponent.name} にカウンターした！</color>");
                    }
                }
                else
                {
                    Damage(this, State);
                    Damage(opponentPlayerMove, PlayerState.PUSH);
                }
                break;

            case PlayerState.COUNTER:
                if (State == PlayerState.PUSH)
                {
                    State = PlayerState.KNOCKBACKED;
                    IsPushBehaviourDone = false;

                    opponentPlayerMove.State = PlayerState.IDLE;
                    opponentPlayerMove.knockbackResistance /= pSO.KnockbackResistanceCoefOnCounter;
                    opponentPlayerMove.IsCounterBehaviourDone = false;

                    Damage(this, PlayerState.KNOCKBACKED);
                    Damage(opponentPlayerMove, PlayerState.COUNTER);

                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{opponent.name} が {name} にカウンターした！</color>");
                    }
                }
                else
                {
                    Damage(this, State);
                    Damage(opponentPlayerMove, PlayerState.COUNTER);
                }
                break;

            case PlayerState.KNOCKBACKED:
                Damage(this, State);
                Damage(opponentPlayerMove, PlayerState.KNOCKBACKED);
                break;
        }
    }

    // 与えられたPlayerMoveクラスのインスタンスに、自身の速さを元にして、そのインスタンスが与えられた状態である場合の、ダメージを与える。
    void Damage(PlayerMove playerMoveInstance, PlayerState State)
    {
        if (isDamageManager)
        {
            switch (State)
            {
                // IDLE状態なら、通常のダメージを食らう。
                case PlayerState.IDLE:
                    playerMoveInstance.Hp -= rb.velocity.magnitude * pSO.DamageCoef;
                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{playerMoveInstance.gameObject.name} に通常のダメージを与える</color>");
                    }
                    break;

                // PUSH状態なら、ダメージが減る。
                case PlayerState.PUSH:
                    playerMoveInstance.Hp -= rb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnPush;
                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{playerMoveInstance.gameObject.name} に与えるダメージを減らす</color>");
                    }
                    break;

                // COUNTER状態なら、ダメージを食らわない。
                case PlayerState.COUNTER:
                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{playerMoveInstance.gameObject.name} にダメージを与えない</color>");
                    }
                    break;

                // KNOCKBACKED状態なら、ダメージが増える。
                case PlayerState.KNOCKBACKED:
                    playerMoveInstance.Hp -= rb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnKnockbacked;
                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{playerMoveInstance.gameObject.name} に与えるダメージを増やす</color>");
                    }
                    break;
            }
        }
    }

    // IDLE => PUSH
    // 【条件】プッシュキーが押された。
    void Idle2Push()
    {
        if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
        {
            if (Input.GetKeyDown(status.PushKey))
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
            if (Input.GetKeyDown(status.PushKey))
            {
                State = PlayerState.PUSH;
                knockbackResistance /= pSO.KnockbackResistanceCoefOnCounter;
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
            if (Input.GetKeyDown(status.CounterKey))
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
            if (Input.GetKeyDown(status.CounterKey))
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
        yield return new WaitForSeconds(pSO.Duration2IdleOnPushFailed);
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
        yield return new WaitForSeconds(pSO.Duration2IdleOnCounterFailed);
        if (State == PlayerState.COUNTER)
        {
            State = PlayerState.IDLE;
            knockbackResistance /= pSO.KnockbackResistanceCoefOnCounter;
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
        yield return new WaitForSeconds(pSO.Duration2IdleWhenKnockbacked);
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
        // 回転処理を行う前に、ベイのローカルy軸（緑）の方向を地面の法線ベクトルに合わせる。
        Ray shotRay = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(shotRay, out RaycastHit ground))
        {
            Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, pSO.PlayerMainAxisChangeSpeed * Time.deltaTime);
        }

        // HPが一定以下になったら、歳差運動をする。
        if (Hp < status.Hp * pSO.AxisSlopeStartHpCoef)
        {
            // 指定秒数ごとに回転軸の傾きを変化させる。
            axisTimer += Time.deltaTime;
            if (axisTimer > pSO.AxisSlopeChangeInterval)
            {
                axisTimer -= pSO.AxisSlopeChangeInterval;
                float theta = UnityEngine.Random.Range(pSO.AxisSlopRange.x, pSO.AxisSlopRange.y);
                axis = Quaternion.AngleAxis(theta, transform.forward) * transform.up;
            }

            // 回転軸を中心軸（transform.up）周りに回転させる。
            float axisSpeed = pSO.AxisRotateSpeed / status.Hp * Hp;
            axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * axis;
        }
        // そうでないなら、自転する。
        else
        {
            axis = transform.up;
        }

        // ベイを回転軸周りに回転させる。
        float rotSpeed = status.RotationSpeed / status.Hp * Hp;
        float minRotSpeed = pSO.RotationSpeedCoefRange.x * status.RotationSpeed;
        float maxRotSpeed = pSO.RotationSpeedCoefRange.y * status.RotationSpeed;
        rotSpeed = Mathf.Clamp(rotSpeed, minRotSpeed, maxRotSpeed);　// 角速度を制限する。
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
                rb.AddForce((opponent.transform.position - transform.position).normalized * status.PushPower, ForceMode.Impulse);
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
                knockbackResistance *= pSO.KnockbackResistanceCoefOnCounter;
            }
        }
    }

    // KNOCKBACKED状態では、1回だけ以下の処理を行う。
    // ・自分と敵の速度ベクトルの大きさを合計し、その定数倍の瞬間的な力（最低保証あり）を、敵と反対方向に加える。
    void Knockbacked()
    {
        if (State == PlayerState.KNOCKBACKED)
        {
            if (!isKnockbackedBehaviourDone)
            {
                isKnockbackedBehaviourDone = true;

                float power = (rb.velocity.magnitude + opponentRb.velocity.magnitude) * pSO.PowerCoefOnKnockbacked;
                if (power < pSO.MinPowerOnKnockbacked)
                {
                    power = pSO.MinPowerOnKnockbacked;
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
    // HPが0を切ったら非アクティブにし、そうでないなら各種変数を表示する。
    void ShowHP()
    {
        if (Hp < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            text.text = $"{Hp}\n{State}";
        }
    }
    #endregion
}
