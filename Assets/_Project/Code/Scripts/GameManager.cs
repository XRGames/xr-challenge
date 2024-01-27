using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  [Header("Config")]
  [SerializeField] private string winScene;
  [SerializeField] private string lossScene;

  [Header("References")]
  [SerializeField] private WinHandler winHandler;

  private void OnEnable()
  {
    winHandler.Win += OnWin;
  }

  private void OnDisable()
  {
    winHandler.Win -= OnWin;
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
