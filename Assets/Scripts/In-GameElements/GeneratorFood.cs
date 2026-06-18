using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorFood : MonoBehaviour
{
    [SerializeField] private List<GameObject> foodsToGenerate;

    [SerializeField] private GameObject menuFridge;

    private PlayerMovement playerControls;
    private DealingController playerDealing;
    private Interaction interactionBehaviour;
    [SerializeField] private EconomyBehaviour economyBehaviour;

    [Header("Sounds")]
    [SerializeField] private AudioClip openFridge;
    [SerializeField] private AudioClip closeFridge;
    private SoundManager soundManager;

    public List<GameObject> _foodsToGenerate => foodsToGenerate;

    private void Start()
    {
        playerControls = GameManager.instance._player.GetComponent<PlayerMovement>();
        playerDealing = GameManager.instance._player.GetComponent<DealingController>();
        interactionBehaviour = GameManager.instance._player.GetComponent<Interaction>();
        
        soundManager = GameManager.instance._soundManager;
    }

    public void OpenMenu()
    {
        soundManager.ReproduceSound(openFridge);
        menuFridge.SetActive(true);
        playerDealing.enabled = false;
        playerControls.enabled = false;

        GameManager.instance._miniGameManager.CursorState(true);
    }

    public void CloseMenu()
    {
        soundManager.ReproduceSound(closeFridge);
        menuFridge.SetActive(false);
        playerDealing.enabled = true;
        playerControls.enabled = true;

        GameManager.instance._miniGameManager.CursorState(false);
    }

    public void GenerateFood(int food) //El int representa la comida a generar. aka no me deja unity usar enums con botones :(
    {
        GameObject foodCreated = Instantiate(foodsToGenerate[food], Vector3.zero, new Quaternion());//La comida a generar según el int accederá al index de la lista y según el número dará cierta comida
        interactionBehaviour._heldObject = foodCreated;
        economyBehaviour.DecreaseMoney(foodCreated.GetComponent<FoodBehaviour>()._valueOfFood);
        foodCreated.GetComponent<Collider>().enabled = false;

        CloseMenu();
    }
}
