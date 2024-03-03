using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ※敵がKNOCKBACKED状態になっている1秒間、自分がPUSH状態またはCOUNTER状態になっても、即座にIDLE状態に戻ってしまう。今後クールタイムをつければ解決するか？
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
    StatusSO status;
    float hp;
    float weight;
    float knockbackResistance; // 内部的にRigidbody.drag（＝抵抗）を操作している

    // 敵への参照を取得する用
    GameObject opponent; // ゲームオブジェクト
    PlayerMove opponentPlayerMove; // PlayerMoveクラス
    Rigidbody opponentRb; // Rigidbody

    // ベイの行動処理用
    bool isHitOpponent = false; // 敵にぶつかっているか
    bool isDamaged = false; // 敵との接触時において、既にダメージを食らっているか
    bool isDamagable = true; // 無敵時間中でない（＝ダメージを食らえる）かどうか
    bool isKnockbackResistanceMultipliedOnCounter = false; // カウンター時のノックバック耐性の乗算処理を、完了しているか
    bool isVelocityInvertedOnKnockbacked = false; // ノックバックされた時の速度の反転処理を、完了しているか
    bool isAddedImpulseOnPush = false; // プッシュ時に、既に力を加えたか
    Vector3 axis; // ベイの回転軸
    float axisTimer = 0; // ベイの回転軸を傾ける時間

    // このベイに対応するテキストを取得する用
    TextMeshProUGUI text;
    #endregion

    void Start()
    {
        // プレイヤーの初期状態を設定・敵オブジェクトとそのPlayerMoveクラスを取得して保持しておく
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
        hp = status.Hp;
        weight = status.Weight;
        knockbackResistance = status.KnockbackResistance;

        // 敵への参照を取得
        int idx = Array.IndexOf(GameManager.Instance.Beys, gameObject);
        if (idx == 0) opponent = GameManager.Instance.Beys[1];
        else if (idx == 1) opponent = GameManager.Instance.Beys[0];
        else Debug.LogError("<color=red>敵オブジェクトの取得に失敗しました</color>");
        opponentPlayerMove = opponent.GetComponent<PlayerMove>();
        opponentRb = opponent.GetComponent<Rigidbody>();

        // このベイに対応するテキストを取得して、HP表示
        text = GameManager.Instance.Texts[Array.IndexOf(GameManager.Instance.Beys, gameObject)];
        text.text = hp.ToString();
    }

    void Update()
    {
        #region PlayerStateの遷移：プレイヤーの入力を取得して、PUSH状態またはCOUNTER状態になる。
        Idle2Push();
        Counter2Push();
        Idle2Counter();
        Push2Counter();
        #endregion

        #region PlayerStateの遷移：KNOCKBACKED状態になる。
        Idle2Knockbacked();
        Push2Knockbacked();
        #endregion

        #region PlayerStateに基づくベイの行動処理
        Rotate(); // 最初に回転処理を行う。
        Idle();
        Push();
        Counter();
        Knockbacked();
        ChangeRigidbodyParameters(); // ベイの物理状態の更新
        #endregion

        #region ベイのHP処理
        DamageManagement();
        ShowHP(); // ベイの状態も表示する。デス判定も行う。
        #endregion

        #region PlayerStateの遷移：IDLE状態になる。
        Push2Idle();
        Counter2Idle();
        Knockbacked2Idle();
        #endregion
    }

    #region 【物理演算】すり鉢状のフィールドにおける、重力による挙動を再現する。
    void FixedUpdate()
    {
        // 重力
        rb.AddForce(Vector3.down * 9.81f * pSO.GravityScale, ForceMode.Force);

        // 常に中心へ移動
        rb.AddForce((stageCenter.transform.position - transform.position).normalized * pSO.SpeedTowardCenter, ForceMode.Force);

    }
    #endregion

    #region 【コリジョン判定】敵との接触状態の変化を検知し、ダメージ処理時に使うフラグを変化させる。
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isHitOpponent = true; // isHitOpponentの切り替え
            isDamaged = false; // isDamagedのリセット
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isHitOpponent = false; // isHitOpponentの切り替え
            isDamaged = false; // isDamagedのリセット
        }
    }
    #endregion



    #region　PlayerStateの遷移の詳細（対応する状態の時、【条件】を満たしたら即座に遷移する。【その他】の処理も追加で行う。）
    // IDLE => PUSH
    // 【条件】プッシュキーが押された。
    void Idle2Push()
    {
        if (State == PlayerState.IDLE)
        {
            if (Input.GetKeyDown(status.PushKey))
            {
                State = PlayerState.PUSH;
            }
        }
    }

    // COUNTER => PUSH
    // 【条件】プッシュキーが押された。
    // 【その他】
    // ・COUNTER状態への遷移時に行ったノックバック耐性の乗算を、打ち消す除算を行う。
    // ・ カウンター時のノックバック耐性の乗算処理に、関わる変数をリセットする。
    void Counter2Push()
    {
        if (State == PlayerState.COUNTER)
        {
            if (Input.GetKeyDown(status.PushKey))
            {
                State = PlayerState.PUSH;
                knockbackResistance /= pSO.KnockbackResistanceCoefOnCounter;
                isKnockbackResistanceMultipliedOnCounter = false;
            }
        }
    }

    // IDLE => COUNTER
    // 【条件】カウンターキーが押された。
    void Idle2Counter()
    {
        if (State == PlayerState.IDLE)
        {
            if (Input.GetKeyDown(status.CounterKey))
            {
                State = PlayerState.COUNTER;
            }
        }
    }

    // PUSH => COUNTER
    // カウンターキーが押された。
    // 【その他】プッシュ時に力を加える処理に、関わる変数をリセットする。
    void Push2Counter()
    {
        if (State == PlayerState.PUSH)
        {
            if (Input.GetKeyDown(status.CounterKey))
            {
                State = PlayerState.COUNTER;
                isAddedImpulseOnPush = false;
            }
        }
    }

    // IDLE => KNOCKBACKED
    // 【条件】敵と接触している、かつ、敵がPUSH状態。
    void Idle2Knockbacked()
    {
        if (State == PlayerState.IDLE)
        {
            if (isHitOpponent && opponentPlayerMove.State == PlayerState.PUSH)
            {
                State = PlayerState.KNOCKBACKED;
            }
        }
    }

    // PUSH => KNOCKBACKED
    // 【条件】敵と接触している、かつ、敵がCOUNTER状態。
    // 【その他】プッシュ時に力を加える処理に、関わる変数をリセットする。
    void Push2Knockbacked()
    {
        if (State == PlayerState.PUSH)
        {
            if (isHitOpponent && opponentPlayerMove.State == PlayerState.COUNTER)
            {
                State = PlayerState.KNOCKBACKED;
                isAddedImpulseOnPush = false;
            }
        }
    }

    // PUSH => IDLE
    // 【条件】以下のどれかを満たした。
    // 　　　　・敵がKNOCKBACKED状態。
    // 　　　　・敵がKNOCKBACKED状態でない、かつ、一定時間が経過した。
    // 【その他】プッシュ時に力を加える処理に、関わる変数をリセットする。
    void Push2Idle()
    {
        if (State == PlayerState.PUSH)
        {
            if (opponentPlayerMove.State == PlayerState.KNOCKBACKED)
            {
                State = PlayerState.IDLE;
                isAddedImpulseOnPush = false;
            }
            else
            {
                StartCoroutine(Push2IdleWithCount());
            }
        }
    }
    IEnumerator Push2IdleWithCount()
    {
        yield return new WaitForSeconds(pSO.Duration2IdleOnPushFailed);
        State = PlayerState.IDLE;
        isAddedImpulseOnPush = false;
    }

    // COUNTER => IDLE
    // 【条件】以下のどれかを満たした。
    // 　　　　・敵がKNOCKBACKED状態。
    // 　　　　・敵がKNOCKBACKED状態でない、かつ、一定時間が経過した。
    // 【その他】
    // ・COUNTER状態への遷移時に行ったノックバック耐性の乗算を、打ち消す除算を行う。
    // ・ カウンター時のノックバック耐性の乗算処理に、関わる変数をリセットする。
    void Counter2Idle()
    {
        if (State == PlayerState.COUNTER)
        {
            if (opponentPlayerMove.State == PlayerState.KNOCKBACKED)
            {
                State = PlayerState.IDLE;
                knockbackResistance /= pSO.KnockbackResistanceCoefOnCounter;
                isKnockbackResistanceMultipliedOnCounter = false;
            }
            else
            {
                StartCoroutine(Counter2IdleWithCount());
            }
        }
    }
    IEnumerator Counter2IdleWithCount()
    {
        yield return new WaitForSeconds(pSO.Duration2IdleOnCounterFailed);
        State = PlayerState.IDLE;
        knockbackResistance /= pSO.KnockbackResistanceCoefOnCounter;
        isKnockbackResistanceMultipliedOnCounter = false;
    }

    // KNOCKBACKED => IDLE
    // 【条件】一定時間が経過した。
    // 【その他】ノックバックされた時の速度の反転処理に、関わる変数をリセットする。
    void Knockbacked2Idle()
    {
        if (State == PlayerState.KNOCKBACKED)
        {
            StartCoroutine(Knockbacked2IdleWithCount());
        }
    }
    IEnumerator Knockbacked2IdleWithCount()
    {
        yield return new WaitForSeconds(pSO.Duration2IdleWhenKnockbacked);
        State = PlayerState.IDLE;
        isVelocityInvertedOnKnockbacked = false;
    }
    #endregion

    #region PlayerStateに基づくベイの行動処理の詳細
    // ベイの現在のステータスに基づいて、Rigidbodyのパラメーターを更新する。
    void ChangeRigidbodyParameters()
    {
        rb.mass = weight; // mass：重量
        rb.drag = pSO.DragCoef * knockbackResistance; // drag：抵抗
    }

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
        if (hp < status.Hp * pSO.AxisSlopeStartHpCoef)
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
            float axisSpeed = pSO.AxisRotateSpeed / status.Hp * hp;
            axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * axis;
        }
        // そうでないなら、自転する。
        else
        {
            axis = transform.up;
        }

        // ベイを回転軸周りに回転させる。
        float rotSpeed = status.RotationSpeed / status.Hp * hp;
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
            if (!isAddedImpulseOnPush)
            {
                isAddedImpulseOnPush = true;
                rb.AddForce((opponent.transform.position - transform.position).normalized * status.PushPower, ForceMode.Impulse);
            }
        }
    }

    // COUNTER状態では、1回だけノックバック耐性を大きく乗算する。
    void Counter()
    {
        if (State == PlayerState.COUNTER)
        {
            if (!isKnockbackResistanceMultipliedOnCounter)
            {
                isKnockbackResistanceMultipliedOnCounter = true;
                knockbackResistance *= pSO.KnockbackResistanceCoefOnCounter;
            }
        }
    }

    // KNOCKBACKED状態では、1回だけRigidbodyの速度ベクトルを反転する。
    void Knockbacked()
    {
        if (!isVelocityInvertedOnKnockbacked)
        {
            isVelocityInvertedOnKnockbacked = true;
            rb.velocity *= -1;
        }
    }
    #endregion

    #region ベイのHP処理の詳細
    // 敵にぶつかっている時、1度だけダメージ処理を行う。
    void DamageManagement()
    {
        if (isHitOpponent && !isDamaged && isDamagable)
        {
            isDamaged = true;
            isDamagable = false;

            // 無敵時間のカウントを行い、isDamagableをtrueにする。
            StartCoroutine(WaitAndBeDamagable());

            switch (State)
            {
                // IDLE状態なら、通常のダメージを食らう。
                case PlayerState.IDLE:
                    hp -= opponentRb.velocity.magnitude * pSO.DamageCoef;
                    Debug.Log($"<color=#64ff64>{gameObject.name}がIDLE：通常のダメージを食らう</color>");
                    break;

                // PUSH状態なら、ダメージが減る。
                case PlayerState.PUSH:
                    hp -= opponentRb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnPush;
                    Debug.Log($"<color=#64ff64>{gameObject.name}がPUSH：ダメージが減る</color>");
                    break;

                // COUNTER状態なら、ダメージを食らわない。
                case PlayerState.COUNTER:
                    Debug.Log($"<color=#64ff64>{gameObject.name}がCOUNTER：ダメージを食らわない</color>");
                    break;

                // KNOCKBACKED状態なら、ダメージが増える。
                case PlayerState.KNOCKBACKED:
                    hp -= opponentRb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnKnockbacked;
                    Debug.Log($"<color=#64ff64>{gameObject.name}がKNOCKBACKED：ダメージが増える</color>");
                    break;

                default:
                    Debug.LogError($"<color=red>ダメージ処理をする際に、{gameObject.name}がどの状態にも属していません。</color>");
                    break;
            }
        }
    }
    IEnumerator WaitAndBeDamagable()
    {
        yield return new WaitForSeconds(pSO.DamagableInterval);
        isDamagable = true;
    }

    // HPが0を切ったら非アクティブにし、そうでないならHPを表示する。
    void ShowHP()
    {
        if (hp < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            text.text = hp.ToString() + "\n" + State.ToString(); // ベイの状態も表示する。
        }
    }
    #endregion
}
