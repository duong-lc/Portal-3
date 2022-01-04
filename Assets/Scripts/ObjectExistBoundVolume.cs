using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectExistBoundVolume : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>()) return;
        other.GetComponent<ObjectInteraction>().parentDropper.ResetCubeTransform();
    }
}
