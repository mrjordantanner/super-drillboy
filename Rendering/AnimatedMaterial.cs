using UnityEngine;

public class AnimatedMaterial : MonoBehaviour
{
    public Vector2 frameOffset = new Vector2(0.125f, 0);

    [Header("Static Speed")]
    public float frameDuration = 0.02f;

    MeshRenderer meshRenderer;
    Material material;
    Vector2 offset;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;

        InvokeRepeating(nameof(UpdateFrame), 0, frameDuration);
    }

    public void UpdateMaterial(Material mat)
    {
        material = mat;
        meshRenderer.material = material;
    }

    void UpdateFrame()
    {
        offset = material.mainTextureOffset;
        offset += frameOffset;
        material.mainTextureOffset = offset;
    }
}
