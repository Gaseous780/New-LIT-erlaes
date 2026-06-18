using UnityEngine;

public class MoldBehaviour : MonoBehaviour
{
    private bool isComplete;

    [SerializeField] private Material materialComplete;
    private Material materialStart;

    private MeshRenderer renderers;

    private void Awake()
    {
        renderers = GetComponent<MeshRenderer>();
        materialStart = renderers.material;
    }

    private void OnEnable()
    {
        isComplete = false;
        renderers.material = materialStart;
    }

    public bool CompleteMold()
    {
        if (isComplete == false)
        {
            isComplete = true;
            renderers.material = materialComplete;
            return true;
        }

        return false;
    }
}
