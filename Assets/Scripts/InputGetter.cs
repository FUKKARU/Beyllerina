using UnityEngine;
using UnityEngine.InputSystem;

namespace IA
{
    public class InputGetter : MonoBehaviour
    {
        #region インスタンスの管理、コールバックとのリンク、staticかつシングルトン化
        IA _inputs;

        public static InputGetter Instance { get; set; } = null;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
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

        public bool IsPush { get; set; } = false;
        public bool IsCounter { get; set; } = false;
        public bool IsSkill { get; set; } = false;
        public bool IsSpecial { get; set; } = false;
        public Vector2 ValueRotate { get; set; } = Vector2.zero;
        public bool IsSelect { get; set; } = false;
        public bool IsCancel { get; set; } = false;
        public bool IsQuit { get; set; } = false;

        void Update()
        {
            if (BaseSystem.PlayerSO.Entity.Dbg.IsShowNormalLog)
            {
                Debug.Log($"<color=#64ff64>IsPush:{IsPush}</color>");
                Debug.Log($"<color=#64ff64>IsCounter:{IsCounter}</color>");
                Debug.Log($"<color=#64ff64>IsSkill:{IsSkill}</color>");
                Debug.Log($"<color=#64ff64>IsSpecial:{IsSpecial}</color>");
                Debug.Log($"<color=#64ff64>ValueRotate:{ValueRotate}</color>");
                Debug.Log($"<color=#64ff64>IsSelect:{IsSelect}</color>");
                Debug.Log($"<color=#64ff64>IsCancel:{IsCancel}</color>");
                Debug.Log($"<color=#64ff64>IsQuit:{IsQuit}</color>");
            }
        }

        void Link(bool isLink)
        {
            // _inputs.Map名.Action名.コールバック
            if (isLink)
            {
                _inputs.MainGame.Push.performed += OnPush;
                _inputs.MainGame.Push.canceled += _OnPush;

                _inputs.MainGame.Counter.performed += OnCounter;
                _inputs.MainGame.Counter.canceled += _OnCounter;

                _inputs.MainGame.Skill.performed += OnSkill;
                _inputs.MainGame.Skill.canceled += _OnSkill;

                _inputs.MainGame.Special.performed += OnSpecial;
                _inputs.MainGame.Special.canceled += _OnSpecial;

                _inputs.SubGame.Rotate.started += ReadRotate;
                _inputs.SubGame.Rotate.performed += ReadRotate;
                _inputs.SubGame.Rotate.canceled += ReadRotate;

                _inputs.System.Select.performed += OnSelect;
                _inputs.System.Select.canceled += _OnSelect;

                _inputs.System.Cancel.performed += OnCancel;
                _inputs.System.Cancel.canceled += _OnCancel;
                
                _inputs.System.Quit.performed += OnQuit;
                _inputs.System.Quit.canceled += _OnQuit;
            }
            else
            {
                _inputs.MainGame.Push.performed -= OnPush;
                _inputs.MainGame.Push.canceled -= _OnPush;

                _inputs.MainGame.Counter.performed -= OnCounter;
                _inputs.MainGame.Counter.canceled -= _OnCounter;

                _inputs.MainGame.Skill.performed -= OnSkill;
                _inputs.MainGame.Skill.canceled -= _OnSkill;

                _inputs.MainGame.Special.performed -= OnSpecial;
                _inputs.MainGame.Special.canceled -= _OnSpecial;

                _inputs.SubGame.Rotate.started -= ReadRotate;
                _inputs.SubGame.Rotate.performed -= ReadRotate;
                _inputs.SubGame.Rotate.canceled -= ReadRotate;

                _inputs.System.Select.performed -= OnSelect;
                _inputs.System.Select.canceled -= _OnSelect;

                _inputs.System.Cancel.performed -= OnCancel;
                _inputs.System.Cancel.canceled -= _OnCancel;

                _inputs.System.Quit.performed -= OnQuit;
                _inputs.System.Quit.canceled -= _OnQuit;
            }
        }

        void OnPush(InputAction.CallbackContext context)
        {
            IsPush = true;
        }
        void _OnPush(InputAction.CallbackContext context)
        {
            IsPush = false;
        }

        void OnCounter(InputAction.CallbackContext context)
        {
            IsCounter = true;
        }
        void _OnCounter(InputAction.CallbackContext context)
        {
            IsCounter = false;
        }

        void OnSkill(InputAction.CallbackContext context)
        {
            IsSkill = true;
        }
        void _OnSkill(InputAction.CallbackContext context)
        {
            IsSkill = false;
        }

        void OnSpecial(InputAction.CallbackContext context)
        {
            IsSpecial = true;
        }
        void _OnSpecial(InputAction.CallbackContext context)
        {
            IsSpecial = false;
        }

        void ReadRotate(InputAction.CallbackContext context)
        {
            ValueRotate = context.ReadValue<Vector2>();
        }

        void OnSelect(InputAction.CallbackContext context)
        {
            IsSelect = true;
        }
        void _OnSelect(InputAction.CallbackContext context)
        {
            IsSelect = false;
        }

        void OnCancel(InputAction.CallbackContext context)
        {
            IsCancel = true;
        }
        void _OnCancel(InputAction.CallbackContext context)
        {
            IsCancel = false;
        }

        void OnQuit(InputAction.CallbackContext context)
        {
            IsQuit = true;
        }
        void _OnQuit(InputAction.CallbackContext context)
        {
            IsQuit = false;
        }
    }
}