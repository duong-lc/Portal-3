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

    // Start is called before the first frame update
    private void Start()
    {
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
            RaycastHit[] hitArray = Physics.RaycastAll(_pos, _dir, 200);
            //Debug.DrawRay(_pos, _dir*30, Color.blue, 0.5f);
            if (hitArray.Length == 0)
            {
                if(gameObject.CompareTag("LaserBlaster") || gameObject.CompareTag("Portal"))
                    _posList.Add(_pos + _dir * 20);
                break;//endloop if hit array doesn't touch anything
            }
            int closestTransformIndex = GetClosestObject(hitArray);//Getting the closest collider from point of raycast
            Collider colliderToAdd = hitArray[closestTransformIndex].collider;
            
            switch (closestTransformIndex < hitArray.Length)
            {
                case true when hitArray[closestTransformIndex].collider.GetComponent<ObjectLaserInteraction>():
                {
                    //posList Add 1
                    Vector3 positionToAdd = hitArray[closestTransformIndex].collider.transform.position;
                    if (_posList.Contains(positionToAdd))
                    {
                        positionToAdd = hitArray[closestTransformIndex].point;
                        _posList.Add(positionToAdd);
                        _colList.Add(colliderToAdd);
                        isInf = false;
                        break;
                    }

                    _posList.Add(positionToAdd);
                    _colList.Add(colliderToAdd);
                    _pos = positionToAdd;
                    _dir = hitArray[closestTransformIndex].collider.transform.forward;
                    break;
                }
                case true when hitArray[closestTransformIndex].collider.GetComponent<PortalBehavior>():
                {
                    //Activate line renderer on the other portal
                    PortalRegistry.Instance.EnableLaserOnPortal(hitArray[closestTransformIndex].collider.GetComponent<PortalBehavior>());
                    Vector3 positionToAdd = hitArray[closestTransformIndex].point;
                    _posList.Add(positionToAdd);
                    _colList.Add(colliderToAdd);
                    isInf = false;
                    break;
                }
                case true:
                {
                    Vector3 positionToAdd = hitArray[closestTransformIndex].point;
                    _posList.Add(positionToAdd);
                    _colList.Add(colliderToAdd);
                    isInf = false;
                    break;
                }
            }
        }
        
        Collider portal1 = PortalRegistry.Instance.portalArray[0].transform.GetComponent<Collider>();
        Collider portal2 = PortalRegistry.Instance.portalArray[1].transform.GetComponent<Collider>();

        if (!_colList.Contains(portal1) && !_colList.Contains(portal2))//TODO fix this crap
        {
            PortalRegistry.Instance.DisableLaserOnAllPortal();
        }
        
        _lineRenderer.positionCount = _posList.Count;
        _lineRenderer.SetPositions(_posList.ToArray());
    }
    

    private int GetClosestObject(IReadOnlyList<RaycastHit> hitArray)
    {
        int closestObjIndex = 0;
        float minDist = 0;
        var distArray = new float[hitArray.Count];
        for (int j = 0; j < hitArray.Count; j++)
        {
            float currDist = Vector3.Distance(hitArray[j].point, _pos);
            distArray[j] = currDist;
        }
        minDist = distArray.Min();
        closestObjIndex = Array.IndexOf(distArray, minDist);
        return closestObjIndex;
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
    }
}
