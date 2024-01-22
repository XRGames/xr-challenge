using Cinemachine;
using System;
using UnityEngine;

namespace DanHenshaw
{
  public class PlayerController : MonoBehaviour
  {
    [Header("Movement Config")]
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private float rotationSpeed = 15.0f;
    [SerializeField] private float smoothTime = 0.2f;

    [Header("References")]
    [SerializeField] private InputManager input;
    [SerializeField] private CharacterController controller;
    [SerializeField] private CinemachineFreeLook freeLookCam;
    //[SerializeField] private Animator animator;

    Transform mainCam;

    float currentSpeed;
    float velocity;

    private void Awake()
    {
      controller = GetComponent<CharacterController>();
      freeLookCam = FindObjectOfType<CinemachineFreeLook>();

      mainCam = Camera.main.transform;

      freeLookCam.Follow = transform;
      freeLookCam.LookAt = transform;
      // When FreeLook Camera target is teleported, adjust cameras position accordingly.
      freeLookCam.OnTargetObjectWarped(
        transform, 
        transform.position - freeLookCam.transform.position - Vector3.forward
      );
    }

    private void Update()
    {
      HandleMovement();
      // UpdateAnimator();
    }

    private void HandleMovement()
    {
      Vector3 moveDir = new Vector3(input.Direction.x, 0.0f, input.Direction.y).normalized;
      // Rotate move direction to match camera rotation
      Vector3 adjustedDir = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * moveDir;

      if(adjustedDir.magnitude > 0.0f)
      {
        HandleRotation(adjustedDir);

        HandleController(adjustedDir);

        SmoothSpeed(adjustedDir.magnitude);
      }
      else
      {
        SmoothSpeed(0.0f);
      }
    }

    private void HandleRotation(Vector3 adjustedDir)
    {
      // Adjust rotation to match movement direction
      Quaternion targetRotation = Quaternion.LookRotation(adjustedDir);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleController(Vector3 adjustedDir)
    {
      // Move the player
      Vector3 adjustedMovement = adjustedDir * (moveSpeed * Time.deltaTime);
      controller.Move(adjustedMovement);
    }

    private void SmoothSpeed(float value)
    {
      currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
  }
}