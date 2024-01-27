using TMPro;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private TextMeshProUGUI scoreText;

  private int score = 0;
  public int Score => score;

  private int collected = 0;
  public int Collected => collected;

  private void Update()
  {
    scoreText.text = "SCORE: " + score;
  }

  private void OnTriggerEnter(Collider c)
  {
    if (c.CompareTag("Pickup"))
    {
      score += c.GetComponent<Pickup>().GetPickedUp();
      collected++;
    }
  }
}
