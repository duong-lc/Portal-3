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
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    
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

    private void OnTriggerEnter(Collider other) {
        //print($"{outline.activeInHierarchy} {viewport.activeInHierarchy} ");
        if((other.CompareTag(_portalableObjTag) || other.CompareTag(_playerTag)) && canTeleport)
        {
            var registry = gameObject.transform.parent.GetComponent<PortalRegistry>();
            if(registry.portalArray[0] == this && registry.portalArray[1].GetComponent<Collider>().enabled)//portal blue
            {
                Teleport(0, 1, other, registry);
            }
            else if (registry.portalArray[1] == this && registry.portalArray[0].GetComponent<Collider>().enabled)//portal red
            {
                Teleport(1, 0, other, registry);
            }
        }
    }


    private void Teleport(int beginPortalID, int endPortalID, Collider col, PortalRegistry registry)
    {
        var beginTransform = registry.portalArray[beginPortalID].gameObject.transform;
        var endTransform = registry.portalArray[endPortalID].gameObject.transform;

        if(col.CompareTag(_playerTag))//if the gameobject is a player
        {
            var charController = col.gameObject.GetComponent<CharacterController>();
            
            //charController.enabled = false;

            //Update position of object
            var teleportPos = endTransform.TransformPoint(-Vector3.forward * registry.portalArray[endPortalID].gameObject.GetComponent<BoxCollider>().size.x/2);
            col.gameObject.transform.position = teleportPos;

            //Update rotation of object
            var dotValue = Vector3.Dot(col.transform.forward, -beginTransform.forward);
            if(dotValue > .01f || dotValue < -.01f)//if the player forward is not orthogonal to start portal forward
            {
                Quaternion relativeRot = Quaternion.Inverse(beginTransform.rotation) * col.transform.rotation;
                relativeRot = halfTurn * relativeRot;
                col.transform.rotation = endTransform.rotation * relativeRot;
            }else{
                if(Input.GetAxis("Horizontal") < 0) //rotate from left to right
                    col.transform.forward = endTransform.forward;
                else if (Input.GetAxis("Horizontal") > 0) //rotate from right to left
                    col.transform.forward = -endTransform.forward;
            }
                

            //update velocity of rigidbody
            Vector3 relativeVel = beginTransform.InverseTransformDirection(col.GetComponent<Rigidbody>().velocity);
            relativeVel = halfTurn * relativeVel;
            col.GetComponent<Rigidbody>().velocity = endTransform.TransformDirection(relativeVel);


            registry.portalArray[endPortalID].StartCoroutine(registry.portalArray[endPortalID].CountingDownCooldown());
            //charController.enabled = true;
            
            
            if(col.transform.rotation.x != 0 && col.transform.rotation.z != 0)
            {
                col.transform.rotation = Quaternion.Euler(0, col.transform.rotation.y, 0);
            }

        }else//if the gameobject is a teleportable obj
        {
            registry.portalArray[endPortalID].StartCoroutine(registry.portalArray[endPortalID].CountingDownCooldown());
            col.gameObject.transform.position = registry.portalArray[endPortalID].gameObject.transform.position;
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


    // private void RotateAfterTeleport(int startPortalID, int endPortalID, Collider col, PortalRegistry registry, Vector3 prePos)
    // {
    //     //direction vector from player to starting portal
    //     var beginTransform = registry.portalArray[startPortalID].gameObject.transform;
    //     var endTransform = registry.portalArray[endPortalID].gameObject.transform;

    //     // var direction = -portalEndT.InverseTransformDirection(col.GetComponent<CharacterController>().velocity);
    //     // var tempT = portalEndT;
    //     // //tempT.RotateAround(direction, -portalEndT.forward, 180);
    //     // //Debug.Draw
    //     // //var angle = Vector3.Angle(-portalEndT.forward, direction);
    //     // //print($"{angle}");
    //     // Debug.DrawLine(portalEndT.position, portalEndT.position + direction * 5, Color.blue, 5f);


    //     // var portalRot = registry.portalArray[endPortalID].gameObject.transform.rotation;
    //     // Quaternion rot = col.transform.rotation;

    //     // col.transform.rotation = portalRot;
    //     // //Rotate based on Verical Input
    //     // if(Input.GetAxis("Vertical") > 0)//if not going backward
    //     //     col.transform.Rotate(Vector3.up, 180);
        
    //     // //Resetting the look rotation to look perpendicular as portal's normal
    //     // if(col.transform.rotation.eulerAngles.x != 0 || col.transform.rotation.eulerAngles.z != 0)
    //     // {
    //     //     rot.eulerAngles = new Vector3(0, col.transform.rotation.eulerAngles.y, 0);
    //     //     col.transform.rotation = rot;
    //     // }
    //     // //Adjusting the look direction based on the walking vector the player is taking
    //     //     //Based on Horizontal Input
    //     // Debug.DrawLine(registry.portalArray[portalID].gameObject.transform.position, Vector3.up * 5, Color.blue, 5f);            
    //     // if(Input.GetAxis("Horizontal") < 0) //rotate from left to right
    //     //     col.transform.Rotate(Vector3.up, -90 * Input.GetAxis("Horizontal") + rot.eulerAngles.y);
    //     // else if (Input.GetAxis("Horizontal") > 0) //rotate from right to left
    //     //     col.transform.Rotate(Vector3.up, -90 * Input.GetAxis("Horizontal")+ rot.eulerAngles.y);
        
    // }

    private void OnDrawGizmosSelected()
    {

    }
}
