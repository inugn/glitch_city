using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Core.Input
{
    public sealed class CoreInputReader : MonoBehaviour, ICoreInputReader
    {
        [Header("Actions (InputActionReference)")]
        [SerializeField] private InputActionReference move;          // A/D or Left Stick
        [SerializeField] private InputActionReference jump;          // Space / A
        [SerializeField] private InputActionReference dash;          // Shift / B
        [SerializeField] private InputActionReference attack;        // LMB / RT
        [SerializeField] private InputActionReference reload;        // R / X
        [SerializeField] private InputActionReference switchWeapon;  // T / RB
        [SerializeField] private InputActionReference interact;      // E / Y
        [SerializeField] private InputActionReference pause;         // Esc / Start

        public float MoveX { get; private set; }
        public bool AttackHeld { get; private set; }

        public event System.Action JumpPressed;
        public event System.Action DashPressed;
        public event System.Action ReloadPressed;
        public event System.Action SwitchWeaponPressed;
        public event System.Action InteractPressed;
        public event System.Action PausePressed;

        private void OnEnable()
        {
            EnableAction(move);
            EnableAction(jump);
            EnableAction(dash);
            EnableAction(attack);
            EnableAction(reload);
            EnableAction(switchWeapon);
            EnableAction(interact);
            EnableAction(pause);

            if (jump != null) jump.action.performed += OnJump;
            if (dash != null) dash.action.performed += OnDash;
            if (reload != null) reload.action.performed += OnReload;
            if (switchWeapon != null) switchWeapon.action.performed += OnSwitchWeapon;
            if (interact != null) interact.action.performed += OnInteract;
            if (pause != null) pause.action.performed += OnPause;

            if (attack != null)
            {
                attack.action.performed += OnAttackPerformed;
                attack.action.canceled  += OnAttackCanceled;
            }
        }

        private void OnDisable()
        {
            if (jump != null) jump.action.performed -= OnJump;
            if (dash != null) dash.action.performed -= OnDash;
            if (reload != null) reload.action.performed -= OnReload;
            if (switchWeapon != null) switchWeapon.action.performed -= OnSwitchWeapon;
            if (interact != null) interact.action.performed -= OnInteract;
            if (pause != null) pause.action.performed -= OnPause;

            if (attack != null)
            {
                attack.action.performed -= OnAttackPerformed;
                attack.action.canceled  -= OnAttackCanceled;
            }
        }

        private void Update()
        {
            // Move может быть float (Axis) или Vector2 (Stick/2D Vector).
            MoveX = ReadMoveX(move);
        }

        private static void EnableAction(InputActionReference r)
        {
            if (r == null) return;
            if (!r.action.enabled) r.action.Enable();
        }

        private static float ReadMoveX(InputActionReference r)
        {
            if (r == null) return 0f;

            var a = r.action;
            if (a == null) return 0f;

            // Если action не включен — ReadValue может вести себя странно
            if (!a.enabled) return 0f;

            // Универсально: сначала пытаемся считать как Vector2, потом как float
            try
            {
                // Частый случай: Move = 2D Vector (WASD/Stick)
                var v = a.ReadValue<Vector2>();
                // если реально был float, v будет (0,0) -> тогда попробуем float ниже
                if (v != Vector2.zero)
                    return v.x;
            }
            catch
            {
                // игнорируем и пробуем float
            }

            try
            {
                // Второй частый случай: Move = Axis (A/D)
                return a.ReadValue<float>();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CoreInputReader] Move action has incompatible value type. Action='{a.name}', expected float or Vector2. Error: {e.Message}");
                return 0f;
            }
        }


        private void OnJump(InputAction.CallbackContext _) => JumpPressed?.Invoke();
        private void OnDash(InputAction.CallbackContext _) => DashPressed?.Invoke();
        private void OnReload(InputAction.CallbackContext _) => ReloadPressed?.Invoke();
        private void OnSwitchWeapon(InputAction.CallbackContext _) => SwitchWeaponPressed?.Invoke();
        private void OnInteract(InputAction.CallbackContext _) => InteractPressed?.Invoke();
        private void OnPause(InputAction.CallbackContext _) => PausePressed?.Invoke();

        private void OnAttackPerformed(InputAction.CallbackContext _) => AttackHeld = true;
        private void OnAttackCanceled (InputAction.CallbackContext _) => AttackHeld = false;
    }
}
