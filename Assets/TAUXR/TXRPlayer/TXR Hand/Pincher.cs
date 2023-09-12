using UnityEngine;

public class Pincher : MonoBehaviour
{
    public float Strength => _pinchStrength;

    private OVRSkeleton _ovrSkeleton;
    private const int INDEX_I = 20, THUMB_I = 19;
    private float _pinchStrength;

    private float _pinchDistance;
    private float PINCH_MAX_DISTANCE = .01f;
    private float PINCH_MIN_DISTANCE = .0006f;

    public void Init(OVRSkeleton ovrSkeleton)
    {
        _ovrSkeleton = ovrSkeleton;
    }

    public void UpdatePincher()
    {
        // set pinch position
        Vector3 indexFingerPosition = _ovrSkeleton.Bones[INDEX_I].Transform.position;
        Vector3 thumbFingerPosition = _ovrSkeleton.Bones[THUMB_I].Transform.position;
        Vector3 pincherTargetPosition = (thumbFingerPosition + indexFingerPosition) / 2;
        transform.position = pincherTargetPosition;

        // set pinch strength based on finger distance
        _pinchDistance = (indexFingerPosition - thumbFingerPosition).sqrMagnitude;
        _pinchStrength = Mathf.InverseLerp(PINCH_MAX_DISTANCE, PINCH_MIN_DISTANCE, _pinchDistance);

    }
}
