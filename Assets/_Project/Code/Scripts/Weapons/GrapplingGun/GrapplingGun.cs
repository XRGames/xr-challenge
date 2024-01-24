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

  public LayerMask whatIsGrappleable;

  [Header("References")]
  [SerializeField] private InputManager input;
  [SerializeField] private Transform player;
  [SerializeField] private Rigidbody playerRigidbody;
  [SerializeField] private Transform cam;
  [SerializeField] private LineRenderer lineRenderer;

  [HideInInspector] public Vector3 grapplePoint { get; private set; }
  private SpringJoint joint;

  bool isLaunching = false;

  private void Awake()
  {
    PlayerController controller = FindObjectOfType<PlayerController>();
    input = controller.Input;
    player = controller.transform;
    playerRigidbody = controller.GetComponent<Rigidbody>();

    cam = Camera.main.transform;

    lineRenderer = GetComponent<LineRenderer>();
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
      Vector3 direction = grapplePoint - player.position;
      playerRigidbody.AddRelativeForce(direction.normalized * launchSpeed, ForceMode.Force);

      if (Vector3.Distance(player.position, grapplePoint) < launchDistance)
      {
        isLaunching = false;
        StopGrapple();
      }
    }
  }

  private void Shot(bool isDown)
  {
    if(isDown)
    {
      StartGrapple();
    } 
    else
    {
      StopGrapple();
    }
  }

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

  private void DrawGrappleRope()
  {
    if (!joint) return;

    lineRenderer.SetPosition(0, muzzle.position);
    lineRenderer.SetPosition(1, grapplePoint);
  }

  private void StopGrapple()
  {
    if (!IsGrappling()) return;

    lineRenderer.positionCount = 0;
    Destroy(joint);
  }

  private void Launch(bool isDown)
  {
    if(isDown)
    {
      if(IsGrappling())
      {
        isLaunching = true;
      }
    }
  }

  public bool IsGrappling()
  {
    return joint != null;
  }
}
