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
    private List<Collider> _colliderList = new List<Collider>();
    private Material _unactivatedMat;
    
    public ActivationType activationType = ActivationType.MoveAToB;
    public GameObject buttonObject;
    public Material activationMat;
    
    public Vector3 startPos;
    public Vector3 endPos;
    public GameObject objectToMove;
    public float timeFromAToB;

    private void Start()
    {
        _unactivatedMat = buttonObject.GetComponent<Renderer>().material;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>() && !other.GetComponent<PlayerController>()) return;
        if (objectToMove.transform.position == endPos) return;
        if(!_colliderList.Contains(other)) {_colliderList.Add(other);}
        if(_colliderList.Count > 1) {return;}
        
        buttonObject.GetComponent<Renderer>().material = activationMat;
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
        if (_colliderList.Contains(other)) { _colliderList.Remove(other);}
        if (_colliderList.Count > 0) { return;}

        buttonObject.GetComponent<Renderer>().material = _unactivatedMat;
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
            Vector3 currPos = objectToMove.transform.position;
            float percentage = (Vector3.Distance(currPos, endPos) / Vector3.Distance(endPos, startPos));
            float totalTime = timeFromAToB * percentage;
            while (alpha <= 1)
            {
                objectToMove.transform.position = Vector3.Lerp(currPos, endPos, alpha);
                alpha = (Time.time - startTime)/ totalTime;
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
