using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ���G��KNOCKBACKED��ԂɂȂ��Ă���1�b�ԁA������PUSH��Ԃ܂���COUNTER��ԂɂȂ��Ă��A������IDLE��Ԃɖ߂��Ă��܂��B����N�[���^�C��������Ή������邩�H
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
    StatusSO status;
    float hp;
    float weight;
    float knockbackResistance; // �����I��Rigidbody.drag�i����R�j�𑀍삵�Ă���

    // �G�ւ̎Q�Ƃ��擾����p
    GameObject opponent; // �Q�[���I�u�W�F�N�g
    PlayerMove opponentPlayerMove; // PlayerMove�N���X
    Rigidbody opponentRb; // Rigidbody

    // �x�C�̍s�������p
    bool isHitOpponent = false; // �G�ɂԂ����Ă��邩
    bool isDamaged = false; // �G�Ƃ̐ڐG���ɂ����āA���Ƀ_���[�W��H����Ă��邩
    bool isDamagable = true; // ���G���Ԓ��łȂ��i���_���[�W��H�炦��j���ǂ���
    bool isKnockbackResistanceMultipliedOnCounter = false; // �J�E���^�[���̃m�b�N�o�b�N�ϐ��̏�Z�������A�������Ă��邩
    bool isVelocityInvertedOnKnockbacked = false; // �m�b�N�o�b�N���ꂽ���̑��x�̔��]�������A�������Ă��邩
    bool isAddedImpulseOnPush = false; // �v�b�V�����ɁA���ɗ͂���������
    Vector3 axis; // �x�C�̉�]��
    float axisTimer = 0; // �x�C�̉�]�����X���鎞��

    // ���̃x�C�ɑΉ�����e�L�X�g���擾����p
    TextMeshProUGUI text;
    #endregion

    void Start()
    {
        // �v���C���[�̏�����Ԃ�ݒ�E�G�I�u�W�F�N�g�Ƃ���PlayerMove�N���X���擾���ĕێ����Ă���
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
        hp = status.Hp;
        weight = status.Weight;
        knockbackResistance = status.KnockbackResistance;

        // �G�ւ̎Q�Ƃ��擾
        int idx = Array.IndexOf(GameManager.Instance.Beys, gameObject);
        if (idx == 0) opponent = GameManager.Instance.Beys[1];
        else if (idx == 1) opponent = GameManager.Instance.Beys[0];
        else Debug.LogError("<color=red>�G�I�u�W�F�N�g�̎擾�Ɏ��s���܂���</color>");
        opponentPlayerMove = opponent.GetComponent<PlayerMove>();
        opponentRb = opponent.GetComponent<Rigidbody>();

        // ���̃x�C�ɑΉ�����e�L�X�g���擾���āAHP�\��
        text = GameManager.Instance.Texts[Array.IndexOf(GameManager.Instance.Beys, gameObject)];
        text.text = hp.ToString();
    }

    void Update()
    {
        #region PlayerState�̑J�ځF�v���C���[�̓��͂��擾���āAPUSH��Ԃ܂���COUNTER��ԂɂȂ�B
        Idle2Push();
        Counter2Push();
        Idle2Counter();
        Push2Counter();
        #endregion

        #region PlayerState�̑J�ځFKNOCKBACKED��ԂɂȂ�B
        Idle2Knockbacked();
        Push2Knockbacked();
        #endregion

        #region PlayerState�Ɋ�Â��x�C�̍s������
        Rotate(); // �ŏ��ɉ�]�������s���B
        Idle();
        Push();
        Counter();
        Knockbacked();
        ChangeRigidbodyParameters(); // �x�C�̕�����Ԃ̍X�V
        #endregion

        #region �x�C��HP����
        DamageManagement();
        ShowHP(); // �x�C�̏�Ԃ��\������B�f�X������s���B
        #endregion

        #region PlayerState�̑J�ځFIDLE��ԂɂȂ�B
        Push2Idle();
        Counter2Idle();
        Knockbacked2Idle();
        #endregion
    }

    #region �y�������Z�z���蔫��̃t�B�[���h�ɂ�����A�d�͂ɂ�鋓�����Č�����B
    void FixedUpdate()
    {
        // �d��
        rb.AddForce(Vector3.down * 9.81f * pSO.GravityScale, ForceMode.Force);

        // ��ɒ��S�ֈړ�
        rb.AddForce((stageCenter.transform.position - transform.position).normalized * pSO.SpeedTowardCenter, ForceMode.Force);

    }
    #endregion

    #region �y�R���W��������z�G�Ƃ̐ڐG��Ԃ̕ω������m���A�_���[�W�������Ɏg���t���O��ω�������B
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isHitOpponent = true; // isHitOpponent�̐؂�ւ�
            isDamaged = false; // isDamaged�̃��Z�b�g
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isHitOpponent = false; // isHitOpponent�̐؂�ւ�
            isDamaged = false; // isDamaged�̃��Z�b�g
        }
    }
    #endregion



    #region�@PlayerState�̑J�ڂ̏ڍׁi�Ή������Ԃ̎��A�y�����z�𖞂������瑦���ɑJ�ڂ���B�y���̑��z�̏������ǉ��ōs���B�j
    // IDLE => PUSH
    // �y�����z�v�b�V���L�[�������ꂽ�B
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
    // �y�����z�v�b�V���L�[�������ꂽ�B
    // �y���̑��z
    // �ECOUNTER��Ԃւ̑J�ڎ��ɍs�����m�b�N�o�b�N�ϐ��̏�Z���A�ł��������Z���s���B
    // �E �J�E���^�[���̃m�b�N�o�b�N�ϐ��̏�Z�����ɁA�ւ��ϐ������Z�b�g����B
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
    // �y�����z�J�E���^�[�L�[�������ꂽ�B
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
    // �J�E���^�[�L�[�������ꂽ�B
    // �y���̑��z�v�b�V�����ɗ͂������鏈���ɁA�ւ��ϐ������Z�b�g����B
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
    // �y�����z�G�ƐڐG���Ă���A���A�G��PUSH��ԁB
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
    // �y�����z�G�ƐڐG���Ă���A���A�G��COUNTER��ԁB
    // �y���̑��z�v�b�V�����ɗ͂������鏈���ɁA�ւ��ϐ������Z�b�g����B
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
    // �y�����z�ȉ��̂ǂꂩ�𖞂������B
    // �@�@�@�@�E�G��KNOCKBACKED��ԁB
    // �@�@�@�@�E�G��KNOCKBACKED��ԂłȂ��A���A��莞�Ԃ��o�߂����B
    // �y���̑��z�v�b�V�����ɗ͂������鏈���ɁA�ւ��ϐ������Z�b�g����B
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
    // �y�����z�ȉ��̂ǂꂩ�𖞂������B
    // �@�@�@�@�E�G��KNOCKBACKED��ԁB
    // �@�@�@�@�E�G��KNOCKBACKED��ԂłȂ��A���A��莞�Ԃ��o�߂����B
    // �y���̑��z
    // �ECOUNTER��Ԃւ̑J�ڎ��ɍs�����m�b�N�o�b�N�ϐ��̏�Z���A�ł��������Z���s���B
    // �E �J�E���^�[���̃m�b�N�o�b�N�ϐ��̏�Z�����ɁA�ւ��ϐ������Z�b�g����B
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
    // �y�����z��莞�Ԃ��o�߂����B
    // �y���̑��z�m�b�N�o�b�N���ꂽ���̑��x�̔��]�����ɁA�ւ��ϐ������Z�b�g����B
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

    #region PlayerState�Ɋ�Â��x�C�̍s�������̏ڍ�
    // �x�C�̌��݂̃X�e�[�^�X�Ɋ�Â��āARigidbody�̃p�����[�^�[���X�V����B
    void ChangeRigidbodyParameters()
    {
        rb.mass = weight; // mass�F�d��
        rb.drag = pSO.DragCoef * knockbackResistance; // drag�F��R
    }

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
        if (hp < status.Hp * pSO.AxisSlopeStartHpCoef)
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
            float axisSpeed = pSO.AxisRotateSpeed / status.Hp * hp;
            axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * axis;
        }
        // �����łȂ��Ȃ�A���]����B
        else
        {
            axis = transform.up;
        }

        // �x�C����]������ɉ�]������B
        float rotSpeed = status.RotationSpeed / status.Hp * hp;
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
            if (!isAddedImpulseOnPush)
            {
                isAddedImpulseOnPush = true;
                rb.AddForce((opponent.transform.position - transform.position).normalized * status.PushPower, ForceMode.Impulse);
            }
        }
    }

    // COUNTER��Ԃł́A1�񂾂��m�b�N�o�b�N�ϐ���傫����Z����B
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

    // KNOCKBACKED��Ԃł́A1�񂾂�Rigidbody�̑��x�x�N�g���𔽓]����B
    void Knockbacked()
    {
        if (!isVelocityInvertedOnKnockbacked)
        {
            isVelocityInvertedOnKnockbacked = true;
            rb.velocity *= -1;
        }
    }
    #endregion

    #region �x�C��HP�����̏ڍ�
    // �G�ɂԂ����Ă��鎞�A1�x�����_���[�W�������s���B
    void DamageManagement()
    {
        if (isHitOpponent && !isDamaged && isDamagable)
        {
            isDamaged = true;
            isDamagable = false;

            // ���G���Ԃ̃J�E���g���s���AisDamagable��true�ɂ���B
            StartCoroutine(WaitAndBeDamagable());

            switch (State)
            {
                // IDLE��ԂȂ�A�ʏ�̃_���[�W��H�炤�B
                case PlayerState.IDLE:
                    hp -= opponentRb.velocity.magnitude * pSO.DamageCoef;
                    Debug.Log($"<color=#64ff64>{gameObject.name}��IDLE�F�ʏ�̃_���[�W��H�炤</color>");
                    break;

                // PUSH��ԂȂ�A�_���[�W������B
                case PlayerState.PUSH:
                    hp -= opponentRb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnPush;
                    Debug.Log($"<color=#64ff64>{gameObject.name}��PUSH�F�_���[�W������</color>");
                    break;

                // COUNTER��ԂȂ�A�_���[�W��H���Ȃ��B
                case PlayerState.COUNTER:
                    Debug.Log($"<color=#64ff64>{gameObject.name}��COUNTER�F�_���[�W��H���Ȃ�</color>");
                    break;

                // KNOCKBACKED��ԂȂ�A�_���[�W��������B
                case PlayerState.KNOCKBACKED:
                    hp -= opponentRb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnKnockbacked;
                    Debug.Log($"<color=#64ff64>{gameObject.name}��KNOCKBACKED�F�_���[�W��������</color>");
                    break;

                default:
                    Debug.LogError($"<color=red>�_���[�W����������ۂɁA{gameObject.name}���ǂ̏�Ԃɂ������Ă��܂���B</color>");
                    break;
            }
        }
    }
    IEnumerator WaitAndBeDamagable()
    {
        yield return new WaitForSeconds(pSO.DamagableInterval);
        isDamagable = true;
    }

    // HP��0��؂������A�N�e�B�u�ɂ��A�����łȂ��Ȃ�HP��\������B
    void ShowHP()
    {
        if (hp < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            text.text = hp.ToString() + "\n" + State.ToString(); // �x�C�̏�Ԃ��\������B
        }
    }
    #endregion
}
