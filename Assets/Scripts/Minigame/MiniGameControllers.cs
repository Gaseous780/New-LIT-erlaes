using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniGameControllers : MonoBehaviour //Aca van todos los controles de los minijuegos
{
    [SerializeField] private PlayerInput controlsMinigame;

    [SerializeField] private Camera cameraM; //La camra sirve para todo lo relacionado con la posición del mouse

    [SerializeField] private float distance; //Distancia de la camara que le permite agarrar objetos
    [SerializeField] private float reDistance; //Para el nuevo agarre de objetos

    private GameObject grabbedObject; //El objeto que se agarra

    private Vector3 input; //Input del mouse

    [SerializeField] private MiniGameManager miniGameManager; //Manager de los minijuegos

    [SerializeField] private TextMeshProUGUI text; //Solo para testeo

    private List<char> inputsRecived; //Con ella se identifican los últimos inputs para el minijuego de Takoyaki

    [SerializeField] private MiniGamesBase currentMinigame;

    [SerializeField] private InputAction Q;

    private bool reWokGrabIsUpdate; //Para que en el re wok se pueda mover la sarten de forma fluida se debe usar el update

    public Vector3 _input => input;
    public MiniGamesBase _currentMinigame { get { return currentMinigame; } set { currentMinigame = value; } }
    public InputAction _Q => Q;

    private void Awake()
    {
        controlsMinigame = GetComponent<PlayerInput>();

        grabbedObject = null;

        //miniGameManager = GetComponent<MiniGameManager>(); La idea sería que el minigameManager este junto al controller y no sea serializado

        RestartLastInputs();

        Q.started += PauseMinigame;
    }

    private void Start()
    {
        cameraM = Camera.main;
    }

    public void ChangeControl(string nameToControl) //Método para cambiar los controles a través del nombre del control deseado
    {
        controlsMinigame.SwitchCurrentActionMap(nameToControl);
    }

    public void PauseMinigame(InputAction.CallbackContext context)
    {
        if (miniGameManager._thereIsFire == false && cameraM.GetComponent<CameraManager>()._brain.IsBlending == false && currentMinigame._inCinematic == false)
        {
            miniGameManager.DefineMinigameStatus();
            Q.Disable();
        }
    }

    public void Drag(InputAction.CallbackContext context) //Método para el arrastre del mouse
    {
        if (Time.timeScale == 0) return;
        string buttonPath = "/Mouse/leftButton"; //Para detectar que es el click izquierdo

        if (context.started) //Al momento de clickear con el click izquierdo
        {
            if (context.control.path == buttonPath)
            {
                RaycastHit hit;
                Ray rayToDetectObject = cameraM.ScreenPointToRay(Mouse.current.position.ReadValue()); //Se crea un raycast para la posición del mouse

                if (Physics.Raycast(rayToDetectObject, out hit) && hit.collider.gameObject.CompareTag("IngredientGenerator") == true) //Si el raycast le pega a un generador de ingredientes. A su vez se define el hit
                {
                    grabbedObject = hit.collider.gameObject.GetComponent<IngredientGenerator>().GrabIngredient(); //El objeto agarrado pasa a ser un ingrediente generado por si generador
                }
                else if (Physics.Raycast(rayToDetectObject, out hit) && (hit.collider.gameObject.CompareTag("Breakable") == true || hit.collider.gameObject.CompareTag("Dragger") == true)) //Si el raycast esta con un objeto que se puede romper
                {
                    grabbedObject = hit.collider.gameObject; //Por ejemplo los huevos generados

                    if (grabbedObject.GetComponent<ICanBeGrabbed>() != null)
                    {
                        ICanBeGrabbed grabbed = grabbedObject.GetComponent<ICanBeGrabbed>();
                        grabbed.Grab(Vector3.zero);
                    }
                }
            }
        }
        else if (context.performed == true) //Mientras que se mantenga apretado el click izquierdo
        {
            if (context.control.path == buttonPath && grabbedObject != null)
            {
                grabbedObject.transform.position = cameraM.ScreenToWorldPoint(input); //La posición del gameObject se va actualizando en base a los inputs del mouse

                if (grabbedObject.GetComponent<ICanBeGrabbed>() != null)
                {
                    ICanBeGrabbed grabbed = grabbedObject.GetComponent<ICanBeGrabbed>();
                    grabbed.Grab(Vector3.zero);
                }
            }
        }
        else if (context.canceled == true) //Al soltarse el clcik izquierdo
        {
            if (context.control.path == buttonPath)
            {
                if (grabbedObject != null && grabbedObject.CompareTag("Dragger") == true)
                {
                    grabbedObject.SetActive(false);

                    if (grabbedObject.GetComponent<ICanBeGrabbed>() != null)
                    {
                        ICanBeGrabbed grabbed = grabbedObject.GetComponent<ICanBeGrabbed>();
                        grabbed.Drop();

                        grabbedObject.SetActive(true);
                    }
                }

                grabbedObject = null; //El objeto que estaba agarrado se suelta y pasa a ser ninguno
            }
        }
    }

    public void Move(InputAction.CallbackContext context) //Método para el movimiento del mouse
    {
        if (Time.timeScale == 0) return;
        if (context.performed) //Mientras que el mouse sea movido
        {
            input = new Vector3(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y, distance); //Se lee los movimientos del mouse
        }
    }

    public void ZUP(InputAction.CallbackContext context) //Al presionar la tecla Z
    {
        if (Time.timeScale == 0) return;
        if (context.started) //Al ser presionada la Z
        {
            miniGameManager.RecivePoints((int)context.ReadValue<float>()); //Se pasa la Z como input al manager
        }
    }

    public void XDOWN(InputAction.CallbackContext context) //Al presionar la tecla X
    {
        if (Time.timeScale == 0) return;
        if (context.started) //Al ser presionada la x
        {
            miniGameManager.RecivePoints(-(int)context.ReadValue<float>()); //Se pasa la X como input al manager
        }
    }


    //Tayouki Controls

    public void WASDInputs(InputAction.CallbackContext context) //Para el minijuego del temperametro, donde se puede recibir cualquier tecla de los wasd
    {
        if (Time.timeScale == 0) return;
        if (context.started)
        {
            if (inputsRecived.Count > 1)
            {
                StopCoroutine(DetectionOfInputs()); //Si ya hay otro input en el poco tiempo antes de que se dispare el input, se cancela la corrutina
            }

            inputsRecived.Add(context.control.path[context.control.path.Length - 1]); //Se agrega el nuevo input presionado en el último espacio como un string

            StartCoroutine(DetectionOfInputs());
        }
    }

    public void RestartLastInputs() //Reinicio de los últimos inputs recividos
    {
        inputsRecived = new List<char>();
    }

    private IEnumerator DetectionOfInputs() //Se usa una corrutina para que se puede detectar el presionar varios botones a la vez
    {
        yield return new WaitForSeconds(0.05f);

        string recived = string.Empty;

        for (int i = 0; i < inputsRecived.Count; i++)
        {
            recived += inputsRecived[i]; //En el string se guarda la convinación de inputs presionados
        }

        IMovePlayerNodes movePlayer = currentMinigame as IMovePlayerNodes; //La interfaz sirve para que no se dependa de un minijuego, por si otra receta incluye este minijuego
        movePlayer.MovePlayer(recived); //El jugador se mueve según el input recivido
        RestartLastInputs(); //Se reinicia para detectar nuevos inputs
    }

    public void WFryInput(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            IFryer miniGameFryer = currentMinigame as IFryer;
            miniGameFryer.FryActionUp();
        }
    }
    public void SFryInput(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            IFryer miniGameFryer = currentMinigame as IFryer;
            miniGameFryer.FryActionDown();
        }
    }

    //Mochis Controls

    public void CollitionWithGameObject(InputAction.CallbackContext context) //Para la parte de cortar ingredientes (tipo fruit ninja)
    {
        if (Time.timeScale == 0) return;
        if (context.performed)
        {
            RaycastHit hit;
            Ray rayToDetectObject = cameraM.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(rayToDetectObject, out hit))
            {
                ICollitionMouse mouseAction = currentMinigame as ICollitionMouse;
                mouseAction.Collition(hit.collider.gameObject);
            }
        }
    }

    public void BeatInputs(InputAction.CallbackContext context)  //Para detectar cuando se bate
    {
        if (Time.timeScale == 0) return;
        if (context.started)
        {
            IWASDInput reciver = currentMinigame as IWASDInput;
            reciver.ReciveInput(context.control.path[context.control.path.Length - 1]);
        }
    }

    public void AutomaticSkillCheckInput(InputAction.CallbackContext context) //Para acciones que se requieren de solo presionar SOLO un input
    {
        if (Time.timeScale == 0) return;
        if (context.started)
        {
            IOneInputDetection oneInputDetection = currentMinigame as IOneInputDetection;
            oneInputDetection.InputEvent();
        }
    }

    //Para el cheesecake japones

    public void Click(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RaycastHit hit;
            Ray rayToDetectObject = cameraM.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(rayToDetectObject, out hit))
            {
                IPointerClick mouseAction = currentMinigame as IPointerClick;
                mouseAction.Click(hit.collider.gameObject);
            }
        }
    }

    //Para el Re-WorkRice

    public void ReDrag(InputAction.CallbackContext context)
    {
        string buttonPath = "/Mouse/leftButton"; //Para detectar que es el click izquierdo

        if (context.started) //Al momento de clickear con el click izquierdo
        {
            if (context.control.path == buttonPath)
            {
                RaycastHit hit;
                Ray rayToDetectObject = cameraM.ScreenPointToRay(Mouse.current.position.ReadValue()); //Se crea un raycast para la posición del mouse

                if (Physics.Raycast(rayToDetectObject, out hit) && hit.collider.CompareTag("ReGrab")) //Si el raycast le pega a un generador de ingredientes. A su vez se define el hit
                {
                    grabbedObject = hit.collider.gameObject;
                    ICanBeGrabbed grabbed = grabbedObject.GetComponent<ICanBeGrabbed>();
                    grabbed.Grab(cameraM.ScreenToWorldPoint(input));
                    reWokGrabIsUpdate = true;
                }
            }
        }
        else if (context.canceled == true) //Al soltarse el clcik izquierdo
        {
            if (context.control.path == buttonPath)
            {
                Debug.Log("Cancel");
                if (grabbedObject != null && grabbedObject.CompareTag("ReGrab") == true)
                {
                    Debug.Log("Object");
                    //grabbedObject.SetActive(false);
                    ICanBeGrabbed grabbed = grabbedObject.GetComponent<ICanBeGrabbed>();
                    grabbed.Drop();
                }

                ReDrop();
            }
        }
    }

    public void ReMove(InputAction.CallbackContext context) //Método para el movimiento del mouse
    {
        if (Time.timeScale == 0) return;
        if (context.performed) //Mientras que el mouse sea movido
        {
            input = new Vector3(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y, reDistance); //Se lee los movimientos del mouse
        }
    }

    public void ReDrop()
    {
        if (grabbedObject != null)
        {
            ICanBeGrabbed grabbed = grabbedObject.GetComponent<ICanBeGrabbed>();
            grabbed.Drop();
        }
        reWokGrabIsUpdate = false;
        grabbedObject = null;
    }

    private void Update()
    {
        if (reWokGrabIsUpdate == true && grabbedObject != null)
        {
            grabbedObject.GetComponent<ICanBeGrabbed>().Grab(cameraM.ScreenToWorldPoint(input)); //La posición del gameObject se va actualizando en base a los inputs del mouse
        }
    }
}
