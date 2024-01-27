using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
  public void SceneTransition(string sceneName)
  {
    SceneManager.LoadScene(sceneName);
  }

  public void QuitApplication()
  {
    Application.Quit();
  }
}
