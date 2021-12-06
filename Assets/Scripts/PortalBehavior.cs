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

        // if(canTeleport && other.CompareTag(_playerTag) && registry.portalArray[1].GetComponent<Collider>().enabled && registry.portalArray[0].GetComponent<Collider>().enabled)
        //     Physics.IgnoreLayerCollision(other.gameObject.layer, PlayerController.Instance.mainMapGeom.layer, true);
        // else
        //     Physics.IgnoreLayerCollision(other.gameObject.layer, PlayerController.Instance.mainMapGeom.layer, false);

        //print($"{outline.activeInHierarchy} {viewport.activeInHierarchy} ");
        if((other.CompareTag(_portalableObjTag) || other.CompareTag(_playerTag)) && canTeleport)
        {
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
            //Update position of object
            var teleportPos = endTransform.TransformPoint(-Vector3.forward * registry.portalArray[endPortalID].gameObject.GetComponent<BoxCollider>().size.x/2);
            col.gameObject.transform.position = teleportPos;

            //Update rotation of object
            var dotValueStart = Vector3.Dot(col.transform.forward, -beginTransform.forward);
            var dotValueEnd = Vector3.Dot(col.transform.forward, -endTransform.forward);

            if((dotValueStart < .01f && dotValueStart > -.01f))//starting portal on the floor
            {
                //if the end portal is on the floor
                if((dotValueEnd < .01f && dotValueEnd > -.01f))
                {
                    Quaternion relativeRot = Quaternion.Inverse(beginTransform.rotation) * col.transform.rotation;
                    relativeRot = halfTurn * relativeRot;
                    col.transform.rotation = endTransform.rotation * relativeRot;
                }
                //if the end portal is on a wall
                else
                {
                    if(Input.GetAxis("Vertical") < 0)//facing away from the portal
                        col.transform.rotation = endTransform.rotation;

                    else if (Input.GetAxis("Vertical") >= 0) //facing same way as portal
                        col.transform.rotation = Quaternion.Euler(endTransform.rotation.eulerAngles.x, endTransform.rotation.eulerAngles.y + 180f, endTransform.rotation.eulerAngles.z);
                        //Debug.DrawLine(col.transform.position, col.transform.position + col.GetComponent<Rigidbody>().velocity.normalized * 3, Color.white, 5f);
                }
            }
            else //starting portal on a wall
            {
                Quaternion relativeRot = Quaternion.Inverse(beginTransform.rotation) * col.transform.rotation;
                relativeRot = halfTurn * relativeRot;
                col.transform.rotation = endTransform.rotation * relativeRot;
            }
            //Resetting the Y-axis rotation if it's been changed
            if(col.transform.rotation.x != 0 || col.transform.rotation.z != 0)
            {
                //print($"my parents beat me");
                col.transform.rotation = Quaternion.Euler(0, col.transform.rotation.eulerAngles.y, 0);
            }
                

            //update velocity of rigidbody
            if((dotValueEnd > .99f || dotValueEnd < -.99f) || (dotValueEnd < .01f && dotValueEnd > -.01f))//on 90 degree wall or on floor respectively
            {
                Vector3 relativeVel = beginTransform.InverseTransformDirection(col.transform.GetComponent<Rigidbody>().velocity);
                relativeVel = halfTurn * relativeVel;
                col.transform.GetComponent<Rigidbody>().velocity = endTransform.TransformDirection(relativeVel);                
            }else//if on a tilted surface
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

    private void OnDrawGizmosSelected()
    {

    }
}
