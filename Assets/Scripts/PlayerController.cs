using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private CharacterController _controller;
    [SerializeField] private GameObject _capsule;

    [Space]
    [Header("Movement Settings")]
    [SerializeField] private float _walkingSpeed = 10f;
    [Space]
    [SerializeField] private float _inertiaForce = 5f;
    [SerializeField] private float _airControl_Intensity = 2f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _gravityScale = 1.0f;
    private float _gravityAcceleration = 9.8f;


    [Header("Look Settings")]
    public Camera playerCam;
    public float lookSpeed = 2.5f;
    public float lookXLimit = 80.0f;
    private float _rotationX = 0;
    [HideInInspector] public Quaternion targetRotation { private set; get; }
    

    [Header("Ground Check Properties")]
    [SerializeField] private GameObject _groundChecker; 
    [SerializeField] private float _checkerRadius;
    private bool _canJump;
    
    [Header("Object Interaction Properties")]
    // [SerializeField] private Transform _holdT;
    // [SerializeField] private float _pickupRadius;
    // [SerializeField] private LayerMask _pickUpLayer;
    // private RaycastHit[] _objArr;
    // private GameObject _objectToHold;
    // private bool isInteract = false;

    [HideInInspector] public bool canMove = true;
    [Space]
    public GameObject mainMapGeom;
 
    private Rigidbody _rgbd;
    
    //Vector3 _inputs;

    private void Start()
    {
        Instance = this;
        targetRotation = transform.rotation;
        _controller = GetComponent<CharacterController>();
        _rgbd = GetComponent<Rigidbody>();
        //mainMapGeom = GameObject.FindWithTag("MainMapGeom");
        

        _rgbd.useGravity = false;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //basic movements
        CheckGround();
        PlayerLook();
        PlayerMovement();
        //basic interactions
        //ObjectInteraction();
    }

    private void LateUpdate() 
    {
        CustomGravity();
        CapsuleReadjustment();
    }

    #region Basic Movement
    private void CheckGround()
    {
        RaycastHit[] colArr = Physics.SphereCastAll(_groundChecker.transform.position, _checkerRadius, Vector3.down, 0);
        for(int i = 0; i < colArr.Length; i++)
        {
            if(colArr[i].collider.tag != gameObject.tag)
            {
                _canJump = true;
                return;  
            }
           
        }
        _canJump = false;
        return;
    }
    private void PlayerLook()
    {
        // Player and Camera rotation
        if (canMove)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
            playerCam.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        else
            return;
    }
    private void PlayerMovement()
    {   
        var moveVector = transform.TransformDirection(canMove ? new Vector3(_walkingSpeed * Input.GetAxis("Horizontal"), 0f, _walkingSpeed * Input.GetAxis("Vertical")) : Vector3.zero);
        if(_canJump)
            _rgbd.velocity = new Vector3(moveVector.x, _rgbd.velocity.y, moveVector.z);
        else
        {
            moveVector.z = 0;
            _rgbd.AddForce(moveVector.normalized * _inertiaForce * _airControl_Intensity, ForceMode.Acceleration);
        }


        if(Input.GetKeyDown(KeyCode.Space) && _canJump)
        {
            _rgbd.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _rgbd.AddForce(moveVector.normalized * _inertiaForce, ForceMode.Impulse);
        }
    }
    #endregion
    #region Custom Realignment
    private void CustomGravity()
    {
        Vector3 gravityVector = -_gravityAcceleration * _gravityScale * Vector3.up;
        _rgbd.AddForce(gravityVector, ForceMode.Acceleration);
    }
    private void CapsuleReadjustment()
    {
        _capsule.transform.localPosition = new Vector3(0,0,0);
        _capsule.transform.localRotation = Quaternion.identity;
    }
    #endregion

    // private void ObjectInteraction()
    // {
        
    //     if(Input.GetKeyDown(KeyCode.E))
    //     {   
    //         _objArr = Physics.SphereCastAll(_holdT.transform.position, _pickupRadius, Vector3.up, 0, _pickUpLayer);
    //         print($"{_objArr.Length}");
    //         if(_objArr.Length > 0)
    //         {
    //             if(_objArr[0].collider != null)
    //             {
    //                 _objectToHold = _objArr[0].collider.gameObject;
    //                 isInteract = true;
    //             }
    //         }
    //     }
    //     if(Input.GetKey(KeyCode.E) && isInteract)
    //     {
    //         _objectToHold.transform.position = _holdT.position;
    //     }
    //     if(Input.GetKeyUp(KeyCode.E) && isInteract)
    //     {
    //         isInteract = false;
    //         _objectToHold = null;
    //     }
    // }
    // private void OnDrawGizmosSelected() {
    //     Gizmos.DrawWireSphere(_groundChecker.transform.position, _checkerRadius);
    //     Gizmos.DrawWireSphere(_holdT.position, _pickupRadius);
    // }


}
