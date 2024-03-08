using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region �ϐ��錾
    // �x�C�̎�ނ����ʂ���B�C���X�y�N�^��Őݒ肵�Ă������ƁI
    enum TYPE { NULL, Ballerina, BreakDancer }
    [SerializeField] TYPE type = TYPE.NULL;

    // �v���C���[�̏��(public)
    public enum PlayerState { IDLE, PUSH, COUNTER, KNOCKBACKED };
    public PlayerState State { get; set; }

    // �������Z�p
    Rigidbody rb;
    GameObject stageCenter;

    // SO����f�[�^���擾����p
    PlayerSO pSO;
    StatusTable status;
    float Hp { get; set; }
    float weight;
    float knockbackResistance; // �����I��Rigidbody.drag�i����R�j�𑀍삵�Ă���

    // �G�ւ̎Q�Ƃ��擾����p
    GameObject opponent; // �Q�[���I�u�W�F�N�g
    PlayerMove opponentPlayerMove; // PlayerMove�N���X
    Rigidbody opponentRb; // Rigidbody

    // �x�C�̍s�������p
    bool isDamageManager = false; // �_���[�W�������s���C���X�^���X�ł��邩�ǂ���
    bool isDamagable = false; // �y��l�����s��Ȃ��z�_���[�W��H�炦�邩�i�����G���ԂłȂ����j
    bool IsPushBehaviourDone { get; set; } = false; // �v�b�V���̏������A�������Ă��邩
    bool IsCounterBehaviourDone { get; set; } = false; // �J�E���^�[�̏������A�������Ă��邩
    bool isKnockbackedBehaviourDone = false; // �m�b�N�o�b�N���ꂽ���̏������A�������Ă��邩
    bool isOnStateChangeCooltime = false; // ���Ԍo�߂ɂ���ԕω����ł��邩�ǂ���
    Vector3 axis; // �x�C�̉�]��
    float axisTimer = 0; // �x�C�̉�]�����X���鎞��

    // ���̃x�C�ɑΉ�����e�L�X�g���擾����p
    TextMeshProUGUI text;
    #endregion



    #region �yStart�z
    void Start()
    {
        // �v���C���[�̏�����Ԃ�ݒ�
        State = PlayerState.IDLE;

        // �������Z�̏����iUnity���̏d�͂��I�t�ɂ��A�l�͂Ōv�Z����d�͂̒��S���擾�j
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        stageCenter = GameObject.FindGameObjectWithTag("Center");

        // PlayerSO���擾
        pSO = PlayerSO.Entity;

        // �Ή�����StatusSO���擾���A�ϓ����鐔�l�����߂āA���̃N���X���̕ϐ��Ɋi�[
        switch (type)
        {
            case TYPE.Ballerina:
                status = BallerinaStatusSO.Entity.Status;
                break;
            case TYPE.BreakDancer:
                status = BreakDancerStatusSO.Entity.Status;
                break;
            case TYPE.NULL:
                Debug.LogError("<color=red>type���ݒ肳��Ă��܂���</color>");
                break;
        }
        Hp = status.Hp;
        weight = status.Weight;
        knockbackResistance = status.KnockbackResistance;

        // �G�ւ̎Q�Ƃ��擾
        int idx = Array.IndexOf(GameManager.Instance.Beys, gameObject);
        if (idx == 0)
        {
            opponent = GameManager.Instance.Beys[1];
            isDamageManager = true; // 0�Ԗڂ̃C���X�^���X�ŁA�_���[�W�������s���B
        }
        else if (idx == 1)
        {
            opponent = GameManager.Instance.Beys[0];
        }
        else
        {
            Debug.LogError("<color=red>�G�I�u�W�F�N�g�̎擾�Ɏ��s���܂���</color>");
        }
        opponentPlayerMove = opponent.GetComponent<PlayerMove>();
        opponentRb = opponent.GetComponent<Rigidbody>();

        // ���̃x�C�ɑΉ�����e�L�X�g���擾���āAHP�\��
        text = GameManager.Instance.Texts[Array.IndexOf(GameManager.Instance.Beys, gameObject)];

        // �Q�[���J�n���班���̊Ԃ́A���G���ԂɂȂ��Ă���B
        StartCoroutine(CountDamagableDuration());
    }
    #endregion

    #region �yFixedUpdate�z�܂��A�x�C�̕�����Ԃ��X�V����B���ɁA���蔫��̃t�B�[���h�ɂ�����A�d�͂ɂ�鋓�����Č�����B
    void FixedUpdate()
    {
        // �x�C�̕�����Ԃ̍X�V
        ChangeRigidbodyParameters();

        // �d��
        rb.AddForce(Vector3.down * 9.81f * pSO.GravityScale, ForceMode.Force);

        // ��ɒ��S�ֈړ�
        rb.AddForce((stageCenter.transform.position - transform.position).normalized * pSO.SpeedTowardCenter, ForceMode.Force);

    }

    // �x�C�̌��݂̃X�e�[�^�X�Ɋ�Â��āARigidbody�̃p�����[�^�[���X�V����B
    void ChangeRigidbodyParameters()
    {
        rb.mass = weight; // mass�F�d��
        rb.drag = pSO.DragCoef * knockbackResistance; // drag�F��R
    }
    #endregion

    #region �yOnCollision�z�G�Ƃ̐ڐG�����m���A���G���ԂłȂ��Ȃ�΁A�_���[�W�������s���������ɂ���Ď����܂��͓G��KNOCKBACKED��Ԃɂ���B
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isDamageManager)
        {
            if (isDamagable)
            {
                isDamagable = false;
                HitBehaviour(); // PlayerState�̑J�ڂȂ�
                StartCoroutine(CountDamagableDuration()); // ���G���Ԃ̃J�E���g
            }
        }
    }

    IEnumerator CountDamagableDuration()
    {
        yield return new WaitForSeconds(pSO.DamagableDuration);
        isDamagable = true;
    }
    #endregion

    #region �yUpdate�z
    void Update()
    {
        #region PlayerState�̑J�ځF�J�ڃN�[���^�C�����łȂ��Ȃ�A�v���C���[�̓��͂��擾���āAPUSH��Ԃ܂���COUNTER��ԂɂȂ�B
        Idle2Push();
        Counter2Push();
        Idle2Counter();
        Push2Counter();
        #endregion

        #region PlayerState�Ɋ�Â��x�C�̍s������
        Rotate(); // �ŏ��ɉ�]�������s���B
        Idle();
        Push();
        Counter();
        Knockbacked();
        #endregion

        #region �x�C�̃X�N���v�g�́A�e��ϐ��̕\��
        ShowHP(); // �f�X������s���B
        #endregion

        #region PlayerState�̑J�ځF�J�ڃN�[���^�C�����łȂ��Ȃ�A�J�ڃN�[���^�C�����J�n���A���Ԍo�߂�IDLE��ԂɂȂ�B
        Push2Idle();
        Counter2Idle();
        Knockbacked2Idle();
        #endregion
    }
    #endregion



    #region�@PlayerState�̑J�ڂ̏ڍׁi�Ή������Ԃ̎��A�y�����z�𖞂������瑦���ɑJ�ڂ���B�x�C�̍s�������Ɋւ��ϐ��̃��Z�b�g���s���B�j
    // �ȉ��̂����ꂩ�̑J�ڂ��s���B
    // �E�y�����zPUSH    => IDLE       �y�G�zIDLE    => KNOCKBACKED
    // �E�y�����zCOUNTER => IDLE       �y�G�zPUSH    => KNOCKBACKED
    // �E�y�����zIDLE    => KNOCKBACKED�y�G�zPUSH    => IDLE
    // �E�y�����zPUSH    => KNOCKBACKED�y�G�zCOUNTER => IDLE
    // �y�����z�i�G�ƐڐG�����ۂɌĂ΂��B�j������PUSH�œG��IDLE�A�܂��́A������COUNTER�œG��PUSH
    // �y���̑��z�_���[�W�������s���B
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
                        Debug.Log($"<color=#64ff64>{name} �� {opponent.name} �Ƀv�b�V�������I</color>");
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
                        Debug.Log($"<color=#64ff64>{opponent.name} �� {name} �Ƀv�b�V�������I</color>");
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
                        Debug.Log($"<color=#64ff64>{name} �� {opponent.name} �ɃJ�E���^�[�����I</color>");
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
                        Debug.Log($"<color=#64ff64>{opponent.name} �� {name} �ɃJ�E���^�[�����I</color>");
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

    // �^����ꂽPlayerMove�N���X�̃C���X�^���X�ɁA���g�̑��������ɂ��āA���̃C���X�^���X���^����ꂽ��Ԃł���ꍇ�́A�_���[�W��^����B
    void Damage(PlayerMove playerMoveInstance, PlayerState State)
    {
        if (isDamageManager)
        {
            switch (State)
            {
                // IDLE��ԂȂ�A�ʏ�̃_���[�W��H�炤�B
                case PlayerState.IDLE:
                    playerMoveInstance.Hp -= rb.velocity.magnitude * pSO.DamageCoef;
                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{playerMoveInstance.gameObject.name} �ɒʏ�̃_���[�W��^����</color>");
                    }
                    break;

                // PUSH��ԂȂ�A�_���[�W������B
                case PlayerState.PUSH:
                    playerMoveInstance.Hp -= rb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnPush;
                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{playerMoveInstance.gameObject.name} �ɗ^����_���[�W�����炷</color>");
                    }
                    break;

                // COUNTER��ԂȂ�A�_���[�W��H���Ȃ��B
                case PlayerState.COUNTER:
                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{playerMoveInstance.gameObject.name} �Ƀ_���[�W��^���Ȃ�</color>");
                    }
                    break;

                // KNOCKBACKED��ԂȂ�A�_���[�W��������B
                case PlayerState.KNOCKBACKED:
                    playerMoveInstance.Hp -= rb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnKnockbacked;
                    if (pSO.IsShowNormalLog)
                    {
                        Debug.Log($"<color=#64ff64>{playerMoveInstance.gameObject.name} �ɗ^����_���[�W�𑝂₷</color>");
                    }
                    break;
            }
        }
    }

    // IDLE => PUSH
    // �y�����z�v�b�V���L�[�������ꂽ�B
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
    // �y�����z�v�b�V���L�[�������ꂽ�B
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
    // �y�����z�J�E���^�[�L�[�������ꂽ�B
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
    // �J�E���^�[�L�[�������ꂽ�B
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
    // �y�����z��莞�Ԃ��o�߂��A����ł��Ȃ�PUSH��ԁB
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
    // �y�����z��莞�Ԃ��o�߂��A����ł��Ȃ�COUNTER��ԁB
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
    // �y�����z��莞�Ԃ��o�߂��A����ł��Ȃ�KNOCKBACKED��ԁB
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

    #region PlayerState�Ɋ�Â��x�C�̍s�������̏ڍ�
    // 1.�n�ʂɐ����Ȏp�����������B
    // 2.���]����B�������AHP���Ⴍ�Ȃ�����΍��^���ɐ؂�ւ��B
    void Rotate()
    {
        // ��]�������s���O�ɁA�x�C�̃��[�J��y���i�΁j�̕�����n�ʂ̖@���x�N�g���ɍ��킹��B
        Ray shotRay = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(shotRay, out RaycastHit ground))
        {
            Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, pSO.PlayerMainAxisChangeSpeed * Time.deltaTime);
        }

        // HP�����ȉ��ɂȂ�����A�΍��^��������B
        if (Hp < status.Hp * pSO.AxisSlopeStartHpCoef)
        {
            // �w��b�����Ƃɉ�]���̌X����ω�������B
            axisTimer += Time.deltaTime;
            if (axisTimer > pSO.AxisSlopeChangeInterval)
            {
                axisTimer -= pSO.AxisSlopeChangeInterval;
                float theta = UnityEngine.Random.Range(pSO.AxisSlopRange.x, pSO.AxisSlopRange.y);
                axis = Quaternion.AngleAxis(theta, transform.forward) * transform.up;
            }

            // ��]���𒆐S���itransform.up�j����ɉ�]������B
            float axisSpeed = pSO.AxisRotateSpeed / status.Hp * Hp;
            axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * axis;
        }
        // �����łȂ��Ȃ�A���]����B
        else
        {
            axis = transform.up;
        }

        // �x�C����]������ɉ�]������B
        float rotSpeed = status.RotationSpeed / status.Hp * Hp;
        float minRotSpeed = pSO.RotationSpeedCoefRange.x * status.RotationSpeed;
        float maxRotSpeed = pSO.RotationSpeedCoefRange.y * status.RotationSpeed;
        rotSpeed = Mathf.Clamp(rotSpeed, minRotSpeed, maxRotSpeed);�@// �p���x�𐧌�����B
        transform.localRotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, axis) * transform.localRotation;
    }

    // IDLE��Ԃł́A���󉽂����Ȃ��B
    void Idle()
    {
        if (State == PlayerState.IDLE)
        {
            return;
        }
    }

    // PUSH��Ԃł́A1�񂾂��G�Ɍ������ďu�ԓI�ɗ͂�������B
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

    // COUNTER��Ԃł́A1�񂾂��m�b�N�o�b�N�ϐ���傫����Z����B
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

    // KNOCKBACKED��Ԃł́A1�񂾂��ȉ��̏������s���B
    // �E�����ƓG�̑��x�x�N�g���̑傫�������v���A���̒萔�{�̏u�ԓI�ȗ́i�Œ�ۏ؂���j���A�G�Ɣ��Ε����ɉ�����B
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

    #region �x�C�̃X�N���v�g�́A�e��ϐ��̕\���̏ڍ�
    // HP��0��؂������A�N�e�B�u�ɂ��A�����łȂ��Ȃ�e��ϐ���\������B
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
