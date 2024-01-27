using System.Runtime.Remoting.Contexts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
  [Header("Config")]
  [SerializeField] private Transform playerVisuals;
  [SerializeField] private LayerMask mask;

  [Header("References")]
  [SerializeField] private Camera cam;

  private void Awake()
  {
    cam = Camera.main;
  }

  private void Update()
  {
    if(cam == null) return;
    if (playerVisuals == null) return;

    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
    if (Physics.Raycast(ray, out RaycastHit raycastHit))
    {
      playerVisuals.LookAt(new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z));
    }
  }
}
