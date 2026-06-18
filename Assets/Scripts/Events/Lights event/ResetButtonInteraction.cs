using UnityEngine;

public class ResetButtonInteraction : MonoBehaviour
{
    public PanelManager manager;
    public bool isEnabled = false;
    public float depth;

    [Header("Visuals")]
    public Renderer buttonRenderer;
    public Color disabledColor = new Color(0.4f, 0f, 0f); 
    public Color enabledColor = Color.red;

    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
        ApplyColor(disabledColor);
    }

    public void SetEnabled(bool status)
    {
        isEnabled = status;
        if (status)
        {
            ApplyColor(enabledColor);
        }
        else
        {
            ApplyColor(disabledColor);
        }
    }

    void ApplyColor(Color targetColor)
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = targetColor;
        }
    }

    void OnMouseDown()
    {
        if (!isEnabled) return;
        if (Time.timeScale == 0) return;

        transform.localPosition = initialPos + new Vector3(0, -depth, 0);
        manager.OnFixComplete.Invoke();
        manager.ResetPuzzleData();
        SetEnabled(false);
    }

    void OnMouseUp()
    {
        transform.localPosition = initialPos;
    }
}
