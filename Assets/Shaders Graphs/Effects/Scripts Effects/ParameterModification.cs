using UnityEngine;

public class ParameterModification : MonoBehaviour
{
    public Renderer targetRenderer;
    [SerializeField] private Renderer windowRenderer;

    private MaterialPropertyBlock mpb;
    private MaterialPropertyBlock mpbWindow;

    [SerializeField] private float amountToInverseExtra;

    private void Start()
    {
        mpb = new MaterialPropertyBlock();
        mpbWindow = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (targetRenderer != null)
        {
            targetRenderer.GetPropertyBlock(mpb);

            if ((targetRenderer.transform.position.magnitude - transform.position.magnitude) + amountToInverseExtra < 0 && mpb.GetFloat("_Inverse") == 0)
            {
                mpb.SetFloat("_Inverse", 1);
            }
            else if ((targetRenderer.transform.position.magnitude - transform.position.magnitude) - amountToInverseExtra/2 > 0 && mpb.GetFloat("_Inverse") == 1)
            {
                mpb.SetFloat("_Inverse", 0);
            }

            mpb.SetVector("_SpherePos", transform.position);
            //Debug.Log((targetRenderer.transform.position.magnitude - transform.position.magnitude) + amountToInverseExtra + "     " + mpb.GetFloat("_Inverse"));
            targetRenderer.SetPropertyBlock(mpb);
        }

        if (windowRenderer != null)
        {
            windowRenderer.GetPropertyBlock(mpb);

            mpbWindow.SetVector("_PlayerPosition", transform.localPosition);
            windowRenderer.SetPropertyBlock(mpbWindow);
        }
    }
}
