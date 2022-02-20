using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    private TurretBehavior _turretBehavior;
    public bool canSeePlayer;
    public Vector3 playerDir;

    private void Start()
    {
        _turretBehavior = GetComponent<TurretBehavior>();
        playerRef = GameObject.FindWithTag("Player");
        StartCoroutine(FOVRoutine());
        StartCoroutine(StandingCheckRoutine());
    }

    private IEnumerator StandingCheckRoutine()
    {   
        WaitForSeconds wait = new WaitForSeconds(0.3f);
        Vector3 objectRot = transform.rotation.eulerAngles;
        while (true)
        {
            objectRot = transform.rotation.eulerAngles;
            //print($"{objectRot.x} {objectRot.y} {objectRot.z}");
            if (((objectRot.x >= 1 || objectRot.x <= -1)
                || (objectRot.z >= 1 || objectRot.z <= -1))
                && GetComponent<Rigidbody>().velocity.magnitude == 0)
            {
                break;
            }
            yield return wait;
        }
        GetComponent<ObjectInteraction>().ResetObjectTransform(true);
    }
    
    
    public IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            if (FieldOfViewCheck() != canSeePlayer)
            {
                if (canSeePlayer)
                {
                    _turretBehavior.bulletSpawner.SetActive(false);
                    _turretBehavior.StopAudioShoot();
                    PlayerHealth.Instance.turretDamageMultiplier -= 1;
                } 
                else
                {
                    _turretBehavior.bulletSpawner.SetActive(true);
                    _turretBehavior.PlayAudioDetect();
                    PlayerHealth.Instance.turretDamageMultiplier += 1;
                }

                canSeePlayer = FieldOfViewCheck();
            }
            else
            {
                if(canSeePlayer)
                {
                    Vector3 playerPos = playerRef.transform.position;
                    playerDir = _turretBehavior.bulletSpawner.transform.forward = (new Vector3(playerPos.x, playerPos.y-1, playerPos.z) - transform.position).normalized;
                    if(_turretBehavior.audioSource.clip != _turretBehavior._turretShoot)
                        _turretBehavior.PlayAudioShoot();
                }
                // if(canSeePlayer)
                // {
                //     _turretBehavior.PlayAudioShoot();
                // }
            }
            
        }
    }

    private bool FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, _targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                //print($"{distanceToTarget}");
                // foreach (LayerMask obstructionMask in _obstructionMaskArray)
                // { 
                //     print($"{Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask)}");
                //     return !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask);
                // }
                var hitArray = Physics.RaycastAll(transform.position, directionToTarget, distanceToTarget);
                foreach (RaycastHit hit in hitArray)
                {
                    if (!hit.collider.isTrigger && !hit.collider.CompareTag("Player"))
                        return false;
                }

                return true;

            }
            else
                return false;
        }
        else if (canSeePlayer)
            return false;

        return false;
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(transform.position, Vector3.down * 0.3f );
    // }

    private void OnDestroy()
    {
        if(canSeePlayer)
            PlayerHealth.Instance.turretDamageMultiplier--;
    }
}
