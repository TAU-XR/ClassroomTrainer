using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicVolumeChecker : MonoBehaviour
{
    private string micName;
    private AudioSource audioSource;
    private float[] samples = new float[2048]; // Increased sample size for 2 seconds
    private float threshold = 0.05f; // Volume threshold to determine if user is talking
    public bool isTalking = false;

    void Start()
    {
        // Check if we have at least one microphone
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = Microphone.Start(micName, true, 2, 44100); // Record 2 seconds worth of audio
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        audioSource.GetOutputData(samples, 0); // Get audio samples

        float maxVolume = 0;
        foreach (float sample in samples)
        {
            maxVolume = Mathf.Max(maxVolume, Mathf.Abs(sample)); // Capture the maximum volume in the samples
        }

        isTalking = maxVolume > threshold; // If the max volume exceeds the threshold, we assume the user is talking

        // Debug log to print the values
        Debug.Log($"Is Talking: {isTalking}, Max Volume: {maxVolume}");

        // You can also optionally move a GameObject based on this information, or trigger other events.
    }
}
