using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public float walkingSpeed = 10f;
    public float jumpForce = 10f;
    public float gravity = 20.0f;
    public Camera playerCam;
    public float lookSpeed = 2.5f;
    public float lookXLimit = 80.0f;

    private CharacterController _controllerComponent;
    private Vector3 _moveDirection = Vector3.zero;
    private float _rotationX = 0;
    [SerializeField] private Vector3 velocity;
 
    [HideInInspector]
    public bool canMove = true;

    private void Start()
    {
        Instance = this;
        _controllerComponent = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        PlayerLook();
        PlayerMovement();
        velocity = _controllerComponent.velocity;

        if(Input.GetKeyDown(KeyCode.Z))
        {
            print($"fly");
            _controllerComponent.enabled = false;
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 100);
            _controllerComponent.enabled = true;
        }
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
        //Calculate move direction based on 2 axes
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);
        
        //Moving the player
        var curSpeedX = canMove ?  walkingSpeed * Input.GetAxis("Vertical") : 0;
        var curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;
        var movementDirectionY = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && _controllerComponent.isGrounded)
            _moveDirection.y = jumpForce;
        else
            _moveDirection.y = movementDirectionY;

        //Damping vertical acceleration to simulate gravity
        if (!_controllerComponent.isGrounded)
            _moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        _controllerComponent.Move(_moveDirection * Time.deltaTime);
    }
}
