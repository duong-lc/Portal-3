using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class LaserLineRenderer : MonoBehaviour
{

    private LineRenderer _lineRenderer;
    //private Vector3[] _posList = new Vector3[2];
    private List<Vector3> _posList = new List<Vector3>();
    private List<Collider> _colList = new List<Collider>();
    [Header("Laser Settings")]
    //[SerializeField] private Transform _nose;//point where laser should start

    private Vector3 _pos, _dir;
    [SerializeField] private LayerMask _triggerVolume = 15;
    //private ActivationButtonInteraction _laserReceiver;
    //private bool _touchReceiver = false;

    // Start is called before the first frame update
    private void Start()
    {
        //_triggerVolume = 15;
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.numCornerVertices = 10;
        _lineRenderer.numCapVertices = 10;
    }

    // Update is called once per frame
    private void Update()
    {
        //Reset the list
        _posList = new List<Vector3> {transform.position};
        _colList = new List<Collider> {GetComponent<Collider>()};
        //_laserReceiver = null;
        
        if (gameObject.CompareTag("LaserBlaster"))
        {
            UpdateLaser(transform.up);
            return;
        }
        UpdateLaser(-transform.forward);
        
    }

    private void UpdateLaser(Vector3 initialDirection)
    {
        _pos = transform.position;
        _dir = initialDirection;

        bool isInf = true;
        while (isInf)
        {
            Physics.Raycast(_pos, _dir, out var hit,  Mathf.Infinity, ~_triggerVolume);
            if (hit.collider == null)
            {
                if(gameObject.CompareTag("LaserBlaster") || gameObject.CompareTag("Portal"))
                    _posList.Add(_pos + _dir * 20);
                break;//endloop if hit array doesn't touch anything
            }
            //int closestTransform = hit.distance;//Getting the closest collider from point of raycast
            Collider colliderToAdd = hit.collider;
            
            switch (hit.collider != null)
            {
                case true when hit.collider.GetComponent<ObjectLaserInteraction>():
                {
                    //posList Add 1
                    Vector3 positionToAdd = hit.collider.transform.position;
                    if (_posList.Contains(positionToAdd))
                    {
                        positionToAdd = hit.point;
                        _posList.Add(positionToAdd);
                        _colList.Add(colliderToAdd);
                        isInf = false;
                        break;
                    }

                    _posList.Add(positionToAdd);
                    _colList.Add(colliderToAdd);
                    _pos = positionToAdd;
                    _dir = hit.collider.transform.forward;
                    break;
                }
                case true:
                {
                    Vector3 positionToAdd = hit.point;
                    _posList.Add(positionToAdd);
                    _colList.Add(colliderToAdd);
                    isInf = false;
                    break;
                }
            }
        }
        
        
        //Adding all the collider lists from laser blaster to portal registry singleton to
        //check all active laser blasters in scene to see if their lasers touching the portal
        PortalRegistry.Instance.registryColList.AddRange(_colList);
        

        _lineRenderer.positionCount = _posList.Count;
        _lineRenderer.SetPositions(_posList.ToArray());
        
        foreach (Collider col in _colList)
        {
            if(col.CompareTag("LaserReceiver")) return;
            
            if (col.GetComponent<TurretBehavior>())
            {
                col.GetComponent<ObjectInteraction>().ResetObjectTransform(true);
            }
            if (col.GetComponent<PlayerController>())
            {
                CameraShake.instance.Shake(0.1f, 0.3f);
            }
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (Vector3 point in _posList)
        {
            Gizmos.DrawWireSphere(point, 1);
        }
    }

    public void ResetLaserRendererArray()
    {
        var temp = Array.Empty<Vector3>();
        _lineRenderer.positionCount = temp.Length;
        _lineRenderer.SetPositions(temp);
        this.enabled = false;
    }
}
