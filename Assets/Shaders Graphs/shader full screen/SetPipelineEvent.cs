using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetPipelineEvent : MonoBehaviour
{
    [SerializeField] private UniversalRenderPipelineAsset URPA;
    [SerializeField] private int indexPipe;

    private void OnTriggerEnter(Collider other)
    {
        SetPipeline(indexPipe);
    }

    public void SetPipeline(int mode)
    {
        UniversalRenderPipelineAsset urpAsset = Resources.Load<UniversalRenderPipelineAsset>(URPA.name);
        Type typ = typeof(UniversalRenderPipelineAsset);
        FieldInfo type = typ.GetField("m_DefaultRendererIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        int currentRendererIndex = (int)type.GetValue(urpAsset);
        int newRendererIndex = currentRendererIndex == 0 ? 1 : 0; //Simple toggle between index 0 and 1
        type.SetValue(urpAsset, mode);
    }
}
