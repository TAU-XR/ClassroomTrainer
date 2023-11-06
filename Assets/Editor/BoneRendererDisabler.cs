using UnityEngine;
using UnityEditor;

public class BoneRendererToggler : EditorWindow
{
    [MenuItem("Tools/Bone Renderers/Enable")]
    private static void EnableBoneRenderers()
    {
        ToggleBoneRenderers(true);
    }

    [MenuItem("Tools/Bone Renderers/Disable")]
    private static void DisableBoneRenderers()
    {
        ToggleBoneRenderers(false);
    }

    private static void ToggleBoneRenderers(bool state)
    {
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            // Check if the GameObject has a component with the name "BoneRenderer"
            Component component = go.GetComponent("BoneRenderer");

            if (component is Behaviour behaviourComponent)
            {
                // Set the enabled state of the component
                behaviourComponent.enabled = state;
            }
        }

        Debug.Log(state ? "All BoneRenderer components have been enabled." : "All BoneRenderer components have been disabled.");
    }
}
