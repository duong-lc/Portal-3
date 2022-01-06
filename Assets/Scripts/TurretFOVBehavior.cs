using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Field of View for the turret the player
///
/// Current script is a modified version of https://github.com/Comp3interactive/FieldOfView
/// </summary>
public class TurretFOVBehavior : MonoBehaviour
{
    public float radius;
    [Range(0,360)] public float angle;

    public GameObject playerRef;

    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask[] _obstructionMaskArray;

    public bool canSeePlayer;

    private void Start()
    {
        playerRef = GameObject.FindWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, _targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                foreach (LayerMask obstructionMask in _obstructionMaskArray)
                {
                    canSeePlayer = !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask);
                }
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
