using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ActivationButtonInteraction))]
public class ActivationButtonInteractionEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var buttonScript = target as ActivationButtonInteraction;
        buttonScript!.activationType = (ActivationButtonInteraction.ActivationType)
            EditorGUILayout.EnumPopup("Activation Type: ", buttonScript.activationType);

        buttonScript.buttonObject = (GameObject) EditorGUILayout.ObjectField("Button Object: ",
            buttonScript.buttonObject, typeof(GameObject), true);
        
        buttonScript.activationMat = (Material) EditorGUILayout.ObjectField("Activation Material: ",
            buttonScript.activationMat, typeof(Material), true);
        
        switch (buttonScript.activationType)
        {
            case ActivationButtonInteraction.ActivationType.MoveAToB:
                EditorGUILayout.Space(5);
                buttonScript.startPos =  EditorGUILayout.Vector3Field("Starting Position: ", buttonScript.startPos);
                buttonScript.endPos = EditorGUILayout.Vector3Field("Ending Position: ", buttonScript.endPos);
                buttonScript.timeFromAToB =
                    EditorGUILayout.Slider("Timer A to B", buttonScript.timeFromAToB, 0.1f, 10f);
                EditorGUILayout.Space(2);
                buttonScript.objectToMove = (GameObject)EditorGUILayout.ObjectField(
                    "Object To Move: ", buttonScript.objectToMove, typeof(GameObject), true);
                
                break;
            case ActivationButtonInteraction.ActivationType.PingPongAToB:
                break;
            case ActivationButtonInteraction.ActivationType.ToggleForceField:
                EditorGUILayout.Space(5);
                buttonScript.objectToMove = (GameObject)EditorGUILayout.ObjectField(
                    "Force Field to Toggle: ", buttonScript.objectToMove, typeof(GameObject), true);
                buttonScript.hasTimer = (bool) EditorGUILayout.Toggle("Has Timer", buttonScript.hasTimer);

                if (buttonScript.hasTimer)
                {
                    buttonScript.countdownAmount = EditorGUILayout.Slider(
                        "Countdown Timer", buttonScript.countdownAmount, 1, 5f);
                    EditorGUILayout.Space(2);
                }
                break;
            default:
                break;
        }
    }
}
