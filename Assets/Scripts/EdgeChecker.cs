using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeChecker : MonoBehaviour
{
    public bool isClear = true;
    private void OnTriggerStay(Collider other) 
    {
        if(other != null)
        {
            isClear = false;
            print($"object blocking {other.gameObject.name}");
        } 
        if(other == null)
            isClear = true;

        
    }
}
