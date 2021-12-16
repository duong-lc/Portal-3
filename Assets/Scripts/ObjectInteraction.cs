using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public float waitOnPickup = 0.2f;
    //public float breakForce = 35f;
    [HideInInspector] public bool pickedUp = false;
    [HideInInspector] public PlayerInteraction playerInteraction;


    private void OnCollisionEnter(Collision collision)
    {
        // if(pickedUp)
        // {
        //     if(collision.relativeVelocity.magnitude > breakForce)
        //     {
        //         playerInteraction.BreakConnection(this);
        //     }
        // }
    }

    //this is used to prevent the connection from breaking when you just picked up the object as it sometimes fires a collision with the ground or whatever it is touching
    public IEnumerator PickUp()
    {
        yield return new WaitForSecondsRealtime(waitOnPickup);
        pickedUp = true;

    }
}