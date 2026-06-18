using UnityEngine;

public class GrassModification : MonoBehaviour
{
    public Renderer targetRenderer;

    private MaterialPropertyBlock mpb;

    private void Start()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetVector("_PlayerPos", transform.position);
        targetRenderer.SetPropertyBlock(mpb);
    }
}
