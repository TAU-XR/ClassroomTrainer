using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource talkingSound; // Assign this through the inspector or find it via code
    public float volumeFadeDuration = 1f; // Duration over which the volume will lerp to 0
    private float targetVolume = 1f; // Target volume to lerp to

    private void Start()
    {
        if (talkingSound == null)
        {
            Debug.LogError("SoundManager: Talking sound AudioSource has not been assigned.");
        }
    }

    private void Update()
    {
        // Check if MicVolumeMover instance is talking
        if (MicVolumeMover.Instance != null && MicVolumeMover.Instance.IsTalking)
        {
            // Start the volume fade coroutine if not already running
            if (targetVolume != 0f)
            {
                StartCoroutine(FadeVolume(talkingSound, 0f, volumeFadeDuration));
            }
        }
    }

    private IEnumerator FadeVolume(AudioSource audioSource, float targetVol, float duration)
    {
        targetVolume = targetVol;
        float currentTime = 0;
        float startVolume = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVol, currentTime / duration);
            yield return null;
        }

        audioSource.volume = targetVol; // Ensure the volume is set to the target value at the end
    }
}
