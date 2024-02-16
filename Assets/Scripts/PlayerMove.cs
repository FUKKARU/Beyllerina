using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    enum TYPE { NULL, Ballerina, BreakDancer }
    [SerializeField] TYPE type = TYPE.NULL;

    // �v���C���[�̏��
    enum PlayerState { IDLE, PUSH, GUARD }
    PlayerState state;

    // �������Z�p
    Rigidbody rb;
    GameObject center;
    bool grounded;

    // SO����f�[�^���擾����p
    PlayerSO pSO;
    StatusSO status;
    float hp;
    float weight;
    float knockbackResistance; // �����I��Rigidbody.drag�i����R�j�𑀍삵�Ă���

    // �x�C�̍s�������p
    float pushTimer;
    Vector3 axis; // ��]��
    float axisTimer = 0; // ��]�����X���鎞��

    // ���̃x�C�ɑΉ�����e�L�X�g���擾����p
    TextMeshProUGUI text;

    void Start()
    {
        // �v���C���[�̏�����Ԃ�ݒ�
        state = PlayerState.IDLE;

        // �������Z�̏����iUnity���̏d�͂��I�t�ɂ��A�l�͂Ōv�Z����d�͂̒��S���擾�j
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        center = GameObject.FindGameObjectWithTag("Center");

        // PlayerSO���擾
        pSO = PlayerSO.Entity;

        // �Ή�����StatusSO���擾���A�ϓ����鐔�l�����߂āA���̃N���X���̕ϐ��Ɋi�[
        if (type == TYPE.Ballerina) status = BallerinaStatusSO.Entity.Status;
        else if (type == TYPE.BreakDancer) status = BreakDancerStatusSO.Entity.Status;
        else Debug.LogError("<color=red>type���ݒ肳��Ă��܂���</color>");
        hp = status.Hp;
        weight = status.Weight;
        knockbackResistance = status.KnockbackResistance;

        // ���̃x�C�ɑΉ�����e�L�X�g���擾���āAHP�\��
        text = GameManager.Instance.Texts[Array.IndexOf(GameManager.Instance.Beys, gameObject)];
        text.text = hp.ToString();
    }

    void Update()
    {
        // Rigidbody�̕����ʁimass��drag�j�̍X�V�A�y�ђn�ʂƂ̐ڐG����
        rb.mass = weight; // mass�F�d��
        rb.drag = pSO.DragCoef * knockbackResistance; // drag�F��R
        grounded = Physics.Raycast(transform.position, -transform.up, pSO.PlayerHeight * 0.5f + 0.2f, pSO.WhatIsGround);

        // PlayerState�Ɋ�Â��āA�x�C�̋����y�эs������������BPlayerState�ɂ��āAGUARD=>PUSH�������A�S�Ă̑J�ڂ��s���B
        PlayerAct();

        // HP�\���B������HP��0��؂��Ă�����A���̃I�u�W�F�N�g���A�N�e�B�u�ɂ���B
        if (hp < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            text.text = hp.ToString();
        }
    }

    // ���蔫��̃t�B�[���h�ɂ�����A�d�͂��Č�����B
    void FixedUpdate()
    {
        // �d��
        rb.AddForce(Vector3.down * 9.81f * pSO.GravityScale, ForceMode.Force);

        //��ɒ��S�ֈړ�
        rb.AddForce((center.transform.position - transform.position).normalized * pSO.SpeedTowardCenter, ForceMode.Force);

    }

    // �v���C���[�Ƃ̐ڐG�����m���A����̑��x�Ɋ�Â��ă_���[�W�������s���B
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �v�b�V�����͎󂯂�_���[�W������
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
        // ��]�������s���O�ɁA�x�C�̃��[�J��y���i�΁j�̕�����n�ʂ̖@���x�N�g���ɍ��킹��B
        VerticalToGround();

        // �x�C����]�����A���̉�]���x�����͈͓��Ɏ��߂�B
        // HP�����ȉ��ɂȂ�����A�w��b�����Ƃɉ�]���̌X����ω������č΍��^����������B
        Rotate();
        
        // IDLE��Ԃ̎��̂݁A�v�b�V�����͂����m�B�N�[���^�C���J�E���g���s���AIDLE<=>PUSH�̑J�ڂ��s���B
        PushBehaviour();

        // �n�ʂɐڐG���Ă��鎞�̂ݓ��͂����m���āA���[���h����������ɃW�����v����B
        if (!status.DebugIsJumpDisable)
        {
            if (grounded && Input.GetKeyDown(status.JumpKey))
                Jump();
        }

        // �n�ʂɐڐG���Ă��鎞�̂ݓ��͂����m���āA�K�[�h�����AIDLE,PUSH=>GUARD�̑J�ڂ��s���B�K�[�h������������AGUARD=>IDLE�̑J�ڂ��s���B
        // �K�[�h���Ƀx�C�̃m�b�N�o�b�N�ϐ���傫����Z���A�K�[�h���I������炻���ł��������Z���s���B
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
            Push(); // �v�b�V���̃^�[�Q�b�g��F������i�v���P�j�B�^�[�Q�b�g���|����Ă��Ȃ��Ȃ�΁A�v�b�V�����s���A���O���o���AIDLE=>PUSH�̑J�ڂ��s���B

        if (state == PlayerState.PUSH)
        {
            pushTimer += Time.deltaTime;

            if (pushTimer >= status.PushCoolTime)
            {
                pushTimer = 0;
                state = PlayerState.IDLE; // �N�[���^�C�����؂ꂽ��APUSH=>IDLE�̑J�ڂ��s���B
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
            Debug.Log($"<color=#64ff64>{gameObject.name}���v�b�V��</color>");
        }
    }

    void VerticalToGround()
    {
        //�n�ʂ̖@���𒲂ׂ郌�C
        Ray shotRay = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(shotRay, out RaycastHit ground))
        {
            //�x�C�̃��[�J��y���i�΁j�̕����@����@�n�ʂ̖@���x�N�g����
            Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, pSO.PlayerMainAxisChangeSpeed * Time.deltaTime);
        }
    }
}
