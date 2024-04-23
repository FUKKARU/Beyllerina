using UnityEngine;
using UnityEngine.InputSystem;

namespace IA
{
    public class InputGetter : MonoBehaviour
    {
        #region �C���X�^���X�̊Ǘ��A�R�[���o�b�N�Ƃ̃����N
        IA _inputs;

        void Awake()
        {
            _inputs = new IA();

            Link(true);
        }

        void OnDestroy()
        {
            Link(false);

            _inputs.Dispose();
        }

        void OnEnable()
        {
            _inputs.Enable();
        }

        void OnDisable()
        {
            _inputs.Disable();
        }
        #endregion

        #region �ϐ��錾
        public bool IsPush { get; set; } = false;
        public bool IsCounter { get; set; } = false;
        public bool IsSkill { get; set; } = false;
        public bool IsSpecial { get; set; } = false;
        public Vector2 ValueRotate { get; set; } = Vector2.zero;
        public bool IsSelect { get; set; } = false;
        public bool IsCancel { get; set; } = false;
        public bool IsQuit { get; set; } = false;
        #endregion

        #region�yLateUpdate�z���t���[���̍Ō�ŁA�t���O������������
        void LateUpdate()
        {
            if (IsPush) IsPush = false;
            if (IsCounter) IsCounter = false;
            if (IsSkill) IsSkill = false;
            if (IsSpecial) IsSpecial = false;
            if (IsSelect) IsSelect = false;
            if (IsCancel) IsCancel = false;
            if (IsQuit) IsQuit = false;
        }
        #endregion

        #region �R�[���o�b�N�Ƃ̃����N
        void Link(bool isLink)
        {
            // �C���X�^���X��.Map��.Action��.�R�[���o�b�N��
            if (isLink)
            {
                _inputs.MainGame.Push.performed += OnPush;

                _inputs.MainGame.Counter.performed += OnCounter;

                _inputs.MainGame.Skill.performed += OnSkill;

                _inputs.MainGame.Special.performed += OnSpecial;

                _inputs.SubGame.Rotate.started += ReadRotate;
                _inputs.SubGame.Rotate.performed += ReadRotate;
                _inputs.SubGame.Rotate.canceled += ReadRotate;

                _inputs.System.Select.performed += OnSelect;

                _inputs.System.Cancel.performed += OnCancel;
                
                _inputs.System.Quit.performed += OnQuit;
            }
            else
            {
                _inputs.MainGame.Push.performed -= OnPush;

                _inputs.MainGame.Counter.performed -= OnCounter;

                _inputs.MainGame.Skill.performed -= OnSkill;

                _inputs.MainGame.Special.performed -= OnSpecial;

                _inputs.SubGame.Rotate.started -= ReadRotate;
                _inputs.SubGame.Rotate.performed -= ReadRotate;
                _inputs.SubGame.Rotate.canceled -= ReadRotate;

                _inputs.System.Select.performed -= OnSelect;

                _inputs.System.Cancel.performed -= OnCancel;

                _inputs.System.Quit.performed -= OnQuit;
            }
        }
        #endregion

        #region �����̏ڍ�
        void OnPush(InputAction.CallbackContext context)
        {
            IsPush = true;
        }

        void OnCounter(InputAction.CallbackContext context)
        {
            IsCounter = true;
        }

        void OnSkill(InputAction.CallbackContext context)
        {
            IsSkill = true;   
        }

        void OnSpecial(InputAction.CallbackContext context)
        {
            IsSpecial = true;   
        }

        void ReadRotate(InputAction.CallbackContext context)
        {
            ValueRotate = context.ReadValue<Vector2>();
        }

        void OnSelect(InputAction.CallbackContext context)
        {
            IsSelect = true;
        }

        void OnCancel(InputAction.CallbackContext context)
        {
            IsCancel = true;
        }

        void OnQuit(InputAction.CallbackContext context)
        {
            IsQuit = true;
        }
        #endregion
    }
}