using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ButtonColliderType { Press, Hover }

// switch to work with unity events
public class ToucherDetector : MonoBehaviour
{
    public UnityEvent<Transform> ToucherEnter;
    public UnityEvent<Transform> ToucherExited;
    public UnityEvent HeadEnter;
    public UnityEvent HeadExit;
    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Toucher":
                ToucherEnter.Invoke(other.transform);
                break;
            case "Head":
                HeadEnter.Invoke();
                break;
            default: return;
        }
        /*
         * TODO: Test switch, if works: remove this and implement down.
        if (!other.CompareTag("Toucher")) return;

        ToucherEnter.Invoke(other.transform);*/
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Toucher":
                ToucherExited.Invoke(other.transform);
                break;
            case "Head":
                HeadExit.Invoke();
                break;
            default: return;
        }

       // if (!other.CompareTag("Toucher")) return;

       // ToucherExited.Invoke(other.transform);
    }
}
