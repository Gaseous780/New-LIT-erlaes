using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public PanelManager manager;
    public bool isOn = true;

    [Header("Visual Settings")]
    public GameObject lightObject;
    public Color colorOn = Color.white;
    public Color colorOff = Color.gray;

    [Header("Button Animation")]
    public float pushDepth = 0.02f;

    private Renderer lightRenderer;
    private Vector3 initialPosition;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip buttonSound;
    private SoundManager soundManager;

    void Start()
    {
        initialPosition = transform.localPosition;

        if (lightObject != null)
        {
            lightRenderer = lightObject.GetComponent<Renderer>();
        }
        else
        {
            lightRenderer = GetComponent<Renderer>();
        }

        UpdateVisuals();

        soundManager = GameManager.instance._soundManager;
    }

    void OnMouseDown()
    {
        if (manager.currentStep != PanelStep.LightsOn) return;
        if (Time.timeScale == 0) return;

        isOn = !isOn;

        transform.localPosition = initialPosition + new Vector3(0, -pushDepth, 0);

        UpdateVisuals();
        manager.CheckLights();
        soundManager.ReproduceSound(buttonSound);

    }

    void OnMouseUp()
    {
        transform.localPosition = initialPosition;
    }

    public void UpdateVisuals()
    {
        if (lightRenderer != null)
        {
            lightRenderer.material.color = isOn ? colorOn : colorOff;
        }
    }
}
