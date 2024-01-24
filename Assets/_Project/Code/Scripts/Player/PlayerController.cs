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
  [SerializeField] private float slideForce = 400f;
  [SerializeField] private float slideSlowdown = 0.2f;

  [Space(15)]

  public LayerMask whatIsGround;
  public LayerMask whatIsWallrunnable;

  [Header("References")]
  [SerializeField] private InputManager input;
  [SerializeField] private Rigidbody _rigidbody;
  [SerializeField] private Transform orientation;
  [SerializeField] private Transform cinemachineCam;

  // Getters & Setters
  public InputManager Input => input;

  // Camera variables
  private float sensMultiplier = 1f;
  private float deviceMultiplier;
  private float desiredX;
  private float xRotation;

  // Moving variables
  private float x, y;
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

  /// <summary>
  /// Handles player movement
  /// </summary>
  private void HandleMovement()
  {
    x = input.Direction.x;
    y = input.Direction.y;

    _rigidbody.AddForce(Vector3.down * Time.deltaTime * 10f);
    Vector2 mag = FindVelRelativeToLook();
    CounterMovement(x, y, mag);

    float speed = isSprinting ? runSpeed : walkSpeed;

    if (isCrouching && isGrounded && readyToJump)
    {
      _rigidbody.AddForce(Vector3.down * Time.deltaTime * 3000f);
      return;
    }
    if (x > 0f && mag.x > speed)
    {
      x = 0f;
    }
    if (x < 0f && mag.x < 0f - speed)
    {
      x = 0f;
    }
    if (y > 0f && mag.y > speed)
    {
      y = 0f;
    }
    if (y < 0f && mag.y < 0f - speed)
    {
      y = 0f;
    }

    float friction_y = 1f;
    float friction_x = 1f;
    if (!isGrounded)
    {
      friction_y = 0.5f;
      friction_x = 0.5f;
    }
    if (isGrounded && isCrouching)
    {
      friction_x = 0f;
    }
    if (isWallrunning)
    {
      friction_x = 0.3f;
      friction_y = 0.3f;
    }
    if (isSurfing)
    {
      friction_y = 0.7f;
      friction_x = 0.3f;
    }
    _rigidbody.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * friction_y * friction_x);
    _rigidbody.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * friction_y);
  }

  /// <summary>
  /// Calculates velocity relative to the look direction
  /// </summary>
  /// <returns> Vector2 - velocity relative to look direction </returns>
  private Vector2 FindVelRelativeToLook()
  {
    float current = orientation.transform.eulerAngles.y;
    float target = Mathf.Atan2(_rigidbody.velocity.x, _rigidbody.velocity.z) * 57.29578f;
    float distance = Mathf.DeltaAngle(current, target);
    float magnitude = _rigidbody.velocity.magnitude;
    return new Vector2(y: magnitude * Mathf.Cos(distance * ((float)Math.PI / 180f)), x: magnitude * Mathf.Cos((90 - distance) * ((float)Math.PI / 180f)));
  }

  /// <summary>
  /// Applies force to counter the movement of the player
  /// </summary>
  /// <param name="x"> float - Player input on the x axis </param>
  /// <param name="y"> float - Player input on the z axis </param>
  /// <param name="mag"> Vector2 - Current magnitude of the player rigidbody </param>
  private void CounterMovement(float x, float y, Vector2 mag)
  {
    if (!isGrounded || isJumping)
    {
      return;
    }
    float mag_multiplier = 0.16f;
    float min_mag = 0.01f;
    if (isCrouching)
    {
      _rigidbody.AddForce(moveSpeed * Time.deltaTime * -_rigidbody.velocity.normalized * slideSlowdown);
      return;
    }
    if ((Math.Abs(mag.x) > min_mag && Math.Abs(x) < 0.05f) || (mag.x < 0f - min_mag && x > 0f) || (mag.x > min_mag && x < 0f))
    {
      _rigidbody.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * (0f - mag.x) * mag_multiplier);
    }
    if ((Math.Abs(mag.y) > min_mag && Math.Abs(y) < 0.05f) || (mag.y < 0f - min_mag && y > 0f) || (mag.y > min_mag && y < 0f))
    {
      _rigidbody.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * (0f - mag.y) * mag_multiplier);
    }
    if (Mathf.Sqrt(Mathf.Pow(_rigidbody.velocity.x, 2f) + Mathf.Pow(_rigidbody.velocity.z, 2f)) > walkSpeed)
    {
      float y_vel = _rigidbody.velocity.y;
      Vector3 direction = _rigidbody.velocity.normalized * walkSpeed;
      _rigidbody.velocity = new Vector3(direction.x, y_vel, direction.z);
    }
  }

  /// <summary>
  /// Apply forces to the player allowing them to stick to the wall they're running on
  /// </summary>
  private void HandleWallRunning()
  {
      _rigidbody.AddForce(-wallNormalVector * Time.deltaTime * moveSpeed);
      _rigidbody.AddForce(Vector3.up * Time.deltaTime * _rigidbody.mass * 100f * wallRunGravity);
  }

  /// <summary>
  /// Stops the wall run and applies a force to the player in the direction of the walls normal
  /// </summary>
  private void CancelWallrun()
  {
    Invoke("GetReadyToWallrun", 0.1f);
    _rigidbody.AddForce(wallNormalVector * 600f);
    readyToWallrun = false;
  }

  private void GetReadyToWallrun()
  {
    readyToWallrun = true;
  }

  /// <summary>
  /// Calculates the rotation of the wall that the player is currently wallrunning
  /// </summary>
  private void FindWallRunRotation()
  {
    if (!isWallrunning)
    {
      wallRunRotation = 0f;
      return;
    }

    float current_cam_y = cinemachineCam.transform.rotation.eulerAngles.y;
    float signed_angle = Vector3.SignedAngle(new Vector3(0f, 0f, 1f), wallNormalVector, Vector3.up);
    float wall_distance = Mathf.DeltaAngle(current_cam_y, signed_angle);
    wallRunRotation = (0f - wall_distance / 90f) * 15f;
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

  /// <summary>
  /// Sets the device multiplier depending on whether the player is using a mouse or gamepad
  /// </summary>
  private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
  {
    // If device is mouse use fixedDeltaTime, otherwise use deltaTime
    deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime * 25;
  }

  /// <summary>
  /// Handles the players look direction
  /// </summary>
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

  /// <summary>
  /// When the player jumps apply force in an upwards direction
  /// </summary>
  private void OnJump()
  {
    if ((isGrounded || isWallrunning || isSurfing) && readyToJump)
    {
      isJumping = true;

      Vector3 velocity = _rigidbody.velocity;
      readyToJump = false;
      _rigidbody.AddForce(Vector3.up * jumpForce * 1.5f);
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

  /// <summary>
  /// When the player slides apply force in an forwards direction and shrink the player model/collisions
  /// </summary>
  private void OnSlide(bool isSliding)
  {
    if (isSliding)
    {
      isCrouching = true;

      transform.localScale = new Vector3(1f, 0.5f, 1f);
      transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
      if (_rigidbody.velocity.magnitude > 0.1f && isGrounded)
      {
        _rigidbody.AddForce(orientation.transform.forward * slideForce);
      }
    } else
    {
      isCrouching = false;

      transform.localScale = new Vector3(1f, 1.5f, 1f);
      transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
  }

  /// <summary>
  /// Detects if the normal param is a floor
  /// </summary>
  /// <param name="v"> Vector3 - Collision normal </param>
  /// <returns> bool - whether or not that normal is a floor </returns>
  private bool IsFloor(Vector3 v)
  {
    return Vector3.Angle(Vector3.up, v) < maxSlopeAngle;
  }

  /// <summary>
  /// Detects if the normal param is surfable
  /// </summary>
  /// <param name="v"> Vector3 - Collision normal </param>
  /// <returns> bool - whether or not that normal is surfable </returns>
  private bool IsSurf(Vector3 v)
  {
    float num = Vector3.Angle(Vector3.up, v);
    if (num < 89f)
    {
      return num > maxSlopeAngle;
    }
    return false;
  }

  /// <summary>
  /// Detects if the normal param is a wall
  /// </summary>
  /// <param name="v"> Vector3 - Collision normal </param>
  /// <returns> bool - whether or not that normal is a wall </returns>
  private bool IsWall(Vector3 v)
  {
    return Math.Abs(90f - Vector3.Angle(Vector3.up, v)) < 0.1f;
  }

  /// <summary>
  /// Detects if the normal param is a roof
  /// </summary>
  /// <param name="v"> Vector3 - Collision normal </param>
  /// <returns> bool - whether or not that normal is a roof </returns>
  private bool IsRoof(Vector3 v)
  {
    return v.y == -1f;
  }

  /// <summary>
  /// Starts a wallrun if the player is ready to wallrun
  /// </summary>
  /// <param name="normal"> Vector3 - wallrun normal </param>
  private void StartWallRun(Vector3 normal)
  {
    if (!isGrounded && readyToWallrun)
    {
      wallNormalVector = normal;
      float up_force = 20f;
      if (!isWallrunning)
      {
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        _rigidbody.AddForce(Vector3.up * up_force, ForceMode.Impulse);
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
    float stop_time = 3f;
    if (!cancellingGrounded)
    {
      cancellingGrounded = true;
      Invoke("StopGrounded", Time.deltaTime * stop_time);
    }
    if (!cancellingWall)
    {
      cancellingWall = true;
      Invoke("StopWall", Time.deltaTime * stop_time);
    }
    if (!cancellingSurf)
    {
      cancellingSurf = true;
      Invoke("StopSurf", Time.deltaTime * stop_time);
    }
  }

  private void StopGrounded()
  {
    isGrounded = false;
  }

  private void StopWall()
  {
    isWallrunning = false;
  }

  private void StopSurf()
  {
    isSurfing = false;
  }
}
