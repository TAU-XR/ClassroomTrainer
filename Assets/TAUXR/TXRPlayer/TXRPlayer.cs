using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class TXRPlayer : TXRSingleton<TXRPlayer>
{
    [Header("Player Trackables")]
    [SerializeField] private Transform ovrRig;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform rightHandAnchor;
    [SerializeField] private Transform leftHandAnchor;
    private OVRManager _ovrManager;
    public Transform PlayerHead => playerHead;
    public Transform RightHand => rightHandAnchor;
    public Transform LeftHand => leftHandAnchor;

    [Header("Hand Tracking")]
    public TXRHand HandLeft;
    public TXRHand HandRight;

    [Header("Eye Tracking")]
    public TXREyeTracker EyeTracker;
    public bool IsEyeTrackingEnabled;
    public Transform FocusedObject => EyeTracker.FocusedObject;
    public Vector3 EyeGazeHitPosition => EyeTracker.EyeGazeHitPosition;
    public Transform RightEye => EyeTracker.RightEye;
    public Transform LeftEye => EyeTracker.LeftEye;


    [Header("Face Tracking")]
    [SerializeField] public OVRFaceExpressions ovrFace;
    public OVRFaceExpressions OVRFace => ovrFace;
    public bool IsFaceTrackingEnabled;

    // Color overlay
    [SerializeField] private MeshRenderer colorOverlayMR;

    // input handling
    private bool isLeftTriggerHolded = false;
    private bool isRightTriggerHolded = false;

    protected override void DoInAwake()
    {
        _ovrManager = GetComponentInChildren<OVRManager>();

        HandRight.Init();
        HandLeft.Init();

        if (IsEyeTrackingEnabled)
            EyeTracker.Init();
        if (IsFaceTrackingEnabled)
        {
            ovrRig.AddComponent<OVRFaceExpressions>();
        }
    }


    void Update()
    {
        HandRight.UpdateHand();
        HandLeft.UpdateHand();

        if (IsEyeTrackingEnabled)
        {
            EyeTracker.UpdateEyeTracker();
        }

        isLeftTriggerHolded = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > .7f;
        isRightTriggerHolded = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > .7f;
    }

    // covers player's view with color. 
    async public UniTask FadeToColor(Color targetColor, float duration)
    {
        if (duration == 0)
        {
            colorOverlayMR.material.color = targetColor;
            return;
        }

        Color currentColor = colorOverlayMR.material.color;
        if (currentColor == targetColor) return;

        float lerpTime = 0;
        while (lerpTime < duration)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / duration;
            colorOverlayMR.material.color = Color.Lerp(currentColor, targetColor, t);

            await UniTask.Yield();
        }
    }
    public void RepositionPlayer(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void RecenterView()
    {
        // IMPLEMENT
    }
    public void CalibrateFloorHeight()
    {
        // IMPLEMENT
    }
    public void SetPassthrough(bool state)
    {
        _ovrManager.isInsightPassthroughEnabled = state;
    }

    #region Controllers
    public bool IsTriggerPressedThisFrame(HandType handType)
    {
        switch (handType)
        {
            case HandType.Left:
                return isLeftTriggerHolded;
            case HandType.Right:
                return isRightTriggerHolded;
            case HandType.Any:
                return isLeftTriggerHolded || isRightTriggerHolded;
            case HandType.None:
                return false;
            default: return false;
        }
    }

    // TODO: add cancelation timer

    async public UniTask WaitForTriggerHold(HandType handType, float duration)
    {
        OVRInput.Controller selectedController = handType == HandType.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

        float holdingDuration = 0;
        while (holdingDuration < duration)
        {
            if (IsTriggerPressedThisFrame(handType))
            {
                holdingDuration += Time.deltaTime;
                OVRInput.SetControllerVibration(holdingDuration / duration, holdingDuration / duration, selectedController);
            }
            else
            {
                holdingDuration = 0;
                OVRInput.SetControllerVibration(0, 0, selectedController);
            }

            await UniTask.Yield();
        }
        OVRInput.SetControllerVibration(0, 0, selectedController);

    }
    #endregion

    #region Hands
    public Action PinchEnter(HandType handType)
    {
        switch (handType)
        {
            case HandType.Left:
                return HandLeft.PinchEnter;
            case HandType.Right:
                return HandRight.PinchEnter;
            case HandType.Any:
                return HandRight.PinchEnter;
            case HandType.None:
                return null;
            default: return null;
        }
    }

    public Action PinchExit(HandType handType)
    {
        switch (handType)
        {
            case HandType.Left:
                return HandLeft.PinchExit;
            case HandType.Right:
                return HandRight.PinchExit;
            case HandType.Any:
                return HandRight.PinchExit;
            case HandType.None:
                return null;
            default: return null;
        }
    }

    public bool IsPlayerPinchingThisFrame(HandType handType)
    {
        switch (handType)
        {
            case HandType.Left:
                return HandLeft.IsPlayerPinchingThisFrame();
            case HandType.Right:
                return HandRight.IsPlayerPinchingThisFrame();
            case HandType.Any:
                return HandRight.IsPlayerPinchingThisFrame() || HandLeft.IsPlayerPinchingThisFrame(); ;
            case HandType.None:
                return false;
            default: return false;
        }
    }

    async public UniTask WaitForPinchHold(HandType handType, float duration, bool waitUntilRelease = false)
    {
        float holdingDuration = 0;
        while (holdingDuration < duration)
        {
            if (IsPlayerPinchingThisFrame(handType))
            {
                holdingDuration += Time.deltaTime;
            }
            else
            {
                holdingDuration = 0;
            }
            await UniTask.Yield();
        }

        while (waitUntilRelease && IsPlayerPinchingThisFrame(handType))
        {
            Debug.Log("WAIT UNTIL RELEASE");
            await UniTask.Yield();
        }
    }

    public Transform GetHandFingerCollider(HandType handType, FingerType fingerType)
    {
        switch (handType)
        {
            case HandType.Left:
                return HandLeft.GetFingerCollider(fingerType);
            case HandType.Right:
                return HandRight.GetFingerCollider(fingerType);
            case HandType.Any:
                return HandLeft.GetFingerCollider(fingerType);
            case HandType.None:
                return null;
            default: return null;
        }
    }
    #endregion


}
