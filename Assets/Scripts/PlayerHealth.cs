using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.PlayerLoop;
using TMPro;
using UnityEditor.SearchService;

public class PlayerHealth : MonoBehaviour
{
    [Header("Canvas GameObjects")]
    [SerializeField] private GameObject _DeathScreenObject;
    [SerializeField] private GameObject _healthDisplayObject;
    
    
    [Header("Other Health Parameters")]
    [SerializeField] private float _maxHealth = 100;
    public float health = 100;
    [SerializeField] private float _turretDamageCheckInterval = 0.2f;

    [Header("Healing Parameters")]
    [SerializeField] private float _waitTimeToHeal;
    [SerializeField] private float _healingInterval;
    [SerializeField] private float _healAmount;
    private float _damageTakenTime;
    private bool _runOnce = false;

    [Header("Turret Damage Parameters")]
    public float turretDamage = 5f;
    public int turretDamageMultiplier = 0;
    public static PlayerHealth instance;

    [Header("TMP Parameters")] 
    [SerializeField] private TMP_Text _healthText;
    
    private void Start()
    {
        _healthText.text = health.ToString(CultureInfo.CurrentCulture);
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

    private void Update()
    {
        if (_runOnce) return;
        if (Time.time - _damageTakenTime >= _waitTimeToHeal)
        {
            _runOnce = true;
            StartCoroutine(HealingCountDownRoutine());
        }
    }
    
    public void DamagePlayer(float damageToInflict)
    {
        //If the player is already dead, return
        if (health <= 0) return;
        //Stop all healing routines when taking damage (and not healing)
        if (damageToInflict > 0)
        {
            StopCoroutine(HealingRoutine());
            StopCoroutine(HealingCountDownRoutine());
            _runOnce = false;
            _damageTakenTime = Time.time;
            CameraShake.instance.Shake(0.15f, 0.3f);
        }

        //apply damage
        health -= damageToInflict;
        //set constraint when healing
        if (health > _maxHealth) health = _maxHealth;
        //show health in UI
        _healthText.text = health.ToString(CultureInfo.CurrentCulture);


        if (health <= 0)
        {
            _healthDisplayObject.SetActive(false);
            _DeathScreenObject.SetActive(true);
            Time.timeScale = 0;
            GetComponent<PlayerController>().canMove = false;
            Cursor.visible = true;
        }
    }
    
    //countdown since the last time taking damage to be able to heal automatically
    private IEnumerator HealingCountDownRoutine()
    {
        yield return new WaitForSeconds(_waitTimeToHeal);
        StartCoroutine(HealingRoutine());
    }

    private IEnumerator HealingRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_healingInterval);
        while (health < _maxHealth && _runOnce)
        {
            DamagePlayer(-_healAmount);
            yield return wait;
        }
    }


}
