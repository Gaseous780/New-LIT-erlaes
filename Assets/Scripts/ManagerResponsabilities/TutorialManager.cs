
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("References")]
    public PlayerMovement playerScript;
    public GameObject tutorialPanel;
    public GameObject UIElements;
    public GameObject tempuraGuide;
    public TextMeshProUGUI tutorialText;
    public Interaction interactionScript;

    public enum TutorialStep
    {
        Welcome,
        GrabClient,
        GuideClient,
        SeatClient,
        TakeOrder,
        GoToKitchen,
        ExplainTempura,
        Finished
    }
    public TutorialStep currentStep;

    private bool waitingForSpace = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        currentStep = TutorialStep.Welcome;
        StartStep();
    }

    void Update()
    {
        if (waitingForSpace && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AdvanceStep();
        }
    }

    public void StartStep()
    {
        UIElements.SetActive(false);
        tutorialPanel.SetActive(true);
        waitingForSpace = true;
        playerScript.canMove = false;
        interactionScript.canInteract = false;
        Time.timeScale = 0f;

        switch (currentStep)
        {
            case TutorialStep.Welcome:
                tutorialText.text = "Welcome to your Restaurant Chaos! \n [Press Space to continue]";
                break;
            case TutorialStep.GrabClient:
                tutorialText.text = "A client is here! Go and grab him. \n [Press Space to continue]";
                break;
            case TutorialStep.GuideClient:
                tutorialText.text = "Now take them to a free table. \n [Press Space to continue]";
                break;
            case TutorialStep.SeatClient:
                tutorialText.text = "Wait for the client to make his order. \n [Press Space to continue]";
                break;
            case TutorialStep.TakeOrder:
                tutorialText.text = "Now go to the kitchen. You can see the order in the bottom left of the screen. \n [Press Space to continue]";
                break;
            case TutorialStep.GoToKitchen:
                tutorialText.text = "You can cook rice by interacting with the pan. \n [Press Space to continue]";
                break;
            case TutorialStep.ExplainTempura:
                tutorialText.text = "To make tempura you need the follow the order below. \n [Press Space to continue]";
                tempuraGuide.SetActive(true);
                break;
            case TutorialStep.Finished:
                tutorialText.text = "And be careful, anything can happen. \n [Press Space to go to Main Menu]";
                tempuraGuide.SetActive(false);
                break;
        }
    }

    void AdvanceStep()
    {
        // Si ya terminamos, volvemos al menú
        if (currentStep == TutorialStep.Finished)
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }

        waitingForSpace = false;
        tutorialPanel.SetActive(false);
        UIElements.SetActive(true);
        playerScript.canMove = true;
        interactionScript.canInteract = true;
        Time.timeScale = 1f;
        if (currentStep == TutorialStep.Welcome)
        {
            currentStep = TutorialStep.GrabClient;
            StartStep();
        }
        else if (currentStep == TutorialStep.GoToKitchen)
        {
            currentStep = TutorialStep.ExplainTempura;
            StartStep();
        }
        else if (currentStep == TutorialStep.ExplainTempura)
        {
            currentStep = TutorialStep.Finished;
            StartStep();
        }
    }

    public void OnActionTriggered(TutorialStep stepFinished)
    {
        if (tutorialPanel.activeSelf) return;

        if (currentStep == stepFinished)
        {
            if (currentStep == TutorialStep.GrabClient)
                currentStep = TutorialStep.GuideClient;
            else if (currentStep == TutorialStep.GuideClient)
                currentStep = TutorialStep.SeatClient;
            else if (currentStep == TutorialStep.SeatClient)
                currentStep = TutorialStep.TakeOrder;
            else if (currentStep == TutorialStep.TakeOrder)
                currentStep = TutorialStep.GoToKitchen;

            StartStep();
        }
    }
}