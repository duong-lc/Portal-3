using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.ComponentModel.Design;

public class PortalRegistry : MonoBehaviour
{
    public PortalBehavior[] portalArray {get; private set;}
    public static PortalRegistry Instance;
    
    [Header("For Individual Access")]
    public Vector3[] colliderCenterArr = new Vector3[2];
    public Vector3[] colliderSizeArr = new Vector3[2];

    [HideInInspector] public List<Collider> registryColList = new List<Collider>();
    public int teleportLayer = 13;

    private GameObject[] _receiverArray;
    
    private void Awake()
    {
        Instance = this;
        portalArray = GetComponentsInChildren<PortalBehavior>();
        
        if(portalArray.Length != 2)
        {
            Debug.LogError("not enough portals in game");
        }
    }

    private void Start()
    {
        _receiverArray = GameObject.FindGameObjectsWithTag("LaserReceiver");
        
    }
    
    // private void LateUpdate()
    // {
    //     foreach (var col in registryColList.Where(col => col.CompareTag("LaserReceiver")))
    //     {
    //         col.GetComponent<ActivationButtonInteraction>().EnableActivation();
    //     }
    //
    //    
    // }
    private void LateUpdate()
    {
        foreach (GameObject obj in _receiverArray)
        {
            if (registryColList.Contains(obj.GetComponent<Collider>()))
            {
                obj.GetComponent<ActivationButtonInteraction>().EnableActivation();
            }
            else
                obj.GetComponent<ActivationButtonInteraction>().DisableActivation();
        }
        //clear the list after each iteration so the next check order have a fresh list 
        registryColList = new List<Collider>();
    }
    
    private void Update()
    {
        for (int i = 0; i < portalArray.Length; i++)
        {
            
            if (registryColList.Contains(portalArray[i].gameObject.GetComponent<Collider>()))
            {
                EnableLaserOnPortal(portalArray[i]);
                break;
            }
            if (i == portalArray.Length-1)
            {
                DisableLaserOnAllPortal();
                //continue;
            }
        }
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
        

        
    }

    public void DisableLaserOnAllPortal()
    {
        //print($"disable all");
        foreach (PortalBehavior portal in portalArray)
        {
            if (!portal.gameObject.GetComponent<LaserLineRenderer>().enabled) continue;
            
            portal.gameObject.GetComponent<LaserLineRenderer>().ResetLaserRendererArray();
            portal.gameObject.GetComponent<LaserLineRenderer>().enabled = false;
        }
        
        
    }
    
}
