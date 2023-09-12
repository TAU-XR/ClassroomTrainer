using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextPopUp))]
public class TextPopUpEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TextPopUp textPopUp = target as TextPopUp;

        if (GUILayout.Button("Get text from component"))
        {
            textPopUp.GetTextFromComponent();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Set text and scale"))
        {
            textPopUp.SetTextAndScaleFromSerializedField();
        }

        if (GUILayout.Button("Set new scale"))
        {
            textPopUp.SetNewScale();
        }

        if (GUILayout.Button("Reset scale"))
        {
            textPopUp.ResetScale();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Debug number of letters and scale multiplier"))
        {
            textPopUp.DebugNumberOfLettersAndScaleMultiplier();
        }
    }
}