using UnityEngine;

public class RotateGrapple : MonoBehaviour
{
  [Header("Config")]
  [SerializeField] private float rotationSpeed = 5f;

  [Header("References")]
  [SerializeField] private GrapplingGun grapple;

  private Quaternion desiredRotation;

  private void Awake()
  {
    grapple = GetComponent<GrapplingGun>();
  }

  private void Update()
  {
    if (grapple.grapple.isGrappling || grapple.swing.isSwinging)
    {
      desiredRotation = Quaternion.LookRotation(grapple.grapplePoint - transform.position);
    }
    else
    {
      desiredRotation = transform.parent.rotation;
    }

    transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
  }
}