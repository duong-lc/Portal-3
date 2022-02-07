using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class SurfaceDeathInteraction : MonoBehaviour
{
   private void OnCollisionEnter(Collision collision)
   {
      if (collision.gameObject.CompareTag("Player"))
      {
         collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(100);
      }
   }
}
