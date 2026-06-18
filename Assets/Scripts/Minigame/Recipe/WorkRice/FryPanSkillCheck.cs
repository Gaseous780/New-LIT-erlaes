using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FryPanSkillCheck : MonoBehaviour, IPointerMoveHandler
{
    [SerializeField] MiniGameControllers controllers;
    [SerializeField] MiniGamesBase minigame;
    private Image imageFeatures;

    [SerializeField] private float YToCompleteSkill = 0;

    [SerializeField] private float skillCheck;

    [SerializeField] private GameObject initialCircle;

    [SerializeField] private float actualSkillCheck;
    private float amountFill;
    private RectTransform rectTransform;

    private bool completeMinigame; //Para que no se llame de nuevo

    [SerializeField] private Vector4 permissiveImage;

    private float positionOriginalYCircle;

    [SerializeField] private float amountPermessive = 200f;
    private void Awake()
    {
        amountFill = 0.10f;

        imageFeatures = GetComponent<Image>();

        rectTransform = GetComponent<RectTransform>();

        positionOriginalYCircle = initialCircle.transform.position.y;
    }

    private void OnEnable()
    {
        Vector3 []worldCorners = new Vector3 [4]; //Para obtener los costados del rectangulo
        rectTransform.GetWorldCorners(worldCorners);

        skillCheck = worldCorners[3].y; //Min Y
        if (YToCompleteSkill == 0) { YToCompleteSkill = worldCorners[2].y; } //Max Y

        if (actualSkillCheck == 0) { actualSkillCheck = skillCheck + 25; }

        completeMinigame = false;
        imageFeatures.fillAmount = amountFill;

        imageFeatures.raycastPadding = Vector4.zero;

        initialCircle.SetActive(true);
        initialCircle.GetComponent<TwinkleBehaviour>().enabled = true;
        initialCircle.GetComponent<TwinkleBehaviour>().StartInit();

        initialCircle.transform.position = new Vector3(initialCircle.transform.position.x, positionOriginalYCircle, initialCircle.transform.position.z);
    }

    public void OnPointerMove (PointerEventData eventData) 
    {
        if (controllers._input.y < actualSkillCheck && Mouse.current.leftButton.isPressed == true)
        {

            imageFeatures.raycastPadding = permissiveImage;
            initialCircle.GetComponent<TwinkleBehaviour>().StopTwinkle();
            initialCircle.GetComponent<TwinkleBehaviour>().enabled = false;
            initialCircle.GetComponent<Image>().color = new Color(1, 1, 1, 0);

            if (controllers._input.y > skillCheck - 5) 
            { 
                skillCheck = controllers._input.y;
                initialCircle.transform.position = new Vector3(initialCircle.transform.position.x, controllers._input.y, initialCircle.transform.position.z);
            }
            actualSkillCheck = skillCheck + amountPermessive;

            imageFeatures.fillAmount = skillCheck / YToCompleteSkill;

            if (skillCheck >= YToCompleteSkill && completeMinigame == false)
            {
                completeMinigame = true;
                ISkillCheck minigameSkillCheck = minigame as ISkillCheck;

                minigameSkillCheck.FryPanAddPoints();
            }
        }
    }
}
