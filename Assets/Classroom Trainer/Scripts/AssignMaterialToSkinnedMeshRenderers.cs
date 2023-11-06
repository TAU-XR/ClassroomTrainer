using UnityEngine;

public class AssignMaterialToSkinnedMeshRenderers : MonoBehaviour
{
    // Public variable to assign the material from the Unity Editor
    public Material sharedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        // Validate if the material is set
        if (sharedMaterial == null)
        {
            Debug.LogError("No material assigned. Please assign a material in the Inspector.");
            return;
        }

        // Find all Skinned Mesh Renderers in the scene
        SkinnedMeshRenderer[] renderers = FindObjectsOfType<SkinnedMeshRenderer>();

        // Loop through all found Skinned Mesh Renderers and assign the specified material
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.material = sharedMaterial;
        }

        Debug.Log("Material has been assigned to all Skinned Mesh Renderers.");
    }
}
