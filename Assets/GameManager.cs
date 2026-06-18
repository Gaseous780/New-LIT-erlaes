using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Managers y controllers pertenecientes al Game Manager, estan como componentes del mismo objeto del GameManager
    private SceneManager sceneManager;
    private SoundManager soundManager;

    //Managers y controllers solo de la escena, se espera que se reseteen por cada cambio de escena
    private UIController UIcontroller;
    private MiniGameManager miniGameManager;
    private GameObject player;
    private EconomyBehaviour economyBehaviour;
    private Conditions gameConditions;
    private FeedbackController feedbackController;
    private Interaction interaction;

    [SerializeField] private Texture2D cursorDefault;
    [SerializeField] private Texture2D knifeCursor;

    public SceneManager _sceneManager => sceneManager;
    public UIController _UIcontroller => UIcontroller;
    public MiniGameManager _miniGameManager => miniGameManager;
    public SoundManager _soundManager => soundManager;
    public GameObject _player => player;
    public EconomyBehaviour _economiyBehaviour => economyBehaviour;
    public Conditions _gameConditions => gameConditions;
    public FeedbackController _feedbackController => feedbackController;
    public Interaction _interaction => interaction;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        sceneManager = GetComponent<SceneManager>();
        soundManager = GetComponent<SoundManager>();

        FindValues();

        DefineCursor(0);
    }

    public void FindValues(Scene scene, LoadSceneMode mode)
    {
        UIcontroller = FindAnyObjectByType<UIController>();
        miniGameManager = FindAnyObjectByType<MiniGameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        economyBehaviour = FindAnyObjectByType<EconomyBehaviour>();
        gameConditions = FindAnyObjectByType<Conditions>();
        feedbackController = FindAnyObjectByType<FeedbackController>();
        interaction = FindAnyObjectByType<Interaction>();

        if (scene.buildIndex == 1) 
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            DefineCursor(0);
        }
        else 
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            DefineCursor(0); 
        }
    }

    public void FindValues()
    {
        UIcontroller = FindAnyObjectByType<UIController>();
        miniGameManager = FindAnyObjectByType<MiniGameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        economyBehaviour = FindAnyObjectByType<EconomyBehaviour>();
        gameConditions = FindAnyObjectByType<Conditions>();
        feedbackController = FindAnyObjectByType<FeedbackController>();
        interaction = FindAnyObjectByType<Interaction>();
    }

    public void DefineCursor (int cursorType) //0 = mouse Default, 1 = mouse Cuchillo
    {
        switch (cursorType) 
        { 
            case 0:
                Cursor.SetCursor(cursorDefault, new Vector2(32, 32), CursorMode.Auto);
                break;

            case 1:
                Cursor.SetCursor(knifeCursor, new Vector2(32, 32), CursorMode.Auto);
                break;
        
        }
    }
}
