using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BaseSystem
{
    public class PlayerMove : MonoBehaviour
    {
        #region �ϐ��錾
        // �x�C�̎�ނ����ʂ���B�C���X�y�N�^��Őݒ肵�Ă������ƁI
        enum TYPE { NULL, Ballerina, BreakDancer, Enemy1 }
        [SerializeField] TYPE type = TYPE.NULL;

        // �v���C���[�̏��(public)
        public enum PlayerState { IDLE, PUSH, COUNTER, KNOCKBACKED };
        public PlayerState State { get; set; }

        // �������Z�p
        Rigidbody rb;
        GameObject stageCenter;

        // SO����f�[�^���擾����p
        PlayerSO pSO;
        BehaviourTable pSOB;
        DamageTable pSOD;
        StatusTable sSO;
        StatusTableName sSON;
        StatusTablePlayable sSOP;
        StatusTableUnPlayable sSOU;
        StatusTableInitStatus sSOI;
        float Hp { get; set; }
        float weight;
        float knoRes; // �����I��Rigidbody.drag�i����R�j�𑀍삵�Ă���
        float rotSpe;

        // �G�ւ̎Q�Ƃ��擾����p
        GameObject opponent; // �Q�[���I�u�W�F�N�g
        PlayerMove opponentPm; // PlayerMove�N���X
        Rigidbody opponentRb; // Rigidbody

        // �x�C�̍s�������p
        bool isDamageManager = false; // �_���[�W�������s���C���X�^���X�ł��邩�ǂ���
        bool isDamagable = false; // �y��l�����s��Ȃ��z�_���[�W��H�炦�邩�i�����G���ԂłȂ����j
        bool isPushIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�v�b�V�����g���t���O
        bool isCounterIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�J�E���^�[���g���t���O
        bool isSkillIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�X�L�����g���t���O
        bool isSpecialIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�K�E�Z���g���t���O
        bool IsPushBehaviourDone { get; set; } = false; // �v�b�V���̏������A�������Ă��邩
        bool IsCounterBehaviourDone { get; set; } = false; // �J�E���^�[�̏������A�������Ă��邩
        bool isKnockbackedBehaviourDone = false; // �m�b�N�o�b�N���ꂽ���̏������A�������Ă��邩
        bool isOnStateChangeCooltime = false; // ���Ԍo�߂ɂ���ԕω����ł��邩�ǂ���
        Vector3 axis; // �x�C�̉�]��
        float axisTimer = 0; // �x�C�̉�]�����X���鎞��
        bool isHpLow = false; // HP�����ȉ��ɂȂ��āA�΍��^�������Ă��邩�ǂ���

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
            pSOB = pSO.BehaviourTable;
            pSOD = pSO.DamageTable;

            // �Ή�����StatusTable���擾���A�ϓ����鐔�l�����߂āA���̃N���X���̕ϐ��Ɋi�[
            switch (type)
            {
                case TYPE.Ballerina:
                    sSO = BallerinaStatusSO.Entity.StatusTable;
                    break;

                case TYPE.BreakDancer:
                    sSO = BreakDancerStatusSO.Entity.StatusTable;
                    break;

                case TYPE.Enemy1:
                    sSO = Enemy1StatusSO.Entity.StatusTable;
                    break;

                case TYPE.NULL:
                    Debug.LogError("<color=red>type���ݒ肳��Ă��܂���</color>");
                    break;
            }
            sSON = sSO.StatusTableName;
            sSOP = sSO.StatusTablePlayable;
            sSOU = sSO.StatusTableUnPlayable;
            sSOI = sSO.StatusTableInitStatus;
            Hp = sSOI.Hp;
            weight = sSOI.Weight;
            knoRes = sSOI.KnockbackResistance;
            rotSpe = sSOI.RotationSpeed + SelectTeam.SceneChange.RotateNumber;

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
            opponentPm = opponent.GetComponent<PlayerMove>();
            opponentRb = opponent.GetComponent<Rigidbody>();

            // ���̃x�C�ɑΉ�����e�L�X�g���擾���āAHP�\��
            text = GameManager.Instance.Texts[Array.IndexOf(GameManager.Instance.Beys, gameObject)];

            // �Q�[���J�n���班���̊Ԃ́A���G���ԂɂȂ��Ă���B
            StartCoroutine(CountDamagableDuration());

            // �A���v���C�A�u���Ȃ�A�v�b�V���ƃX�L�����g���t���O���A�����I�����݂�Update���\�b�h�ɑ���
            if (!sSO.IsPlayable)
            {
                StartCoroutine(InputPushAndSkillPeriodically());
            }
        }

        IEnumerator InputPushAndSkillPeriodically()
        {
            bool isSkill2Push = true; // �X�L�� �� �v�b�V���̑J�ڂ��s���Ԃł��邩�ǂ���

            while (true)
            {
                if (isSkill2Push)
                {
                    isSkill2Push = false;

                    float dTime = sSOU.Skill2PushInterval;
                    float ofst = sSOU.Skill2PushIntervalOffset;
                    float time = Random.Range(dTime - ofst, dTime + ofst);
                    yield return new WaitForSeconds(time);
                    isPushIfUnplayable = true;
                }
                else
                {
                    isSkill2Push = true;

                    float dTime = sSOU.Push2SkillInterval;
                    float ofst = sSOU.Push2SkillIntervalOffset;
                    float time = Random.Range(dTime - ofst, dTime + ofst);
                    yield return new WaitForSeconds(time);
                    isSkillIfUnplayable = true;
                }
            }
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
            rb.drag = pSOB.DragCoef * knoRes; // drag�F��R
        }
        #endregion

        #region �yOnCollision�z�y��l�����s��Ȃ��z�G�Ƃ̐ڐG�����m���A���G���ԂłȂ��Ȃ�΁A�_���[�W�������s���������ɂ���Ď����܂��͓G��KNOCKBACKED��Ԃɂ���B
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(pSO.BeyTagName) && isDamageManager)
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
            yield return new WaitForSeconds(pSOD.DamagableDuration);
            isDamagable = true;
        }
        #endregion

        #region �yUpdate�z
        void Update()
        {
            #region PlayerState�̑J�ځF�J�ڃN�[���^�C�����łȂ��Ȃ�A�v���C���[����̓��͂܂��̓R���[�`������̃t���O���擾���āAPUSH��Ԃ܂���COUNTER��ԂɂȂ�B
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
        // �y�����z�i�G�ƐڐG�����ۂɌĂ΂��B�j
        // �y���̑��z�_���[�W�������s���B
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
                            Debug.Log($"<color=#64ff64>{name} �� {opponent.name} �Ƀv�b�V�������I</color>");
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
                            Debug.Log($"<color=#64ff64>{opponent.name} �� {name} �Ƀv�b�V�������I</color>");
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
                            Debug.Log($"<color=#64ff64>{name} �� {opponent.name} �ɃJ�E���^�[�����I</color>");
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
                            Debug.Log($"<color=#64ff64>{opponent.name} �� {name} �ɃJ�E���^�[�����I</color>");
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
        // �^����ꂽPlayerMove�N���X�̃C���X�^���X�ɁA���̃C���X�^���X���^����ꂽ��Ԃł���ꍇ�̃_���[�W��^����B
        void Damage(PlayerMove pm, PlayerState state)
        {
            // ��b�_���[�W
            float momentumNorm = (rb.mass * rb.velocity).magnitude; // �_���[�W�}�l�[�W���[���g�̉^����
            float opponentMomentumNorm = (opponentRb.mass * opponentRb.velocity).magnitude; // �_���[�W�}�l�[�W���[�̑���̉^����
            float baseDamage = momentumNorm + opponentMomentumNorm;

            // �X�e�[�^�X�␳�l
            float weightAdjustValue = CalcMrkDamage(weight, sSOI.Weight); // �d�ʕ␳�l
            float rotSpeAdjustValue = CalcMrkDamage(rotSpe, sSOI.RotationSpeed); // ��]���x�␳�l
            float knoResAdjustValue = CalcMrkDamage(knoRes, sSOI.KnockbackResistance); // �m�b�N�o�b�N�ϐ��␳�l
            float hpAdjustValue = pm.isHpLow ? pSOD.HpAdjustValue[0] : pSOD.HpAdjustValue[1]; // �̗͕␳�l
            float statusAdjustValue = weightAdjustValue * rotSpeAdjustValue * knoResAdjustValue * hpAdjustValue;

            // ��ԕ␳�l
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

            // �_���[�W�W��
            float damageCoef = pSOD.DamageCoef;

            float damage = baseDamage * statusAdjustValue * stateAdjustValue * damageCoef; // �_���[�W�v�Z

            if (pSOD.MinDamage <= damage && damage <= pSOD.MaxDamage)
            {
                pm.Hp -= damage; // �^����ꂽ�C���X�^���X�Ƀ_���[�W��^����
            }
            
            if (pSO.IsShowNormalLog) // ���O��\��
            {
                Debug.Log($"<color=#64ff64>{pm.gameObject.name} �� {damage} �_���[�W�I</color>");
            }
        }
        // �d�ʕ␳�l/��]���x�␳�l/�m�b�N�o�b�N�ϐ��␳�l �̌v�Z
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
        // �y�����z�v���C�A�u�����v�b�V���L�[�������ꂽ�A�܂��̓A���v���C�A�u�����v�b�V���̃t���O���󂯎�����B
        void Idle2Push()
        {
            if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
            {
                if (sSO.IsPlayable)
                {
                    if (Input.GetKeyDown(sSOP.PushKey))
                    {
                        State = PlayerState.PUSH;
                    }
                }
                else
                {
                    if (isPushIfUnplayable)
                    {
                        isPushIfUnplayable = false;

                        State = PlayerState.PUSH;
                    }
                }
            }
        }

        // COUNTER => PUSH
        // �y�����z�v���C�A�u�����v�b�V���L�[�������ꂽ�A�܂��̓A���v���C�A�u�����v�b�V���̃t���O���󂯎�����B
        void Counter2Push()
        {
            if (State == PlayerState.COUNTER && !isOnStateChangeCooltime)
            {
                if (sSO.IsPlayable)
                {
                    if (Input.GetKeyDown(sSOP.PushKey))
                    {
                        State = PlayerState.PUSH;
                        knoRes /= pSOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;
                    }
                }
                else
                {
                    if (isPushIfUnplayable)
                    {
                        isPushIfUnplayable = false;

                        State = PlayerState.PUSH;
                        knoRes /= pSOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;
                    }
                }
            }
        }

        // IDLE => COUNTER
        // �y�����z�v���C�A�u�����J�E���^�[�L�[�������ꂽ�A�܂��̓A���v���C�A�u�����J�E���^�[�̃t���O���󂯎�����B
        void Idle2Counter()
        {
            if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
            {
                if (sSO.IsPlayable)
                {
                    if (Input.GetKeyDown(sSOP.CounterKey))
                    {
                        State = PlayerState.COUNTER;
                    }
                }
                else
                {
                    if (isCounterIfUnplayable)
                    {
                        isCounterIfUnplayable = false;

                        State = PlayerState.COUNTER;
                    }
                }
            }
        }

        // PUSH => COUNTER
        // �y�����z�v���C�A�u�����J�E���^�[�L�[�������ꂽ�A�܂��̓A���v���C�A�u�����J�E���^�[�̃t���O���󂯎�����B
        void Push2Counter()
        {
            if (State == PlayerState.PUSH && !isOnStateChangeCooltime)
            {
                if (sSO.IsPlayable)
                {
                    if (Input.GetKeyDown(sSOP.CounterKey))
                    {
                        State = PlayerState.COUNTER;
                        IsPushBehaviourDone = false;
                    }
                }
                else
                {
                    if (isCounterIfUnplayable)
                    {
                        isCounterIfUnplayable = false;

                        State = PlayerState.COUNTER;
                        IsPushBehaviourDone = false;
                    }
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
            yield return new WaitForSeconds(pSOB.Duration2IdleOnPushFailed);
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
            yield return new WaitForSeconds(pSOB.Duration2IdleWhenKnockbacked);
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
            // HP�����ȉ����ǂ����`�F�b�N���A�t���O��؂�ւ���B
            isHpLow = Hp < sSOI.Hp * pSOB.AxisSlopeStartHpCoef ? true : false;

            // ��]�������s���O�ɁA�x�C�̃��[�J��y���i�΁j�̕�����n�ʂ̖@���x�N�g���ɍ��킹��B
            Ray shotRay = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(shotRay, out RaycastHit ground))
            {
                Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, pSOB.PlayerMainAxisChangeSpeed * Time.deltaTime);
            }

            // HP�����ȉ��ɂȂ�����A�΍��^��������B
            if (isHpLow)
            {
                // �w��b�����Ƃɉ�]���̌X����ω�������B
                axisTimer += Time.deltaTime;
                if (axisTimer > pSOB.AxisSlopeChangeInterval)
                {
                    axisTimer -= pSOB.AxisSlopeChangeInterval;
                    float theta = UnityEngine.Random.Range(pSOB.AxisSlopRange.x, pSOB.AxisSlopRange.y);
                    axis = Quaternion.AngleAxis(theta, transform.forward) * transform.up;
                }

                // ��]���𒆐S���itransform.up�j����ɉ�]������B
                float axisSpeed = pSOB.AxisRotateSpeed / sSOI.Hp * Hp;
                axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * axis;
            }
            // �����łȂ��Ȃ�A���]����B
            else
            {
                axis = transform.up;
            }

            // �x�C����]������ɉ�]������B
            float rotSpeed = rotSpe / sSOI.Hp * Hp;
            float minRotSpeed = pSOB.RotationSpeedCoefRange.x * rotSpe;
            float maxRotSpeed = pSOB.RotationSpeedCoefRange.y * rotSpe;
            rotSpeed = Mathf.Clamp(rotSpeed, minRotSpeed, maxRotSpeed); // �p���x�𐧌�����B
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
                    rb.AddForce((opponent.transform.position - transform.position).normalized * sSOI.PushPower, ForceMode.Impulse);
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
                    knoRes *= pSOB.KnockbackResistanceCoefOnCounter;
                }
            }
        }

        // KNOCKBACKED��Ԃł́A1�񂾂��ȉ��̏������s���B
        // �E�����ƓG�̉^���ʂ̑傫�������v���A���̒萔�{�̏u�ԓI�ȗ́i�Œ�ۏ؂���j���A�G�Ɣ��Ε����ɉ�����B
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

        #region �x�C�̃X�N���v�g�́A�e��ϐ��̕\���̏ڍ�
        void ShowHP()
        {
            if (Hp < 0) // �f�X����
            {
                GameManager.Instance.IsGameEnded = true; // �Q�[���I���̍��}�𑗂�
                StopAllCoroutines(); // �R���[�`�������ׂĒ�~
                text.text = "0"; // HP�\����0�ɂ���

                gameObject.SetActive(false); // ��A�N�e�B�u�ɂ���
            }
            else
            {
                text.text = $"{Hp}\n{State}";
            }
        }
        #endregion
    }
}
