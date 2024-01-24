using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
  [Header("Weapon Config")]
  [SerializeField] private float maxDistance = 100f;
  [SerializeField] private Transform muzzle;

  [Header("Grapple Config")]
  [SerializeField] private float launchSpeed = 0.5f;
  [SerializeField] private float launchDistance = 3f;
  [SerializeField] private float maxJointDistance = 0.8f;
  [SerializeField] private float minJointDistance = 0.25f;
  [SerializeField] private float jointSpring = 4.5f;
  [SerializeField] private float jointDamper = 7f;
  [SerializeField] private float jointMassScale = 4.5f;

  [Space(15)]

  [SerializeField] private LayerMask whatIsGrappleable;

  [Header("References")]
  [SerializeField] private InputManager input;
  [SerializeField] private Transform player;
  [SerializeField] private Rigidbody playerRigidbody;
  [SerializeField] private Transform cam;
  [SerializeField] private LineRenderer lineRenderer;

  public Vector3 grapplePoint { get; private set; }
  private SpringJoint joint;

  bool isLaunching = false;

  private void Awake()
  {
    input = player.GetComponent<PlayerController>().Input;
    playerRigidbody = player.GetComponent<Rigidbody>();

    cam = Camera.main.transform;

    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.positionCount = 0;
  }

  private void OnEnable()
  {
    input.LeftClick += Shot;
    input.RightClick += Launch;
  }

  private void OnDisable()
  {
    input.LeftClick -= Shot;
    input.RightClick -= Launch;
  }

  private void LateUpdate()
  {
    DrawGrappleRope();
  }
  
  private void Update()
  {
    if (isLaunching)
    {
      LaunchGrapple();
    }
  }

  /// <summary>
  /// Adds force to the player in the direction of the current grapple point
  /// </summary>
  private void LaunchGrapple()
  {
    Vector3 direction = grapplePoint - player.position;
    playerRigidbody.AddRelativeForce(direction.normalized * launchSpeed, ForceMode.Force);

    if (Vector3.Distance(player.position, grapplePoint) < launchDistance)
    {
      isLaunching = false;
      StopGrapple();
    }
  }

  /// <summary>
  /// Starts/Stops the grapple gun
  /// </summary>
  /// <param name="isDown">bool - whether the mouse button is pressed or released</param>
  private void Shot(bool isDown)
  {
    if(isDown && !IsGrappling())
    {
      StartGrapple();
    } 
    else
    {
      StopGrapple();
    }
  }

  /// <summary>
  /// Hooks the player onto the closest raycast hit with the "whatIsGrappleable" layer
  /// </summary>
  private void StartGrapple()
  {
    RaycastHit hit;
    if(Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, whatIsGrappleable)) 
    { 
      grapplePoint = hit.point;
      joint = player.gameObject.AddComponent<SpringJoint>();
      joint.autoConfigureConnectedAnchor = false;
      joint.connectedAnchor = grapplePoint;

      float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

      joint.maxDistance = distanceFromPoint * maxJointDistance;
      joint.minDistance = distanceFromPoint * minJointDistance;

      joint.spring = jointSpring;
      joint.damper = jointDamper;
      joint.massScale = jointMassScale;

      lineRenderer.positionCount = 2;
    }
  }

  /// <summary>
  /// Draws a line renderer from the muzzle of the gun to the grapple point
  /// </summary>
  private void DrawGrappleRope()
  {
    if (!IsGrappling()) return;

    lineRenderer.SetPosition(0, muzzle.position);
    lineRenderer.SetPosition(1, grapplePoint);
  }

  /// <summary>
  /// Removes the spring joint and hides the LineRenderer
  /// </summary>
  private void StopGrapple()
  {
    if (!IsGrappling()) return;

    lineRenderer.positionCount = 0;
    Destroy(joint);
  }

  /// <summary>
  /// Launches the player
  /// </summary>
  /// <param name="isDown">bool - whether the mouse button is pressed or released</param>
  private void Launch(bool isDown)
  {
    if(isDown && IsGrappling())
    {
      isLaunching = true;
    }
  }

  public bool IsGrappling()
  {
    return joint != null;
  }
}
