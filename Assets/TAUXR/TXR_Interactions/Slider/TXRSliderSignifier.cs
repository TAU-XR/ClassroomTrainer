using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum SliderSignifier{ HoverIn, HoverOut, ButtonPress, ButtonRelease, BeforeRating, AfterRating }
public class TXRSliderSignifier : MonoBehaviour
{
    [SerializeField] TXRButtonTouch sliderButton;


    public UnityEvent IdlePreRating;
    public UnityEvent DuringRating;
    public UnityEvent IdlePostRating;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Sign(SliderSignifier signState)
    {
        switch (signState)
        {
            case SliderSignifier.HoverIn:
                break;
            case SliderSignifier.HoverOut:
                break;
            case SliderSignifier.ButtonPress:
                sliderButton.TriggerButtonEvent(ButtonEvent.Pressed, ButtonColliderResponse.Internal);
                break;
            case SliderSignifier.ButtonRelease:
                sliderButton.TriggerButtonEvent(ButtonEvent.Released, ButtonColliderResponse.Internal);
                break;
            case SliderSignifier.BeforeRating:
                break;
            case SliderSignifier.AfterRating:
                break;

            default: return;
        }
    }
}
