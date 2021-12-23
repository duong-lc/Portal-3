using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class LaserLineRenderer : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Vector3[] _posArray = new Vector3[2];
    [Header("Laser Settings")]
    [SerializeField] private Transform _nose;//point where laser should start

    private Vector3 _pos, _dir;

    // Start is called before the first frame update
    private void Start()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _posArray[0] = transform.TransformPoint(transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < _posArray.Length; ++i)
        {
            _pos = _posArray[i];
            if (i == 0)
            {
                _dir = _nose.position - transform.root.position;
            }
            else
            {
                _dir = _posArray[i] - _posArray[i - 1];
            }
            RaycastHit[] hitArray = Physics.RaycastAll(_pos, _dir, Mathf.Infinity);
            if (hitArray.Length > 0)
            {
                _posArray[i] = hitArray[0].point;
            }
        }

        _lineRenderer.positionCount = _posArray.Length;
        _lineRenderer.SetPositions(_posArray);
    }

    private void LateUpdate()
    {
        
    }


    //TODO: Add a function that expand the position array 
}
