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
    [SerializeField] private float _blindSightDistance = 4f;
    private float _currentSpeed = 0f;
    private float _currentDist = 0f;

    [Header("Rotation Properties")]
    [SerializeField] private float _rotationSpeed = 100f;
    private Quaternion _lookRot;

    private void Start()
    {
        mainCamera = Camera.main;
    }


    //Interactable Object detections and distance check
    void Update()
    {
        //Here we check if we're currently looking at an interactable object
        _raycastPos = mainCamera.transform.position;
        RaycastHit hit;
        //if spherecast valid, get the gameobj, if not then null
        _targetObj = Physics.SphereCast(_raycastPos, _sphereCastRadius, mainCamera.transform.forward, out hit, _maxDistance, _interactableLayer) ? hit.collider.transform.root.gameObject : null;


        //if we press the button of choice
        if (Input.GetKeyDown(KeyCode.E))
        {
            //and we're not holding anything
            if (_currPickedUpObj == null)
            {//and we are looking an interactable object
                if (_targetObj != null) { PickUpObject(_targetObj.GetComponent<ObjectInteraction>()); }
            }
            //if we press the pickup button and have something, we drop it
            else { BreakConnection(_targetObj.GetComponent<ObjectInteraction>()); }
        }

      
    }

    //Velocity movement toward pickup parent and rotation
    private void FixedUpdate()
    {
        if (_currPickedUpObj != null)
        {
            _currentDist = Vector3.Distance(_pickupParent.position, _pickupRB.position);
            _currentSpeed = Mathf.SmoothStep(_minSpeed, _maxSpeed, _currentDist / _maxDistance);
            _currentSpeed *= Time.fixedDeltaTime;
            Vector3 direction = _pickupParent.position - _pickupRB.position;
            _pickupRB.velocity = direction.normalized * _currentSpeed;
            //Rotation
            _lookRot = Quaternion.LookRotation(mainCamera.transform.position - _pickupRB.position);
            _lookRot = Quaternion.Slerp(mainCamera.transform.rotation, _lookRot, _rotationSpeed * Time.fixedDeltaTime);
            _pickupRB.MoveRotation(_lookRot);
        }

    }

    //Release the object
    public void BreakConnection(ObjectInteraction obj)
    {
        _pickupRB.constraints = RigidbodyConstraints.None;
        _currPickedUpObj = null;
        obj.pickedUp = false;
        _currentDist = 0;
    }

    public void PickUpObject(ObjectInteraction obj)
    {
        obj = _targetObj.GetComponentInChildren<ObjectInteraction>();
        _currPickedUpObj = _targetObj;
        _pickupRB = _currPickedUpObj.GetComponent<Rigidbody>();
        _pickupRB.constraints = RigidbodyConstraints.FreezeRotation;
        obj.playerInteraction = this;
        StartCoroutine(obj.PickUp());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_pickupParent.position, _sphereCastRadius);
        Gizmos.DrawWireSphere(_pickupParent.position + _pickupParent.forward * _maxDistance, _sphereCastRadius);
    }


}