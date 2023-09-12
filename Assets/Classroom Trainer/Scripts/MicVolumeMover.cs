using UnityEngine;
using System.Collections; // For IEnumerator and Coroutine
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MicVolumeMover : TXRSingleton<MicVolumeMover>
{
    private string micName;
    private AudioSource audioSource;
    private float[] samples = new float[2048]; // Sample size for 2 seconds
    private float highestVolume = 0.1f;
    public float threshold = 0.05f; // Default threshold; adjust in Inspector if needed
    public bool IsTalking = false;
    public bool yell = false;
    public AudioMixer mixer;
    public AnimationCurve GazeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    protected override void DoInAwake()
    {
        
    }

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
        mixer.SetFloat("Volume", -80);
        // Start the coroutine to print maximum volume
        StartCoroutine(PrintMaxVolumeAndUpdateTalkingStatus());
        
    }

    void Update()
    {
        audioSource.GetOutputData(samples, 0); // Get audio samples

        float average = 0;
        foreach (float sample in samples)
        {
            average += Mathf.Abs(sample);
        }

        average /= samples.Length;

        //float moveY = Mathf.Clamp01(average / highestVolume);
        //transform.position = new Vector3(transform.position.x, moveY, transform.position.z);
    }

    IEnumerator PrintMaxVolumeAndUpdateTalkingStatus()
    {
        while (true)
        {
            // Wait for two seconds
            yield return new WaitForSeconds(2);

            // Calculate the maximum volume over the last two seconds
            float maxVolume = 0;
            foreach (float sample in samples)
            {
                maxVolume = Mathf.Max(maxVolume, Mathf.Abs(sample));
            }

            // Update the IsTalking status
            IsTalking = maxVolume > threshold;

            // Print the maximum volume and talking status
            Debug.Log($"Max Volume over the last 2 seconds: {maxVolume}, Is Talking: {IsTalking}");
        }
    }
}
