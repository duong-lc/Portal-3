using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        //Setting the material for the projectile
        gameObject.GetComponent<MeshRenderer>().material = portalID == 0 ? _blueMat : _redMat;
    }

    public void OnTouchSurface(PortalPlacement placementScript, RaycastHit hit)
    {
        placementScript.CreatePortal(portalID, hit);
        SpawnParticle();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        print($"hello");
        if (other.gameObject.GetComponent<Collider>().isTrigger) return;
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
