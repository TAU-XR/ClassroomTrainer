using OVR.OpenVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TXREyeTracker : MonoBehaviour
{
    public Transform FocusedObject => _focusedObject;
    public Vector3 EyeGazeHitPosition => _eyeGazeHitPosition;
    public Transform RightEye => _rightEye;
    public Transform LeftEye => _leftEye;


    [SerializeField] private Transform _rightEye;
    [SerializeField] private Transform _leftEye;
    private const float EYERAYMAXLENGTH = 100000;
    private const float EYETRACKINGCONFIDENCETHRESHOLD = .5f;
    private Vector3 NOTTRACKINGVECTORVALUE = new Vector3(-1f, -1f, -1f);
    private OVREyeGaze _ovrEyeR;
    private Transform _focusedObject;
    private Vector3 _eyeGazeHitPosition;
    private int _eyeTrackingIgnoreLayer = 7;
    LayerMask _eyeTrackingLayerMask = ~(1 << 7);

    public void Init()
    {
        if (_rightEye.TryGetComponent(out OVREyeGaze er))
        {
            _ovrEyeR = er;
        }

        _focusedObject = null;
        _eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;
    }

    public void UpdateEyeTracker()
    {
        // don't track if there is no OVREye componenet (enough to check only on one eye).
        if (_ovrEyeR == null) return;

        // don't track on low confidence.
        if (_ovrEyeR.Confidence < EYETRACKINGCONFIDENCETHRESHOLD)
        {
            Debug.LogWarning("EyeTracking confidence value is low. Eyes are not tracked");
            //_focusedObject = null;
            //_eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;

           // return;
        }

        // cast from middle eye
        Vector3 eyePosition = (_rightEye.position + _leftEye.position) / 2;
        
        // eye forward is same for both eyes.
        Vector3 eyeForward = _rightEye.forward;

        RaycastHit hit;
        if (Physics.Raycast(eyePosition, eyeForward, out hit, EYERAYMAXLENGTH, _eyeTrackingLayerMask))
        {
            _focusedObject = hit.transform;
            _eyeGazeHitPosition = hit.point;
        }
        else
        {
            _focusedObject = null;
            _eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;
        }

        Debug.DrawRay(eyePosition, eyeForward);
    }
}
