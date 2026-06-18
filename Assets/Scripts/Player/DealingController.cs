using UnityEngine;
using UnityEngine.InputSystem;

public class DealingController : MonoBehaviour
{
    private PlayerInput playerControls;

    [SerializeField] private float distanceRayCast = 2;

    private MiniGameManager miniGameManager;
    private Interaction interactionBehaviour;

    private ItemsBase itemGrabbed;

    private bool useGrabItem;

    public ItemsBase _itemGrabbed => itemGrabbed;

    [SerializeField] private InputAction F;

    [SerializeField] private InputAction E;

    [Header("SoundsSection")]
    [SerializeField] private AudioClip openPanelSound;
    private SoundManager soundManager;

    [SerializeField] private GameObject sil;

    private void Awake()
    {
        useGrabItem = false;

        playerControls = GetComponent<PlayerInput>();
        miniGameManager = GetComponent<MiniGameManager>();
        interactionBehaviour = GetComponent<Interaction>();
    }

    private void Start()
    {
        F.canceled += F_canceled;

        F.Enable();
        E.Enable();

        soundManager = GameManager.instance._soundManager;
    }

    private void F_canceled(InputAction.CallbackContext obj)
    {
        useGrabItem = false;
    }

    private void Update()
    {
        if (useGrabItem == true && itemGrabbed != null)
        {
            itemGrabbed.Use();
        }
        else if( itemGrabbed != null)
        {
            itemGrabbed.CancelUse();
        }

        if (E.triggered)//activa input x frame
        {
            Dealing();
        }

        if (F.triggered)
        {
            UseGrabbedObject();
        }
    }

    public void Dealing()
    {
        RaycastHit detectingRay;

        if (Physics.Raycast(transform.position, transform.forward, out detectingRay, distanceRayCast) == true)
        {
            if (detectingRay.transform.gameObject.CompareTag("MinigameTable") == true && Camera.main.GetComponent<CameraManager>()._brain.IsBlending == false)
            {
                miniGameManager.SelectActualMinigame((int)char.GetNumericValue(detectingRay.transform.gameObject.name[0])); //Donde se obtiene el minijuego según la mesa

                if (miniGameManager._minigamesPaused.Contains(miniGameManager._currentMinigame) == true)
                {
                    miniGameManager._miniGameController._Q.Enable();
                    miniGameManager.DefineMinigameStatus();
                }
                else //if (interactionBehaviour._heldObject != null)
                //{
                    if (miniGameManager._currentMinigame.Request(interactionBehaviour._heldObject) == true)
                    {
                        miniGameManager.StartMiniGame();

                        if (interactionBehaviour._heldObject != null)
                        {
                            if (interactionBehaviour._heldObject.GetComponent<FoodBehaviour>() != null)
                            {
                                foreach (TypeOfFoods food in miniGameManager._currentMinigame._requestFood)
                                {
                                    if (food == interactionBehaviour._heldObject.GetComponent<FoodBehaviour>()._food)
                                    {
                                        interactionBehaviour._heldObject.SetActive(false);
                                        interactionBehaviour._heldObject = null;

                                        break;
                                    }
                                }
                            }
                        }

                        miniGameManager._miniGameController._Q.Enable();
                    }
               // }
            }
            else if (detectingRay.transform.gameObject.CompareTag("FoodGenerator") == true && interactionBehaviour._heldObject ==  null)
            {
                detectingRay.collider.gameObject.GetComponent <GeneratorFood>().OpenMenu();
            }
            else if (detectingRay.transform.gameObject.CompareTag("Oven") == true)
            {
                if (interactionBehaviour._heldObject != null)
                {
                    if (interactionBehaviour._heldObject.GetComponent<FoodBehaviour>() != null)
                    {
                        if (detectingRay.collider.gameObject.GetComponent<OvenBehaviour>().StartOven(interactionBehaviour._heldObject.GetComponent<FoodBehaviour>()) == true)
                        {
                            interactionBehaviour._heldObject.SetActive(false);
                            interactionBehaviour._heldObject = null;
                        }
                    }
                }
            }
            else if (detectingRay.transform.gameObject.CompareTag("Panel") == true)
            {
                sil.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Camera.main.gameObject.GetComponent<CameraManager>().ChangeCamera(2);

                soundManager.ReproduceSound(openPanelSound);
            }
        }
    }

    public void UseGrabbedObject()
    {
        //if (context.started == true)
        {
            useGrabItem = true;
        }
        //else if (context.canceled == true)
        //{
        //    useGrabItem = false;
        //}
    }

    public void Cancel(InputAction.CallbackContext context)
    {
        useGrabItem = false;
    }

    public void SetItemGrabbed(ItemsBase itemToSet)
    {
        itemGrabbed = itemToSet;
    }
}
