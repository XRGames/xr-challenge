using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "InputManager", menuName = "Custom/Input/InputManager")]
public class InputManager : ScriptableObject, IPlayerActions
{
  public event UnityAction<Vector2> Move = delegate { };
  public event UnityAction Jump = delegate { };
  public event UnityAction<bool> Slide = delegate { };
  public event UnityAction<Vector2, bool> Look = delegate { };
  public event UnityAction<bool> LeftClick = delegate { };
  public event UnityAction<bool> RightClick = delegate { };

  private Controls controls;

  public Vector2 Direction => controls.Player.Move.ReadValue<Vector2>();
  public Vector2 Mouse => controls.Player.Look.ReadValue<Vector2>();

  private void OnEnable()
  {
    if (controls == null)
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

  public void OnJump(InputAction.CallbackContext context)
  {
    switch (context.phase) 
    {
      case InputActionPhase.Started:
        Jump.Invoke();
        break;
    }
  }

  public void OnSlide(InputAction.CallbackContext context)
  {
    switch (context.phase) 
    {
      case InputActionPhase.Started:
        Slide.Invoke(true); 
        break;
      case InputActionPhase.Canceled: 
        Slide.Invoke(false); 
        break;
    }
  }

  public void OnLook(InputAction.CallbackContext context)
  {
    Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
  }
  private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

  public void OnLeftClick(InputAction.CallbackContext context)
  {
    switch (context.phase)
    {
      case InputActionPhase.Started:
        LeftClick.Invoke(true);
        break;
      case InputActionPhase.Canceled:
        LeftClick.Invoke(false);
        break;
    }
  }

  public void OnRightClick(InputAction.CallbackContext context)
  {
    switch (context.phase)
    {
      case InputActionPhase.Started:
        RightClick.Invoke(true);
        break;
      case InputActionPhase.Canceled:
        RightClick.Invoke(false);
        break;
    }
  }
}
