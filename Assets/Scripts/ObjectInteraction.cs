using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class ObjectInteraction : MonoBehaviour
{
    public float waitOnPickup = 0.2f;
    //public float breakForce = 35f;
    public bool pickedUp = false;
    [HideInInspector] public LayerMask trueLayer;
    [HideInInspector] public ObjectDropperInteraction parentDropper;
    [SerializeField] private float _fadeTimer = 2f;
    private MeshRenderer[] _rendererArray;
    private Color[] _originalColorArray;
    private Rigidbody _rb;
    private bool isFading = false;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        if(!GetComponent<TurretBehavior>())
            parentDropper = transform.parent.root.GetComponent<ObjectDropperInteraction>();
        
        trueLayer = gameObject.layer;
        
        //MainObject = GetComponentInChildren<ObjectInteraction>().gameObject;
        _rendererArray = GetComponentsInChildren<MeshRenderer>();
        int count = _rendererArray.Sum(ren => ren.materials.Length);
        _originalColorArray = new Color[count];
        int j = 0;
        foreach (MeshRenderer ren in _rendererArray)
        {
            foreach (var t in ren.materials)
            {
                _originalColorArray[j] = t.color;
                j++;
            }
            
        }
    }   
    
    public void ResetObjectTransform(bool toDestroy)
    {
        //foreach (Material mat in _renderer.materials) { ToTransparentMode(mat); }
        if (isFading) return;
        PlayerController.Instance.gameObject.GetComponent<PlayerInteraction>().BreakConnection(this);
        if (GetComponent<TurretBehavior>()) { GetComponent<TurretBehavior>().PlayAudioDeath(); }
        StartCoroutine(ObjectFade(Time.time, toDestroy));
        _rb.velocity *= 0.3f;
        _rb.useGravity = false;
    }

    private IEnumerator ObjectFade(float startTime, bool toDestroy)
    {
        //Color _black = Color.black;
        isFading = true;
        float alpha = (Time.time - startTime) / _fadeTimer;
        while (alpha <= 1)
        {
            foreach (MeshRenderer ren in _rendererArray)
            {
                foreach (Material mat in ren.materials)
                {
                    mat.color = Color.Lerp(mat.color, Color.black, alpha);
                    alpha = (Time.time - startTime) / _fadeTimer;
                }
            }
            yield return null;
        }

        isFading = false;
        if (!toDestroy)
        {
            //foreach (Material mat in _renderer.materials) { ToOpaqueMode(mat); }
            transform.position = parentDropper._objectSpawnTransform.position;
            GetComponent<Rigidbody>().useGravity = true;
            int j = 0;
            foreach (MeshRenderer ren in _rendererArray)
            {
                foreach (var t in ren.materials)
                {
                    t.color = _originalColorArray[j];
                    j++;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    
    //this is used to prevent the connection from breaking when you just picked up the object as it sometimes fires a collision with the ground or whatever it is touching
    public IEnumerator PickUp()
    {
        yield return new WaitForSecondsRealtime(waitOnPickup);
        pickedUp = true;

    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * 5);
    }

}