using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interactable Info")]
    [SerializeField] private float _sphereCastRadius = 0.5f;
    [SerializeField] private LayerMask _interactableLayer;
    private Vector3 _raycastPos;
    [SerializeField] private GameObject _targetObj;
    private Camera mainCamera;
    [HideInInspector] public bool isPickingUp = false;

    [Header("Pickup")]
    public Transform pickupParent;
    [SerializeField] GameObject _currPickedUpObj;
    private Rigidbody _pickupRB;

    [Header("Pick Up Object Properties")]
    [SerializeField] private float _minSpeed = 0;
    [SerializeField] private float _maxSpeed = 300f;
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _blindSightRadius = 4f;//if distance between pickup object and player exceed this amount, automatically break conenction
    private float _currentSpeed = 0f;
    private float _currentDist = 0f;
    private Transform _originalParent;

    [Header("Object Spawn Button Properties")] 
    [SerializeField] private float _castDistance = 3f;
    [SerializeField] private LayerMask _buttonLayer;

    [Header("Rotation Properties")]
    [SerializeField] private float _rotationSpeed = 100f;
    private Quaternion _lookRot;
    private void Start()
    {
        mainCamera = Camera.main;
    }


    //Interactable Object detections and distance check
    private void Update()
    {
        //Pressing Pickup Button
        if (Input.GetKeyDown(KeyCode.E))
        {
            AttemptObjectInteraction();
            AttemptObjectSpawnButtonInteraction();
        }
    }

    private void AttemptObjectInteraction()
    {
        //Spherecast to check for pickup-able object
        _targetObj = Physics.SphereCast(mainCamera.transform.position, _sphereCastRadius, mainCamera.transform.forward, out var hit, _maxDistance, _interactableLayer) ? hit.collider.transform.root.gameObject : null;
        //and we're not holding anything
        if (_currPickedUpObj == null)
        {
            //and we are looking an interactable object
            if (_targetObj != null) { PickUpObject(_targetObj.GetComponentInChildren<ObjectInteraction>(), false); }
        }
        else
        {
            //Reset collider to re-trigger portal's trigger volume should object be released inside portal
            _currPickedUpObj.GetComponent<Collider>().enabled = false;
            _currPickedUpObj.GetComponent<Collider>().enabled = true;
            //Break the connection
            BreakConnection(_currPickedUpObj.GetComponent<ObjectInteraction>()); 
                
        }
    }
    private void AttemptObjectSpawnButtonInteraction()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out var hit, _castDistance, _buttonLayer))
        {
            hit.collider.transform.root.GetComponent<ObjectDropperInteraction>().ResetCubeTransform();
        }
    }
    
    private void FixedUpdate()
    {
        if (_currPickedUpObj == null)
            return;
        
        //If the object goes past a threshold while being picked up, release it
        if(_currentDist > _blindSightRadius)
        {
            if (!PortalRegistry.Instance.GetIsAnyOnCoolDown())//none of the portals on cooldown
            {
                print($"too far, breaking");
                BreakConnection(_currPickedUpObj.GetComponent<ObjectInteraction>());
                return;
            }
        }


        //Setting Velocity of pick up object
        //getting distance from pickup object to the point where the pickup should be
        _currentDist = Vector3.Distance(pickupParent.position, _pickupRB.position);
        //smooth step lerp velocity magnitude of pickup object relative to how far current distance is to max distance
        _currentSpeed = Mathf.SmoothStep(_minSpeed, _maxSpeed, _currentDist / _maxDistance);
        _currentSpeed *= Time.fixedDeltaTime;
        //Getting direction and apply that direction and speed to the rigid body of pickup
        Vector3 direction = pickupParent.position - _pickupRB.position;
        _pickupRB.velocity = direction.normalized * _currentSpeed;

        //Setting rotation of pickup object
        //Getting the vector from object pickup to the camera
        _lookRot = Quaternion.LookRotation(mainCamera.transform.position - _pickupRB.position);
        //always lerp that rotation so that the pickup object would always face that vector
        _lookRot = Quaternion.Slerp(mainCamera.transform.rotation, _lookRot, _rotationSpeed * Time.fixedDeltaTime);
        _pickupRB.MoveRotation(_lookRot);
    }

    //Release the object
    public void BreakConnection(ObjectInteraction obj)
    {
        isPickingUp = false;
        
        //Return the obj's original after being dropped
        _currPickedUpObj.transform.parent = _originalParent;
        //Release all constraints of rigid body
        _pickupRB.constraints = RigidbodyConstraints.None;
        _targetObj = null;
        _pickupRB = null;
        //Nullify the currPickedupObj is linked to
        _currPickedUpObj = null;
        //Set pickup status of object to false
        obj.pickedUp = false;
        //set Current Dist to 0
        _currentDist = 0;
    }

    public void PickUpObject(ObjectInteraction obj, bool isAfterTp)
    {
        isPickingUp = true;
        _currPickedUpObj = obj.gameObject;
        print($"{_currPickedUpObj}");
        if(!isAfterTp)
            _originalParent = _currPickedUpObj.transform.parent;
        print($"{_originalParent}");
        
        
        //Setting parent of pickup Object to the player so that player can teleport with the object
        _currPickedUpObj.transform.SetParent(transform, true);

        //Getting the Rigidbody component
        _pickupRB = _currPickedUpObj.GetComponent<Rigidbody>();
        //Freeze rotation for rigid body
        _pickupRB.constraints = RigidbodyConstraints.FreezeRotation;
        //Call routine for validating pickup status of object
        StartCoroutine(obj.PickUp());
    }

    public void TeleportCurrPickedUpObj()
    {
        _currPickedUpObj.transform.position = pickupParent.transform.position;
        PickUpObject(_currPickedUpObj.GetComponent<ObjectInteraction>(), true);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pickupParent.position, _sphereCastRadius);
        Gizmos.DrawWireSphere(pickupParent.position + pickupParent.forward * _maxDistance, _sphereCastRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(pickupParent.position, _blindSightRadius);
        Gizmos.color = Color.green;
        if (mainCamera == null) return;
        Gizmos.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * _castDistance);
    }


}