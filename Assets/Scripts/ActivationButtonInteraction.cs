using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ActivationButtonInteraction : MonoBehaviour
{
    
    public enum ActivationType
    {
        MoveAToB,
        PingPongAToB,
        ToggleForceField
    }

    private IEnumerator _moveAToBFalse, _moveAToBTrue;
    private List<Collider> _colliderList = new List<Collider>();//number of objects pushing on button
    private Material _unactivatedMat;
    
    public ActivationType activationType = ActivationType.MoveAToB;
    public GameObject buttonObject;
    public Material activationMat;
    
    public Vector3 startPos;
    public Vector3 endPos;
    public GameObject objectToMove;
    public float timeFromAToB;

    private ActivationButtonGuideNodes[] _guideNodesArr;
    public bool isEnabled = false;

    public bool hasTimer;
    public float countdownAmount;
    private bool _isActivated = false;


    private void Start()
    {
        if(!gameObject.CompareTag("LaserReceiver") && !gameObject.CompareTag("DropperButton"))
            _unactivatedMat = buttonObject.GetComponent<Renderer>().material;
        _guideNodesArr = GetComponents<ActivationButtonGuideNodes>();

        //StartCoroutine(CheckIfActivatedRoutine());
        //_moveAToBTrue = MoveAToB(Time.time, true);
        //_moveAToBFalse = MoveAToB(Time.time, false);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        EnableActivation(other);
    }

    private void OnTriggerExit(Collider other)
    {
       DisableActivation(other);
    }

    public void EnableActivation()
    {
        if (objectToMove.transform.position == endPos) return;
        if(_colliderList.Count > 1) {return;}
        
        //Calling the function once so it doesn't stack the coroutine on update call
        if (!isEnabled)
            isEnabled = true;
        else
            return;
        
        //print($"activate");
        PlayerSoundManager.Instance.PlayBigButtonAudio(transform.position);
        foreach(ActivationButtonGuideNodes t in _guideNodesArr)
            t.ToggleGuideNodes(true);
        if(_moveAToBTrue != null) StopCoroutine(_moveAToBTrue);
        switch (activationType)
        {
            case ActivationType.MoveAToB:
                _moveAToBFalse = MoveAToB(Time.time, false);
                StartCoroutine(_moveAToBFalse);
                break;
            case ActivationType.PingPongAToB:
                break;
            case ActivationType.ToggleForceField:
                _isActivated = true;
                ToggleForceField(true);
                break;
            default:
                break;
        }
    }

    public void DisableActivation()
    {
        if (_colliderList.Count > 0) { return;}
        
        //Calling the function once so it doesn't stack the coroutine on update call
        if (isEnabled)
            isEnabled = false;
        else
            return;
        
        print($"not activate");
        
        foreach(ActivationButtonGuideNodes t in _guideNodesArr)
            t.ToggleGuideNodes(false);
        if(_moveAToBFalse != null) StopCoroutine(_moveAToBFalse);
        switch (activationType)
        {
            case ActivationType.MoveAToB:
                _moveAToBTrue = MoveAToB(Time.time, true);
                StartCoroutine(_moveAToBTrue);
                break;
            case ActivationType.PingPongAToB:
                break;
            case ActivationType.ToggleForceField:
                _isActivated = false;
                ToggleForceField(false);
                break;
            default:
                break;
        }
    }
    
    private void EnableActivation(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>() && !other.GetComponent<PlayerController>()) return;
        if (objectToMove.transform.position == endPos) return;
        if(!_colliderList.Contains(other)) {_colliderList.Add(other);}
        if(_colliderList.Count > 1) {return;}
        
        PlayerSoundManager.Instance.PlayBigButtonAudio(transform.position);
        buttonObject.GetComponent<Renderer>().material = activationMat;
        foreach(ActivationButtonGuideNodes t in _guideNodesArr)
            t.ToggleGuideNodes(true);
        if(_moveAToBTrue != null) StopCoroutine(_moveAToBTrue);
        switch (activationType)
        {
            case ActivationType.MoveAToB:
                _moveAToBFalse = MoveAToB(Time.time, false);
                StartCoroutine(_moveAToBFalse);
                break;
            case ActivationType.PingPongAToB:
                break;
            case ActivationType.ToggleForceField:
                print($"enable");
                _isActivated = true;
                ToggleForceField(true);
                break;
            default:
                break;
        }
    }
    private void DisableActivation(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>() && !other.GetComponent<PlayerController>()) return;
        if (_colliderList.Contains(other)) { _colliderList.Remove(other);}
        if (_colliderList.Count > 0) { return;}

        foreach(ActivationButtonGuideNodes t in _guideNodesArr)
            t.ToggleGuideNodes(false);
        buttonObject.GetComponent<Renderer>().material = _unactivatedMat;
        if(_moveAToBFalse != null) StopCoroutine(_moveAToBFalse);
        switch (activationType)
        {
            case ActivationType.MoveAToB:
                _moveAToBTrue = MoveAToB(Time.time, true);
                StartCoroutine(_moveAToBTrue);
                break;
            case ActivationType.PingPongAToB:
                break;
            case ActivationType.ToggleForceField:
                print($"disable");
                ToggleForceField(false);
                _isActivated = false;
                break;
            default:
                break;
        }
    }

    private void ToggleForceField(bool isTurnOn)
    {
        if (isTurnOn && !_isActivated) return;
        
        objectToMove.SetActive(!objectToMove.activeInHierarchy);
        
        if (!isTurnOn) return;
        if (!hasTimer) return;
        StartCoroutine(TickDownRoutine());
    }

    private IEnumerator TickDownRoutine()
    {
        float counter = countdownAmount;
        while (counter > 0)
        {
            print($"{counter}");
            counter--;
            PlayerSoundManager.Instance.PlayTickAudio();
            yield return new WaitForSeconds(1f);
        }
        DisableActivation();

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
