using System;
using UnityEngine;

namespace Assets.Scripts.NewCode.Singletons
{
    public class InputSingleton : Singleton<InputSingleton>
    {
        private const KeyCode jumpKey = KeyCode.Space;
        private const KeyCode crouchKey = KeyCode.LeftControl;
        private const KeyCode escapeKey = KeyCode.Escape;
        private const KeyCode inventoryKey = KeyCode.E;

        public Action OnJumpKeyDown { get; set; }
        public Action OnJumpKeyUp { get; set; }
        public Action OnCrouchKeyPressed { get; set; }
        public Action OnLMBDown { get; set; }
        public Action OnEscapeKeyPressed { get; set; }
        public Action OnInventoryOpenKeyPressed { get; set; }

        public bool JumpKeyPressed { get; private set; }
        public float MouseXValue { get; private set; }
        public float MouseYValue { get; private set; }
        public Vector2 MouseLook { get => new Vector2(MouseXValue, MouseYValue); }
        public Vector2 MoveDirection { get; private set; }

        private void Update()
        {
            MouseXValue = Input.GetAxis("Mouse X");
            MouseYValue = Input.GetAxis("Mouse Y");

            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");
            MoveDirection = new Vector2(x, y);

            if(Input.GetKeyDown(inventoryKey))
            {
                OnInventoryOpenKeyPressed?.Invoke();
            }

            if(Input.GetKeyDown(escapeKey))
            {
                OnEscapeKeyPressed?.Invoke();
            }

            if(Input.GetMouseButtonDown(0))
            {
                OnLMBDown?.Invoke();
            }

            if (Input.GetKeyDown(jumpKey))
            {
                OnJumpKeyDown?.Invoke();
                JumpKeyPressed = true;
            }

            if (Input.GetKeyUp(jumpKey))
            {
                OnJumpKeyUp?.Invoke();
                JumpKeyPressed = false;
            }

            if (Input.GetKeyDown(crouchKey))
                OnCrouchKeyPressed?.Invoke();
        }
    }
}
