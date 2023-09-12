using UnityEngine;

public class NoddingEffect : MonoBehaviour
{
    public bool IsNodding = true; // Public variable to control nodding behavior
    public float minNodSpeed = 1f; // Minimum speed of nodding
    public float maxNodSpeed = 3f; // Maximum speed of nodding
    public float minNodAmount = 0.05f; // Minimum distance the teacher object will move up and down
    public float maxNodAmount = 0.15f; // Maximum distance the teacher object will move up and down
    public float changeInterval = 3f; // Interval in seconds to change the nodding behavior
    public float lerpSpeed = 0.5f; // Speed at which the nodding behavior changes

    private Vector3 startingPosition;
    private float targetNodSpeed;
    private float targetNodAmount;
    private float currentNodSpeed;
    private float currentNodAmount;
    private float nextChangeTime;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        SetRandomNoddingTargets();
        currentNodSpeed = targetNodSpeed;
        currentNodAmount = targetNodAmount;
        nextChangeTime = Time.time + changeInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsNodding)
        {
            if (Time.time > nextChangeTime)
            {
                SetRandomNoddingTargets();
                nextChangeTime = Time.time + changeInterval;
            }
        }
        else
        {
            // If not nodding, set targets to zero
            targetNodAmount = 0f;
            targetNodSpeed = 0f;
        }

        // Smoothly interpolate towards the target nodding behavior
        currentNodSpeed = Mathf.Lerp(currentNodSpeed, targetNodSpeed, Time.deltaTime * lerpSpeed);
        currentNodAmount = Mathf.Lerp(currentNodAmount, targetNodAmount, Time.deltaTime * lerpSpeed);

        // Calculating the new position using a sine wave to create a smooth up and down motion
        float newY = startingPosition.y + Mathf.Sin(Time.time * currentNodSpeed) * currentNodAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void SetRandomNoddingTargets()
    {
        // Randomly set the target nodding behavior
        targetNodSpeed = Random.Range(minNodSpeed, maxNodSpeed);
        targetNodAmount = Random.Range(minNodAmount, maxNodAmount);

        // Occasionally set the targetNodAmount to zero to stop nodding
        if (Random.value < 0.2f)
        {
            targetNodAmount = 0f;
        }
    }
}
