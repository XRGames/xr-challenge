using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

namespace DanHenshaw
{
  public class CameraManager : MonoBehaviour
  {
    [Header("Config")]
    [SerializeField, Range(0.5f, 10.0f)] float speedMultiplier = 1.0f;

    [Header("References")]
    [SerializeField] private InputManager input;
    [SerializeField] private CinemachineFreeLook freeLookCam;

    bool isRMBPressed;
    bool cameraMovementLock;

    private void Awake()
    {
      freeLookCam = GetComponentInChildren<CinemachineFreeLook>();
    }

    private void OnEnable()
    {
      input.Look += OnLook;
      input.EnableMouseControlCamera += OnEnableMouseControlCamera;
      input.DisableMouseControlCamera += OnDisableMouseControlCamera;
    }

    private void OnDisable()
    {
      input.Look -= OnLook;
      input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
      input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
    }

    private void OnLook(Vector2 cameraMove, bool isDeviceMouse)
    {
      if (cameraMovementLock) return;
      if (isDeviceMouse && !isRMBPressed) return;

      // If device is mouse use fixedDeltaTime, otherwise use deltaTime
      float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

      // Set the camera axis values
      freeLookCam.m_XAxis.m_InputAxisValue = cameraMove.x * speedMultiplier * deviceMultiplier;
      freeLookCam.m_YAxis.m_InputAxisValue = cameraMove.y * speedMultiplier * deviceMultiplier;
    }

    private void OnEnableMouseControlCamera()
    {
      isRMBPressed = true;

      // Lock the cursor to center of screen & hide it
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;

      StartCoroutine(DisableMouseForFrame());
    }

    private IEnumerator DisableMouseForFrame()
    {
      cameraMovementLock = true;
      yield return new WaitForEndOfFrame();
      cameraMovementLock = false;
    }

    private void OnDisableMouseControlCamera()
    {
      isRMBPressed = false;

      // Unlock the cursor and unhide it
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;

      // Reset camera axis to prevent jumping when re-enabling mouse control
      freeLookCam.m_XAxis.m_InputAxisValue = 0.0f;
      freeLookCam.m_YAxis.m_InputAxisValue = 0.0f;
    }
  }
}
