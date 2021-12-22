using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalProjectileBehavior : MonoBehaviour
{
    [HideInInspector] public int portalID;
    [HideInInspector] public bool isTouched;
    [Header("Bullet Material")]
    [SerializeField] private Material _redMat;
    [SerializeField] private Material _blueMat;


    private void Start()
    {
        //Setting the material for the projectile
        gameObject.GetComponent<MeshRenderer>().material = portalID == 0 ? _blueMat : _redMat;
    }

    public void OnTouchSurface(PortalPlacement placementScript, RaycastHit hit)
    {
        placementScript.CreatePortal(portalID, hit);
        Destroy(gameObject);
    }
}
