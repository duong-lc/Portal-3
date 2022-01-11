using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerController))]

public class PortalPlacement : MonoBehaviour
{
    [Header("Portal Placement Properties")]
    private PlayerController _controller;
    private Camera _playerCam;
    public LayerMask portalableSurfaceLayer;
    public PortalRegistry portalPair;
    [Header("Spawn Projectile Properties")]
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private LayerMask _triggerVolume;
    [SerializeField] private LayerMask _portalLayer;
    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _playerCam = _controller.playerCam;
    }

    private void Update()
    {
        //Mouse input handle
        if (Input.GetMouseButtonDown(0))
            FireProjectile(0);
        else if (Input.GetMouseButtonDown(1))
            FireProjectile(1);
    }
    
    public void CreatePortal(int portalID, RaycastHit hit)
    {
        PortalBehavior portal = portalPair.portalArray[portalID];;
        portal.edgeChecker.transform.position = hit.point;

        //portal on ceiling or floor
        if ((hit.normal.x == 0 && hit.normal.y != 0 && hit.normal.z == 0))
        {
            Vector3 portalFwd = -hit.normal;
            Vector3 portalRight = _playerCam.transform.rotation * Vector3.right;

            if(Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
                portalRight = portalRight.x > 0 ? Vector3.right : -Vector3.right;
            else
                portalRight = portalRight.z > 0 ? Vector3.forward : -Vector3.forward;

            Vector3 portalUp = Vector3.Cross(portalRight, portalFwd);
            //portal.transform.rotation = Quaternion.LookRotation(portalFwd, portalUp);
            portal.edgeChecker.transform.rotation = Quaternion.LookRotation(portalFwd, portalUp);
        }
        else //if((hit.normal.x != 0 && hit.normal.z == 0) || (hit.normal.x == 0 && hit.normal.z != 0) || (hit.normal.x != 0 && hit.normal.y == 0 && hit.normal.z != 0))//on a wall or angled wall
        {
            //portal.transform.forward = -hit.normal;//print($"wall");   
            portal.edgeChecker.transform.forward = -hit.normal;
        }
        portalPair.portalArray[portalID].AttemptPlacingPortal();
        //portal.transform.position = new Vector3 ( Mathf.Round(portal.transform.position.x), Mathf.Round(portal.transform.position.y), Mathf.Round(portal.transform.position.z));
    }

    private void FireProjectile(int portalID)
    {
        if(!Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, out var hit, Mathf.Infinity, ~_triggerVolume|~_portalLayer)) { return; }

        //Debug.DrawLine(_playerCam.transform.position, hit.point, Color.red, 5f);
        //Destroy any same type bullet if that's in the scene if there's a new one about to spawn.
        var bulletScript = Object.FindObjectOfType<PortalProjectileBehavior>();
        if(bulletScript != null && portalID == bulletScript.portalID) 
            Destroy(bulletScript.gameObject);
        //Instantiate the bullet
        var proj = Instantiate(_projectile, _playerCam.gameObject.transform.position, Quaternion.identity);
        proj.GetComponent<PortalProjectileBehavior>().portalID = portalID;
        proj.GetComponent<PortalProjectileBehavior>().surfaceNormal = hit.normal;
        proj.transform.forward = _spawnTransform.forward;//update rotation
        StartCoroutine(IEProjectileLerp(hit, Time.time, proj));
        

    }

    private IEnumerator IEProjectileLerp(RaycastHit hit, float startTime, GameObject bullet)
    {
        var distTotal = hit.distance;
        var distTravelled = (Time.time - startTime) * _bulletSpeed;
        var startingPos = _spawnTransform.position;
        while (distTravelled <= distTotal)
        {
            if(bullet == null)
                yield break;
            bullet.transform.position = Vector3.Lerp(startingPos, hit.point, distTravelled/distTotal);
            distTravelled = (Time.time - startTime) * _bulletSpeed;
            yield return null;
        }
        if(bullet != null )
            bullet.GetComponent<PortalProjectileBehavior>().OnTouchSurface(this, hit);
        
    }
}
