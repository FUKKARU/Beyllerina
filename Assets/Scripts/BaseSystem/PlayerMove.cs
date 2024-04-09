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
        [SerializeField] CameraShake_Battle CameraS_B;

        //�q�b�g�G�t�F�N�g
        [SerializeField] GameObject hitEffect;

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
        bool[] isSkillIfUnplayables; // �A���v���C�A�u���ł��鎞�A�X�L�����g���t���O�̃��X�g
        bool isSpecialIfUnplayable = false; // �A���v���C�A�u���ł��鎞�A�K�E�Z���g���t���O
        bool IsPushBehaviourDone { get; set; } = false; // �v�b�V���̏������A�������Ă��邩
        bool IsCounterBehaviourDone { get; set; } = false; // �J�E���^�[�̏������A�������Ă��邩
        bool isKnockbackedBehaviourDone = false; // �m�b�N�o�b�N���ꂽ���̏������A�������Ă��邩
        bool isOnPushCooltime = false; // �v�b�V���̃N�[���^�C�����ł��邩�ǂ���
        bool isOnCounterCooltime = false; // �J�E���^�[�̃N�[���^�C�����ł��邩�ǂ���
        bool[] isOnSkillCooltimes; // �X�L���̃N�[���^�C�����ł��邩�ǂ���
        bool isOnSpecialCooltime = false; // �K�E�Z�̃N�[���^�C�����ł��邩�ǂ���
        bool isOnStateChangeCooltime = false; // ���Ԍo�߂ɂ���ԕω����ł��邩�ǂ���
        Vector3 axis; // �x�C�̉�]��
        float axisTimer = 0; // �x�C�̉�]�����X���鎞��
        bool isHpLow = false; // HP�����ȉ��ɂȂ��āA�΍��^�������Ă��邩�ǂ���
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
            Hp = S_SOI.Hp;
            weight = S_SOI.Weight;
            knoRes = S_SOI.KnockbackResistance;
            rotSpe = S_SOI.RotationSpeed + SelectTeam.SceneChange.RotateNumber;

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

            // �x�C�̍s�������p�̔z����C���X�^���X�����āA����������B
            if (S_SO.IsPlayable)
            {
                isOnSkillCooltimes = new bool[S_SOP.SkillNum];
                for (int i = 0; i < isOnSkillCooltimes.Length; i++)
                {
                    isOnSkillCooltimes[i] = false;
                }
            }
            else
            {
                isOnSkillCooltimes = new bool[S_SOU.SkillNum];
                for (int i = 0; i < isOnSkillCooltimes.Length; i++)
                {
                    isOnSkillCooltimes[i] = false;
                }

                isSkillIfUnplayables = new bool[S_SOU.SkillNum];
                for (int i = 0; i < isSkillIfUnplayables.Length; i++)
                {
                    isSkillIfUnplayables[i] = false;
                }
            }

            // �Q�[���J�n���班���̊Ԃ́A���G���ԂɂȂ��Ă���B
            if (gameObject.activeSelf) StartCoroutine(CountDamagableDuration());

            // �A���v���C�A�u���Ȃ�A�v�b�V���ƃX�L�����g���t���O���A�����I�����݂�Update���\�b�h�ɑ���
            if (!S_SO.IsPlayable)
            {
                if (gameObject.activeSelf) StartCoroutine(InputPushAndSkillPeriodically());
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
                    int i = Random.Range(0, S_SOU.SkillNum); // �����_���ȃX�L��
                    isSkillIfUnplayables[i] = true;
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
            rb.AddForce(Vector3.down * 9.81f * P_SO.GravityScale, ForceMode.Force);

            // ��ɒ��S�ֈړ�
            rb.AddForce((stageCenter.transform.position - transform.position).normalized * P_SO.SpeedTowardCenter, ForceMode.Force);

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
                Instantiate(hitEffect,( gameObject.transform.position + collision.gameObject.transform.position ) / 2 ,Quaternion.identity);
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
            #endregion

            #region PlayerState�Ɋ�Â��x�C�̍s������
            Rotate(); // �ŏ��ɉ�]�������s���B
            Idle();
            Push();
            Counter();
            Knockbacked();
            #endregion

            #region �f�X����
            JudgeDeath();
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

                        if (P_SO.IsShowNormalLog)
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

                        if (P_SO.IsShowNormalLog)
                        {
                            Debug.Log($"<color=#64ff64>{opponent.name} �� {name} �Ƀv�b�V�������I</color>");
                        }
                    }
                    else if (State == PlayerState.COUNTER)
                    {
                        State = PlayerState.IDLE;
                        knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;

                        opponentPm.State = PlayerState.KNOCKBACKED;
                        opponentPm.IsPushBehaviourDone = false;

                        Damage(this, PlayerState.COUNTER);
                        Damage(opponentPm, PlayerState.KNOCKBACKED);

                        if (P_SO.IsShowNormalLog)
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
                        opponentPm.knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        opponentPm.IsCounterBehaviourDone = false;

                        Damage(this, PlayerState.KNOCKBACKED);
                        Damage(opponentPm, PlayerState.COUNTER);

                        if (P_SO.IsShowNormalLog)
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

            if (P_SO.IsShowNormalLog) // ���O��\��
            {
                Debug.Log($"<color=#64ff64>{pm.gameObject.name} �� {damage} �_���[�W�I</color>");
            }
        }
        // �d�ʕ␳�l/��]���x�␳�l/�m�b�N�o�b�N�ϐ��␳�l �̌v�Z
        float CalcMrkDamage(float x, float d)
        {
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
                    if (!isOnPushCooltime && Input.GetKeyDown(S_SOP.PushKey) && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.PUSH;
                        isOnPushCooltime = true;
                        if (gameObject.activeSelf) StartCoroutine(CountPushCooltime());
                    }
                }
                else
                {
                    if (isPushIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isPushIfUnplayable = false;

                        State = PlayerState.PUSH;
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
                    if (!isOnPushCooltime && Input.GetKeyDown(S_SOP.PushKey) && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.PUSH;
                        isOnPushCooltime = true;
                        knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;
                        if (gameObject.activeSelf) StartCoroutine(CountPushCooltime());
                    }
                }
                else
                {
                    if (isPushIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isPushIfUnplayable = false;

                        State = PlayerState.PUSH;
                        knoRes /= P_SOB.KnockbackResistanceCoefOnCounter;
                        IsCounterBehaviourDone = false;
                    }
                }
            }
        }

        IEnumerator CountPushCooltime()
        {
            float ct = S_SOI.PushCoolTime;
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
                    if (!isOnCounterCooltime && Input.GetKeyDown(S_SOP.CounterKey) && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.COUNTER;
                        isOnCounterCooltime = true;
                        if (gameObject.activeSelf) StartCoroutine(CountCounterCooltime());
                    }
                }
                else
                {
                    if (isCounterIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isCounterIfUnplayable = false;

                        State = PlayerState.COUNTER;
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
                    if (!isOnCounterCooltime && Input.GetKeyDown(S_SOP.CounterKey) && !IsSkillDirection && !IsSpecialDirection)
                    {
                        State = PlayerState.COUNTER;
                        isOnCounterCooltime= true;
                        IsPushBehaviourDone = false;
                        if (gameObject.activeSelf) StartCoroutine(CountCounterCooltime());
                    }
                }
                else
                {
                    if (isCounterIfUnplayable && !IsSkillDirection && !IsSpecialDirection)
                    {
                        isCounterIfUnplayable = false;

                        State = PlayerState.COUNTER;
                        IsPushBehaviourDone = false;
                    }
                }
            }
        }

        IEnumerator CountCounterCooltime()
        {
            float ct = S_SOI.CounterCoolTime;
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
        #endregion

        #region �X�L���E�K�E�Z�̏ڍ�
        void Skill()
        {
            if (S_SO.IsPlayable)
            {
                for (int i = 0; i < isOnSkillCooltimes.Length; i++)
                {
                    if (!isOnSkillCooltimes[i] && Input.GetKeyDown(S_SOP.SkillKeys[i]))
                    {
                        isOnSkillCooltimes[i] = true;

                        if (gameObject.activeSelf) StartCoroutine(CountSkillCooltime(i));

                        SkillBehaviour(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < isSkillIfUnplayables.Length; i++)
                {
                    if (isSkillIfUnplayables[i])
                    {
                        isSkillIfUnplayables[i] = false;

                        SkillBehaviour(i);
                    }
                }
            }
        }

        // i�Ԗڂ̃X�L�����g��
        void SkillBehaviour(int i)
        {
            IsSkillDirection = true;

            StartCoroutine(SkillBehaviourDirection());

            switch (type)
            {
                case TYPE.Ballerina:
                    break;

                case TYPE.BreakDancer:
                    break;

                case TYPE.Enemy1:
                    break;
            }
        }

        IEnumerator SkillBehaviourDirection()
        {
            switch (type)
            {
                case TYPE.Ballerina:
                    break;

                case TYPE.BreakDancer:
                    break;

                case TYPE.Enemy1:
                    break;
            }

            IsSkillDirection = false;

            yield break;
        }

        IEnumerator CountSkillCooltime(int idx) // idx��0�n�܂�Ȃ��Ƃɒ��ӁI
        {
            float ct = S_SOI.SkillCooltimes[idx];
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            // �������ɂ���B
            Color _col = gm.SkillCooltimeGauges[idx].color;
            _col.a = P_SOB.GaugeAOnCooltime / (float)255;
            gm.SkillCooltimeGauges[idx].color = _col;

            gm.SkillCooltimeGauges[0].fillAmount = 0f;

            while (time >= 0f)
            {
                gm.SkillCooltimeGauges[0].fillAmount = -time / ct + 1;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            gm.SkillCooltimeGauges[0].fillAmount = 1f;

            // �s�����ɖ߂��B
            Color col = gm.SkillCooltimeGauges[idx].color;
            col.a = 255 / (float)255;
            gm.SkillCooltimeGauges[idx].color = col;

            isOnSkillCooltimes[idx] = false;
        }

        void Special()
        {
            if (S_SO.IsPlayable)
            {
                if (!isOnSpecialCooltime && Input.GetKeyDown(S_SOP.SpecialKey))
                {
                    isOnSpecialCooltime = true;

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

            switch (type)
            {
                case TYPE.Ballerina:
                    break;

                case TYPE.BreakDancer:
                    break;

                case TYPE.Enemy1:
                    break;
            }
        }

        // �K�E�Z�̉��o���s��
        IEnumerator SpecialBehaviourDirection()
        {
            switch (type)
            {
                case TYPE.Ballerina:
                    break;

                case TYPE.BreakDancer:
                    break;

                case TYPE.Enemy1:
                    break;
            }
            
            IsSpecialDirection = false;

            yield break;
        }

        IEnumerator CountSpecialCooltime()
        {
            float ct = S_SOI.SpecialCooltime;
            float time = ct;
            float interval = P_SOB.CooltimeBehaviourInterval;

            //// �������ɂ���B
            //Color _col = gm.SpecialCooltimeGauge.color;
            //_col.a = P_SOB.GaugeAOnCooltime / (float)255;
            //gm.SpecialCooltimeGauge.color = _col;

            // gm.SpecialCooltimeGauge.fillAmount = 0f;

            while (time >= 0f)
            {
                 // gm.SpecialCooltimeGauge.fillAmount = -time / ct + 1;
                yield return new WaitForSeconds(interval);
                time -= interval;
            }

            // gm.SpecialCooltimeGauge.fillAmount = 1f;

            //// �s�����ɖ߂��B
            //Color col = gm.SpecialCooltimeGauge.color;
            //col.a = 255 / (float)255;
            //gm.SpecialCooltimeGauge.color = col;

            isOnSpecialCooltime = false;
        }
        #endregion

        #region PlayerState�Ɋ�Â��x�C�̍s�������̏ڍ�
        // 1.�n�ʂɐ����Ȏp�����������B
        // 2.���]����B�������AHP���Ⴍ�Ȃ�����΍��^���ɐ؂�ւ��B
        void Rotate()
        {
            // HP�����ȉ����ǂ����`�F�b�N���A�t���O��؂�ւ���B
            isHpLow = Hp < S_SOI.Hp * P_SOB.AxisSlopeStartHpCoef ? true : false;

            // ��]�������s���O�ɁA�x�C�̃��[�J��y���i�΁j�̕�����n�ʂ̖@���x�N�g���ɍ��킹��B
            Ray shotRay = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(shotRay, out RaycastHit ground))
            {
                Quaternion toSlope = Quaternion.FromToRotation(transform.up, ground.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, toSlope * transform.rotation, P_SOB.PlayerMainAxisChangeSpeed * Time.deltaTime);
            }

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
                    rb.AddForce((opponent.transform.position - transform.position).normalized * S_SOI.PushPower, ForceMode.Impulse);
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

        #region �f�X����̏ڍ�
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

                gameObject.SetActive(false); // ��A�N�e�B�u�ɂ���
            }
        }
        #endregion
    }
}
