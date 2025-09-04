using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class Gpu : MonoBehaviour
{
    public void Awake()
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}
