using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private float _laserLength = 2.0f;
    private Vector3[] _laserArray = new Vector3[2];
    [SerializeField] private Transform _laserStartPoint;
    private TurretFOVBehavior _fovBehavior;
    // Start is called before the first frame update
    void Start()
    {
        _fovBehavior = GetComponent<TurretFOVBehavior>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.positionCount = 2;
        StartCoroutine(LaserLineUpdateRoutine());

    }

    private IEnumerator LaserLineUpdateRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);
        while (true)
        {
            _laserArray[0] = _laserStartPoint.position;
            _laserArray[1] = _laserStartPoint.position + _laserStartPoint.forward * _laserLength;
            _lineRenderer.SetPositions(_laserArray);
            yield return wait;
        }
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (_fovBehavior.canSeePlayer)
        {
            print($"fire");
        }
    }
}
