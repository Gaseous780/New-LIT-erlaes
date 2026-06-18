using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum PanelStep { ManualMode, LeverUp, LightsOn, AutoMode, Completed }
public class PanelManager : MonoBehaviour
{
    public PanelStep currentStep = PanelStep.ManualMode;

    [Header("Reset Button")]
    public ResetButtonInteraction resetButton;

    [Header("Lights Configuration")]
    public List<LightSwitch> switches;

    [Header("Step Indicators")]
    public List<Renderer> stepLights;
    public Color darkGreen = new Color(0, 0.2f, 0);
    public Color lightGreen = Color.green;

    [Header("Output Events")]
    public UnityEvent OnFixComplete;

    [Header("Puzzle Components")]
    public LeverInteraction leverScript; 
    public KnobInteraction knobScript;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip buttonResetSound;
    [SerializeField] private AudioClip powerOnSound;
    [SerializeField] private AudioClip powerOffSound;
    [SerializeField] private AudioClip closePanelSound;
    private SoundManager soundManager;

    void Start()
    {
        InitializeStepLights();
        SetupRandomLights();

        soundManager = GameManager.instance._soundManager;
    }

    void InitializeStepLights()
    {
        foreach (var lightRend in stepLights)
        {
            lightRend.material.color = darkGreen;
        }
    }

    void UpdateStepLights()
    {
        int currentStepIndex = (int)currentStep;

        for (int i = 0; i < stepLights.Count; i++)
        {
            if (i < currentStepIndex)
            {
                stepLights[i].material.color = lightGreen;
            }
            else
            {
                stepLights[i].material.color = darkGreen;
            }
        }
    }

    public void TryAdvanceStep(PanelStep step)
    {
        if (currentStep == step)
        {
            if (step == PanelStep.LightsOn)
            {
                CheckLights();
            }
            else
            {
                currentStep++;
                UpdateStepLights();
            }
        }
    }
    public void EnableReset()
    {
        currentStep = PanelStep.Completed;
        UpdateStepLights();

        if (resetButton != null)
        {
            resetButton.SetEnabled(true);
        }
    }

    public void CheckLights()
    {
        if (currentStep != PanelStep.LightsOn) return;

        bool allOn = true;
        foreach (var s in switches)
        {
            if (!s.isOn) allOn = false;
        }

        if (allOn)
        {
            currentStep = PanelStep.AutoMode;
            UpdateStepLights();
        }
    }

    void SetupRandomLights()
    {
        if (switches == null || switches.Count < 5) return;
        foreach (var s in switches) s.isOn = true;
        int lightsToTurnOff = Random.Range(2, 6);
        List<LightSwitch> tempSwitches = new List<LightSwitch>(switches);
        for (int i = 0; i < lightsToTurnOff; i++)
        {
            int randomIndex = Random.Range(0, tempSwitches.Count);
            tempSwitches[randomIndex].isOn = false;
            tempSwitches.RemoveAt(randomIndex);
        }
        foreach (var s in switches) s.UpdateVisuals();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
        }
    }
    public void ResetPuzzleData()
    {
        currentStep = PanelStep.ManualMode;

        foreach (Renderer lightRend in stepLights)
        {
            if (lightRend != null) lightRend.material.color = darkGreen;
        }

        if (leverScript != null)
        {
            leverScript.ResetLever();
        }
        if (knobScript != null)
        {
            knobScript.ResetKnob();
        }

        SetupRandomLights();

        soundManager.ReproduceSound(closePanelSound);
        soundManager.ReproduceSound(powerOnSound);
    }
}
