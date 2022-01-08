using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    public float health = 100;
    [SerializeField] private float _turretDamageCheckInterval = 0.2f;
    public float turretDamage = 5f;
    public int turretDamageMultiplier = 0;
    public static PlayerHealth instance;

    private void Start()
    {
        instance = this;
        health = _maxHealth;
        StartCoroutine(TurretDamageCheckerRoutine());
    }

    private IEnumerator TurretDamageCheckerRoutine()
    {
        var wait = new WaitForSeconds(_turretDamageCheckInterval);
        while (true)
        {
            yield return wait;
            DamagePlayer(turretDamageMultiplier * turretDamage);
        }
    }
    
    public void DamagePlayer(float damageToInflict)
    {
        if (health <= 0) return;
        health -= damageToInflict;
        
        //if(health <= 0) Destroy(gameObject);
        if(health <= 0) print($"you're ded ajjaja");
    }

    
}
