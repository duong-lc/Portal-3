using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRegistry : MonoBehaviour
{
    public PortalBehavior[] portalArray {get; private set;}
    public static PortalRegistry Instance;
    private void Awake()
    {
        portalArray = GetComponentsInChildren<PortalBehavior>();

        if(portalArray.Length != 2)
        {
            Debug.LogError("not enough portals in game");
        }
    }

    private void Start()
    {
        Instance = this;
    }

    public bool GetIsAnyOnCoolDown()
    {
        foreach (PortalBehavior portal in portalArray)
        {
            if (!portal.canTeleport)
                return true;
        }

        return false;
    }
}
