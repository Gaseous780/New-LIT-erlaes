using UnityEngine;

public class ReFryVariant : MonoBehaviour, ICanBeGrabbed
{
    private bool isMoving;

    private bool isSafe; //Booleanos para tener cierto margen de error para el jugador
    private bool isOnFirstBase;
    private bool isOnSecondBase;

    private int directionIn;

    private Vector3 defaultPosition;
    [SerializeField] float positionOfY;

    private bool isReseting; //Sección de movimiento de reseteo de la sarten
    [SerializeField] private float speedReseting;
    private float progressReseting;

    [SerializeField] private MiniGamesBase miniGameToAct;

    private Vector3 beforePosition;
    private float positionYGlobal;

    [SerializeField] private AnimatorMinigames animator;

    [SerializeField] private float sensibility;

    private Vector3 originalEulersRotation;
    [SerializeField] private Vector2 amountRotationFinish; //Se usa un Vector2 porque solo necesito de X y Z. Y = Z
    [SerializeField] private float maxRotation = 5f;

    private bool isOutside;

    public int _directionIn => directionIn;
    public bool _isMoving => isMoving;
    public bool _isOnFirstBase { get { return isOnFirstBase; } set { isOnFirstBase = value; } }
    public bool _isOnSecondBase { get { return isOnSecondBase; } set { isOnSecondBase = value; } }
    public bool _isSafe { get { return isSafe; } set { isSafe = value; } }

    private void Awake()
    {
        defaultPosition = transform.localPosition;
        originalEulersRotation = transform.eulerAngles;
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

        transform.eulerAngles = originalEulersRotation;

        isOutside = false;
    }

    private void OnDisable()
    {
        Drop();
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
        if (isReseting == false) { transform.position = new Vector3(position.x, positionYGlobal, position.z * sensibility); }
        RotateFryPrecisly();
        isMoving = true;
    }

    public void Drop()
    {
        if (isMoving == false) { return; }
        isMoving = false;
        transform.eulerAngles = originalEulersRotation;

        if (isOutside == true) { SetReset(true); }

        GameManager.instance._miniGameManager._miniGameController.ReDrop();
    }

    public void DefineDirection(int direction)
    {
        int newDirection = Mathf.Clamp(direction, 0, 7);

        directionIn = newDirection;
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
        transform.eulerAngles = originalEulersRotation;
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
        transform.localPosition = Vector3.Lerp(beforePosition, defaultPosition, progressReseting);

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

    public void GiveDirection(int direction)
    {
        directionIn = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Borders") == true)
        {
            switch ((int)char.GetNumericValue(other.gameObject.name[0]))
            {
                case 0:
                    if (transform.position.z > other.transform.position.z)
                    {
                        isOutside = false;
                    }
                    else if (transform.position.z < other.transform.position.z)
                    {
                        isOutside = true;
                    }
                    break;

                case 1:
                    if (transform.position.z > other.transform.position.z)
                    {
                        isOutside = true;
                    }
                    else if (transform.position.z < other.transform.position.z)
                    {
                        isOutside = false;
                    }
                    break;

                case 2:
                    break;

                case 3:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Borders") == true)
        {
            switch ((int)char.GetNumericValue(other.gameObject.name[0]))
            {
                case 0:
                    if (transform.position.z > other.transform.position.z)
                    {
                        isOutside = false;
                    }
                    else if (transform.position.z < other.transform.position.z)
                    {
                        isOutside = true;
                    }
                    break;

                case 1:
                    if (transform.position.z > other.transform.position.z)
                    {
                        isOutside = true;
                    }
                    else if (transform.position.z < other.transform.position.z)
                    {
                        isOutside = false;
                    }
                    break;

                case 2:
                    break;

                case 3:
                    break;
            }
        }
    }

    private void RotateFryPrecisly()
    {
        float distanceRotationX;
        float distanceRotationY;

        if (transform.localPosition.x > defaultPosition.x)
        {
            distanceRotationY = Mathf.Lerp(originalEulersRotation.z, amountRotationFinish.y, Mathf.Abs ((transform.localPosition.x - defaultPosition.x))/maxRotation);
        }
        else
        {
            distanceRotationY = Mathf.Lerp(originalEulersRotation.z, -amountRotationFinish.y, Mathf.Abs((transform.localPosition.x - defaultPosition.x))/maxRotation);
        }

        if (transform.localPosition.z > defaultPosition.z)
        {
            distanceRotationX = Mathf.Lerp(originalEulersRotation.x, amountRotationFinish.x, Mathf.Abs((transform.localPosition.z - defaultPosition.z))/maxRotation);
        }
        else
        {
            distanceRotationX = Mathf.Lerp(originalEulersRotation.x, -amountRotationFinish.x, Mathf.Abs((transform.localPosition.z - defaultPosition.z)) / maxRotation);
        }

        transform.eulerAngles = new Vector3(distanceRotationX, originalEulersRotation.y, distanceRotationY);
    }
}
