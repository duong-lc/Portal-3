using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectExistBoundVolume : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        var dropper = transform.parent.GetComponent<ObjectDropperInteraction>();
        if (other.gameObject == dropper.MainObject)
        {
            dropper.ResetCubeTransform();
        }
    }
}
