using UnityEngine;
using UnityEngine.Events;

public class TriggerHandler : MonoBehaviour
{
  public event UnityAction Win = delegate { };
  public event UnityAction Loss = delegate { };

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
    else if (c.CompareTag("Killbox"))
    {
      Loss.Invoke();
    }
  }
}
