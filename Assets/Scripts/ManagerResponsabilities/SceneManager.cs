using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private GameManager manager;

    private void Start()
    {
        if (GameManager.instance != null)
        {
            manager = GameManager.instance;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += manager.FindValues;
        }

        if (GameManager.instance._soundManager != null)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += manager._soundManager.DefineMusicOfScene;
        }
    }
    public void LoadScene(int scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void ExitGame()
    {
        Debug.Log("quitting...");
        Application.Quit();
    }
}
