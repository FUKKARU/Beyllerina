using System;
using System.Collections;
using System.Collections.Generic;
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

        //�@�J�����V�F�C�N
        CameraShake_Battle CameraS_B;

        //�G�t�F�N�g
        [SerializeField] GameObject hitEffect;
        Transform hit_effect_parent;
        GameObject shield;
        GameObject[] enemyEffect;

        // GM����f�[�^���擾����悤
        GameManager gm;

        // SO����f�[�^���擾����p
        public PlayerSO P_SO { get; set; }
        public BehaviourTable P_SOB { get; set; }
        public DamageTable P_SOD { get; set; }
        public StatusTable S_SO { get; set; }
        public StatusTableName S_SON { get; set; }
        public StatusTablePlayable S_SOP { get; set; }
        public StatusTableUnPlayable S_SOU { get; set; }
        public StatusTableInitStatus S_SOI { get; set; }
        public float Hp { get; set; }
        float weight;
        float knoRes; // �����I��Rigidbody.drag�i����R�j�𑀍삵�Ă���
        float rotSpe;
        // �i�ȉ��́A�v���C�A�u���̂ݎg�p�j
        float pushCooltime;
        float counterCooltime;
        float skillCooltime;
        float specialCooltime;
        int maxSpecialPoint;

        // �G�ւ̎Q�Ƃ��擾����p
        GameObject opponent; // �Q�[���I�u�W�F�N�g
        PlayerMove opponentPm; // PlayerMove�N���X
        Rigidbody opponentRb; // Rigidbody

        // �x�C�̍s�������p
        bool isDamageManager = false; // �_���[�W�������s���C���X�^���X�ł��邩�ǂ���
        bool isDamagable = false; // �y��l�����s��Ȃ��z�_���[�W��H�炦�邩�i�����G���ԂłȂ����j
        bool IsSkillDirection { get; set; } = false; // �X�L���̉��o�����ǂ���
        bool IsSpecialDirection { get; set; } = false; // �K�E�Z�̉��o�����ǂ���
        bool isPushIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�v�b�V�����g���t���O
        bool isCounterIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�J�E���^�[���g���t���O
        bool isSkillIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�X�L�����g���t���O�̃��X�g
        bool isSpecialIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�K�E�Z���g���t���O
        bool IsPushBehaviourDone { get; set; } = false; // �v�b�V���̏������A�������Ă��邩
        bool IsCounterBehaviourDone { get; set; } = false; // �J�E���^�[�̏������A�������Ă��邩
        bool isKnockbackedBehaviourDone = false; // �m�b�N�o�b�N���ꂽ���̏������A�������Ă��邩
        bool isOnPushCooltime = false; // �v�b�V���̃N�[���^�C�����ł��邩�ǂ���
        bool isOnCounterCooltime = false; // �J�E���^�[�̃N�[���^�C�����ł��邩�ǂ���
        bool isOnSkillCooltime = false; // �X�L���̃N�[���^�C�����ł��邩�ǂ���
        bool isOnSpecialCooltime = false; // �K�E�Z�̃N�[���^�C�����ł��邩�ǂ���
        bool isOnStateChangeCooltime = false; // ���Ԍo�߂ɂ���ԕω����ł��邩�ǂ���
        bool ifUnplayableOnSkill = false; // �A���v���C�A�u�����A�X�L�����g���ċ��剻���Ă���Œ����ǂ���
        Vector3 axis; // �x�C�̉�]��
        float axisTimer = 0; // �x�C�̉�]�����X���鎞��
        bool isHpLow = false; // HP�����ȉ��ɂȂ��āA�΍��^�������Ă��邩�ǂ���
        Vector3 rePos; // ���X�|�[���|�C���g
        float GenericDamageCoef { get; set; } = 1; // �ėp�_���[�W�W���i�����ɏ�Z�j
        float OpponentGenericDamageCoef { get; set; } = 1; // �ėp�_���[�W�W���i����ɏ�Z�j
        float rotDir = 1; // ����]�Ȃ�1�A�t��]�Ȃ�-1
        float pushPowerCoef = 1; // �v�b�V���͂̌W���i�����p�j
        int specialPoint = 0; // �K�E�Z�̔����|�C���g

        bool antiGravity = false;//�d�͒�~
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

            // GameManager���擾
            gm = GameManager.Instance;

            // PlayerSO���擾
            P_SO = PlayerSO.Entity;
            P_SOB = P_SO.BehaviourTable;
            P_SOD = P_SO.DamageTable;

            // �Ή�����StatusTable���擾���A�ϓ����鐔�l�����߂āA���̃N���X���̕ϐ��Ɋi�[
            switch (type)
            {
                case TYPE.Ballerina:
                    S_SO = BallerinaStatusSO.Entity.StatusTable;
                    break;

                case TYPE.BreakDancer:
                    S_SO = BreakDancerStatusSO.Entity.StatusTable;
                    break;

                case TYPE.Enemy1:
                    S_SO = Enemy1StatusSO.Entity.StatusTable;
                    break;

                case TYPE.NULL:
                    Debug.LogError("<color=red>type���ݒ肳��Ă��܂���</color>");
                    break;
            }
            S_SON = S_SO.StatusTableName;
            S_SOP = S_SO.StatusTablePlayable;
            S_SOU = S_SO.StatusTableUnPlayable;
            S_SOI = S_SO.StatusTableInitStatus;
            if (S_SO.IsPlayable)
            {
                Hp = S_SOI.Hp;
                gm.PlayableBar.fillAmount = 1;
                gm.PlayableDamagedBar.fillAmount = 1;
            }
            else
            {
                Hp = S_SOI.Hp;
                gm.UnPlayableBar.fillAmount = 1;
                gm.UnPlayableDamagedBar.fillAmount = 1;
            }

            weight = S_SOI.Weight;
            knoRes = S_SOI.KnockbackResistance;
            rotSpe = S_SOI.RotationSpeed * SelectTeam.RotData.RotateNumber;

            if (S_SO.IsPlayable)
            {
                pushCooltime = S_SOI.PushCooltime;
                counterCooltime = S_SOI.CounterCooltime;
                skillCooltime = S_SOI.SkillCooltime;
                specialCooltime = S_SOI.SpecialCooltime;
                maxSpecialPoint = S_SOI.SpecialPoint;

                if (P_SO.Dbg.IsInfinityAction)
                {
                    pushCooltime = 1;
                    counterCooltime = 1;
                    skillCooltime = 1;
                    specialCooltime = 1;
                    maxSpecialPoint = 1;
                }
            }

            // �G�ւ̎Q�Ƃ��擾
            int idx = Array.IndexOf(gm.Beys, gameObject);
            if (idx == 0)
            {
                opponent = gm.Beys[1];
                isDamageManager = true; // 0�Ԗڂ̃C���X�^���X�ŁA�_���[�W�������s���B
            }
            else if (idx == 1)
            {
                opponent = gm.Beys[0];
            }
            else
            {
                Debug.LogError("<color=red>�G�I�u�W�F�N�g�̎擾�Ɏ��s���܂���</color>");
            }
            opponentPm = opponent.GetComponent<PlayerMove>();
            opponentRb = opponent.GetComponent<Rigidbody>();

            // ���X�|�[���|�C���g�̎擾
            string tag = (this == GameManager.Instance.P_Pm) ? P_SOB.P_RePosTag : P_SOB.U_RePosTag;
            rePos = GameObject.FindGameObjectWithTag(tag).transform.position;

            // CameraShake_Battle ���擾
            CameraS_B = GameObject.FindGameObjectWithTag("CameraShakeGameObject").GetComponent<CameraShake_Battle>();

            // �G�t�F�N�g���擾
            hit_effect_parent = GameObject.FindGameObjectWithTag("hit_effect_parent").transform;
            if (S_SO.IsPlayable)
            {
                shield = GameObject.FindGameObjectWithTag("Shield");
                Shield(false);
            }
            enemyEffect = GameObject.FindGameObjectsWithTag("EnemyEffect");

            // �Q�[���J�n���班���̊Ԃ́A���G���ԂɂȂ��Ă���B
            if (gameObject.activeSelf) StartCoroutine(CountDamagableDuration());

            if (!S_SO.IsPlayable)
            {
                // �A���v���C�A�u���Ȃ�A�v�b�V���ƃX�L�����g���t���O���A�����I�����݂�Update���\�b�h�ɑ���
                if (gameObject.activeSelf) StartCoroutine(InputPushAndSkillPeriodically());
            }
            else
            {
                // �����łȂ��Ȃ�A���b�|�C���g�𒙂߂�
                if (gameObject.activeSelf) StartCoroutine(PointIncrease());
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

                    float dTime = S_SOU.Skill2PushInterval;
                    float ofst = S_SOU.Skill2PushIntervalOffset;
                    float time = Random.Range(dTime - ofst, dTime + ofst);
                    yield return new WaitForSeconds(time);
                    isPushIfUnplayable = true;
                }
                else
                {
                    isSkill2Push = true;

                    float dTime = S_SOU.Push2SkillInterval;
                    float ofst = S_SOU.Push2SkillIntervalOffset;
                    float time = Random.Range(dTime - ofst, dTime + ofst);
                    yield return new WaitForSeconds(time);
                    isSkillIfUnplayable = true;
                }
            }
        }

        IEnumerator PointIncrease()
        {
            while (true)
            {
                specialPoint += P_SOB.PointAmount;
                PointBonus();
                specialPoint = Mathf.Clamp(specialPoint, 0, maxSpecialPoint);
                yield return new WaitForSeconds(P_SOB.PointDur);
            }
        }

        // �̗͂�����قǁA�{�[�i�X�|�C���g�����炦��B
        void PointBonus()
        {
            int[] bonusPoint = P_SOB.BonusPoint;
            int len = bonusPoint.Length;
            float hpRange = S_SOI.Hp / len; // ���苫�E�Ԃ̒���
            if (Hp == 0)
            {
                return;
            }
            else
            {
                // (0, S_SOI.Hp] �܂Ŕ���ł���B
                for (int i = 0; i < len; i++)
                {
                    if (hpRange * i < Hp && Hp <= hpRange * (i + 1))
                    {
                        specialPoint += bonusPoint[i];
                    }
                }
            }
        }
        #endregion

        #region �yFixedUpdate�z�܂��A�x�C�̕�����Ԃ��X�V����B���ɁA���蔫��̃t�B�[���h�ɂ�����A�d�͂ɂ�鋓�����Č�����B
        void FixedUpdate()
        {
            // �x�C�̕�����Ԃ̍X�V
            ChangeRigidbodyParameters();

            if (!antiGravity)
            {
                //// ���͂����m���āA���������ړ�
                //if (S_SO.IsPlayable && P_SO.Dbg.IsBeyMovable)
                //{
                //    Vector3 myPos = transform.position;
                //    Vector3 myNormPos = new Vector3(myPos.x, 0, myPos.z);
                //    Vector3 cameraPos = GameManager.Instance.GameCamera.transform.position;
                //    Vector3 cameraNormPos = new Vector3(cameraPos.x, 0, cameraPos.z);

                //    Vector3 dir = myNormPos - cameraNormPos;
                //    Vector2 val = IA.InputGetter.Instance.ValueDirection;
                //    Vector2 lMove = new Vector3(val.x, 0, val.y);

                //    float amp = Mathf.Atan2(dir.z, dir.x); // dir�̕Ίp
                //    float theta = 90 - amp * Mathf.Rad2Deg; // �x�N�g�������x��]�����Ă��邩

                //    Vector3 move = Quaternion.AngleAxis(theta, Vector3.up) * lMove;
                //    Debug.Log(move);
                //    rb.AddForce(move * 20, ForceMode.Force);
                //}

                // �d��
                rb.AddForce(Vector3.down * 9.81f * P_SO.GravityScale, ForceMode.Force);

                // ��ɒ��S�ֈړ�
                rb.AddForce((stageCenter.transform.position - transform.position).normalized * P_SO.SpeedTowardCenter, ForceMode.Force);
            }
        }

        // �x�C�̌��݂̃X�e�[�^�X�Ɋ�Â��āARigidbody�̃p�����[�^�[���X�V����B
        void ChangeRigidbodyParameters()
        {
            rb.mass = weight; // mass�F�d��
            rb.drag = P_SOB.DragCoef * knoRes; // drag�F��R
        }
        #endregion

        #region �yOnCollision�z�y��l�����s��Ȃ��z�G�Ƃ̐ڐG�����m���A���G���ԂłȂ������҂��X�L���E�K�E�Z�̉��o���łȂ��Ȃ�΁A�_���[�W�������s���������ɂ���Ď����܂��͓G��KNOCKBACKED��Ԃɂ���B
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(P_SO.BeyTagName) && isDamageManager)
            {
                CameraS_B.ShakeOn();
                Instantiate(hitEffect, (gameObject.transform.position + collision.gameObject.transform.position) / 2 + new Vector3(0,1,0), Quaternion.identity, hit_effect_parent);
                if (isDamagable && (!IsSpecialDirection && !opponentPm.IsSpecialDirection) && (!IsSkillDirection && !opponentPm.IsSkillDirection))
                {
                    isDamagable = false;
                    HitBehaviour(); // PlayerState�̑J�ڂȂ�
                    if (gameObject.activeSelf) StartCoroutine(CountDamagableDuration()); // ���G���Ԃ̃J�E���g
                }
            }
        }
        IEnumerator CountDamagableDuration()
        {
            yield return new WaitForSeconds(P_SOD.DamagableDuration);
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

            #region �X�L���E�K�E�Z�F�v���C���[����̓��͂܂��̓R���[�`������̃t���O���擾
            Skill();
            Special();
            ShowSpecialPoint();
            #endregion

            #region PlayerState�Ɋ�Â��x�C�̍s������
            Rotate(); // �ŏ��ɉ�]�������s���B
            Idle();
            Push();
            Counter();
            Knockbacked();
            Clamp();
            #endregion

            #region �f�X����
            JudgeFall(); // ��O����
            JudgeDeath();
            #endregion

            #region PlayerState�̑J�ځF�J�ڃN�[���^�C�����łȂ��Ȃ�A�J�ڃN�[���^�C�����J�n���A���Ԍo�߂�IDLE��ԂɂȂ�B
            Push2Idle();
            Counter2Idle();
            Knockbacked2Idle();
            #endregion
        }
        #endregion

        #region �yLateUpdate�z
        void LateUpdate()
        {
            Posture(); // �p������
        }

        // �n�ʂɐ����Ȏp�����������B
        void Posture()
        {
            // �x�C�̃��[�J��y���i�΁j�̕�����n�ʂ̖@���x�N�g���ɍ��킹��B
            Ray shotRay = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(shotRay, out RaycastHit ground))
            {
                Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, P_SOB.PlayerMainAxisChangeSpeed * Time.deltaTime);
            }
        }
        #endregion



        #region�@PlayerState�̑J�ڂ̏ڍׁi�Ή������Ԃ̎��A�y�����z�𖞂������瑦���ɑJ�ڂ���B�x�C�̍s�������Ɋւ��ϐ��̃��Z�b�g���s���B�j
        // �ȉ��̂����ꂩ�̑J�ڂ��s���B
        // �E�y�����zPUSH    => IDLE       �y�G�zIDLE    => KNOCKBACKED
        // �E�y�����zCOUNTER => IDLE       �y�G�zPUSH    => KNOCKBACKED     (�J�E���^�[�񐔂𑝂₷�B)
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
                        SoundManager.Instance.PlaySE(2);

                        Damage(this, PlayerState.PUSH);
                        Damage(opponentPm, PlayerState.KNOCKBACKED);

                        if (P_SO.Dbg.IsShowNormalLog)
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

                        if (P_SO.Dbg.IsShowNormalLog)
                        {
                            Debug.Log($"<color=#64ff64>{opponent.name} �� {name} �Ƀv�b�V�������I</color>");
                        }
                    }
                    else if (State == PlayerState.COUNTER)
                    {
                        State = PlayerState.IDLE;
                        Shield(false);
                        knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;

                        opponentPm.State = PlayerState.KNOCKBACKED;
                        opponentPm.IsPushBehaviourDone = false;
                        SoundManager.Instance.PlaySE(2);

                        Damage(this, PlayerState.COUNTER);
                        Damage(opponentPm, PlayerState.KNOCKBACKED);

                        if (P_SO.Dbg.IsShowNormalLog)
                        {
                            Debug.Log($"<color=#64ff64>{name} �� {opponent.name} �ɃJ�E���^�[�����I</color>");
                        }

                        GameData.GameData.CounterNum++;
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
                        SoundManager.Instance.PlaySE(2);

                        opponentPm.State = PlayerState.IDLE;
                        opponentPm.Shield(false);
                        opponentPm.knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        opponentPm.IsCounterBehaviourDone = false;

                        Damage(this, PlayerState.KNOCKBACKED);
                        Damage(opponentPm, PlayerState.COUNTER);

                        if (P_SO.Dbg.IsShowNormalLog)
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
            float weightAdjustValue = CalcMrkDamage(weight, S_SOI.Weight); // �d�ʕ␳�l
            float rotSpeAdjustValue = CalcMrkDamage(rotSpe, S_SOI.RotationSpeed); // ��]���x�␳�l
            float knoResAdjustValue = CalcMrkDamage(knoRes, S_SOI.KnockbackResistance); // �m�b�N�o�b�N�ϐ��␳�l
            float hpAdjustValue = pm.isHpLow ? P_SOD.HpAdjustValue[0] : P_SOD.HpAdjustValue[1]; // �̗͕␳�l
            float statusAdjustValue = weightAdjustValue * rotSpeAdjustValue * knoResAdjustValue * hpAdjustValue;

            // ��ԕ␳�l
            float stateAdjustValue = 0f;
            switch (state)
            {
                case PlayerState.IDLE:
                    stateAdjustValue = P_SOD.StateAdjustValue[0];
                    break;

                case PlayerState.PUSH:
                    stateAdjustValue = P_SOD.StateAdjustValue[1];
                    break;

                case PlayerState.COUNTER:
                    stateAdjustValue = P_SOD.StateAdjustValue[2];
                    break;

                case PlayerState.KNOCKBACKED:
                    stateAdjustValue = P_SOD.StateAdjustValue[3];
                    break;
            }

            // �_���[�W�W��
            float damageCoef = P_SOD.DamageCoef;

            // �_���[�W�v�Z
            float damage = baseDamage * statusAdjustValue * stateAdjustValue * damageCoef;
            damage = Mathf.Clamp(damage, P_SOD.MinDamage, P_SOD.MaxDamage);
            damage *= pm.GenericDamageCoef * pm.opponentPm.OpponentGenericDamageCoef;

            if (pm.S_SO.IsPlayable && pm.P_SO.Dbg.P_DamageMul)
            {
                damage *= 100;
            }
            else if (!pm.S_SO.IsPlayable && pm.P_SO.Dbg.U_DamageMul)
            {
                damage *= 100;
            }
            else if (pm.S_SO.IsPlayable && pm.P_SO.Dbg.P_DamageImmune)
            {
                damage *= 0;
            }
            else if (!pm.S_SO.IsPlayable && pm.P_SO.Dbg.U_DamageImmune)
            {
                damage *= 0;
            }

            pm.Hp -= damage; // �^����ꂽ�C���X�^���X�Ƀ_���[�W��^����

            // Bar��ω�������
            if (pm.S_SO.IsPlayable)
            {
                pm.gm.PlayableBar.fillAmount = pm.Hp / pm.S_SOI.Hp;
                pm.gm.IsChangePlayableBar = true;
            }
            else
            {
                pm.gm.UnPlayableBar.fillAmount = pm.Hp / pm.S_SOI.Hp;
                pm.gm.IsChangeUnPlayableBar = true;
            }

            if (P_SO.Dbg.IsShowNormalLog) // ���O��\��
            {
                Debug.Log($"<color=#64ff64>{pm.gameObject.name} �� {damage} �_���[�W�I</color>");
            }
        }
        // �d�ʕ␳�l/��]���x�␳�l/�m�b�N�o�b�N�ϐ��␳�l �̌v�Z
        float CalcMrkDamage(float x, float d)
        {
            x = Mathf.Abs(x);
            float k = -2 / d * Mathf.Log(2 - P_SOD.MrkAdjustValueY);
            if (x < d)
            {
                return -Mathf.Exp(k * (x - d)) + 2;
            }
            else
            {
                float m = -k * Mathf.Exp(k * (P_SOD.MrkAdjustValueX - d));
                return m * (x - d) + 1;
            }
        }

        // IDLE => PUSH
        // �y�����z�X�L���E�K�E�Z�̉��o���łȂ��A���A�v���C�A�u�����v�b�V���L�[�������ꂽ�܂��̓A���v���C�A�u�����v�b�V���̃t���O���󂯎�����B
        // �A���v���C�A�u���̎��A�v�b�V���̃t���O���󂯎��Ԋu�ɂ���ẮA�v�b�V�����X�L�b�v�����B
        void Idle2Push()
        {
            if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
            {
                if (S_SO.IsPlayable)
                {
                    if (!isOnPushCooltime && IA.InputGetter.Instance.IsPush && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.PUSH;
                        isOnPushCooltime = true;
                        if (gameObject.activeSelf) StartCoroutine(CountPushCooltime());
                        SoundManager.Instance.PlaySE(3);
                        SoundManager.Instance.PlaySE(11);
                    }
                }
                else
                {
                    if (isPushIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isPushIfUnplayable = false;

                        StartCoroutine(UnPlayablePushDir(true));

                        SoundManager.Instance.PlaySE(3);
                        SoundManager.Instance.PlaySE(11);
                    }
                }
            }
        }

        // COUNTER => PUSH
        // �y�����z�X�L���E�K�E�Z�̉��o���łȂ��A���A�v���C�A�u�����v�b�V���L�[�������ꂽ�܂��̓A���v���C�A�u�����v�b�V���̃t���O���󂯎�����B
        // �A���v���C�A�u���̎��A�v�b�V���̃t���O���󂯎��Ԋu�ɂ���ẮA�v�b�V�����X�L�b�v�����B
        void Counter2Push()
        {
            if (State == PlayerState.COUNTER && !isOnStateChangeCooltime)
            {
                if (S_SO.IsPlayable)
                {
                    if (!isOnPushCooltime && IA.InputGetter.Instance.IsPush && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.PUSH;
                        Shield(false);
                        isOnPushCooltime = true;
                        knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;
                        if (gameObject.activeSelf) StartCoroutine(CountPushCooltime());
                        SoundManager.Instance.PlaySE(3);
                        SoundManager.Instance.PlaySE(11);
                    }
                }
                else
                {
                    if (isPushIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isPushIfUnplayable = false;

                        StartCoroutine(UnPlayablePushDir(false));

                        SoundManager.Instance.PlaySE(3);
                        SoundManager.Instance.PlaySE(11);
                    }
                }
            }
        }

        IEnumerator UnPlayablePushDir(bool isFromIdle)
        {
            GameObject pushEffect = Instantiate(Enemy1StatusSO.Entity.PushEffectObj, transform.position, Quaternion.identity, transform);
            if (ifUnplayableOnSkill)
            {
                pushEffect.transform.localScale *= Enemy1StatusSO.Entity.SkillSizeCoef;
            }

            yield return new WaitForSeconds(Enemy1StatusSO.Entity.PushDirDur);

            State = PlayerState.PUSH;

            if (!isFromIdle)
            {
                knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                IsCounterBehaviourDone = false;
            }

            SoundManager.Instance.PushSE(false);
        }

        IEnumerator CountPushCooltime()
        {
            float ct = pushCooltime;
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            // �������ɂ���B
            Color _col = gm.PushCooltimeGauge.color;
            _col.a = P_SOB.GaugeAOnCooltime / (float)255;
            gm.PushCooltimeGauge.color = _col;

            gm.PushCooltimeGauge.fillAmount = 0f;

            while (time >= 0f)
            {
                gm.PushCooltimeGauge.fillAmount = -time / ct + 1;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.PushCooltimeGauge.fillAmount = 1f;

            // �s�����ɖ߂��B
            Color col = gm.PushCooltimeGauge.color;
            col.a = 255 / (float)255;
            gm.PushCooltimeGauge.color = col;

            isOnPushCooltime = false;
        }

        // IDLE => COUNTER
        // �y�����z�X�L���E�K�E�Z�̉��o���łȂ��A���A�v���C�A�u�����J�E���^�[�L�[�������ꂽ�܂��̓A���v���C�A�u�����J�E���^�[�̃t���O���󂯎�����B
        // �A���v���C�A�u���̎��A�J�E���^�[�̃t���O���󂯎��Ԋu�ɂ���ẮA�J�E���^�[���X�L�b�v�����B
        void Idle2Counter()
        {
            if (State == PlayerState.IDLE && !isOnStateChangeCooltime)
            {
                if (S_SO.IsPlayable)
                {
                    if (!isOnCounterCooltime && IA.InputGetter.Instance.IsCounter && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.COUNTER;
                        Shield(true);
                        isOnCounterCooltime = true;
                        if (gameObject.activeSelf) StartCoroutine(CountCounterCooltime());

                        SoundManager.Instance.PlaySE(5);
                        SoundManager.Instance.PlaySE(12);
                    }
                }
                else
                {
                    if (isCounterIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isCounterIfUnplayable = false;

                        State = PlayerState.COUNTER;

                        SoundManager.Instance.PlaySE(5);
                        SoundManager.Instance.PlaySE(12);
                    }
                }
            }
        }

        // PUSH => COUNTER
        // �y�����z�X�L���E�K�E�Z�̉��o���łȂ��A���A�v���C�A�u�����J�E���^�[�L�[�������ꂽ�܂��̓A���v���C�A�u�����J�E���^�[�̃t���O���󂯎�����B
        // �A���v���C�A�u���̎��A�J�E���^�[�̃t���O���󂯎��Ԋu�ɂ���ẮA�J�E���^�[���X�L�b�v�����B
        void Push2Counter()
        {
            if (State == PlayerState.PUSH && !isOnStateChangeCooltime)
            {
                if (S_SO.IsPlayable)
                {
                    if (!isOnCounterCooltime && IA.InputGetter.Instance.IsCounter && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.COUNTER;
                        Shield(true);
                        isOnCounterCooltime = true;
                        IsPushBehaviourDone = false;
                        if (gameObject.activeSelf) StartCoroutine(CountCounterCooltime());

                        SoundManager.Instance.PlaySE(5);
                        SoundManager.Instance.PlaySE(12);
                    }
                }
                else
                {
                    if (isCounterIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isCounterIfUnplayable = false;

                        State = PlayerState.COUNTER;
                        IsPushBehaviourDone = false;

                        SoundManager.Instance.PlaySE(5);
                        SoundManager.Instance.PlaySE(12);
                    }
                }
            }
        }

        IEnumerator CountCounterCooltime()
        {
            float ct = counterCooltime;
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            // �������ɂ���B
            Color _col = gm.CounterCooltimeGauge.color;
            _col.a = P_SOB.GaugeAOnCooltime / (float)255;
            gm.CounterCooltimeGauge.color = _col;

            gm.CounterCooltimeGauge.fillAmount = 0f;

            while (time >= 0f)
            {
                gm.CounterCooltimeGauge.fillAmount = -time / ct + 1;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.CounterCooltimeGauge.fillAmount = 1f;

            // �s�����ɖ߂��B
            Color col = gm.CounterCooltimeGauge.color;
            col.a = 255 / (float)255;
            gm.CounterCooltimeGauge.color = col;

            isOnCounterCooltime = false;
        }

        // PUSH => IDLE
        // �y�����z��莞�Ԃ��o�߂��A����ł��Ȃ�PUSH��ԁB
        void Push2Idle()
        {
            if (State == PlayerState.PUSH && !isOnStateChangeCooltime)
            {
                if (gameObject.activeSelf) StartCoroutine(Push2IdleWithCount());
            }
        }
        IEnumerator Push2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(P_SOB.Duration2IdleOnPushFailed);
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
                if (gameObject.activeSelf) StartCoroutine(Counter2IdleWithCount());
            }
        }
        IEnumerator Counter2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(P_SOB.Duration2IdleOnCounterFailed);
            if (State == PlayerState.COUNTER)
            {
                State = PlayerState.IDLE;
                Shield(false);
                knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
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
                if (gameObject.activeSelf) StartCoroutine(Knockbacked2IdleWithCount());
            }
        }
        IEnumerator Knockbacked2IdleWithCount()
        {
            isOnStateChangeCooltime = true;
            yield return new WaitForSeconds(P_SOB.Duration2IdleWhenKnockbacked);
            if (State == PlayerState.KNOCKBACKED)
            {
                State = PlayerState.IDLE;
                isKnockbackedBehaviourDone = false;
            }
            isOnStateChangeCooltime = false;
        }

        void Shield(bool isActivate)
        {
            if (S_SO.IsPlayable)
            {
                shield.SetActive(isActivate);
            }
        }
        #endregion

        #region �X�L���E�K�E�Z�̏ڍ�
        void Skill()
        {
            if (S_SO.IsPlayable)
            {
                if (!isOnSkillCooltime && IA.InputGetter.Instance.IsSkill)
                {
                    isOnSkillCooltime = true;

                    if (gameObject.activeSelf) StartCoroutine(CountSkillCooltime());

                    SoundManager.Instance.SkillSE(true);

                    SkillBehaviour();
                }
            }
            else
            {
                if (isSkillIfUnplayable)
                {
                    isSkillIfUnplayable = false;

                    SoundManager.Instance.SkillSE(false);

                    SkillBehaviour();
                }
            }
        }

        // �X�L�����g��
        void SkillBehaviour()
        {
            IsSkillDirection = true;

            StartCoroutine(SkillBehaviourDirection());
        }


        Vector3 sp, op;
        bool action = true, _action = true;
        IEnumerator SkillBehaviourDirection()
        {
            switch (type)
            {
                case TYPE.Ballerina:
                    GenericDamageCoef *= BallerinaStatusSO.Entity.OnSkillGenericDamageCoefCoef;

                    int pow = BallerinaStatusSO.Entity.SkillPow;
                    float _t = 0;
                    float _d = BallerinaStatusSO.Entity.SkillRiseDur;
                    float h = BallerinaStatusSO.Entity.SkillRiseHeight;
                    float coef = h / Mathf.Pow(_d, 2);

                    float t = 0;
                    float d = BallerinaStatusSO.Entity.SkillRushDur;
                    sp = transform.position;
                    op = opponent.transform.position;
                    float ax = (Mathf.Pow(d, pow) + sp.x - op.x) / (pow * d);
                    float ay = (Mathf.Pow(d, pow) + sp.y - op.y) / (pow * d);
                    float az = (Mathf.Pow(d, pow) + sp.z - op.z) / (pow * d);
                    Vector3 a = new Vector3(ax, ay, az);
                    float bx = sp.x - Mathf.Pow(a.x, pow);
                    float by = sp.y - Mathf.Pow(a.y, pow);
                    float bz = sp.z - Mathf.Pow(a.z, pow);
                    Vector3 b = new Vector3(bx, by, bz);

                    SoundManager.Instance.PlaySE(8);

                    Vector3 lPos = transform.localPosition;
                    while (_action)
                    {
                        _t += Time.deltaTime;

                        float y = -coef * Mathf.Pow(_t - _d, 2) + h;
                        transform.localPosition = lPos + Vector3.up * y;
                        if (_t > _d)
                        {
                            _action = false;
                        }

                        yield return null;
                    }

                    SoundManager.Instance.PlaySE(9);
                    SoundManager.Instance.PlaySE(13);

                    while (action)
                    {
                        t += Time.deltaTime;

                        float yx = Mathf.Pow(t - a.x, pow) + b.x;
                        float yy = Mathf.Pow(t - a.y, pow) + b.y;
                        float yz = Mathf.Pow(t - a.z, pow) + b.z;

                        transform.position = new Vector3(yx, yy, yz);
                        if (t > d) action = false;

                        yield return null;
                    }

                    break;

                case TYPE.BreakDancer:
                    StartCoroutine(RotationChange());
                    break;

                case TYPE.Enemy1:
                    antiGravity = true;
                    while (transform.position.y < Enemy1StatusSO.Entity.SkillHeight)
                    {
                        Vector3 cPos = transform.position;
                        cPos.y += Enemy1StatusSO.Entity.SkillSpeed * Time.deltaTime;
                        transform.position = cPos;

                        yield return null;
                    }
                    ifUnplayableOnSkill = true;
                    transform.localScale *= Enemy1StatusSO.Entity.SkillSizeCoef;
                    foreach (GameObject e in enemyEffect)
                    {
                        e.transform.localScale *= Enemy1StatusSO.Entity.SkillSizeCoef;
                    }
                    weight *= Enemy1StatusSO.Entity.SkillWeightCoef;
                    antiGravity = false;
                    SoundManager.Instance.SkillSE(false);
                    break;
            }

            IsSkillDirection = false;

            switch (type)
            {
                case TYPE.Ballerina:
                    opponentRb.AddForce((op - sp).normalized * S_SOI.PushPower * BallerinaStatusSO.Entity.SkillPushPowerCoef, ForceMode.Impulse);
                    Damage(opponentPm, PlayerState.KNOCKBACKED);
                    StartCoroutine(NormalizeGenericDamageCoef());
                    break;

                case TYPE.BreakDancer:
                    break;

                case TYPE.Enemy1:
                    float time = 0;
                    while (true)
                    {
                        time += Time.deltaTime;
                        if (time >= Enemy1StatusSO.Entity.SkillDuration)
                        {
                            ifUnplayableOnSkill = false;
                            transform.localScale /= Enemy1StatusSO.Entity.SkillSizeCoef;
                            foreach (GameObject e in enemyEffect)
                            {
                                e.transform.localScale /= Enemy1StatusSO.Entity.SkillSizeCoef;
                            }
                            weight /= Enemy1StatusSO.Entity.SkillWeightCoef;
                            break;
                        }
                        yield return null;
                    }
                    break;
            }

            yield break;
        }

        IEnumerator NormalizeGenericDamageCoef()
        {
            yield return new WaitForSeconds(BallerinaStatusSO.Entity.OnSkillDamageNormalizeDur);

            GenericDamageCoef /= BallerinaStatusSO.Entity.OnSkillGenericDamageCoefCoef;
        }

        IEnumerator RotationChange()
        {
            float t = 0;
            float d = BreakDancerStatusSO.Entity.SkillRotChangeDur;
            while (true)
            {
                t += Time.deltaTime;
                rotDir = -2 * t / d + 1;
                if (t > d)
                {
                    rotDir = -1;
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(BreakDancerStatusSO.Entity.SkillDur);

            t = 0;
            while (true)
            {
                t += Time.deltaTime;
                rotDir = 2 * t / d - 1;
                if (t > d)
                {
                    rotDir = 1;
                    break;
                }

                yield return null;
            }
        }

        IEnumerator CountSkillCooltime() // idx��0�n�܂�Ȃ��Ƃɒ��ӁI
        {
            float ct = skillCooltime;
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            // �������ɂ���B
            Color _col = gm.SkillCooltimeGauge.color;
            _col.a = P_SOB.GaugeAOnCooltime / (float)255;
            gm.SkillCooltimeGauge.color = _col;

            gm.SkillCooltimeGauge.fillAmount = 0f;

            while (time >= 0f)
            {
                gm.SkillCooltimeGauge.fillAmount = -time / ct + 1;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.SkillCooltimeGauge.fillAmount = 1f;

            // �s�����ɖ߂��B
            Color col = gm.SkillCooltimeGauge.color;
            col.a = 255 / (float)255;
            gm.SkillCooltimeGauge.color = col;

            isOnSkillCooltime = false;

            action = true; _action = true;
        }

        void Special()
        {
            if (S_SO.IsPlayable)
            {
                if (specialPoint == maxSpecialPoint && !isOnSpecialCooltime && IA.InputGetter.Instance.IsSpecial)
                {
                    isOnSpecialCooltime = true;

                    specialPoint = 0;

                    if (gameObject.activeSelf) StartCoroutine(CountSpecialCooltime());

                    SpecialBehaviour();
                }
            }
            else
            {
                if (isSpecialIfUnplayable)
                {
                    isSpecialIfUnplayable = false;

                    SpecialBehaviour();
                }
            }
        }

        // �K�E�Z���g��
        void SpecialBehaviour()
        {
            IsSpecialDirection = true;

            StartCoroutine(SpecialBehaviourDirection());
        }

        // �K�E�Z�̉��o���s��
        IEnumerator SpecialBehaviourDirection()
        {
            /*
            switch (type)
            {
                case TYPE.Ballerina:
                    break;

                case TYPE.BreakDancer:
                    break;

                case TYPE.Enemy1:
                    break;
            }*/

            IsSpecialDirection = false;
            switch (type)
            {
                case TYPE.Ballerina:
                    StartCoroutine(BallerinaCountSpecialTime());
                    break;

                case TYPE.BreakDancer:
                    StartCoroutine(BreakDancerCountSpecialTime());
                    break;

                case TYPE.Enemy1:
                    break;
            }
            yield break;
        }

        IEnumerator BallerinaCountSpecialTime()
        {
            SoundManager.Instance.PlaySE(6);
            SoundManager.Instance.PlaySE(14);
            SoundManager.Instance.Special(true);

            OpponentGenericDamageCoef *= BallerinaStatusSO.Entity.OnSpecialGenericDamageCoefCoef;

            GameManager.Instance.OnSpecialLightDir(true);
            GameObject rightWeapon = GameObject.FindGameObjectWithTag("RightWeapon").transform.GetChild(0).gameObject;
            GameObject leftWeapon = GameObject.FindGameObjectWithTag("LeftWeapon").transform.GetChild(0).gameObject;
            rightWeapon.SetActive(true);
            leftWeapon.SetActive(true);
            Coroutine rightCor = StartCoroutine(rightWeapon.GetComponent<MeshTrail>().TrailCreate());
            Coroutine leftCor = StartCoroutine(leftWeapon.GetComponent<MeshTrail>().TrailCreate());

            yield return new WaitForSeconds(BallerinaStatusSO.Entity.SpecialDur);

            OpponentGenericDamageCoef /= BallerinaStatusSO.Entity.OnSpecialGenericDamageCoefCoef;

            SoundManager.Instance.Special(false);

            OpponentGenericDamageCoef /= BallerinaStatusSO.Entity.OnWeakGenericDamageCoefCoef;

            GameManager.Instance.OnSpecialLightDir(false);
            StopCoroutine(rightCor);
            rightCor = null;
            StopCoroutine(leftCor);
            leftCor = null;
            rightWeapon.SetActive(false);
            leftWeapon.SetActive(false);

            yield return new WaitForSeconds(BallerinaStatusSO.Entity.WeakDur);

            OpponentGenericDamageCoef *= BallerinaStatusSO.Entity.OnWeakGenericDamageCoefCoef;

            yield break;
        }

        IEnumerator BreakDancerCountSpecialTime()
        {
            /*����*/
            pushPowerCoef *= BreakDancerStatusSO.Entity.SpecialPushPowerCoefCoef;
            rotSpe *= BreakDancerStatusSO.Entity.SpecialRotSpeedCoef;

            yield return new WaitForSeconds(BreakDancerStatusSO.Entity.SpecialDur);

            /*����Ȃ�����*/
            pushPowerCoef /= BreakDancerStatusSO.Entity.SpecialPushPowerCoefCoef;
            rotSpe /= BreakDancerStatusSO.Entity.SpecialRotSpeedCoef;

            yield break;
        }

        IEnumerator CountSpecialCooltime()
        {
            float ct = specialCooltime;
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            gm.SpecialCooltimeGauge.fillAmount = 1f;

            while (time >= 0f)
            {
                gm.SpecialCooltimeGauge.fillAmount = time / ct;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.SpecialCooltimeGauge.fillAmount = 0f;

            isOnSpecialCooltime = false;
        }

        void ShowSpecialPoint()
        {
            if (S_SO.IsPlayable)
            {
                gm.SpecialGauge.fillAmount = specialPoint / (float)maxSpecialPoint;
                if (specialPoint == maxSpecialPoint && !isOnSpecialCooltime)
                {
                    // �K�E�Z�������ł��邱�Ƃ�m�点��
                    gm.SpecialGauge.color = Color.yellow;
                }
                else
                {
                    gm.SpecialGauge.color = Color.green;
                }
            }
        }
        #endregion

        #region PlayerState�Ɋ�Â��x�C�̍s�������̏ڍ�
        // ���]����B�������AHP���Ⴍ�Ȃ�����΍��^���ɐ؂�ւ��B
        void Rotate()
        {
            // HP�����ȉ����ǂ����`�F�b�N���A�t���O��؂�ւ���B
            isHpLow = Hp < S_SOI.Hp * P_SOB.AxisSlopeStartHpCoef ? true : false;

            // HP�����ȉ��ɂȂ�����A�΍��^��������B
            if (isHpLow)
            {
                // �w��b�����Ƃɉ�]���̌X����ω�������B
                axisTimer += Time.deltaTime;
                if (axisTimer > P_SOB.AxisSlopeChangeInterval)
                {
                    axisTimer -= P_SOB.AxisSlopeChangeInterval;
                    float theta = Random.Range(P_SOB.AxisSlopRange.x, P_SOB.AxisSlopRange.y);
                    axis = Quaternion.AngleAxis(theta, transform.forward) * transform.up;
                }

                // ��]���𒆐S���itransform.up�j����ɉ�]������B
                float axisSpeed = P_SOB.AxisRotateSpeed / S_SOI.Hp * Hp;
                axis = Quaternion.AngleAxis(axisSpeed * Time.deltaTime, transform.up) * axis;
            }
            // �����łȂ��Ȃ�A���]����B
            else
            {
                axis = transform.up;
            }

            // �x�C����]������ɉ�]������B
            float rotSpeed = rotSpe / S_SOI.Hp * Hp;
            float minRotSpeed = P_SOB.RotationSpeedCoefRange.x * rotSpe;
            float maxRotSpeed = P_SOB.RotationSpeedCoefRange.y * rotSpe;
            rotSpeed = Mathf.Clamp(rotSpeed, minRotSpeed, maxRotSpeed); // �p���x�𐧌�����B
            rotSpeed *= rotDir;
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

                    float power = S_SOI.PushPower * pushPowerCoef;
                    rb.AddForce((opponent.transform.position - transform.position).normalized * power, ForceMode.Impulse);
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
                    knoRes *= P_SOB.KnockbackResistanceCoefOnCounter;
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
                    float power = (self + oppo) * P_SOB.PowerCoefOnKnockbacked;
                    if (power < P_SOB.MinPowerOnKnockbacked)
                    {
                        power = P_SOB.MinPowerOnKnockbacked;
                    }
                    rb.AddForce((transform.position - opponent.transform.position).normalized * power, ForceMode.Impulse);
                }
            }
        }

        void Clamp()
        {
            Vector3 pos = transform.position;
            float r = Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.z, 2));
            float theta = Mathf.Atan2(pos.z, pos.x);
            r = Mathf.Clamp(r, P_SOB.ClampR.x, P_SOB.ClampR.y);
            pos.x = r * Mathf.Cos(theta);
            pos.z = r * Mathf.Sin(theta);
            pos.y = Mathf.Clamp(pos.y, P_SOB.ClampY.x, P_SOB.ClampY.y);
            transform.position = pos;
        }
        #endregion

        #region �f�X����̏ڍ�

        void JudgeFall()
        {
            if (transform.position.y < P_SOB.FallJudgeY) transform.position = rePos;
        }

        void JudgeDeath()
        {
            if (Hp < 0)
            {
                StopAllCoroutines(); // �R���[�`�������ׂĒ�~
                if (S_SO.IsPlayable)
                {
                    gm.PlayableBar.fillAmount = 0f;
                }
                else
                {
                    gm.UnPlayableBar.fillAmount = 0f;
                }

                // KO/KOed�̏����𔭉΂���
                if (!S_SO.IsPlayable)
                {
                    GameManager.Instance.KO();
                }
                else
                {
                    GameManager.Instance.KOed();
                }

                gameObject.SetActive(false); // ��A�N�e�B�u�ɂ���
            }
        }
        #endregion
    }
}