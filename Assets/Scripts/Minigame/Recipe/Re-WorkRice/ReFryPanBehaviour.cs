using UnityEngine;

public class ReFryPanBehaviour : MonoBehaviour, ICanBeGrabbed
{
    private bool isMoving;

    private bool isSafe; //Booleanos para tener cierto margen de error para el jugador
    private bool isOnFirstBase;
    private bool isOnSecondBase;

    private int directionToGo;

    private Vector3 defaultPosition;
    [SerializeField] float positionOfY;

    private bool isReseting; //Sección de movimiento de reseteo de la sarten
    [SerializeField] private float speedReseting;
    private float progressReseting;

    [SerializeField] private MiniGamesBase miniGameToAct;

    private Vector3 beforePosition;
    private float positionYGlobal;

    [SerializeField] private AnimatorMinigames animator;

    public int _directionToGo => directionToGo;
    public bool _isMoving => isMoving;
    public bool _isOnFirstBase { get { return isOnFirstBase; } set { isOnFirstBase = value; } }
    public bool _isOnSecondBase { get { return isOnSecondBase; } set { isOnSecondBase = value; } }
    public bool _isSafe { get { return isSafe; } set { isSafe = value; } }

    private void Awake()
    {
        defaultPosition = transform.localPosition;
    }

    private void Start()
    {
        animator._executeAfterFinishAnimation = StopMG;
    }

    private void OnEnable()
    {
        isMoving = false;

        isSafe = true;
        isOnFirstBase = false;
        isOnSecondBase = false;

        isReseting = false;
        progressReseting = 0;

        transform.localPosition = defaultPosition;
        defaultPosition.y = positionOfY;

        beforePosition = defaultPosition;

        positionYGlobal = transform.position.y;
    }

    private void Update()
    {
        if (isReseting == true)
        {
            progressReseting += speedReseting * Time.deltaTime;
            ResetPosition();
        }
    }

    public bool CompleteSkillCheck()
    {
        return true;
    }

    public void Grab(Vector3 position)
    {
        if (isReseting == false) { transform.position = new Vector3 (position.x, positionYGlobal, position.z); }
        isMoving = true;
    }

    public void Drop()
    {
        isMoving = false;

        GameManager.instance._miniGameManager._miniGameController.ReDrop();
    }

    public void DefineDirection (int direction)
    {
        int newDirection = Mathf.Clamp(direction, 0, 7);

        directionToGo = newDirection;
    }

    public void Succes(bool status)
    {
        if (isReseting == true) { return; }

        IReSkillCheck minigame = miniGameToAct as IReSkillCheck;
        minigame.CompleteSkillCheck(status);
    }

    public void SetReset(bool status)
    {
        beforePosition = transform.localPosition;
        isReseting = status;

        Drop();
        isSafe = true;
        isOnFirstBase = false;
        isOnSecondBase = false;

        if (status == true)
        {
            progressReseting = 0;
        }
    }

    private void ResetPosition()
    {
        transform.localPosition = Vector3.Lerp(beforePosition, defaultPosition, progressReseting) ; 

        if (progressReseting >= 1)
        {
            SetReset(false);
        }
    }

    public void PlayFinalAnimation() 
    {
        animator.ReproduceAnimation();
    }

    public void StopMG()
    {
        gameObject.GetComponent<Collider>().enabled = true;

        ReWorkRiceBehaviour minigame = miniGameToAct as ReWorkRiceBehaviour; //Al final de la animación se debe de para el juego
        minigame.StopMinigame(true);
    }
}
