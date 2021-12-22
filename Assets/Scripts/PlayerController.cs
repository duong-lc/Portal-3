using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
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

    [Header("Ground Check Properties")]
    [SerializeField] private LayerMask _groundLayer1;
    [SerializeField] private LayerMask _groundLayer2;
    [SerializeField] private GameObject _groundChecker; 
    [SerializeField] private float _checkerRadius;
    private bool _canJump;
    

    [HideInInspector] public bool canMove = true;
    [Space]
    public GameObject mainMapGeom;
    private Rigidbody _rgbd;
    

    private void Start()
    {
        Instance = this;
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
    }

    private void LateUpdate() 
    {
        CustomGravity();
        CapsuleReadjustment();
    }

    #region Basic Movement
    private void CheckGround()
    {
        RaycastHit[] colArr = Physics.SphereCastAll(_groundChecker.transform.position, _checkerRadius, Vector3.down, 0, _groundLayer1|_groundLayer2);
        if (colArr.Any(hit => !hit.collider.CompareTag(gameObject.tag)))
        {
            _canJump = true;
            return;
        }
        _canJump = false;
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
    


}
