using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class PortalRegistry : MonoBehaviour
{
    public PortalBehavior[] portalArray {get; private set;}
    public static PortalRegistry Instance;
    
    [Header("For Individual Access")]
    public Vector3[] colliderCenterArr = new Vector3[2];
    public Vector3[] colliderSizeArr = new Vector3[2];

    public int teleportLayer = 13;
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
        return portalArray.Any(portal => !portal.canTeleport);
    }

    public void EnableLaserOnPortal(PortalBehavior inputPortal)
    {
        int inputIndex = Array.IndexOf(portalArray, inputPortal);
        PortalBehavior outputPortal = inputIndex == 1 ? portalArray[0] : portalArray[1];
        outputPortal.gameObject.GetComponent<LaserLineRenderer>().enabled = outputPortal.gameObject.GetComponent<BoxCollider>().enabled;
        print($"{outputPortal.gameObject.GetComponent<LaserLineRenderer>().enabled}");
    }

    public void DisableLaserOnAllPortal()
    {
        print($"disable all");
        foreach (PortalBehavior portal in portalArray)
        {
            if (!portal.gameObject.GetComponent<LaserLineRenderer>().enabled) continue;
            
            portal.gameObject.GetComponent<LaserLineRenderer>().ResetLaserRendererArray();
            portal.gameObject.GetComponent<LaserLineRenderer>().enabled = false;
        }
    }
    
}
