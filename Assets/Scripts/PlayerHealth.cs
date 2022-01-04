using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    [HideInInspector]public float health = 100;

    private void Start()
    {
        health = _maxHealth;
    }

    public void DamagePlayer(float damageToInflict)
    {
        if (health <= 0) return;
        health -= damageToInflict;
        
        if(health <= 0) Destroy(gameObject);
    }
    
    
    
}
