using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectExistBoundVolume : MonoBehaviour
{
    private Transform _dropperParent;

    private void Start()
    {
        _dropperParent = transform.root;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>()) return;
        if (other.GetComponent<TurretBehavior>()) return;
        if (other.GetComponent<ObjectInteraction>().parentDropper.transform == _dropperParent)
            other.GetComponent<ObjectInteraction>().ResetObjectTransform(false);
        else return;
    }
}
