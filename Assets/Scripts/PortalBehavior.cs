using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehavior : MonoBehaviour
{
    public GameObject outline;
    public GameObject edgeChecker;
    public GameObject viewport;
    public float cooldownTimer = 0.1f;
    public bool canTeleport = true;

    
    [SerializeField] private GameObject _cam;
    private string _portalTag = "Portal", _portalableObjTag = "PortalableObject", _playerTag = "Player";
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private PortalRegistry registry;

    
    //[HideInInspector] public float cooldownCounting;

    private void Start() {
        registry = gameObject.transform.parent.GetComponent<PortalRegistry>();

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
        
        if((other.CompareTag(_portalableObjTag) || other.CompareTag(_playerTag)) && canTeleport)
        {
            if(registry.portalArray[0] == this && registry.portalArray[1].GetComponent<Collider>().enabled)//portal blue
            {
                //Disable collision between player layer and map geometry layer
                Physics.IgnoreLayerCollision(other.gameObject.layer, PlayerController.Instance.mainMapGeom.layer, true);
                Teleport(0, 1, other, registry);
            }
            else if (registry.portalArray[1] == this && registry.portalArray[0].GetComponent<Collider>().enabled)//portal red
            {
                Physics.IgnoreLayerCollision(other.gameObject.layer, PlayerController.Instance.mainMapGeom.layer, true);
                Teleport(1, 0, other, registry);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if((other.CompareTag(_portalableObjTag) || other.CompareTag(_playerTag)) && canTeleport)
        {
            //reenable the collision between player layer and map geometry layer
            Physics.IgnoreLayerCollision(other.gameObject.layer, PlayerController.Instance.mainMapGeom.layer, false);
        }
    }


    private void Teleport(int beginPortalID, int endPortalID, Collider col, PortalRegistry registry)
    {
        var beginTransform = registry.portalArray[beginPortalID].gameObject.transform;
        var endTransform = registry.portalArray[endPortalID].gameObject.transform;

        //Getting Dot product of start and end portal to see the alignment of the portal to their surface
        var dotValueStart = Vector3.Dot(PlayerController.Instance.gameObject.transform.forward, -beginTransform.forward);
        var dotValueEnd = Vector3.Dot(PlayerController.Instance.gameObject.transform.forward, -endTransform.forward);
        
        //Postion to teleport the object to
        var teleportPos = endTransform.TransformPoint(-Vector3.forward * registry.portalArray[endPortalID].gameObject.GetComponent<BoxCollider>().size.x/2);
        ///Teleport object infront of the teleport collision of portal with offset
        col.gameObject.transform.position = teleportPos;

        if(col.CompareTag(_playerTag))//if the gameobject is a player
        {
            ///Update rotation of object after teleport
            
            /*If the starting portal is on the ground, it's hard to convert the look direction player will have after teleportation 
            should the end portal have different rotatate orientation from the first
            -So if the starting portal is on the floor, there are 2 cases that handle if the ending portal is
                + on the floor
                + on a wall
            -Otherwise, if starting portal on a wall surface, it's easy rototate and only have 1 case
            */

            if((dotValueStart < .01f && dotValueStart > -.01f))//starting portal on the floor
            {
                //if the end portal is on the floor
                if((dotValueEnd < .01f && dotValueEnd > -.01f))
                {
                    SetRotationUpdateTP(beginTransform, endTransform, col);
                }
                //if the end portal is on a wall
                else
                {
                    //Based on the velocity of the player, if player moving backward, then face the portal, if not then away.
                    if(Input.GetAxis("Vertical") < 0)//facing away from the portal
                        col.transform.rotation = endTransform.rotation;
                    else if (Input.GetAxis("Vertical") >= 0) //facing same way as portal
                        col.transform.rotation = Quaternion.Euler(endTransform.rotation.eulerAngles.x, endTransform.rotation.eulerAngles.y + 180f, endTransform.rotation.eulerAngles.z);
                }
            }
            else //starting portal on a wall
            {
                SetRotationUpdateTP(beginTransform, endTransform, col);
            }
            
            //Resetting the Y-axis rotation of the player if it's been changed
            if(col.transform.rotation.x != 0 || col.transform.rotation.z != 0) { col.transform.rotation = Quaternion.Euler(0, col.transform.rotation.eulerAngles.y, 0); }
                

            ///update velocity of rigidbody

            /*There are 2 case to update rotation
                - One for when the object is on a 90 wall or on the floor
                - Other is for when the object is on a != 90 degree wall
            */

            if((dotValueEnd > .99f || dotValueEnd < -.99f) || (dotValueEnd < .01f && dotValueEnd > -.01f))//on 90 degree wall or on floor respectively
            {
                SetVelocityUpdateTP(beginTransform, endTransform, col); 
            }
            else//if on a tilted surface
            {
                var velocityMagnitude = col.transform.GetComponent<Rigidbody>().velocity.magnitude;
                //this section to find direction is utterly retarded, i should go to programmer's hell for this line
                var velocityDir = ((endTransform.position + -endTransform.forward * 3) - endTransform.position).normalized;

                //This is the part where this mechanic is really dodgy. This is due to the fact that the player has the ability to air strafe.
                //this led to very different result of velocity conversion after teleportation. So to somewhat make the two scenarios result in a same result, 
                //there are 2 ways to handle this "bug".
                //this could become problematic in the future, so I'll make sure to turn this into a feature since i can't figure out to fix this,
                //and properly guide player around the mechanic in terms of manipulating air strafings. 
                if(Input.GetAxis("Vertical") > .1f || Input.GetAxis("Vertical") < -.1f)//if player holding down w or s key while moving through
                    col.transform.GetComponent<Rigidbody>().velocity = (velocityDir * velocityMagnitude * 1.4f);
                else
                    col.transform.GetComponent<Rigidbody>().AddForce(velocityDir * velocityMagnitude * 1.6f, ForceMode.VelocityChange);
            }

            //Portal Teleport Cooldown
            registry.portalArray[endPortalID].StartCoroutine(registry.portalArray[endPortalID].CountingDownCooldown());
        }
        else//if the gameobject is a teleportable obj
        {
            SetRotationUpdateTP(beginTransform, endTransform, col);
            SetVelocityUpdateTP(beginTransform, endTransform, col);

            registry.portalArray[endPortalID].StartCoroutine(registry.portalArray[endPortalID].CountingDownCooldown());
        } 
    }

    //Update rotation of a collider after teleportation
    private void SetRotationUpdateTP(Transform beginTransform, Transform endTransform, Collider col)
    {
        Quaternion relativeRot = Quaternion.Inverse(beginTransform.rotation) * col.transform.rotation;
        relativeRot = halfTurn * relativeRot;
        col.transform.rotation = endTransform.rotation * relativeRot;
    }
    private void SetVelocityUpdateTP(Transform beginTransform, Transform endTransform, Collider col)
    {
        Vector3 relativeVel = beginTransform.InverseTransformDirection(col.transform.GetComponent<Rigidbody>().velocity);
        relativeVel = halfTurn * relativeVel;
        col.transform.GetComponent<Rigidbody>().velocity = endTransform.TransformDirection(relativeVel);
    }

    #region PortalPlacement
    public void AttemptPlacingPortal()
    {
        if(CheckPerimeter() && CheckNormalOverlap())
        {
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
