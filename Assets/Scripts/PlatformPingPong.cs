using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPingPong : MonoBehaviour
{
    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private Vector3 _endPosition;
    [SerializeField] private float _travelTime;

    private void Start()
    {
        StartCoroutine(PingPongPosition());
    }

    private IEnumerator PingPongPosition()
    {
        while(true){
            yield return StartCoroutine(MovePlatform(_startPosition, _endPosition));
            yield return StartCoroutine(MovePlatform(_endPosition, _startPosition));
        }
    }

    IEnumerator MovePlatform(Vector3 startPos, Vector3 endPos)
    {
        float alpha = 0.0f;
        float rate = 1.0f/_travelTime;
        while (alpha < 1.0f)    
        {
            alpha += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(startPos, endPos, alpha);
            yield return null;
        }
    }
}
