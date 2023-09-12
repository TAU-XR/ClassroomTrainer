using UnityEngine;

public class SinWaveMovement : MonoBehaviour
{
    public float frequency = 1.0f;  // Frequency of the sine wave
    public float maxMagnitude = 1.0f; // Maximum magnitude of the sine wave (how tall it is)
    public float rampUpSpeed = 0.1f; // Speed at which the magnitude increases
    public bool enableSineMovement = true; // Enable or disable sine wave movement

    private float currentMagnitude = 0f; // Current magnitude (starts from zero)
    private float previousY = 0;

    void Update()
    {
        if (enableSineMovement)
        {
            // Gradually increase the magnitude towards the maximum
            currentMagnitude = Mathf.MoveTowards(currentMagnitude, maxMagnitude, rampUpSpeed * Time.deltaTime);

            float newY = Mathf.Sin(Time.time * frequency) * currentMagnitude;
            transform.position = transform.position + new Vector3(0, newY - previousY, 0);
            previousY = newY;
        }
        else
        {
            // Reset the current magnitude when the movement is disabled
            currentMagnitude = 0f;
            previousY = 0;
        }
    }
}
