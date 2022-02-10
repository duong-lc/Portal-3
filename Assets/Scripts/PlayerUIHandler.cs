using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    [Header("Canvas GameObjects")]
    [SerializeField] private GameObject _deathScreenObject;
    [SerializeField] private GameObject _healthDisplayObject;
    [SerializeField] private GameObject _pauseScreenObject;

    public void Start()
    {
        TogglePauseScreen(false);
        ToggleDeathScreenUI(false);
        ToggleHealthUI(true);
    }

    public void ToggleHealthUI(bool isOn)
    {
        _healthDisplayObject.SetActive(isOn);
    }

    public void ToggleDeathScreenUI(bool isOn)
    {
        _deathScreenObject.SetActive(isOn);
    }

    public void TogglePauseScreen(bool isOn)
    {
        var controller = PlayerController.Instance;
        _pauseScreenObject.SetActive(isOn);
        Time.timeScale = isOn ? 0 : 1;
        controller.canMove = !isOn;
        
        if (isOn)
            controller.ReleaseMouseCursor();
        else 
            controller.LockMouseCursor();
        
       //print($"{Time.timeScale}");
    }
    
}
