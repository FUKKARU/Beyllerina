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
    bool isKnockbackResistanceMultipliedOnCounter = false; // カウンター時のノックバック耐性の乗算処理を、完了しているか
    bool isVelocityInvertedOnKnockbacked = false; // ノックバックされたときの速度の反転処理を、完了しているか
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
        #region PlayerStateの遷移（遷移するだけ）
        Idle2Push();
        Idle2Counter();
        Idle2Knockbacked();
        Push2Idle();
        Push2Counter();
        Push2Knockbacked();
        Counter2Idle();
        Counter2Push();
        Knockbacked2Idle();
        #endregion

        #region PlayerStateに基づくベイの行動処理
        Rotate(); // 最初に回転処理を行う。
        Idle();
        Push();
        Counter();
        Knockbacked();
        ChangeRigidbodyParameters(); // ベイの物理状態の更新
        #endregion

        #region ダメージ処理
        DamageManagement();
        ShowHP(); // デス判定も行う。
        #endregion
    }

    #region 【物理演算】すり鉢状のフィールドにおける、重力を再現する。
    void FixedUpdate()
    {
        // 重力
        rb.AddForce(Vector3.down * 9.81f * pSO.GravityScale, ForceMode.Force);

        //常に中心へ移動
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



    #region　PlayerStateの遷移の詳細
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

    void Push2Idle()
    {
        if (State == PlayerState.PUSH)
        {
            if (opponentPlayerMove.State == PlayerState.KNOCKBACKED)
            {
                State = PlayerState.IDLE;
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
    }

    void Push2Counter()
    {
        if (State == PlayerState.PUSH)
        {
            if (Input.GetKeyDown(status.CounterKey))
            {
                State = PlayerState.COUNTER;
            }
        }
    }

    void Push2Knockbacked()
    {
        if (State == PlayerState.PUSH)
        {
            if (isHitOpponent && opponentPlayerMove.State == PlayerState.PUSH)
            {
                State = PlayerState.KNOCKBACKED;
            }
        }
    }

    void Counter2Idle()
    {
        if (State == PlayerState.COUNTER)
        {
            if (opponentPlayerMove.State == PlayerState.KNOCKBACKED)
            {
                State = PlayerState.IDLE;
                knockbackResistance /= pSO.SelfKnockbackResistanceCoefOnCounter;
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
        knockbackResistance /= pSO.SelfKnockbackResistanceCoefOnCounter;
        isKnockbackResistanceMultipliedOnCounter = false;
    }

    void Counter2Push()
    {
        if (State == PlayerState.COUNTER)
        {
            if (Input.GetKeyDown(status.PushKey))
            {
                State = PlayerState.PUSH;
                knockbackResistance /= pSO.SelfKnockbackResistanceCoefOnCounter;
                isKnockbackResistanceMultipliedOnCounter = false;
            }
        }
    }

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

    void ChangeRigidbodyParameters()
    {
        // Rigidbodyの物理量の更新
        rb.mass = weight; // mass：重量
        rb.drag = pSO.DragCoef * knockbackResistance; // drag：抵抗
    }

    // 回転処理を行う前に、ベイのローカルy軸（緑）の方向を地面の法線ベクトルに合わせる。
    // ベイを回転させ、その回転速度を一定範囲内に収める。
    // HPが一定以下になったら、指定秒数ごとに回転軸の傾きを変化させて歳差運動をさせる。
    void Rotate()
    {
        //地面の法線を調べるレイ
        Ray shotRay = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(shotRay, out RaycastHit ground))
        {
            //ベイのローカルy軸（緑）の方向　から　地面の法線ベクトルへ
            Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, pSO.PlayerMainAxisChangeSpeed * Time.deltaTime);
        }

        if (hp < status.Hp * pSO.AxisSlopeStartHpCoef)
        {
            axisTimer += Time.deltaTime;
            if (axisTimer > pSO.AxisSlopeChangeInterval)
            {
                axisTimer -= pSO.AxisSlopeChangeInterval;
                float theta = UnityEngine.Random.Range(pSO.AxisSlopRange.x, pSO.AxisSlopRange.y);
                axis = Quaternion.AngleAxis(theta, transform.forward) * transform.up;
            }
        }

        // 回転軸をtransform.up周りに回転させる
        float axisSpeed = pSO.AxisRotateSpeed / status.Hp * hp;
        axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * transform.up;

        // 物体に角速度を与えて回転させる
        float rotSpeed = status.RotationSpeed / status.Hp * hp;
        float minRotSpeed = pSO.RotationSpeedCoefRange.x * status.RotationSpeed;
        float maxRotSpeed = pSO.RotationSpeedCoefRange.y * status.RotationSpeed;
        rotSpeed = Mathf.Clamp(rotSpeed, minRotSpeed, maxRotSpeed);　// 角速度を制限する
        transform.localRotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, axis) * transform.localRotation;

    }

    void Idle()
    {
        return;
    }

    // プッシュを行う。
    void Push()
    {
        rb.AddForce((opponent.transform.position - transform.position).normalized * status.PushPower, ForceMode.Impulse);
    }

    // カウンター時にベイのノックバック耐性を大きく乗算し、カウンターが終わったらそれを打ち消す除算を行う。
    void Counter()
    {
        if (!isKnockbackResistanceMultipliedOnCounter)
        {
            isKnockbackResistanceMultipliedOnCounter = true;
            knockbackResistance *= pSO.SelfKnockbackResistanceCoefOnCounter;
        }
    }

    void Knockbacked()
    {
        if (!isVelocityInvertedOnKnockbacked)
        {
            isVelocityInvertedOnKnockbacked = true;
            rb.velocity *= -1;
        }
    }
    #endregion

    #region ダメージ処理の詳細
    void DamageManagement()
    {
        if (isHitOpponent && !isDamaged)
        {
            isDamaged = true;

            if (State == PlayerState.PUSH) // プッシュ中は受けるダメージが減る
            {
                hp -= opponentRb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnPush;
                Debug.Log($"<color=#64ff64>{gameObject.name}がプッシュ中：ダメージが減る</color>");
                return;
            }
            else if (State == PlayerState.COUNTER) // カウンター中はダメージを食らわない
            {
                Debug.Log($"<color=#64ff64>{gameObject.name}がカウンター中：ダメージを食らわない</color>");
                return;
            }
            else // ノックバックされた（Idle状態で敵に接触した場合も、この処理を行う）：通常ダメージ処理
            {
                hp -= opponentRb.velocity.magnitude * pSO.DamageCoef;
                Debug.Log($"<color=#64ff64>{gameObject.name}がノックバックされた：通常ダメージ</color>");
                return;
            }
        }
    }

    // HP表示。もしもHPが0を切っていたら、このオブジェクトを非アクティブにする。
    void ShowHP()
    {
        if (hp < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            text.text = hp.ToString();
        }
    }
    #endregion
}
