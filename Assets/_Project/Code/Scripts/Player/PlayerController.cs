using Cinemachine;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [Header("Camera Config")]
  [SerializeField] private float sensitivity = 50f;

  [Header("Movement Config")]
  [SerializeField] private float moveSpeed = 4500.0f;
  [SerializeField] private float walkSpeed = 20.0f;
  [SerializeField] private float runSpeed = 10.0f;

  [Header("Jumping Config")]
  [SerializeField] private float jumpForce = 550f;
  [SerializeField] private float jumpCooldown = 0.25f;

  [Header("Sliding Config")]
  [SerializeField] private float slideSlowdown = 0.2f;

  [Space(15)]

  public LayerMask whatIsGround;
  public LayerMask whatIsWallrunnable;

  [Header("References")]
  [SerializeField] private InputManager input;
  [SerializeField] private Rigidbody _rigidbody;
  [SerializeField] private Collider _collider;
  [SerializeField] private Transform orientation;
  [SerializeField] private Transform cinemachineCam;

  // Camera variables
  private float sensMultiplier = 1f;
  private float deviceMultiplier;
  private float desiredX;
  private float xRotation;

  // Moving variables
  private float x, y;
  private float vel;
  private bool isSprinting = false;
  private bool isSurfing = false;
  private bool cancellingSurf;
  float maxSlopeAngle = 35f;

  // Sliding variables
  private bool isCrouching = false;

  // Jumping variables
  private bool isJumping = false;
  private bool readyToJump;
  private Vector3 normalVector;

  // Wall running variables
  private float wallRunGravity = 1f;
  private Vector3 wallNormalVector;
  private bool isWallrunning = false;
  private bool readyToWallrun = true;
  private bool isCancelling;
  private float actualWallRotation;
  private float wallRunRotation;
  private float wallRotationVel;
  private bool onWall;
  private bool cancellingWall;

  // Grounded variables
  private bool isGrounded;
  private bool cancellingGrounded;

  private void OnEnable()
  {
    input.Jump += OnJump;
    input.Slide += OnSlide;

    input.Look += OnLook;
  }

  private void OnDisable()
  {
    input.Jump -= OnJump;
    input.Slide -= OnSlide;

    input.Look -= OnLook;
  }

  private void Awake()
  {
    _rigidbody = GetComponent<Rigidbody>();
    _collider = GetComponent<Collider>();

    cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>().transform;
  }

  private void Start()
  {
    input.EnablePlayerActions();

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    readyToJump = true;
    wallNormalVector = Vector3.up;
  }

  private void LateUpdate()
  {
    if(isWallrunning) HandleWallRunning();
  }

  private void FixedUpdate()
  {
    HandleMovement();
  }

  private void Update()
  {
    HandleLook();
  }

  private void HandleMovement()
  {
    x = input.Direction.x;
    y = input.Direction.y;

    _rigidbody.AddForce(Vector3.down * Time.deltaTime * 10f);
    Vector2 mag = FindVelRelativeToLook();
    float num = mag.x;
    float num2 = mag.y;
    CounterMovement(x, y, mag);
    float num3 = walkSpeed;
    if (isSprinting)
    {
      num3 = runSpeed;
    }
    if (isCrouching && isGrounded && readyToJump)
    {
      _rigidbody.AddForce(Vector3.down * Time.deltaTime * 3000f);
      return;
    }
    if (x > 0f && num > num3)
    {
      x = 0f;
    }
    if (x < 0f && num < 0f - num3)
    {
      x = 0f;
    }
    if (y > 0f && num2 > num3)
    {
      y = 0f;
    }
    if (y < 0f && num2 < 0f - num3)
    {
      y = 0f;
    }
    float num4 = 1f;
    float num5 = 1f;
    if (!isGrounded)
    {
      num4 = 0.5f;
      num5 = 0.5f;
    }
    if (isGrounded && isCrouching)
    {
      num5 = 0f;
    }
    if (isWallrunning)
    {
      num5 = 0.3f;
      num4 = 0.3f;
    }
    if (isSurfing)
    {
      num4 = 0.7f;
      num5 = 0.3f;
    }
    _rigidbody.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * num4 * num5);
    _rigidbody.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * num4);
  }
  private Vector2 FindVelRelativeToLook()
  {
    float current = orientation.transform.eulerAngles.y;
    float target = Mathf.Atan2(_rigidbody.velocity.x, _rigidbody.velocity.z) * 57.29578f;
    float num = Mathf.DeltaAngle(current, target);
    float num2 = 90f - num;
    float magnitude = _rigidbody.velocity.magnitude;
    return new Vector2(y: magnitude * Mathf.Cos(num * ((float)Math.PI / 180f)), x: magnitude * Mathf.Cos(num2 * ((float)Math.PI / 180f)));
  }
  private void CounterMovement(float x, float y, Vector2 mag)
  {
    if (!isGrounded || isJumping)
    {
      return;
    }
    float num = 0.16f;
    float num2 = 0.01f;
    if (isCrouching)
    {
      _rigidbody.AddForce(moveSpeed * Time.deltaTime * -_rigidbody.velocity.normalized * slideSlowdown);
      return;
    }
    if ((Math.Abs(mag.x) > num2 && Math.Abs(x) < 0.05f) || (mag.x < 0f - num2 && x > 0f) || (mag.x > num2 && x < 0f))
    {
      _rigidbody.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * (0f - mag.x) * num);
    }
    if ((Math.Abs(mag.y) > num2 && Math.Abs(y) < 0.05f) || (mag.y < 0f - num2 && y > 0f) || (mag.y > num2 && y < 0f))
    {
      _rigidbody.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * (0f - mag.y) * num);
    }
    if (Mathf.Sqrt(Mathf.Pow(_rigidbody.velocity.x, 2f) + Mathf.Pow(_rigidbody.velocity.z, 2f)) > walkSpeed)
    {
      float num3 = _rigidbody.velocity.y;
      Vector3 vector = _rigidbody.velocity.normalized * walkSpeed;
      _rigidbody.velocity = new Vector3(vector.x, num3, vector.z);
    }
  }

  private void HandleWallRunning()
  {
      _rigidbody.AddForce(-wallNormalVector * Time.deltaTime * moveSpeed);
      _rigidbody.AddForce(Vector3.up * Time.deltaTime * _rigidbody.mass * 100f * wallRunGravity);
  }
  private void CancelWallrun()
  {
    MonoBehaviour.print("cancelled");
    Invoke("GetReadyToWallrun", 0.1f);
    _rigidbody.AddForce(wallNormalVector * 600f);
    readyToWallrun = false;
  }
  private void GetReadyToWallrun()
  {
    readyToWallrun = true;
  }

  private void FindWallRunRotation()
  {
    if (!isWallrunning)
    {
      wallRunRotation = 0f;
      return;
    }
    _ = new Vector3(0f, cinemachineCam.transform.rotation.y, 0f).normalized;
    new Vector3(0f, 0f, 1f);
    float num = 0f;
    float current = cinemachineCam.transform.rotation.eulerAngles.y;
    if (Math.Abs(wallNormalVector.x - 1f) < 0.1f)
    {
      num = 90f;
    }
    else if (Math.Abs(wallNormalVector.x - -1f) < 0.1f)
    {
      num = 270f;
    }
    else if (Math.Abs(wallNormalVector.z - 1f) < 0.1f)
    {
      num = 0f;
    }
    else if (Math.Abs(wallNormalVector.z - -1f) < 0.1f)
    {
      num = 180f;
    }
    num = Vector3.SignedAngle(new Vector3(0f, 0f, 1f), wallNormalVector, Vector3.up);
    float num2 = Mathf.DeltaAngle(current, num);
    wallRunRotation = (0f - num2 / 90f) * 15f;
    if (!readyToWallrun)
    {
      return;
    }
    if ((Mathf.Abs(wallRunRotation) < 4f && y > 0f && Math.Abs(x) < 0.1f) || (Mathf.Abs(wallRunRotation) > 22f && y < 0f && Math.Abs(x) < 0.1f))
    {
      if (!isCancelling)
      {
        isCancelling = true;
        CancelInvoke("CancelWallrun");
        Invoke("CancelWallrun", 0.2f);
      }
    }
    else
    {
      isCancelling = false;
      CancelInvoke("CancelWallrun");
    }
  }

  private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
  {
    // If device is mouse use fixedDeltaTime, otherwise use deltaTime
    deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime * 25;
  }
  private void HandleLook()
  {
    Vector3 cameraMovement = new Vector3(input.Mouse.x, input.Mouse.y, 0);
    cameraMovement *= sensitivity * deviceMultiplier * sensMultiplier;

    desiredX = cinemachineCam.transform.localRotation.eulerAngles.y + cameraMovement.x;
    xRotation -= cameraMovement.y;
    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    FindWallRunRotation();
    actualWallRotation = Mathf.SmoothDamp(actualWallRotation, wallRunRotation, ref wallRotationVel, 0.2f);
    cinemachineCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, actualWallRotation);
    orientation.transform.localRotation = Quaternion.Euler(0f, desiredX, 0f);
  }

  private void ResetJump()
  {
    readyToJump = true;
    isJumping = false;
  }
  private void OnJump()
  {
    if ((isGrounded || isWallrunning || isSurfing) && readyToJump)
    {
      isJumping = true;

      Vector3 velocity = _rigidbody.velocity;
      readyToJump = false;
      _rigidbody.AddForce(Vector2.up * jumpForce * 1.5f);
      _rigidbody.AddForce(normalVector * jumpForce * 0.5f);
      if (_rigidbody.velocity.y < 0.5f)
      {
        _rigidbody.velocity = new Vector3(velocity.x, 0f, velocity.z);
      }
      else if (_rigidbody.velocity.y > 0f)
      {
        _rigidbody.velocity = new Vector3(velocity.x, velocity.y / 2f, velocity.z);
      }
      if (isWallrunning)
      {
        _rigidbody.AddForce(wallNormalVector * jumpForce * 3f);
      }
      Invoke("ResetJump", jumpCooldown);
      if (isWallrunning)
      {
        isWallrunning = false;
      }
    }
  }

  private void OnSlide(bool isSliding)
  {
    if (isSliding)
    {
      isCrouching = true;

      float num = 400f;
      transform.localScale = new Vector3(1f, 0.5f, 1f);
      transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
      if (_rigidbody.velocity.magnitude > 0.1f && isGrounded)
      {
        _rigidbody.AddForce(orientation.transform.forward * num);
      }
    } else
    {
      isCrouching = false;

      transform.localScale = new Vector3(1f, 1.5f, 1f);
      transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
  }
  private bool IsFloor(Vector3 v)
  {
    return Vector3.Angle(Vector3.up, v) < maxSlopeAngle;
  }
  private bool IsSurf(Vector3 v)
  {
    float num = Vector3.Angle(Vector3.up, v);
    if (num < 89f)
    {
      return num > maxSlopeAngle;
    }
    return false;
  }
  private bool IsWall(Vector3 v)
  {
    return Math.Abs(90f - Vector3.Angle(Vector3.up, v)) < 0.1f;
  }
  private bool IsRoof(Vector3 v)
  {
    return v.y == -1f;
  }
  private void StartWallRun(Vector3 normal)
  {
    if (!isGrounded && readyToWallrun)
    {
      wallNormalVector = normal;
      float num = 20f;
      if (!isWallrunning)
      {
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        _rigidbody.AddForce(Vector3.up * num, ForceMode.Impulse);
      }
      isWallrunning = true;
    }
  }

  private void OnCollisionStay(Collision other)
  {
    int layer = other.gameObject.layer;
    if ((int)whatIsGround != ((int)whatIsGround | (1 << layer)))
    {
      return;
    }
    for (int i = 0; i < other.contactCount; i++)
    {
      Vector3 normal = other.contacts[i].normal;
      if (IsFloor(normal))
      {
        if (isWallrunning)
        {
          isWallrunning = false;
        }
        isGrounded = true;
        normalVector = normal;
        cancellingGrounded = false;
        CancelInvoke("StopGrounded");
      }
      if (IsWall(normal) && layer == LayerMask.NameToLayer("Ground"))
      {
        StartWallRun(normal);
        onWall = true;
        cancellingWall = false;
        CancelInvoke("StopWall");
      }
      if (IsSurf(normal))
      {
        isSurfing = true;
        cancellingSurf = false;
        CancelInvoke("StopSurf");
      }
      IsRoof(normal);
    }
    float num = 3f;
    if (!cancellingGrounded)
    {
      cancellingGrounded = true;
      Invoke("StopGrounded", Time.deltaTime * num);
    }
    if (!cancellingWall)
    {
      cancellingWall = true;
      Invoke("StopWall", Time.deltaTime * num);
    }
    if (!cancellingSurf)
    {
      cancellingSurf = true;
      Invoke("StopSurf", Time.deltaTime * num);
    }
  }

  private void StopGrounded()
  {
    isGrounded = false;
  }

  private void StopWall()
  {
    onWall = false;
    isWallrunning = false;
  }

  private void StopSurf()
  {
    isSurfing = false;
  }
}
