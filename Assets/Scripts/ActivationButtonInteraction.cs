using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

public class ActivationButtonInteraction : MonoBehaviour
{
    
    public enum ActivationType
    {
        MoveAToB,
        PingPongAToB
    }

    private IEnumerator _moveAToBFalse, _moveAToBTrue;
    public ActivationType activationType = ActivationType.MoveAToB;

    public Vector3 startPos;
    public Vector3 endPos;
    public GameObject objectToMove;
    public float timeFromAToB;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>() && !other.GetComponent<PlayerController>()) return;
        if (objectToMove.transform.position == endPos) return;
        
        if(_moveAToBTrue != null) StopCoroutine(_moveAToBTrue);
        switch (activationType)
        {
            case ActivationType.MoveAToB:
                _moveAToBFalse = MoveAToB(Time.time, false);
                StartCoroutine(_moveAToBFalse);
                break;
            case ActivationType.PingPongAToB:
                break;
            default:
                break;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>() && !other.GetComponent<PlayerController>()) return;
        
        StopCoroutine(_moveAToBFalse);
        switch (activationType)
        {
            case ActivationType.MoveAToB:
                _moveAToBTrue = MoveAToB(Time.time, true);
                StartCoroutine(_moveAToBTrue);
                break;
            case ActivationType.PingPongAToB:
                break;
            default:
                break;
        }
    }

    private IEnumerator MoveAToB(float startTime, bool isReverted)
    {
        float alpha = (Time.time - startTime)/ timeFromAToB;
        if (isReverted)
        {
            Vector3 currPos = objectToMove.transform.position;
            float percentage = (Vector3.Distance(currPos, startPos) / Vector3.Distance(endPos, startPos));
            float totalTime = timeFromAToB * percentage;
            while (alpha <= 1)
            {
                objectToMove.transform.position = Vector3.Lerp(currPos, startPos, alpha);
                alpha = (Time.time - startTime)/ totalTime;
                yield return null;
            }
        }
        else
        {
            while (alpha <= 1)
            {
                objectToMove.transform.position = Vector3.Lerp(startPos, endPos, alpha);
                alpha = (Time.time - startTime)/ timeFromAToB;
                print($"{alpha}");
                yield return null;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch (activationType)
        {
            case ActivationType.MoveAToB:
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(startPos, 1f);
                Gizmos.DrawWireSphere(endPos, 1f);
                Gizmos.DrawLine(startPos, endPos);
                break;
            case ActivationType.PingPongAToB:
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(startPos, 1f);
                Gizmos.DrawWireSphere(endPos, 1f);
                Gizmos.DrawLine(startPos, endPos);
                break;
            default:
                break;
        }
    }
}
