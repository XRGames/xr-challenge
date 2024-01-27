using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  [Header("Config")]
  [SerializeField] private string winScene;
  [SerializeField] private string lossScene;

  [Header("References")]
  [SerializeField] private TriggerHandler triggerHandler;

  private void OnEnable()
  {
    triggerHandler.Win += OnWin;
    triggerHandler.Loss += OnLoss;
  }

  private void OnDisable()
  {
    triggerHandler.Win -= OnWin;
    triggerHandler.Loss -= OnLoss;
  }

  private void OnWin()
  {
    SceneManager.LoadScene(winScene); 
  }

  private void OnLoss()
  {
    SceneManager.LoadScene(lossScene);
  }
}
