using UnityEngine;
using UnityEditor;

public class OptimizeMaterialsAndMeshes : EditorWindow
{
    [MenuItem("Tools/Optimize Materials and Meshes")]
    public static void Optimize()
    {
        // Iterate through all selected GameObjects
        foreach (GameObject selectedGameObject in Selection.gameObjects)
        {
            ApplyOptimizationsRecursively(selectedGameObject);
        }
        Debug.Log("Optimization complete!");
    }

    private static void ApplyOptimizationsRecursively(GameObject obj)
    {
        // Disable "Update When Offscreen" for Skinned Mesh Renderers
        SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.updateWhenOffscreen = false;
        }

        // Go through all materials and apply changes
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.sharedMaterials;
            foreach (Material mat in materials)
            {
                if (mat != null)
                {
                    // Ensure the shader has the necessary properties before modifying
                    if (mat.HasProperty("_SpecularHighlights"))
                    {
                        mat.SetInt("_SpecularHighlights", 0);  // Disable Specular Highlights
                    }
                    if (mat.HasProperty("_GlossyReflections"))
                    {
                        mat.SetInt("_GlossyReflections", 0);   // Disable Glossy Reflections
                    }

                    // Enable GPU Instancing
                    mat.enableInstancing = true;
                }
            }
        }

        // Recur for all children
        foreach (Transform child in obj.transform)
        {
            ApplyOptimizationsRecursively(child.gameObject);
        }
    }
}
