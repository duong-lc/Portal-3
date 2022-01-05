using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private float _laserLength = 2.0f;
    private Vector3[] _laserArray = new Vector3[2];
    [SerializeField] private Transform _laserStartPoint;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        
        _laserArray[0] = _laserStartPoint.position;
        _laserArray[1] = (_laserStartPoint.position + _laserStartPoint.forward) * _laserLength;
        _lineRenderer.positionCount = 2;
        
    }

    // Update is called once per frame
    void Update()
    {
        _lineRenderer.SetPositions(_laserArray);
    }
}
