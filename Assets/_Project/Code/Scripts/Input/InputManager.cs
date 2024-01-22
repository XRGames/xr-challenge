using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Controls;

namespace DanHenshaw
{
  [CreateAssetMenu(fileName = "InputManager", menuName = "Custom/Input/InputManager")]
  public class InputManager : ScriptableObject, IPlayerActions
  {
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2, bool> Look = delegate { };
    public event UnityAction EnableMouseControlCamera = delegate { };
    public event UnityAction DisableMouseControlCamera = delegate { };

    private Controls controls;

    public Vector3 Direction => controls.Player.Move.ReadValue<Vector2>();

    private void OnEnable()
    {
      if(controls == null)
      {
        controls = new Controls();
        controls.Player.SetCallbacks(this);
      }
    }

    public void EnablePlayerActions()
    {
      controls.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
      Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
      // noop
    }

    public void OnJump(InputAction.CallbackContext context)
    {
      // noop
    }

    public void OnLook(InputAction.CallbackContext context)
    {
      Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }
    private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
      switch(context.phase)
      {
        case InputActionPhase.Started:
          EnableMouseControlCamera.Invoke(); 
          break;
        case InputActionPhase.Canceled: 
          DisableMouseControlCamera.Invoke(); 
          break;
      }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
      // noop
    }
  }
}
