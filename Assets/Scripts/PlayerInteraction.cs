using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interactable Info")]
    [SerializeField] private float _sphereCastRadius = 0.5f;
    [SerializeField] private LayerMask _interactableLayer;
    private Vector3 _raycastPos;
    [SerializeField] private GameObject _targetObj;
    private Camera mainCamera;

    [Header("Pickup")]
    [SerializeField] private Transform _pickupParent;
    [SerializeField] GameObject _currPickedUpObj;
    private Rigidbody _pickupRB;

    [Header("Pick Up Object Properties")]
    [SerializeField] private float _minSpeed = 0;
    [SerializeField] private float _maxSpeed = 300f;
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _blindSightRadius = 4f;//if distance between pickup object and player exceed this amount, automatically break conenction
    private float _currentSpeed = 0f;
    private float _currentDist = 0f;

    [Header("Rotation Properties")]
    [SerializeField] private float _rotationSpeed = 100f;
    private Quaternion _lookRot;
    private PortalBehavior[] _portaArray;
    private void Start()
    {
        mainCamera = Camera.main;
        _portaArray = GetComponent<PortalPlacement>().portalPair.portalArray;
    }


    //Interactable Object detections and distance check
    private void Update()
    {
        //Pressing Pickup Button
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Spherecast to check for pickup-able object
            _targetObj = Physics.SphereCast(mainCamera.transform.position, _sphereCastRadius, mainCamera.transform.forward, out var hit, _maxDistance, _interactableLayer) ? hit.collider.transform.root.gameObject : null;
            //and we're not holding anything
            if (_currPickedUpObj == null)
            {
                //and we are looking an interactable object
                if (_targetObj != null) { PickUpObject(_targetObj.GetComponent<ObjectInteraction>()); }
            }
        }
        //Let go Pickup button
        if(Input.GetKeyUp(KeyCode.E))
        {
            //If we're holding something
            if(_currPickedUpObj != null)
            {
                //Break the connection
                BreakConnection(_currPickedUpObj.GetComponent<ObjectInteraction>()); 
            }
        }

      
    }
    private void FixedUpdate()
    {
        if (_currPickedUpObj == null)
            return;
        
        //Setting Velocity of pick up object
        //getting distance from pickup object to the point where the pickup should be
        _currentDist = Vector3.Distance(_pickupParent.position, _pickupRB.position);
        //smoothstep lerp elocity magnitude of pickup object relative to how far current distance is to max distance
        _currentSpeed = Mathf.SmoothStep(_minSpeed, _maxSpeed, _currentDist / _maxDistance);
        _currentSpeed *= Time.fixedDeltaTime;
        //Getting direction and apply that direction and speed to the rigid body of pickup
        Vector3 direction = _pickupParent.position - _pickupRB.position;
        _pickupRB.velocity = direction.normalized * _currentSpeed;

        //Setting rotation of pickup object
        //Getting the vector from object pickup to the camera
        _lookRot = Quaternion.LookRotation(mainCamera.transform.position - _pickupRB.position);
        //alawys lerp that rotation so that the pickup object would alawys face that vector
        _lookRot = Quaternion.Slerp(mainCamera.transform.rotation, _lookRot, _rotationSpeed * Time.fixedDeltaTime);
        _pickupRB.MoveRotation(_lookRot);

        //If the object goes past a threshold while being picked up, release it
        if(_currentDist > _blindSightRadius)
        {
            foreach(PortalBehavior portal in _portaArray)
            {
                if(portal.canTeleport == false)
                {
                    return;
                }
            }
            BreakConnection(_currPickedUpObj.GetComponent<ObjectInteraction>());
        }
        
        

    }

    //Release the object
    public void BreakConnection(ObjectInteraction obj)
    {
        //De-Parent the obj after being dropped
        _currPickedUpObj.transform.parent = null;
        //Release all constraints of rigid body
        _pickupRB.constraints = RigidbodyConstraints.None;
        //Nullify the currPickedupObj is linked to
        _currPickedUpObj = null;
        //Set pickup status of object to false
        obj.pickedUp = false;
        //set Current Dist to 0
        _currentDist = 0;
    }

    public void PickUpObject(ObjectInteraction obj)
    {
        obj = _targetObj.GetComponentInChildren<ObjectInteraction>();
        _currPickedUpObj = _targetObj;
        
        //Setting parent of pickup Object to the player so that player can teleport with the object
        _currPickedUpObj.transform.SetParent(transform, true);

        //Getting the Rigidbody component
        _pickupRB = _currPickedUpObj.GetComponent<Rigidbody>();
        //Freeze rotation for rigid body
        _pickupRB.constraints = RigidbodyConstraints.FreezeRotation;
        //Call routine for validating pickup status of object
        StartCoroutine(obj.PickUp());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_pickupParent.position, _sphereCastRadius);
        Gizmos.DrawWireSphere(_pickupParent.position + _pickupParent.forward * _maxDistance, _sphereCastRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_pickupParent.position, _blindSightRadius);
    }


}