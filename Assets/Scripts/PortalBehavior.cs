using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehavior : MonoBehaviour
{
    public GameObject outline;
    public GameObject edgeChecker;
    public GameObject viewport;
    public float cooldownTimer = 0.5f;
    public bool canTeleport = true;
    
    [SerializeField] private GameObject _cam;
    [SerializeField] private float _speedOut;
    private string _portalTag = "Portal", _portalableObjTag = "PortalableObject", _playerTag = "Player";
    
    //[HideInInspector] public float cooldownCounting;

    private void Start() {
        canTeleport = true;
        outline.SetActive(false);
        viewport.SetActive(false);
    }
    private void Update() {
        if(outline.activeInHierarchy && viewport.activeInHierarchy)
        {
            gameObject.GetComponent<Collider>().enabled = true;
        }else
            gameObject.GetComponent<Collider>().enabled = false;
    }


    public IEnumerator CountingDownCooldown()
    {
        canTeleport = false;
        yield return new WaitForSeconds(cooldownTimer);
        canTeleport = true;
    }

    private void OnTriggerStay(Collider other) {
        //print($"{outline.activeInHierarchy} {viewport.activeInHierarchy} ");
        if((other.CompareTag(_portalableObjTag) || other.CompareTag(_playerTag)) && canTeleport)
        {
            var registry = gameObject.transform.parent.GetComponent<PortalRegistry>();
            if(registry.portalArray[0] == this && registry.portalArray[1].GetComponent<Collider>().enabled)//portal blue
            {
                Teleport(registry.portalArray[1].gameObject.transform.position, 1, other, registry);
            }
            else if (registry.portalArray[1] == this && registry.portalArray[0].GetComponent<Collider>().enabled)//portal red
            {
                Teleport(registry.portalArray[0].gameObject.transform.position, 0, other, registry);
            }
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if((other.CompareTag(_portalableObjTag) || other.CompareTag(_playerTag)))
    //     {
    //         canTeleport == true
    //     }
    // }

    private void Teleport(Vector3 destination, int portalID, Collider col, PortalRegistry registry)
    {
        var portalDir = -registry.portalArray[portalID].gameObject.transform.forward;
        //col.transform.forward = portalDir;//chaging rotation
        if(col.CompareTag(_playerTag))//if the gameobject is a player
        {
            var charController = col.gameObject.GetComponent<CharacterController>();

            charController.enabled = false;
            registry.portalArray[portalID].StartCoroutine(registry.portalArray[portalID].CountingDownCooldown());
            col.gameObject.transform.position = registry.portalArray[portalID].gameObject.transform.position;
            //Changing rotation of the player camera
            col.GetComponentInChildren<Camera>().transform.forward = portalDir;

            //adding velocity when exit portal
            col.gameObject.GetComponent<Rigidbody>().AddForce(portalDir * _speedOut, ForceMode.Impulse);
            Debug.DrawLine(col.transform.position, portalDir * _speedOut, Color.blue, 3f);
            charController.enabled = true;
            
            
        }else//if the gameobject is a teleportable obj
        {
            registry.portalArray[portalID].StartCoroutine(registry.portalArray[portalID].CountingDownCooldown());
            col.gameObject.transform.position = registry.portalArray[portalID].gameObject.transform.position;
        }

        
        
    }

    #region PortalPlacement
    public void AttemptPlacingPortal()
    {
        if(CheckPerimeter() && CheckNormalOverlap())
        {
            //outline.GetComponentInChildren<ParticleSystem>().Stop();
            //assigning portal transform if edge checker valid
            transform.position = edgeChecker.transform.position;  
            transform.rotation = edgeChecker.transform.rotation;
            //reset edge checker position to in center of portal parent obj
            edgeChecker.transform.position = transform.position;
            edgeChecker.transform.rotation = transform.rotation;
            outline.SetActive(true);
            outline.GetComponentInChildren<ParticleSystem>().Simulate(0.1f, false, true, false);
            outline.GetComponentInChildren<ParticleSystem>().Play();
            viewport.SetActive(true);
        }else{
            
           _cam.GetComponent<CameraShake>().Shake(0.2f,0.1f);

        }
    }
    public bool CheckPerimeter()
    {
        bool canPlace = true;
        //var vectorArray =  edgeChecker.GetComponentsInChildren<Transform>();
        var posPerimList = new List<Vector3>()
        {
            new Vector3(-1.2f, 0, 0),
            new Vector3(1.2f, 0, 0),
            new Vector3(0, 2, 0),
            new Vector3(0, -2, 0),
            new Vector3(0, 0 , 0)
        };

        var dirPerimList = new List<Vector3>()
        {
            Vector3.right,
            -Vector3.right,
            Vector3.up,
            -Vector3.up
        };        
 

        //Checking out of bounds on current plane that's spawning on
        for (int i = 0; i < posPerimList.Count; ++i)
        {
            //turning world rot and pos to edgechecker local relative pos and rot
            var edgePos = edgeChecker.transform.TransformPoint(posPerimList[i]);
            
            //Check if the edge touch a portable surface
            var hitCol = Physics.OverlapSphere(edgePos, 0.1f, PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer);

            if(hitCol.Length == 0 && i != posPerimList.Count-1)//has no surface on that point
            {
                RaycastHit hit;
                var edgeDir = edgeChecker.transform.TransformDirection(dirPerimList[i]);
                if(Physics.Raycast(edgePos, edgeDir, out hit, 2f, PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer))
                {
                    print($"smt");
                    var offset = hit.point - edgePos;
                    edgeChecker.transform.Translate(offset, Space.World);
                    canPlace = true;
                }else
                    canPlace = false;
            }   
            
            var portalCol = Physics.OverlapSphere(edgePos, 0.2f);

            for(int j = 0; j < portalCol.Length; ++j)
            {
                if(portalCol[j].tag == _portalTag)
                {
                    //print($"found different");
                    //portalCol[j].gameObject.transform.position = new Vector3(0,0,0);
                    portalCol[j].gameObject.GetComponent<PortalBehavior>().viewport.SetActive(false);
                    portalCol[j].gameObject.GetComponent<PortalBehavior>().outline.SetActive(false);
                }
            }
        }
       
        

        if(!canPlace)
            print($"can't place here");
        return canPlace;
    }
    public bool CheckNormalOverlap()    
    {
        bool canPlace = true;

        var normalDrawPoints = new List<Vector3>()
        {
            new Vector3 (-1.2f, 2, -0.2f),//top left
            new Vector3 (1.2f, 2, -0.2f),//top right
            new Vector3 (1.2f, -2, -0.2f),//bottom right
            new Vector3 (-1.2f,-2,-0.2f)//bottom left
            
        };

        var normalDrawDists = new List<float>()
        {
            2.4f, 4f, 2.4f, 4f
        };

        var normalDirPoint = new List<Vector3>()
        {
            Vector3.right,
            -Vector3.up,
            -Vector3.right,
            Vector3.up
        };

        for (int i = 0; i < normalDirPoint.Count; ++i)
        {
            Vector3 startPoint, dir;
            startPoint = edgeChecker.transform.TransformPoint(normalDrawPoints[i]);
            dir = edgeChecker.transform.TransformDirection(normalDirPoint[i]);
            
            RaycastHit hit;
            if(Physics.Raycast(startPoint, dir , out hit, normalDrawDists[i] , PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer)){
                canPlace = false;
                print($"normal overlapped ! ");  
            }
        }
        return canPlace;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {

    }
}
