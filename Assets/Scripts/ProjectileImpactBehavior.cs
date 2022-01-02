using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileImpactBehavior : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunParticleSystem());
    }

    private IEnumerator RunParticleSystem()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
