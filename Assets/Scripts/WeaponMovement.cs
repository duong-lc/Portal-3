using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    [Header("Sway Properties")]
    [SerializeField] private float _smooth;
    [SerializeField] private float _swayMultiplier;
    [Header("Bob Properties")]
    [SerializeField] private float _bobAmount;
    [SerializeField] private float _bobSpeed = 1;
    [SerializeField] private CharacterController _charController;
    private Vector3 _originalWeaponPos;
    [Header("Recoil Properties")]
    [SerializeField] private float _recoilAmount = 0.35f;
    //[SerializeField] private float _recoilRecoverTime = 0.2f;
    
    private float counter = 0;

    private void Awake() {
        _originalWeaponPos = transform.localPosition;
    }
    private void Update()
    {
        WeaponBob();
        WeaponSway();
    }

    private float WeaponRecoil()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            return _recoilAmount;
        else
            return 0;
    }

    private void WeaponBob()
    {
        //Handling Weapon Recoil
        var recoilMagnitude = WeaponRecoil();
        transform.localPosition += -Vector3.forward * recoilMagnitude;
        //Lerping back to original position
        if(_charController.velocity == new Vector3(0,0,0))//lerping if not moving (whehter firing or not)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _originalWeaponPos, _bobSpeed * Time.deltaTime);
            return;//return if not moving to not bob
        }
        else//lerping if moving (handling lerping back from recoiling when moving as well)
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, 0), _bobSpeed * Time.deltaTime);
        //Weapon bobling while moving
        var bobMagnitude = Mathf.Sin(counter += Time.deltaTime * _bobSpeed) * _bobAmount;
        transform.localPosition += Vector3.up * bobMagnitude;
        
    }
    private void WeaponSway()
    {
        //Get Mouse Input
        float mouseX = Input.GetAxisRaw("Mouse X") * _swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * _swayMultiplier;

        //calculate target rotation
        Quaternion rotX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRot = rotX * rotY;

        //rotate
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, _smooth*Time.deltaTime);
    }
}
