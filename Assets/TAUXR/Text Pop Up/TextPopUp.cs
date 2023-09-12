using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//TODO: Do the animation in code
public class TextPopUp : MonoBehaviour
{
    private const float ScaleMultiplierScalar = 0.5f;
    private const float DefaultTextWidth = 0.48f;
    private const float DefaultTextHeight = 0.08f;
    private const int NumberOfLettersWhenScaleIsOne = 55;

    [SerializeField] private bool _useAnimation = true;

    [SerializeField] private GameObject _backFace;
    [SerializeField] private GameObject _pointer;
    [SerializeField] private TextMeshPro _textUI;


    [TextArea(1, 10)] [SerializeField] private string _text;

    private void OnEnable()
    {
        if (!_useAnimation)
        {
            GetComponent<Animator>().Play("Birth", -1, 1);
        }
    }

    public void GetTextFromComponent()
    {
        _text = _textUI.text;
    }

    public void SetText(string newText)
    {
        _textUI.text = newText;
    }

    public void SetTextAndScale(string newText)
    {
        _textUI.text = newText;
        SetNewScale();
    }

    public void SetTextAndScaleFromSerializedField()
    {
        _textUI.text = _text;
        SetNewScale();
    }


    public void SetNewScale()
    {
        float scaleMultiplier = CalculateScaleMultiplier();
        UpdateBackFace(scaleMultiplier);
        UpdateTextUI(scaleMultiplier);
        UpdatePointer(scaleMultiplier);
    }


    private float CalculateScaleMultiplier()
    {
        float scaleMultiplier = (float)_textUI.text.Length / NumberOfLettersWhenScaleIsOne;
        scaleMultiplier = Mathf.Lerp(1, scaleMultiplier, ScaleMultiplierScalar);
        if (_textUI.text.Length > 300)
        {
            scaleMultiplier /= 1.5f;
        }

        return scaleMultiplier;
    }

    private void UpdateBackFace(float scaleMultiplier)
    {
        //TODO: instead of changing scale, change rectangle size
        _backFace.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
    }

    private void UpdateTextUI(float scaleMultiplier)
    {
        _textUI.rectTransform.sizeDelta = new Vector2(DefaultTextWidth * scaleMultiplier,
            DefaultTextHeight * scaleMultiplier);
    }

    private void UpdatePointer(float scaleMultiplier)
    {
        //TODO: instead of changing scale, change rectangle size
        _pointer.transform.localPosition = new Vector3(-0.0034f, 0,
            _textUI.rectTransform.offsetMax.x + 0.02f * scaleMultiplier);
        _pointer.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
    }

#if UNITY_EDITOR
    public void ResetScale()
    {
        _textUI.rectTransform.sizeDelta = new Vector2(0.48f, 0.08f);
        _pointer.transform.localPosition = new Vector3(-0.0034f, 0, 0.26f);
        _pointer.transform.localScale = Vector3.one;
        _backFace.transform.localScale = Vector3.one;
    }

    public void DebugNumberOfLettersAndScaleMultiplier()
    {
        Debug.Log(_textUI.text.Length);
        Debug.Log(CalculateScaleMultiplier());
    }
#endif
}