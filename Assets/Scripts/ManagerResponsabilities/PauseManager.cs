using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject generalCanvas;
    public GameObject wokCanvas;
    private bool isPaused = false;
    private bool wasGeneralActive = false;

    [Header("Sounds")]
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip resumeSound;
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = GameManager.instance._soundManager;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        soundManager.ReproduceSound(resumeSound);

        pausePanel.SetActive(false);

        if (wasGeneralActive) generalCanvas.SetActive(true);

        foreach (Canvas canvasFromMinigame in GameManager.instance._miniGameManager._canvasMinigames)
        {
            if (canvasFromMinigame.gameObject.activeSelf == true)
            {
                canvasFromMinigame.enabled = true;
            }
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        soundManager.ReproduceSound(pauseSound);

        wasGeneralActive = generalCanvas.activeSelf;

        pausePanel.SetActive(true);
        generalCanvas.SetActive(false);

        foreach (Canvas canvasFromMinigame in GameManager.instance._miniGameManager._canvasMinigames)
        {
            if (canvasFromMinigame.gameObject.activeSelf == true)
            {
                canvasFromMinigame.enabled = false;
            }
        }

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        GameManager.instance._sceneManager.LoadScene(0);
    }
}
