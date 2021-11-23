using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRegistry : MonoBehaviour
{
    public PortalBehavior[] portalArray {get; private set;}
    void Awake()
    {
        portalArray = GetComponentsInChildren<PortalBehavior>();

        if(portalArray.Length != 2)
        {
            Debug.LogError("not enough portals in game");
        }
    }
}
