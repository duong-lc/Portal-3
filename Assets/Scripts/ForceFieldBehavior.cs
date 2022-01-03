using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<ObjectInteraction>() )
        {
            StartCoroutine(ResetTrigger());
            return;
        }
        other.GetComponent<ObjectInteraction>().parentDropper.ResetCubeTransform();
    }

    private IEnumerator ResetTrigger()
    {
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        GetComponent<BoxCollider>().enabled = true;
    }
}
