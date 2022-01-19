using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PortalProjectileBehavior : MonoBehaviour
{
    [HideInInspector] public int portalID;
    [HideInInspector] public Vector3 surfaceNormal;
    [Header("Bullet Material")]
    [SerializeField] private Material _redMat;
    [SerializeField] private Material _blueMat;
    [SerializeField] private GameObject _particlefx;
    [SerializeField] private Color _blueColor;
    [SerializeField] private Color _redColor;

    [SerializeField] private LayerMask[] _ignoreLayerArray;
    [HideInInspector] public RaycastHit hitData;

    private void Start()
    {
        surfaceNormal = hitData.normal;
        //Setting the material for the projectile
        gameObject.GetComponent<MeshRenderer>().material = portalID == 0 ? _blueMat : _redMat;
    }

    public void OnTouchSurface(PortalPlacement placementScript, RaycastHit hit)
    {
        if (_ignoreLayerArray.Any(layer => hit.collider.gameObject.layer == layer) || (hit.collider.isTrigger && hit.collider.CompareTag("Portal")))
        {
            //print($"{hit.collider.gameObject.layer}");
            SpawnParticle();
            Destroy(gameObject);
            return;
        }
       
        placementScript.CreatePortal(portalID, hit);
        SpawnParticle();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //print($"{other.gameObject.name}");
        if (other.gameObject.GetComponent<Collider>().isTrigger) return;
        if (other == hitData.collider) return;
        SpawnParticle();
        Destroy(gameObject);
        //spawn a cool hit particle fx
        //destroy self
    }

    private void SpawnParticle()
    {
        var particleObj = Instantiate(_particlefx, transform.position, Quaternion.identity);
        particleObj.transform.forward = surfaceNormal;
        var main = particleObj.GetComponent<ParticleSystem>().main;
        main.startColor = portalID == 0 ? _blueColor : _redColor;
    }
}
