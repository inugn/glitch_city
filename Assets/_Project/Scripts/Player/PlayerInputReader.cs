using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [Header("Assign InputActionReference in Inspector")]
        [SerializeField] private InputActionReference move; // Vector2 or 1D axis
        [SerializeField] private InputActionReference jump; // Button
        [SerializeField] private InputActionReference dash; // Button

        public float MoveX { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool DashPressed { get; private set; }

        private void OnEnable()
        {
            move?.action.Enable();
            jump?.action.Enable();
            dash?.action.Enable();

            if (jump != null) jump.action.performed += OnJump;
            if (dash != null) dash.action.performed += OnDash;
        }

        private void OnDisable()
        {
            if (jump != null) jump.action.performed -= OnJump;
            if (dash != null) dash.action.performed -= OnDash;

            move?.action.Disable();
            jump?.action.Disable();
            dash?.action.Disable();
        }

        private void Update()
        {
            // Move может быть Vector2 (stick) или float (A/D). Берём X.
            if (move != null)
            {
                var v = move.action.ReadValue<Vector2>();
                MoveX = v.x;
            }
            else
            {
                MoveX = 0f;
            }
        }

        private void LateUpdate()
        {
            // “одно нажатие” на кадр
            JumpPressed = false;
            DashPressed = false;
        }

        private void OnJump(InputAction.CallbackContext _) => JumpPressed = true;
        private void OnDash(InputAction.CallbackContext _) => DashPressed = true;
    }
}