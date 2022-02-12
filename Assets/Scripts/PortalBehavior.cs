using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// <para>  This Class handles <br/>
///         1. Teleportation mechanics of teleport-able objects, as well as issuing cooldown <br/>
///         2. Checking surrounding environment for valid placement of current portal
///         (normal overlap and perimeter crossover checks)
/// </para>
/// </summary>
public class PortalBehavior : MonoBehaviour
{
    public bool canTeleport = true;

    ///<summary>Primary Game Object of Portal, contains out line portal mesh, and particle system component</summary>
    [FormerlySerializedAs("outline")] [SerializeField]
    private GameObject _outline;

    ///<summary>Placed at Vector3.zero relative to portal parent transform calculate perimeter overlap for portal placement</summary>
    public GameObject edgeChecker;

    ///<summary>Surface of portal with Fresnel's swirling fx shader</summary>
    [FormerlySerializedAs("viewport")] [SerializeField]
    private GameObject _viewport;

    ///<summary>Cooldown timer for portal</summary>
    [FormerlySerializedAs("cooldownTimer")] [SerializeField]
    private float _cooldownTimer = 0.1f;

    ///<summary>main Camera game object of the scene </summary>
    [SerializeField] private GameObject _cam;


    
    //const string tags
    private const string PortalTag = "Portal", PortalObjTag = "PortalableObject", PlayerTag = "Player";


    ///<summary>quaternion offset to rotate player or object's rotation or velocity direction after teleportation</summary>
    private static readonly Quaternion HalfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    ///<summary>Registry of all the portal</summary>
    private PortalRegistry _registry;


    private void Start()
    {
        _registry = gameObject.transform.parent.GetComponent<PortalRegistry>(); //Getting portal registry

        //Disable portal and Deactivate visual of portal at start
        canTeleport = true;
        TogglePortal(false);
    }

    private void Update()
    {
        //If visual of portal is active, enable the collider for teleportation
        if (_outline.activeInHierarchy && _viewport.activeInHierarchy)
            gameObject.GetComponent<Collider>().enabled = true;
        else
            gameObject.GetComponent<Collider>().enabled = false;
    }
    

    /// <summary>
    /// Coroutine class that deactivate teleportation mechanic for "_cooldownTimer" amount of time then turn it back on
    /// </summary>
    /// <returns></returns>
    private IEnumerator CountingDownCooldown()
    {
        canTeleport = false;
        yield return new WaitForSeconds(_cooldownTimer);
        canTeleport = true;
    }

    /// <summary>
    /// <para>  Handles collision detection when an object enters the portal.<br/>
    ///         Handle filtering and only interact with player or teleport-able objects.
    ///         If current portal is blue, then teleport to red and vice versa.<br/>
    /// </para>
    /// </summary>
    /// <param name="other">Collider of Game Object that collides with portal's trigger collider</param>
    private void OnTriggerStay(Collider other)
    {
        int tpLayer = PortalRegistry.Instance.teleportLayer;
        //Comparing tag and make sure if current teleport can teleport
        if ((!other.CompareTag(PortalObjTag) && !other.CompareTag(PlayerTag))) return;
        if (_registry.portalArray[0] == this &&
            _registry.portalArray[1].GetComponent<Collider>().enabled) //portal blue
        {
            other.gameObject.layer = tpLayer;
            //Disable collision between player layer and map geometry layer
            Physics.IgnoreLayerCollision(tpLayer, PlayerController.Instance.mainMapGeom.layer, true);
            
            //Call teleport method to teleport
            if (!canTeleport) return;
            Teleport(0, 1, other, _registry);
        }
        else if (_registry.portalArray[1] == this &&
                 _registry.portalArray[0].GetComponent<Collider>().enabled) //portal red
        {
            other.gameObject.layer = tpLayer;
            Physics.IgnoreLayerCollision(tpLayer, PlayerController.Instance.mainMapGeom.layer, true);
            if (!canTeleport) return;
            Teleport(1, 0, other, _registry);
        }
    }

    /// <summary>
    /// <para>  Handle game object's collider leaving portal's collider,
    ///         filter the incoming collider by player and portable-able objects.
    /// </para>
    /// </summary>
    /// <param name="other">Collider of Game Object that leaves the trigger collider of portal</param>
    private void OnTriggerExit(Collider other)
    {
        if ((!other.CompareTag(PortalObjTag) && !other.CompareTag(PlayerTag))) return;
        
        // if(other.GetComponent<ObjectInteraction>())
        //     print($"{other.GetComponent<ObjectInteraction>().trueLayer.value}");
        
        other.gameObject.layer = other.CompareTag(PortalObjTag) ? other.GetComponent<ObjectInteraction>().trueLayer : other.GetComponent<PlayerController>().trueLayer;
        
        //reenable the collision between player layer and map geometry layer
        Physics.IgnoreLayerCollision(other.gameObject.layer, PlayerController.Instance.mainMapGeom.layer, false);
    }

    /// <summary>
    /// <para>  Handle teleport mechanic between red and blue portal in the scene.<br/>
    ///         Handle rotation and velocity vector preservation after teleportation to replicate the "feel" of teleportation
    ///         transitioning from one to another.
    /// </para>
    /// </summary>
    /// <param name="beginPortalID">ID of begin portal</param>
    /// <param name="endPortalID">ID of destination portal</param>
    /// <param name="col"> Incoming collider of player or portal-able object</param>
    /// <param name="registry"> registry of all the available portals in the scene</param>
    private void Teleport(int beginPortalID, int endPortalID, Collider col, PortalRegistry registry)
    {
        //Portal Teleport Cooldown
        registry.portalArray[endPortalID].StartCoroutine(registry.portalArray[endPortalID].CountingDownCooldown());
        //Caching values
        Transform beginTransform = registry.portalArray[beginPortalID].gameObject.transform; //transform of start portal
        Transform endTransform = registry.portalArray[endPortalID].gameObject.transform; //transform of end portal
        Vector3 playerFwd = PlayerController.Instance.gameObject.transform.forward; //forward vector of player

        //Getting Dot product of start and end portal to see the alignment of the portal to their surface
        float dotValueStart = Vector3.Dot(playerFwd, -beginTransform.forward);
        float dotValueEnd = Vector3.Dot(playerFwd, -endTransform.forward);
        
        
        if (col.CompareTag(PlayerTag)) //if the game object is a player
        {
            //print($"tp player");
            
            //Teleport to position
            SetPositionUpdateTP(endTransform, registry, endPortalID, col);
            /*
            Update rotation of object after teleport
            If the starting portal is on the ground, it's hard to convert the look direction player will have after teleportation 
            should the end portal have different rotate orientation from the first
            -So if the starting portal is on the floor, there are 2 cases that handle if the ending portal is
                + on the floor
                + on a wall
            -Otherwise, if starting portal on a wall surface, it's easy rotate and only have 1 case
            */
            if ((dotValueStart < .01f && dotValueStart > -.01f)) //starting portal on the floor
            {
                //if the end portal is on the floor
                if ((dotValueEnd < .01f && dotValueEnd > -.01f))
                {
                    SetRotationUpdateTP(beginTransform, endTransform, col);
                }
                //if the end portal is on a wall
                else
                {
                    //Based on the velocity of the player, if player moving backward, then face the portal, if not then away.
                    if (Input.GetAxis("Vertical") < 0)
                    {
                        col.transform.rotation = endTransform.rotation;
                    } //facing away from the portal
                    else if (Input.GetAxis("Vertical") >= 0)
                    {
                        //facing same way as portal
                        Quaternion endRot = endTransform.rotation;
                        col.transform.rotation = Quaternion.Euler(endRot.eulerAngles.x, endRot.eulerAngles.y + 180f,
                            endRot.eulerAngles.z);
                    }
                }
            }
            else
            {
                SetRotationUpdateTP(beginTransform, endTransform, col);
            } //starting portal on a wall

            //Resetting the Y-axis rotation of the player if it's been changed
            if (col.transform.rotation.x != 0 || col.transform.rotation.z != 0)
            {
                col.transform.rotation = Quaternion.Euler(0, col.transform.rotation.eulerAngles.y, 0);
            }

            /*
             update velocity of rigidbody
            There are 2 case to update rotation
                - One for when the object is on a 90 wall or on the floor
                - Other is for when the object is on a != 90 degree wall
            */
            if ((dotValueEnd > .99f || dotValueEnd < -.99f) ||
                (dotValueEnd < .01f && dotValueEnd > -.01f)) //on 90 degree wall or on floor respectively
            {
                SetVelocityUpdateTP(beginTransform, endTransform, col);
            }
            else //if on a tilted surface
            {
                float velocityMagnitude = col.transform.GetComponent<Rigidbody>().velocity.magnitude;
                //this section to find direction is relatively dumb, i should go to programmer's hell for this line
                Vector3 velocityDir = ((endTransform.position + -endTransform.forward * 3) - endTransform.position)
                    .normalized;

                //This is the part where this mechanic is really dodgy. This is due to the fact that the player has the ability to air strafe.
                //this led to very different result of velocity conversion after teleportation. So to somewhat make the two scenarios result in a same result, 
                //there are 2 ways to handle this 'bug'.
                //this could become problematic in the future, so I'll make sure to turn this into a feature since i can't figure out to fix this,
                //and properly guide player around the mechanic in terms of manipulating air strafing. 
                if (Input.GetAxis("Vertical") > .1f ||
                    Input.GetAxis("Vertical") < -.1f) //if player holding down w or s key while moving through
                    col.transform.GetComponent<Rigidbody>().velocity = (velocityDir * velocityMagnitude * 1.2f);
                else
                    col.transform.GetComponent<Rigidbody>().AddForce(velocityDir * velocityMagnitude * 1.6f,
                        ForceMode.VelocityChange);
            }

            var temp = col.GetComponent<PlayerInteraction>();
            if (temp.isPickingUp)
            {
                temp.TeleportCurrPickedUpObj();
                StartCoroutine(ColliderWarp(endTransform));
            }
        }
        else //if the game object is a teleport-able obj
        {
            if (col.GetComponent<ObjectInteraction>().pickedUp)
                return;
            
            SetPositionUpdateTP(endTransform, registry, endPortalID, col);
            SetRotationUpdateTP(beginTransform, endTransform, col);
            SetVelocityUpdateTP(beginTransform, endTransform, col);
        }
    }

    private IEnumerator ColliderWarp(Transform endTransform)
    {
        BoxCollider col = endTransform.GetComponent<BoxCollider>();
        col.center = PortalRegistry.Instance.colliderCenterArr[1];
        col.size = PortalRegistry.Instance.colliderSizeArr[1];
        yield return new WaitForSeconds(2);
        col.center = PortalRegistry.Instance.colliderCenterArr[0];
        col.size = PortalRegistry.Instance.colliderSizeArr[0];
    }
    
    /// <summary>
    /// <para>  Translate the position of portal-able object from starting portal to destination portal. <br/>
    ///         - Note that the position to be teleported to is in front of the end portal's trigger volume
    /// </para></summary>
    /// <param name="endTransform">Transform of destination portal</param>
    /// <param name="registry">Registry of all available portal in scene</param>
    /// <param name="endPortalID">ID of destination portal</param>
    /// <param name="col">collider of incoming portal-able object</param>
    private void SetPositionUpdateTP(Transform endTransform, PortalRegistry registry, int endPortalID, Collider col)
    {
        //Position to teleport the object to
        var teleportPos = endTransform.TransformPoint(-Vector3.forward *
            registry.colliderSizeArr[0].z * 2f);
        //Teleport object in front of the teleport collision of portal with offset
        if(col != null)
            col.gameObject.transform.position = teleportPos;
    }

    /// <summary>
    /// <para>  Converts rotation of collider coming in from portal A and mirror that rotation based on the the normal of
    ///         out coming portal B that it faces.<br/>
    /// <example>   Assuming that 2 portals are on the 90-degree wall, and player is coming into portal and facing in a
    ///             direction that's paralleled to the normal of portal A at opposite direction. You will come out of portal
    ///             B facing in a direction paralleled to normal of portal B with the same direction.<br/><br/>
    ///             (The direction the object is facing is not the same for directional vector because that's applied to velocity)
    /// </example></para></summary>
    /// <param name="beginTransform">Transform of starting portal</param>
    /// <param name="endTransform">Transform of destination portal</param>
    /// <param name="col">Collider of object being teleported</param>
    private void SetRotationUpdateTP(Transform beginTransform, Transform endTransform, Collider col)
    {
        //create a relative rotation to equal to the invert the quaternion of the portal-able object coming into the portal
        Quaternion relativeRot = Quaternion.Inverse(beginTransform.rotation) * col.transform.rotation;
        //Rotate the relative rotation by 180 degree on the y-axis
        relativeRot = HalfTurn * relativeRot;
        //Assign that value to the rotation of the object
        col.transform.rotation = endTransform.rotation * relativeRot;
    }
    /// <summary>
    /// <para>  Translate directional vector of velocity coming in from portal A to have correct relativity coming out portal B.
    /// <example>   Imagine the 2 portals facing back to back like a going through a window on a wall, the directional vector
    ///             should be the same on both sides of the wall going through the window.
    /// </example> </para> </summary>
    /// <param name="beginTransform"></param>
    /// <param name="endTransform"></param>
    /// <param name="col"></param>
    private void SetVelocityUpdateTP(Transform beginTransform, Transform endTransform, Collider col)
    {
        //Transform the world space velocity to local so that when rotate, the center won't based on the center of the world scene
        //and be on the player instead
        Vector3 relativeVel = beginTransform.InverseTransformDirection(col.transform.GetComponent<Rigidbody>().velocity);
        //Apply 180 degree on y-axis
        relativeVel = HalfTurn * relativeVel;
        //Convert back to world space velocity and assign it back to velocity
        col.transform.GetComponent<Rigidbody>().velocity = endTransform.TransformDirection(relativeVel);
    }

    public void TogglePortal(bool toggleState)
    {
        _outline.SetActive(toggleState);    
        _viewport.SetActive(toggleState);
        if (toggleState)
        {
            StartCoroutine(ExistConditionCheckRoutine());
        }
        else
        {
            PortalRegistry.Instance.DisableLaserOnAllPortal();
        }
    }

    private IEnumerator ExistConditionCheckRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (_outline.activeInHierarchy && _viewport.activeInHierarchy)
        {
            if (CheckPerimeter() && CheckNormalOverlap())
                yield return wait;
            else
                TogglePortal(false);
        }
       
    }
    
    
    #region PortalPlacement
    /// <summary>
    /// <para>  Checks for if it is possible to enable a portal at the current position set by PortalPlacement class<br/>
    ///         - Each portal has a edge checker game object that gets thrown around the scene as player attempt to place
    ///         a portal there, this function uses said game object as center to scan the surrounding and the front of the
    ///         portal.<br/>
    ///         Checks by calling CheckPerimeter() and CheckNormalOverlap() 
    /// </para></summary>
    public void AttemptPlacingPortal()
    {
        if(CheckPerimeter() && CheckNormalOverlap())//if the portal is qualified to be placed
        {
            //assigning portal transform if edge checker valid
            transform.position = edgeChecker.transform.position;  
            transform.rotation = edgeChecker.transform.rotation;
            //reset edge checker position to in center of portal parent obj
            edgeChecker.transform.position = transform.position;
            edgeChecker.transform.rotation = transform.rotation;
            //enable components to display the portal
            //enable the swirling shader viewport
            //activate the particle system
            TogglePortal(true);
            _outline.GetComponentInChildren<ParticleSystem>().Simulate(0.1f, false, true, false);
            _outline.GetComponentInChildren<ParticleSystem>().Play();


        }else{//if the portal can't be placed, shake screen to warn
            CameraShake.instance.Shake(0.2f,0.1f);
        }
    }
    public bool CheckPerimeter()
    {
        bool canPlace = true;
        var scale = transform.localScale.x;
        var posPerimList = new List<Vector3>()
        {
            new Vector3(-1.2f * scale, 0, 0),
            new Vector3(1.2f * scale, 0, 0),
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
            //turning world rot and pos to edgeChecker local relative pos and rot
            Vector3 edgePos = edgeChecker.transform.TransformPoint(posPerimList[i]);
            //Check if the edge touches a portal-able surface
            Collider[] hitCol = Physics.OverlapSphere(edgePos, 0.1f, PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer);
            //Debug.DrawLine(edgePos, edgePos + Vector3.forward, Color.red, 3f);
            
            
            if(hitCol.Length == 0 && i != posPerimList.Count-1)//has no portal-able surface on that point
            {
                //based on the edge checker position, get 4 directional vectors to check the surround max distance if
                //there's any portal-able surface nearby to shift the edgeChecker to that position as a part of auto-correct
                //system if player shoots at a non-portal-able surface that's in close proximity to one that does.
                var edgeDir = edgeChecker.transform.TransformDirection(dirPerimList[i]);
                //if nearby portal-able surface is found
                if(Physics.Raycast(edgePos, edgeDir, out var hit, 2f, PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer))
                {
                    var offset = hit.point - edgePos;//get the offset
                    edgeChecker.transform.Translate(offset, Space.World);//shift to that offset
                    canPlace = true;
                }else
                    canPlace = false;
            }   
            
            // var portalCol = Physics.OverlapSphere(edgePos, 0.2f);
            //
            // for(int j = 0; j < portalCol.Length; ++j)
            // {
            //     if(portalCol[j].tag == PortalTag)
            //     {
            //         portalCol[j].gameObject.GetComponent<PortalBehavior>().TogglePortal(false);
            //     }
            // }
        }
        //if(!canPlace)
            //print($"can't place here");
        return canPlace;
    }
    public bool CheckNormalOverlap()    
    {
        bool canPlace = true;
        
        var scaleX = transform.localScale.x;
        var scaleY = transform.localScale.y;
        
        var normalDrawPoints = new List<Vector3>()
        {
            new Vector3 (-1.2f * scaleX, 2 * scaleY, -0.2f),//top left
            new Vector3 (1.2f * scaleX, 2 * scaleY, -0.2f),//top right
            new Vector3 (1.2f * scaleX, -2 * scaleY, -0.2f),//bottom right
            new Vector3 (-1.2f * scaleX,-2 * scaleY,-0.2f)//bottom left
            
        };

        var normalDrawDists = new List<float>()
        {
            2.4f * scaleX, 4f * scaleY, 2.4f * scaleX, 4f * scaleY
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
            
            if(Physics.Raycast(startPoint, dir , out var hit, normalDrawDists[i] , PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer)){
                canPlace = false;
                //print($"normal overlapped ! ");  
            }
        }
        return canPlace;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {

    }
}
