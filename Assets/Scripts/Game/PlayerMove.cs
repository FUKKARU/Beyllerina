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
    bool isKnockbackResistanceMultipliedOnCounter = false; // �J�E���^�[���̃m�b�N�o�b�N�ϐ��̏�Z�������A�������Ă��邩
    bool isVelocityInvertedOnKnockbacked = false; // �m�b�N�o�b�N���ꂽ�Ƃ��̑��x�̔��]�������A�������Ă��邩
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
        #region PlayerState�̑J�ځi�J�ڂ��邾���j
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

        #region PlayerState�Ɋ�Â��x�C�̍s������
        Rotate(); // �ŏ��ɉ�]�������s���B
        Idle();
        Push();
        Counter();
        Knockbacked();
        ChangeRigidbodyParameters(); // �x�C�̕�����Ԃ̍X�V
        #endregion

        #region �_���[�W����
        DamageManagement();
        ShowHP(); // �f�X������s���B
        #endregion
    }

    #region �y�������Z�z���蔫��̃t�B�[���h�ɂ�����A�d�͂��Č�����B
    void FixedUpdate()
    {
        // �d��
        rb.AddForce(Vector3.down * 9.81f * pSO.GravityScale, ForceMode.Force);

        //��ɒ��S�ֈړ�
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



    #region�@PlayerState�̑J�ڂ̏ڍ�
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

    #region PlayerState�Ɋ�Â��x�C�̍s�������̏ڍ�

    void ChangeRigidbodyParameters()
    {
        // Rigidbody�̕����ʂ̍X�V
        rb.mass = weight; // mass�F�d��
        rb.drag = pSO.DragCoef * knockbackResistance; // drag�F��R
    }

    // ��]�������s���O�ɁA�x�C�̃��[�J��y���i�΁j�̕�����n�ʂ̖@���x�N�g���ɍ��킹��B
    // �x�C����]�����A���̉�]���x�����͈͓��Ɏ��߂�B
    // HP�����ȉ��ɂȂ�����A�w��b�����Ƃɉ�]���̌X����ω������č΍��^����������B
    void Rotate()
    {
        //�n�ʂ̖@���𒲂ׂ郌�C
        Ray shotRay = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(shotRay, out RaycastHit ground))
        {
            //�x�C�̃��[�J��y���i�΁j�̕����@����@�n�ʂ̖@���x�N�g����
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

        // ��]����transform.up����ɉ�]������
        float axisSpeed = pSO.AxisRotateSpeed / status.Hp * hp;
        axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * transform.up;

        // ���̂Ɋp���x��^���ĉ�]������
        float rotSpeed = status.RotationSpeed / status.Hp * hp;
        float minRotSpeed = pSO.RotationSpeedCoefRange.x * status.RotationSpeed;
        float maxRotSpeed = pSO.RotationSpeedCoefRange.y * status.RotationSpeed;
        rotSpeed = Mathf.Clamp(rotSpeed, minRotSpeed, maxRotSpeed);�@// �p���x�𐧌�����
        transform.localRotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, axis) * transform.localRotation;

    }

    void Idle()
    {
        return;
    }

    // �v�b�V�����s���B
    void Push()
    {
        rb.AddForce((opponent.transform.position - transform.position).normalized * status.PushPower, ForceMode.Impulse);
    }

    // �J�E���^�[���Ƀx�C�̃m�b�N�o�b�N�ϐ���傫����Z���A�J�E���^�[���I������炻���ł��������Z���s���B
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

    #region �_���[�W�����̏ڍ�
    void DamageManagement()
    {
        if (isHitOpponent && !isDamaged)
        {
            isDamaged = true;

            if (State == PlayerState.PUSH) // �v�b�V�����͎󂯂�_���[�W������
            {
                hp -= opponentRb.velocity.magnitude * pSO.DamageCoef * pSO.DamageCoefOnPush;
                Debug.Log($"<color=#64ff64>{gameObject.name}���v�b�V�����F�_���[�W������</color>");
                return;
            }
            else if (State == PlayerState.COUNTER) // �J�E���^�[���̓_���[�W��H���Ȃ�
            {
                Debug.Log($"<color=#64ff64>{gameObject.name}���J�E���^�[���F�_���[�W��H���Ȃ�</color>");
                return;
            }
            else // �m�b�N�o�b�N���ꂽ�iIdle��ԂœG�ɐڐG�����ꍇ���A���̏������s���j�F�ʏ�_���[�W����
            {
                hp -= opponentRb.velocity.magnitude * pSO.DamageCoef;
                Debug.Log($"<color=#64ff64>{gameObject.name}���m�b�N�o�b�N���ꂽ�F�ʏ�_���[�W</color>");
                return;
            }
        }
    }

    // HP�\���B������HP��0��؂��Ă�����A���̃I�u�W�F�N�g���A�N�e�B�u�ɂ���B
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
