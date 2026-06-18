using UnityEngine;

public class KnobInteraction : MonoBehaviour
{
    public PanelManager manager;
    private bool isManual = false;

    [Header("Sound Section")]
    [SerializeField] private AudioClip [] knobSounds;
    [SerializeField] private float intervalBetweenSounds;
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = GameManager.instance._soundManager;
    }

    void OnMouseDown()
    {
        if (Time.timeScale == 0) return;
        if (manager.currentStep == PanelStep.ManualMode && !isManual)
        {
            isManual = true;
            ApplyRotation();
            manager.TryAdvanceStep(PanelStep.ManualMode);
            return;
        }

        if (manager.currentStep == PanelStep.AutoMode && isManual)
        {
            isManual = false;
            ApplyRotation();
            manager.TryAdvanceStep(PanelStep.AutoMode);
            manager.EnableReset();
            return;
        }
    }

    void ApplyRotation()
    {
        soundManager.ReproduceChainSounds(knobSounds, intervalBetweenSounds);
        transform.localRotation = Quaternion.Euler(0, isManual ? 90f : 0f, 0);
    }
    public void ResetKnob()
    {
        isManual = false;
        transform.localRotation = Quaternion.Euler(0, 0, 0); 
    }
}

