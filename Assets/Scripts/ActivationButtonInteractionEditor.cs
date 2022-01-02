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
        buttonScript.activationType = (ActivationButtonInteraction.ActivationType)
            EditorGUILayout.EnumPopup("Activation Type: ", buttonScript.activationType);
        
        switch (buttonScript.activationType)
        {
            case ActivationButtonInteraction.ActivationType.MoveAToB:
                EditorGUILayout.Space(2);
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
            default:
                break;
        }
    }
}
