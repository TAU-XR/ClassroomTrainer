using UnityEngine;

// Determines the player's position and orientation in scenes.
public class PlayerScenePositioner : MonoBehaviour
{
    private void Awake()
    {
        // destroy the OVRManager and all other objects when scene is loaded.
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
