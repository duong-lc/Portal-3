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

    CharacterController _controllerComponent;
    Vector3 _moveDirection = Vector3.zero;
    float _rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        Instance = this;
        _controllerComponent = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        PlayerLook();
        PlayerMovement();
    }

    void PlayerLook()
    {
        // Player and Camera rotation
        if (canMove)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
            playerCam.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
    void PlayerMovement()
    {   
        //Calculate move direction based on 2 axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        //Moving the player
        float curSpeedX = canMove ?  walkingSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = _moveDirection.y;
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
