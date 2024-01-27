using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
    SceneManager.LoadScene("WinScene"); 
  }
}
