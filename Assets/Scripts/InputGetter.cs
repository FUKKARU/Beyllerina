using UnityEngine;
using UnityEngine.InputSystem;

namespace IA
{
    public class InputGetter : MonoBehaviour
    {
        #region インスタンスの管理、コールバックとのリンク、staticかつシングルトンにする
        IA _inputs;

        public static InputGetter Instance { get; set; } = null;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

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

        #region 変数宣言
        public bool IsPush { get; private set; } = false;
        public bool IsCounter { get; private set; } = false;
        public bool IsSkill { get; private set; } = false;
        public bool IsSpecial { get; private set; } = false;
        public Vector2 ValueDirection { get; private set; } = Vector2.zero;
        public bool IsSelect { get; private set; } = false;
        public bool IsCancel { get; private set; } = false;
        public bool IsQuit { get; private set; } = false;
        #endregion

        #region【LateUpdate】毎フレームの最後で、フラグを初期化する
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

        #region コールバックとのリンクの詳細
        void Link(bool isLink)
        {
            // インスタンス名.Map名.Action名.コールバック名
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

        #region 処理の詳細
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
            ValueDirection = context.ReadValue<Vector2>();
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