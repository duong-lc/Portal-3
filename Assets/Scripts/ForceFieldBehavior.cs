using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>() )
        {
            if (other.CompareTag("Player"))
            {
                foreach (var portal in PortalRegistry.Instance.portalArray)
                {
                    portal.TogglePortal(false);
                }
               
            }
            StartCoroutine(ResetTrigger());
            return;
        }
        //print($"touch field");
        other.GetComponent<ObjectInteraction>().ResetObjectTransform(other.GetComponent<TurretBehavior>());
    }

    private IEnumerator ResetTrigger()
    {
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        GetComponent<BoxCollider>().enabled = true;
    }
}
