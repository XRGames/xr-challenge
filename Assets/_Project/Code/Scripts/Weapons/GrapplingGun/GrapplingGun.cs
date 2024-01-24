using System;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
  [Header("Weapon Config")]
  [SerializeField] private float maxDistance;
  public Transform muzzle;

  [Header("Prediction")]
  [SerializeField] private RaycastHit predictionHit;
  [SerializeField] private float predictionSphereCastRadius;
  [SerializeField] private GameObject predictionPointObject;

  [Space(15)]

  public Grapple grapple;
  public Swing swing;

  [Space(15)]

  [SerializeField] private LayerMask whatIsGrappleable;

  [Header("References")]
  [SerializeField] private PlayerController playerController;
  [SerializeField] private InputManager input;
  [SerializeField] private Transform cam;

  public Vector3 grapplePoint { get; private set; }
  private Transform predictionPoint;

  private void Awake()
  {
    input = playerController.Input;

    playerController.grapplingGun = this;

    cam = Camera.main.transform;
  }

  private void OnEnable()
  {
    input.RightClick += HandleGrapple;
    input.LeftClick += HandleSwing;
  }

  private void OnDisable()
  {
    input.RightClick -= HandleGrapple;
    input.LeftClick += HandleSwing;
  }

  private void Start()
  {
    predictionPoint = Instantiate(predictionPointObject, transform.position, transform.rotation).transform;
    predictionPoint.gameObject.SetActive(false);
  }

  private void Update()
  {
    if(grapple.cooldownTimer > 0)
      grapple.cooldownTimer -= Time.deltaTime;

    CheckForSwingPoints();
  }

  /// <summary>
  /// Starts the grapple hook when input has been detected
  /// </summary>
  /// <param name="isDown"> bool - whether right click is pressed or released </param>
  private void HandleGrapple(bool isDown)
  {
    if(isDown)
    {
      StartGrapple();
    }
  }

  /// <summary>
  /// Starts and Stops the Swing when input has been detected
  /// </summary>
  /// <param name="isDown"> bool - whether right click is pressed or released </param>
  private void HandleSwing(bool isDown)
  {
    if(isDown && !swing.isSwinging)
    {
      StartSwinging();
    } else
    {
      StopSwinging();
    }
  }

  /// <summary>
  /// Starts the grapple - freeze the player in place and execute the next stage for the grapple
  /// </summary>
  private void StartGrapple()
  {
    if (grapple.cooldownTimer > 0) return;

    grapple.isGrappling = true;

    playerController.freeze = true;

    if (predictionHit.point == Vector3.zero)
    {
      grapplePoint = cam.position + cam.forward * maxDistance;
      Invoke(nameof(StopGrapple), grapple.delayTime);
    }
    else
    {
      grapplePoint = predictionHit.point;

      Invoke(nameof(ExecuteGrapple), grapple.delayTime);
    }
  }

  /// <summary>
  /// Executes the grapple - Launches the player towards the grapple point
  /// </summary>
  private void ExecuteGrapple()
  {
    playerController.freeze = false;

    Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

    float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
    float hightestPointOnArc = grapplePointRelativeYPos + grapple.overShootYAxis;

    if(grapplePointRelativeYPos < 0) hightestPointOnArc = grapple.overShootYAxis;

    playerController.JumpToPosition(grapplePoint, hightestPointOnArc);

    Invoke(nameof(StopGrapple), 1f);
  }

  /// <summary>
  /// Stops the grapple - Unfreezes the player movement and resets the grapple
  /// </summary>
  private void StopGrapple()
  {
    grapple.isGrappling = false;

    playerController.freeze = false;

    grapple.cooldownTimer = grapple.cooldown;
  }

  /// <summary>
  /// Starts the swing - attaches a spring joint betweem the player and grapple point allowing the player to swing around
  /// </summary>
  private void StartSwinging()
  {
    // return if predictionHit not found
    if(predictionHit.point == Vector3.zero) return;

    swing.isSwinging = true;

    grapplePoint = predictionHit.point;
    swing.joint = playerController.gameObject.AddComponent<SpringJoint>();
    swing.joint.autoConfigureConnectedAnchor = false;
    swing.joint.connectedAnchor = grapplePoint;

    float distanceFromPoint = Vector3.Distance(playerController.transform.position, grapplePoint);

    swing.joint.maxDistance = distanceFromPoint * swing.jointMaxDistance;
    swing.joint.minDistance = distanceFromPoint * swing.jointMinDistance;

    swing.joint.spring = swing.jointSpring;
    swing.joint.damper = swing.jointDamper;
    swing.joint.massScale = swing.jointMassScale;
  }

  /// <summary>
  /// Stops the swing - destroys the spring joint attached between the player and grapple point
  /// </summary>
  private void StopSwinging()
  {
    swing.isSwinging = false;
    Destroy(swing.joint);
  }

  /// <summary>
  /// Creates a prediction for where the player grapples/swings - if the player isnt directly aiming at a grappleable point spherecast to find the nearest grapple point
  /// </summary>
  private void CheckForSwingPoints()
  {
    if (swing.isSwinging) return;

    RaycastHit sphereCastHit;
    Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxDistance, whatIsGrappleable);

    RaycastHit raycastHit;
    Physics.Raycast(cam.position, cam.forward, out raycastHit, maxDistance, whatIsGrappleable);

    Vector3 realHitPoint;

    // Option 1 - Direct Hit
    if (raycastHit.point != Vector3.zero)
      realHitPoint = raycastHit.point;

    // Option 2 - Indirect (predicted) Hit
    else if (sphereCastHit.point != Vector3.zero)
      realHitPoint = sphereCastHit.point;

    // Option 3 - Miss
    else
      realHitPoint = Vector3.zero;

    // realHitPoint found
    if (realHitPoint != Vector3.zero)
    {
      predictionPoint.gameObject.SetActive(true);
      predictionPoint.position = realHitPoint;
    }
    // realHitPoint not found
    else
    {
      predictionPoint.gameObject.SetActive(false);
    }

    predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
  }
}

[System.Serializable]
public class Grapple
{
  public float delayTime;

  [Space(10)]

  public float cooldown;
  [HideInInspector] public float cooldownTimer;

  [Space(10)]

  [HideInInspector] public bool isGrappling;
  public float overShootYAxis;
}

[System.Serializable]
public class Swing
{
  public float jointMaxDistance = 0.8f;
  public float jointMinDistance = 0.25f;
  public float jointSpring = 4.5f;
  public float jointDamper = 7f;
  public float jointMassScale = 4.5f;

  [HideInInspector] public SpringJoint joint;
  [HideInInspector] public bool isSwinging;
}