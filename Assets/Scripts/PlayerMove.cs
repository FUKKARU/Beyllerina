using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    enum TYPE { NULL, Ballerina, BreakDancer }
    [SerializeField] TYPE type = TYPE.NULL;

    // プレイヤーの状態
    enum PlayerState { IDLE, PUSH, GUARD }
    PlayerState state;

    // 物理演算用
    Rigidbody rb;
    GameObject center;
    bool grounded;

    // SOからデータを取得する用
    PlayerSO pSO;
    StatusSO status;
    float hp;
    float weight;
    float knockbackResistance; // 内部的にRigidbody.drag（＝抵抗）を操作している

    // ベイの行動処理用
    float pushTimer;
    Vector3 axis; // 回転軸
    float axisTimer = 0; // 回転軸を傾ける時間

    // このベイに対応するテキストを取得する用
    TextMeshProUGUI text;

    void Start()
    {
        // プレイヤーの初期状態を設定
        state = PlayerState.IDLE;

        // 物理演算の準備（Unity側の重力をオフにし、人力で計算する重力の中心を取得）
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        center = GameObject.FindGameObjectWithTag("Center");

        // PlayerSOを取得
        pSO = PlayerSO.Entity;

        // 対応するStatusSOを取得し、変動する数値を改めて、このクラス内の変数に格納
        if (type == TYPE.Ballerina) status = BallerinaStatusSO.Entity.Status;
        else if (type == TYPE.BreakDancer) status = BreakDancerStatusSO.Entity.Status;
        else Debug.LogError("<color=red>typeが設定されていません</color>");
        hp = status.Hp;
        weight = status.Weight;
        knockbackResistance = status.KnockbackResistance;

        // このベイに対応するテキストを取得して、HP表示
        text = GameManager.Instance.Texts[Array.IndexOf(GameManager.Instance.Beys, gameObject)];
        text.text = hp.ToString();
    }

    void Update()
    {
        // Rigidbodyの物理量（massとdrag）の更新、及び地面との接触判定
        rb.mass = weight; // mass：重量
        rb.drag = pSO.DragCoef * knockbackResistance; // drag：抵抗
        grounded = Physics.Raycast(transform.position, -transform.up, pSO.PlayerHeight * 0.5f + 0.2f, pSO.WhatIsGround);

        // PlayerStateに基づいて、ベイの挙動及び行動を処理する。PlayerStateについて、GUARD=>PUSHを除く、全ての遷移を行う。
        PlayerAct();

        // HP表示。もしもHPが0を切っていたら、このオブジェクトを非アクティブにする。
        if (hp < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            text.text = hp.ToString();
        }
    }

    // すり鉢状のフィールドにおける、重力を再現する。
    void FixedUpdate()
    {
        // 重力
        rb.AddForce(Vector3.down * 9.81f * pSO.GravityScale, ForceMode.Force);

        //常に中心へ移動
        rb.AddForce((center.transform.position - transform.position).normalized * pSO.SpeedTowardCenter, ForceMode.Force);

    }

    // プレイヤーとの接触を検知し、相手の速度に基づいてダメージ処理を行う。
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // プッシュ中は受けるダメージが減る
            if (state == PlayerState.PUSH)
            {
                hp -= collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnPush;
            }
            else if (state == PlayerState.IDLE)
            {
                hp -= collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude * pSO.DamageCoef;
            }
            else
            {
                return;
            }
        }
    }

    void PlayerAct()
    {
        // 回転処理を行う前に、ベイのローカルy軸（緑）の方向を地面の法線ベクトルに合わせる。
        VerticalToGround();

        // ベイを回転させ、その回転速度を一定範囲内に収める。
        // HPが一定以下になったら、指定秒数ごとに回転軸の傾きを変化させて歳差運動をさせる。
        Rotate();
        
        // IDLE状態の時のみ、プッシュ入力を検知。クールタイムカウントも行い、IDLE<=>PUSHの遷移を行う。
        PushBehaviour();

        // 地面に接触している時のみ入力を検知して、ワールド鉛直上方向にジャンプする。
        if (!status.DebugIsJumpDisable)
        {
            if (grounded && Input.GetKeyDown(status.JumpKey))
                Jump();
        }

        // 地面に接触している時のみ入力を検知して、ガードをし、IDLE,PUSH=>GUARDの遷移を行う。ガードを解除したら、GUARD=>IDLEの遷移を行う。
        // ガード時にベイのノックバック耐性を大きく乗算し、ガードが終わったらそれを打ち消す除算を行う。
        if (!status.DebugIsGuardDisable)
        {
            #region
            if (state == PlayerState.IDLE || state == PlayerState.PUSH)
            {
                if (grounded && Input.GetKeyDown(status.GuardKey))
                {
                    knockbackResistance *= pSO.KnockbackResistanceCoefOnGuard;
                    state = PlayerState.GUARD;
                }

            }

            if (state == PlayerState.GUARD)
            {
                if (Input.GetKeyUp(status.GuardKey))
                {
                    knockbackResistance /= pSO.KnockbackResistanceCoefOnGuard;
                    state = PlayerState.IDLE;
                }
            }
            #endregion
        }
    }
    
    void Jump()
    {
        rb.AddForce(transform.up * status.JumpPower, ForceMode.Impulse);
    }

    void PushBehaviour()
    {
        if (Input.GetKeyDown(status.PushKey) && state == PlayerState.IDLE)
            Push(); // プッシュのターゲットを認識する（要改善）。ターゲットが倒されていないならば、プッシュを行い、ログを出し、IDLE=>PUSHの遷移を行う。

        if (state == PlayerState.PUSH)
        {
            pushTimer += Time.deltaTime;

            if (pushTimer >= status.PushCoolTime)
            {
                pushTimer = 0;
                state = PlayerState.IDLE; // クールタイムが切れたら、PUSH=>IDLEの遷移を行う。
            }
        }
    }
    
    void Rotate()
    {
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

    void Push()
    {
        GameObject target;
        int idx = Array.IndexOf(GameManager.Instance.Beys, gameObject);
        if (idx == 0) target = GameManager.Instance.Beys[1];
        else target = GameManager.Instance.Beys[0];

        if (target.activeSelf)
        {
            state = PlayerState.PUSH;
            pushTimer = 0;
            rb.AddForce((target.transform.position - transform.position).normalized * status.PushPower, ForceMode.Impulse);
            Debug.Log($"<color=#64ff64>{gameObject.name}がプッシュ</color>");
        }
    }

    void VerticalToGround()
    {
        //地面の法線を調べるレイ
        Ray shotRay = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(shotRay, out RaycastHit ground))
        {
            //ベイのローカルy軸（緑）の方向　から　地面の法線ベクトルへ
            Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, pSO.PlayerMainAxisChangeSpeed * Time.deltaTime);
        }
    }
}
