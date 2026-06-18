using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Collections;

public class MiniGameManager : MonoBehaviour //Se encarga de la activación de los minijuegos, la selección de los mismos
{
    [SerializeField] private MiniGameControllers miniGameController; //El controlador de los minijuegos
    [SerializeField] private MiniGamesBase [] miniGameList; //Array de minijuegos que se pueden utilizar
    
    private MiniGamesBase currentMinigame; //El minijuego actual ejecutandose

    //Elementos del mundo y del jugador
    private CameraManager cameraManager; 
    private PlayerMovement movement; 
    private Interaction interaction;
    private DealingController dealing;

    private bool thereIsFire; //Si hay fuego activo

    private OrderManager orderManager; //Manager de los pedidos realizados (aún no se utiliza)
    private bool isInMinigame; //Si se esta en un minijuego

    [SerializeField] private float timeToResumeMinigame; //Tiempo que se le da al jugador para volver al minijuego pausado
    private bool isInPause;

    private List <MiniGamesBase> minigamesPaused;

    private List<Canvas> canvasMinigames;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip transitionSound;
    private SoundManager soundManager;

    public bool _thereIsFire { get { return thereIsFire; } set { thereIsFire = value; } }
    public OrderManager _orderManager => orderManager;
    public MiniGamesBase _currentMinigame => currentMinigame;
    public bool _isInPause => isInPause;
    public MiniGameControllers _miniGameController => miniGameController;
    public List <MiniGamesBase> _minigamesPaused => minigamesPaused;
    public List<Canvas> _canvasMinigames => canvasMinigames;

    private void Awake()
    { //Inicialización de variables
        isInMinigame = false;

        //miniGameControllers = GetComponent<MiniGameControllers>();
        movement = GetComponent<PlayerMovement>();
        interaction = GetComponent<Interaction>();
        dealing = GetComponent<DealingController>();

        isInPause = false;

        minigamesPaused = new List<MiniGamesBase>();
        canvasMinigames = new List<Canvas>();

        CursorState(false);
    }

    private void Start()
    {
        cameraManager = Camera.main.GetComponent<CameraManager>(); //Se obtiene el camraManager desde la cámara principal
        SelectActualMinigame(0); //Solo para test BORRAR DESPUÉS

        orderManager = new OrderManager();
        soundManager = GameManager.instance._soundManager;
    }

    public void StartMiniGame() //Se llama al recibir la llamada del método desde el mundo del juego
    {
        if (thereIsFire == false && isInMinigame == false) //Mientras que no haya fuego y no se este en un minijuego
        {
            CursorState(true);
            isInMinigame = true;

            soundManager.ReproduceSound(transitionSound);
            soundManager.ChangeVolumeMusic();

            movement.enabled = false;
            interaction.enabled = false;
            dealing.enabled = false;
            GameManager.instance._UIcontroller.gameObject.SetActive(false); //Se desactiva la interfaz del juego
            cameraManager.ChangeCamera(0); //Se hace la transición de cámara

            miniGameController.gameObject.SetActive(true); //Se activan los controles de los minijuegos
            currentMinigame.gameObject.SetActive(true); //Se activa el minijuego actual
        }
    }

    public void SelectActualMinigame(int miniGameToChange) //Para seleccionar que minijuego será el siguiente, se debe pasar un número para acceder al array y de esta forma seleccionar el minijuego. 0 = Arroz con wok
    {
        currentMinigame = miniGameList[miniGameToChange];
        miniGameController._currentMinigame = currentMinigame;
    }

    public MiniGamesBase GetMinigameInfo (int miniGameIndex)
    {
        return miniGameList[miniGameIndex];
    }

    public void RecivePoints (int points) //Se reciben los inputs del minijuego de ritmo. Z = 1 y X = 1
    {
        WokRiceBehaviour work = currentMinigame as WokRiceBehaviour; //Se pasa el minijuego a arroz con wok

        work.AddPointsOnRythm (points); //Se agregan los puntos desde el arroz con wok
    }

    public void StopMiniGameExecute() //Se llama para parar y desactivar todos los elementos de los minijuegos
    {
        CursorState(false);
        miniGameController._Q.Disable();
        cameraManager.ChangeCamera(1); //Cambio de camara hacia la del mundo

        soundManager.ReproduceSound(transitionSound);
        soundManager.SetDefaultVolumeMusic();

        currentMinigame.gameObject.SetActive (false);
        miniGameController.gameObject.SetActive (false);
        interaction.enabled = true;
        dealing.enabled = true;
        movement.enabled = true;
        GameManager.instance._UIcontroller.gameObject.SetActive(true); //Se activan de vuelta los elementos de la interfaz del minijuego

        currentMinigame = null;
        isInMinigame = false;
    }

    public void DefineMinigameStatus()
    {
        if (isInMinigame == true)
        {
            PauseMinigame();
        }
        else
        {
             ResumeMinigame();
        }
    }

    private void PauseMinigame()
    {
        CursorState(false);
        cameraManager.ChangeCamera (1);

        soundManager.ReproduceSound(transitionSound);
        soundManager.SetDefaultVolumeMusic();

        StartCoroutine(DurationMinigame());

        currentMinigame.PauseMinigame();
        minigamesPaused.Add (currentMinigame);
        miniGameController.gameObject.SetActive(false);

        interaction.enabled = true;
        dealing.enabled = true;
        movement.enabled = true;
        GameManager.instance._UIcontroller.gameObject.SetActive(true); //Se activan de vuelta los elementos de la interfaz del minijuego

        isInMinigame = false;
    }

    private void ResumeMinigame()
    {
        CursorState(true);
        StopCoroutine(DurationMinigame());

        soundManager.ReproduceSound(transitionSound);
        soundManager.ChangeVolumeMusic();

        miniGameController.gameObject.SetActive(true);
        minigamesPaused.Remove (currentMinigame);
        currentMinigame.ResumeMinigame();
        cameraManager.ChangeCamera(0);

        interaction.enabled = false;
        dealing.enabled = false;
        movement.enabled = false;
        GameManager.instance._UIcontroller.gameObject.SetActive(false); 

        isInMinigame = true;
    }

    private IEnumerator DurationMinigame()
    {
        yield return new WaitForSeconds(timeToResumeMinigame);

        if (minigamesPaused.Count > 0)
        {
            StopMinigameOnPause(minigamesPaused[0]);
        }

        if (minigamesPaused.Count > 0) { minigamesPaused.RemoveAt(0); }
    }

    private void StopMinigameOnPause(MiniGamesBase miniGameToStop)
    {
        miniGameToStop.gameObject.SetActive(false);

        if (isInMinigame == false)
        {
            currentMinigame = null;
        }
    }

    public void CursorState (bool isVisible)
    {
        if (isVisible == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
