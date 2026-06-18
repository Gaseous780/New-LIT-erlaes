using System.Collections;
using UnityEngine;

public abstract class MiniGamesBase : MonoBehaviour
{
    protected int actualPhase; //Sirve para determinar que tipo de controles usar
    protected int errorCounters; //Contador de errores en el minijuego

    [SerializeField] protected float valueOfFood; //El valor máximo de la comida hecha

    [SerializeField] protected string[] controlsToUse; //Un string que guarda los nombres de los controles que se usan en cada fase del minijuego, estos luego son cargados para cambiar los controles. Su index es la fase del juego (el número esta con uno menos, debido a que se cuenta la 0 como 1, 1 como 2 y así progresivamente). Deben de ser exactos a como estan en el playerInput de los minijuegos

    [SerializeField] protected TypeOfFoods [] requestFood;

    [SerializeField] protected Canvas minigameCanvas;

    [SerializeField] protected AudioClip grabFood;

    protected MiniGameManager miniGameManager;
    protected MiniGameControllers miniGameControllers;

    protected SoundManager soundManager;

    protected bool inCinematic;

    public TypeOfFoods[] _requestFood => requestFood;
    public bool _inCinematic => inCinematic;

    protected virtual void Start()
    {
        miniGameManager = GameManager.instance._miniGameManager;
        miniGameControllers = GameManager.instance._miniGameManager._miniGameController;

        soundManager = GameManager.instance._soundManager;

        if (minigameCanvas != null) { miniGameManager._canvasMinigames.Add(minigameCanvas); }
    }

    public virtual void StartMinigame() { } //Se tiene que ejecutar al momento en el que se inicializa el minijuego

    //Los minijuegos no deben de incluir las 4 fases, dependiendo de cual sea puede variar
    public virtual void PhaseOne() { } //Pase a la fase 1 del minujuego
    public virtual void PhaseTwo() { } //Pase a la fase 2 del minujuego
    public virtual void PhaseThree() { } //Pase a la fase 3 del minijuego
    public virtual void PhaseFour() { } //Pase a la fase 4 del minijuego
    public virtual void StopMinigame(bool succes) { if (grabFood != null) { soundManager.ReproduceSound(grabFood); } }  //Se tiene que ejecutar en el momento que se para el minijuego y se desea volver al mundo principal. El booleano es para saber si se debe parar el juego con un error o con exito
    public virtual bool Error() { return default; } //Devuelve bool para saber si cancelar corrutinas o procesos al cerrar el minijuego en el limite de errores

    public virtual IEnumerator InitWithDelay() { yield return null; }  //Corrutina para que se puede inicializar el juego de forma correcta, sin que ninguna variable sea nula

    public virtual void PauseMinigame() { } //Método para pausar el juego
    public virtual void ResumeMinigame () { }

    public virtual bool Request(GameObject objectNeeded) 
    {
        if (objectNeeded == null)
        {
            return true;
        }

        return false;
    }
}
