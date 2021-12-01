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
    [SerializeField] private float _jumpForce = 10f;
    //[SerializeField] private float _gravityScale = 1.0f;
    private Vector3 _moveVector = Vector3.zero;
    //private float _gravityAcceleration = 9.8f;


    [Header("Look Settings")]
    public Camera playerCam;
    public float lookSpeed = 2.5f;
    public float lookXLimit = 80.0f;
    private float _rotationX = 0;
    
    [Header("Debug Menu")]
    [SerializeField] private Vector3 _velocity;


    // [Header("Ground Check Properties")]
    // [SerializeField] private GameObject _groundChecker; 
    // [SerializeField] private float _checkerRadius;
    // [SerializeField] private bool _canJump;
    
    [HideInInspector]
    public bool canMove = true;
 
    //[HideInInspector] public bool canMove = true;
    private Rigidbody _rgbd;
    //Vector3 _inputs;

    private void Start()
    {
        Instance = this;
        _controller = GetComponent<CharacterController>();
        //_rgbd = GetComponent<Rigidbody>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //CheckGround();
        PlayerLook();
        PlayerMovement();
        _velocity = _controller.velocity;
    }

    // private void CheckGround()
    // {
    //     RaycastHit[] colArr = Physics.SphereCastAll(_groundChecker.transform.position, _checkerRadius, Vector3.down, 0);
    //     for(int i = 0; i < colArr.Length; i++)
    //     {
    //         print($"{colArr[i].collider.tag}");
    //         if(colArr[i].collider.tag != gameObject.tag)
    //         {
    //             _canJump = true;
    //             return;  
    //         }
    //     }
    //     _canJump = false;
    //     return;
    // }
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
        //Calculate move direction based on 2 axes
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);
        
        //Moving the player
        var curSpeedX = canMove ?  _walkingSpeed * Input.GetAxis("Vertical") : 0;
        var curSpeedY = canMove ? _walkingSpeed * Input.GetAxis("Horizontal") : 0;
        var movementDirectionY = _moveVector.y;
        _moveVector = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && _controller.isGrounded)
            _moveVector.y = _jumpForce;
        else
            _moveVector.y = movementDirectionY;

        //Damping vertical acceleration to simulate gravity
        if (!_controller.isGrounded)
            _moveVector.y -= 20f * Time.deltaTime;

        // Move the controller
        _controller.Move(_moveVector * Time.deltaTime);
    }

    private void OnDrawGizmosSelected() {
        //Gizmos.DrawWireSphere(_groundChecker.transform.position, _checkerRadius);
    }


}
