using UnityEngine;
using UnityEngine.Events;

public class WinHandler : MonoBehaviour
{
  public event UnityAction Win = delegate { };

  [Header("Config")]
  [SerializeField] private int totalToCollect = 5;

  [Header("References")]
  [SerializeField] private PickupHandler pickupHandler;

  private void Awake()
  {
    pickupHandler = GetComponent<PickupHandler>();
  }

  private void OnTriggerEnter(Collider c)
  {
    if(c.CompareTag("EndPoint"))
    {
      if(pickupHandler.Collected >= totalToCollect)
      {
        Win.Invoke();
      }
    }
  }
}
