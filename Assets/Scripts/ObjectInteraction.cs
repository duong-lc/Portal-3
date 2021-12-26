using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class ObjectInteraction : MonoBehaviour
{
    public float waitOnPickup = 0.2f;
    //public float breakForce = 35f;
    public bool pickedUp = false;
    public LayerMask trueLayer;

    private void Start()
    {
        trueLayer = gameObject.layer;
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