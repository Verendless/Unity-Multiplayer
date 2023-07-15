using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Input.Controls;

namespace Input
{
    [CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event Action<Vector2> MovementEvent;
        public event Action<bool> PrimaryFireEvent;

        private Controls controls;

        private void OnEnable()
        {
            if( controls == null )
            {
                controls = new Controls();
                controls.Player.SetCallbacks(this);
            }

            controls.Player.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPrimaryFire(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryFireEvent?.Invoke(true);
            else
                PrimaryFireEvent?.Invoke(false);
        }
    }
}

