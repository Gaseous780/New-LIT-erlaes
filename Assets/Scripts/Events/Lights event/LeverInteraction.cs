using UnityEngine;

public class LeverInteraction : MonoBehaviour
{
    public PanelManager manager;
    [Header("Lever path")]
    public float distanceToMove = 0.5f; 
    public float sensitivity = 0.8f;

    private float minZ;
    private float maxZ;
    private bool wasTriggered = false;
    private Vector3 lastMousePosition;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
        minZ = transform.localPosition.z;
        maxZ = minZ + distanceToMove;
    }

    void OnMouseDown()
    {
        if (Time.timeScale == 0) return;
        lastMousePosition = Input.mousePosition;

    }

    void OnMouseDrag()
    {
        if (manager.currentStep != PanelStep.LeverUp) return;

        Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        float moveAmount = deltaMouse.y * sensitivity * Time.deltaTime;
        float newZ = Mathf.Clamp(transform.localPosition.z + moveAmount, minZ, maxZ);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);


        if (newZ >= maxZ - 0.05f && !wasTriggered)
        {
            wasTriggered = true;
            manager.TryAdvanceStep(PanelStep.LeverUp);
        }
    }
    public void ResetLever()
    {
        wasTriggered = false;
        transform.localPosition = startPos;
    }
}
