using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
  [Header("Config")]
  [SerializeField] private int quality;
  [SerializeField] private float damper;
  [SerializeField] private float strength;
  [SerializeField] private float velocity;
  [SerializeField] private float waveCount;
  [SerializeField] private float waveHeight;
  [SerializeField] private AnimationCurve affectCurve;

  [Header("References")]
  [SerializeField] private GrapplingGun grapplingGun;
  [SerializeField] private LineRenderer lineRenderer;

  private Spring spring;
  private Vector3 currentGrapplePosition;

  private void Awake()
  {
    grapplingGun = GetComponent<GrapplingGun>();
    lineRenderer = GetComponent<LineRenderer>();

    spring = new Spring();
    spring.SetTarget(0);
  }

  private void LateUpdate()
  {
    DrawRope();
  }

  void DrawRope()
  {
    if (!grapplingGun.grapple.isGrappling && !grapplingGun.swing.isSwinging)
    {
      currentGrapplePosition = grapplingGun.muzzle.position;
      spring.Reset();
      if (lineRenderer.positionCount > 0)
        lineRenderer.positionCount = 0;
      return;
    }

    if (lineRenderer.positionCount == 0)
    {
      spring.SetVelocity(velocity);
      lineRenderer.positionCount = quality + 1;
    }

    spring.SetDamper(damper);
    spring.SetStrength(strength);
    spring.Update(Time.deltaTime);

    Vector3 grapplePoint = grapplingGun.grapplePoint;
    Vector3 gunTipPosition = grapplingGun.muzzle.position;
    Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

    currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

    for (var i = 0; i < quality + 1; i++)
    {
      float delta = i / (float)quality;
      Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                   affectCurve.Evaluate(delta);

      lineRenderer.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
    }
  }
}
