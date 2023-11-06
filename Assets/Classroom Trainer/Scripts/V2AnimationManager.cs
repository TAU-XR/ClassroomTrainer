using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class V2AnimationManager : MonoBehaviour

{
    public GameObject EyegazeObject;
    public GameObject Teacher;
    public GameObject Friend;
    public float LookAtTarget;
    public float IdleProb;
    public float LookAtTeacherProb;
    public float LookAtFriendProb;
    public float WritingProb; // Probability to enter Writing state
    public float GazeToggleInterval = 5f; // Time between gaze toggles
    private BipedIK bipedIK;
    private Animator animator; // Animator reference
    private float randomspeed;
    public AnimationCurve MoveGazelerpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    private enum State { Idle, LookAtTeacher, LookAtFriend, Writing }
    private State currentState;
    private AudioSource WritingSound;


    void Start()
    {
        MoveGazelerpCurve = MicVolumeMover.Instance.GazeCurve;
        randomspeed = 1.0f + Random.RandomRange(-0.2f, 0.2f);

        WritingSound = GameObject.Find("WritingSound").GetComponent<AudioSource>();
        // Normalize probabilities
        float totalProb = IdleProb + LookAtTeacherProb + LookAtFriendProb + WritingProb;
        IdleProb /= totalProb;
        LookAtTeacherProb /= totalProb;
        LookAtFriendProb /= totalProb;
        WritingProb /= totalProb; // Normalizing the Writing state probability as well

        LookAtTarget = 0;
        bipedIK = GetComponent<BipedIK>();
        bipedIK.solvers.lookAt.target = EyegazeObject.transform.Find("Nodder").transform;
        animator = GetComponent<Animator>();
        animator.SetFloat("WritingSpeed", randomspeed);
        StartCoroutine(ToggleState());
    }


    void Update()
    {
        if (bipedIK.solvers.lookAt != null)
        {
            bipedIK.solvers.lookAt.IKPositionWeight = LookAtTarget;
        }
    }

    IEnumerator ToggleState()
    {
        while (true)
        {
            // Increase the probability of staying in the same state
            float increaseProb = 0.2f;
            float currentIdleProb = currentState == State.Idle ? IdleProb + increaseProb : IdleProb;
            float currentLookAtTeacherProb = currentState == State.LookAtTeacher ? LookAtTeacherProb + increaseProb : LookAtTeacherProb;
            float currentLookAtFriendProb = currentState == State.LookAtFriend ? LookAtFriendProb + increaseProb : LookAtFriendProb;
            float currentWritingProb = currentState == State.Writing ? WritingProb + increaseProb : WritingProb;
            if (TXRPlayer.Instance.FocusedObject != null)
            {
                Debug.Log(TXRPlayer.Instance.FocusedObject.name);
                /*if (TXRPlayer.Instance.FocusedObject.name == gameObject.name)
                    {
                    currentLookAtTeacherProb += 5f;
                    }*/
                if (TXRPlayer.Instance.FocusedObject.transform == gameObject.transform)
                {
                    currentLookAtTeacherProb += 5f;
                }
            }
            if (MicVolumeMover.Instance.IsTalking)
            {
                currentLookAtTeacherProb += 10f;
            }
            // Normalize probabilities after modifying
            float totalProb = currentIdleProb + currentLookAtTeacherProb + currentLookAtFriendProb + currentWritingProb;
            currentIdleProb /= totalProb;
            currentLookAtTeacherProb /= totalProb;
            currentLookAtFriendProb /= totalProb;
            currentWritingProb /= totalProb;

            // Determine next state
            float randomValue = Random.Range(0f, 1f);
            if (randomValue < currentIdleProb)
                currentState = State.Idle;
            else if (randomValue < currentIdleProb + currentLookAtTeacherProb)
                currentState = State.LookAtTeacher;
            else if (randomValue < currentIdleProb + currentLookAtTeacherProb + currentLookAtFriendProb)
                currentState = State.LookAtFriend;
            else
                currentState = State.Writing;

            Debug.Log((currentIdleProb, currentLookAtTeacherProb, currentLookAtFriendProb, currentWritingProb));







            switch (currentState)
            {
                case State.Writing: // New case for Writing state
                    animator.SetBool("Writing", true);
                    StartCoroutine(LerpLookAtTarget(0f));
                    StartCoroutine(LerpNoddValue(0f)); // Set Nodd to 0
                    StartCoroutine(LerpWritingSound(0.7f));
                    break;
                case State.Idle:
                    animator.SetBool("Writing", false);
                    StartCoroutine(LerpLookAtTarget(0f));
                    StartCoroutine(LerpNoddValue(0f)); // Set Nodd to 0
                    StartCoroutine(LerpWritingSound(0f));
                    break;
                case State.LookAtTeacher:
                    StartCoroutine(LerpWritingSound(0f));
                    // Stop previous FollowTeacher coroutine if it's running
                    if (followTeacherCoroutine != null)
                    {
                        StopCoroutine(followTeacherCoroutine);
                        followTeacherCoroutine = null;
                    }
                    if (TXRPlayer.Instance.FocusedObject != null)
                    {
                        Debug.Log(TXRPlayer.Instance.FocusedObject.name);
                        if (TXRPlayer.Instance.FocusedObject.transform == gameObject.transform)

                        {
                            StartCoroutine(LerpNoddValue(1f));
                        }
                        else
                        {
                            StartCoroutine(LerpNoddValue(0f));
                        }
                    }

                    StartCoroutine(LerpLookAtTarget(1f));
                    animator.SetBool("Writing", false);
                    // StartCoroutine(LerpWritingSound(0f));

                    yield return StartCoroutine(MoveGaze(Teacher.transform.position)); // Wait for MoveGaze to finish
                    // Start a new coroutine to follow the Teacher's position
                    // Set Nodd to 1
                    followTeacherCoroutine = StartCoroutine(FollowTeacher());
                    break;
                case State.LookAtFriend:
                    StartCoroutine(LerpWritingSound(0f));
                    animator.SetBool("Writing", false);
                    Vector3 friendCenterOfMass = Friend.GetComponent<Collider>().bounds.center;
                    StartCoroutine(MoveGaze(friendCenterOfMass));
                    StartCoroutine(LerpLookAtTarget(1f));
                    StartCoroutine(LerpNoddValue(0f));
                    // Set Nodd to 0
                    break;

            }
            yield return new WaitForSeconds(GazeToggleInterval + Random.Range(0f, 1f));
        }
    }
    private Coroutine followTeacherCoroutine; // Keep track of the coroutine

    IEnumerator FollowTeacher()
    {
        float startTime;
        Vector3 startPosition;
        while (currentState == State.LookAtTeacher)
        {
            startTime = Time.time;
            startPosition = EyegazeObject.transform.position;
            while (Time.time - startTime < 1f)
            {
                float lerpFactor = (Time.time - startTime) / 1f;
                EyegazeObject.transform.position = Vector3.Lerp(startPosition, Teacher.transform.position, lerpFactor);
                yield return null;
            }
            EyegazeObject.transform.position = Teacher.transform.position;
        }
    }
    IEnumerator LerpWritingSound(float targetVolume)
    {
        float startVolume = WritingSound.volume;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            WritingSound.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }

        // To ensure the target volume is precisely set at the end of the lerp.
        WritingSound.volume = targetVolume;
    }

    IEnumerator LerpNoddValue(float targetValue)
    {
        float startTime = Time.time;
        Animator nodderAnimator = EyegazeObject.transform.Find("Nodder").GetComponent<Animator>();
        float lerpdurarion = targetValue == 0f ? 0.1f : 2f;
        float startValue = nodderAnimator.GetFloat("Nodd");
        while (Time.time - startTime < lerpdurarion)
        {
            float lerpFactor = (Time.time - startTime) / lerpdurarion;
            float value = Mathf.Lerp(startValue, targetValue, lerpFactor);
            nodderAnimator.SetFloat("Nodd", value);
            yield return null;
        }
        nodderAnimator.SetFloat("Nodd", targetValue);
    }



    // IEnumerator ToggleWriting()
    // {
    // while (currentState == State.Idle)
    //  {
    //    bool writingValue = Random.Range(0f, 1f) < WritingProb;
    //    animator.SetBool("Writing", writingValue);
    //   yield return new WaitForSeconds(ToggleInterval);
    //    // }
    // }

    IEnumerator LerpLookAtTarget(float targetValue)
    {
        float startTime = Time.time;
        float startValue = LookAtTarget;
        while (Time.time - startTime < 0.5f)
        {
            float lerpFactor = (Time.time - startTime) / 0.5f;
            LookAtTarget = Mathf.Lerp(startValue, targetValue, lerpFactor);
            yield return null;

        }
        LookAtTarget = targetValue;
    }

    IEnumerator MoveGaze(Vector3 targetPosition)
    {
        float startTime = Time.time;
        Vector3 startPosition = EyegazeObject.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);

        while (Time.time - startTime < 1f * distance)
        {
            float timePassed = Time.time - startTime;
            float lerpFactor = timePassed / (1f * distance);
            float curveValue = MoveGazelerpCurve.Evaluate(lerpFactor);
            EyegazeObject.transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, curveValue);
            yield return null;
        }

        EyegazeObject.transform.position = targetPosition;
    }
}