using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]

public class PortalPlacement : MonoBehaviour
{
    [Header("Portal Placement Properties")]
    private PlayerController _controller;
    private Camera _playerCam;
    public LayerMask portalableSurfaceLayer;
    [SerializeField] private PortalRegistry _portalPair;
    [Header("Spawn Projectile Properties")]
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private float _bulletSpeed;

    private void Awake() 
    {
        _controller = GetComponent<PlayerController>();
        _playerCam = _controller.playerCam;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Fire portal 1
            CreatePortal(0);
            FireProjectile(0);
            
        }else if (Input.GetMouseButtonDown(1))
        {
            //Fire portal 2
            CreatePortal(1);
            FireProjectile(1);
        }
        
    }
    
    private void CreatePortal(int portalID)
    {
        PortalBehavior portal;
        RaycastHit hit;
        switch(Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, out hit, Mathf.Infinity, portalableSurfaceLayer))
        {
            case true:
                portal = _portalPair.portalArray[portalID];
                portal.edgeChecker.transform.position = hit.point;
                //portal.transform.position = hit.point;
                //portal.outline.SetActive(false);

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
                _portalPair.portalArray[portalID].AttemptPlacingPortal();
                //portal.transform.position = new Vector3 ( Mathf.Round(portal.transform.position.x), Mathf.Round(portal.transform.position.y), Mathf.Round(portal.transform.position.z));
                break;
            case false:
                break;

        }
    }

    private void FireProjectile(int portalID)
    {
        RaycastHit hit;
        Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, out hit, Mathf.Infinity, portalableSurfaceLayer);
        Debug.DrawLine(_playerCam.transform.position, hit.point, Color.red, 5f);


        var proj = Instantiate(_projectile, _playerCam.gameObject.transform.position, Quaternion.identity);
        proj.transform.forward = _spawnTransform.up;//update rotation
        StartCoroutine(IProjectileLerp(hit, Time.time, proj));
        

    }

    private IEnumerator IProjectileLerp(RaycastHit hit, float startTime, GameObject bullet)
    {
        var distTotal = hit.distance;
        var distTravelled = (Time.time - startTime) * _bulletSpeed;

        bullet.transform.position = Vector3.Lerp(_playerCam.transform.position, hit.point, distTravelled/distTotal);
        yield return null;
    }
}
