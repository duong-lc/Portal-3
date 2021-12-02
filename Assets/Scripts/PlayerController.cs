using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private CharacterController _controller;

    [Header("Movement Settings")]
    [SerializeField] private float _walkingSpeed = 10f;
    [Space]
    [SerializeField] private float _inertiaForce = 5f;
    [SerializeField] private float _airControl_Intensity = 2f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _gravityScale = 1.0f;
    //private Vector3 _moveVector = Vector3.zero;
    private float _gravityAcceleration = 9.8f;


    [Header("Look Settings")]
    public Camera playerCam;
    public float lookSpeed = 2.5f;
    public float lookXLimit = 80.0f;
    private float _rotationX = 0;
    
    [Header("Debug Menu")]
    [SerializeField] private Vector3 _velocity;


    [Header("Ground Check Properties")]
    [SerializeField] private GameObject _groundChecker; 
    [SerializeField] private float _checkerRadius;
    [SerializeField] private bool _canJump;
    
    [HideInInspector]
    public bool canMove = true;
 
    //[HideInInspector] public bool canMove = true;
    private Rigidbody _rgbd;
    //Vector3 _inputs;

    private void Start()
    {
        Instance = this;
        _controller = GetComponent<CharacterController>();
        _rgbd = GetComponent<Rigidbody>();
        

        _rgbd.useGravity = false;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckGround();
        PlayerLook();
        PlayerMovement();

        _velocity = _controller.velocity;
    }

    private void LateUpdate() {
        CustomGravity();
    }

    private void CheckGround()
    {
        RaycastHit[] colArr = Physics.SphereCastAll(_groundChecker.transform.position, _checkerRadius, Vector3.down, 0);
        for(int i = 0; i < colArr.Length; i++)
        {
            //print($"{colArr[i].collider.tag}");
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
        //_rgbd.AddForce(moveVector*0.1f, ForceMode.VelocityChange);


        if(Input.GetKeyDown(KeyCode.Space) && _canJump)
        {
            _rgbd.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _rgbd.AddForce(moveVector.normalized * _inertiaForce, ForceMode.Impulse);
        }
            

    
    }

    private void CustomGravity()
    {
        Vector3 gravityVector = -_gravityAcceleration * _gravityScale * Vector3.up;
        _rgbd.AddForce(gravityVector, ForceMode.Acceleration);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(_groundChecker.transform.position, _checkerRadius);
    }


}
