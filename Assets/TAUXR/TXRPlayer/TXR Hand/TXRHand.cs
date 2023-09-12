using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HandType { Left, Right, None, Any }
public enum FingerType { Thumb = 19, Index = 20, Middle = 21, Ring = 22, Pinky = 23 }

public class TXRHand : MonoBehaviour
{
    public HandType HandType;
    public Action PinchEnter, PinchExit;

    private OVRSkeleton _ovrSkeleton;
    private List<HandCollider> _handColliders;

    private Pincher _pincher;
    private bool _isPinching = false;
    private const float PINCH_EXIT_THRESHOLD = .97f;
    private const float PINCH_ENTER_THRESHOLD = .99f;


    public void Init()
    {
        _ovrSkeleton = GetComponentInChildren<OVRSkeleton>();
        _pincher = GetComponentInChildren<Pincher>();

        _handColliders = GetComponentsInChildren<HandCollider>().ToList();
        foreach (HandCollider hc in _handColliders)
        {
            hc.Init(_ovrSkeleton);
        }

        _pincher.Init(_ovrSkeleton);
    }

    public void UpdateHand()
    {
        if (_ovrSkeleton.IsDataHighConfidence)
        {
            foreach (HandCollider hc in _handColliders)
            {
                hc.UpdateHandCollider();
            }

            _pincher.UpdatePincher();
            HandlePinchEvents();
        }
    }

    private void HandlePinchEvents()
    {
        if (!_isPinching)
        {
            if (_pincher.Strength > PINCH_ENTER_THRESHOLD)
            {
                _isPinching = true;
                PinchEnter?.Invoke();
            }

        }
        else
        {
            if (_pincher.Strength < PINCH_EXIT_THRESHOLD)
            {
                _isPinching = false;
                PinchExit?.Invoke();
            }
        }
    }

    public bool IsPlayerPinchingThisFrame()
    {
        return _isPinching;
    }

    public Transform GetFingerCollider(FingerType fingerType)
    {
        foreach (HandCollider hc in _handColliders)
        {
            if (hc.fingerIndex == (int)fingerType)
            {
                return hc.transform;
            }
        }

        return null;
    }
}

