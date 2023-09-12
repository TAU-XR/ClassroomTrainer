using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading;

public class TXRPlayer : TXRSingleton<TXRPlayer>
{
    [Header("Player Trackables")] [SerializeField]
    private Transform ovrRig;

    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform rightHandAnchor;
    [SerializeField] private Transform leftHandAnchor;
    private OVRManager _ovrManager;
    public Transform PlayerHead => playerHead;
    public Transform RightHand => rightHandAnchor;
    public Transform LeftHand => leftHandAnchor;

    [Header("Hand Tracking")] public TXRHand HandLeft;
    public TXRHand HandRight;

    [Header("Eye Tracking")] public TXREyeTracker EyeTracker;
    public bool IsEyeTrackingEnabled;
    public Transform FocusedObject => EyeTracker.FocusedObject;
    public Vector3 EyeGazeHitPosition => EyeTracker.EyeGazeHitPosition;
    public Transform RightEye => EyeTracker.RightEye;
    public Transform LeftEye => EyeTracker.LeftEye;


    [Header("Face Tracking")] [SerializeField]
    public OVRFaceExpressions ovrFace;

    public OVRFaceExpressions OVRFace => ovrFace;
    public bool IsFaceTrackingEnabled;

    // Color overlay
    [SerializeField] private MeshRenderer colorOverlayMR;

    public ControllersInputManager ControllersInputManager;
    public PinchingInputManager PinchingInputManager;

    protected override void DoInAwake()
    {
        _ovrManager = GetComponentInChildren<OVRManager>();

        HandRight.Init();
        HandLeft.Init();

        ControllersInputManager = new ControllersInputManager();
        PinchingInputManager = new PinchingInputManager(HandLeft, HandRight);

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
    }

    // covers player's view with color. 
    public async UniTask FadeViewToColor(Color targetColor, float duration)
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

    #region Hands

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