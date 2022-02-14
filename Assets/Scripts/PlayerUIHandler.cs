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
    [SerializeField] private GameObject _winScreenObject;
    private PlayerController _controller;

    public void Start()
    {
        _controller = PlayerController.Instance;
        if(_controller == null)
            print($"xd");
        TogglePauseScreenUI(false);
        ToggleDeathScreenUI(false);
        ToggleWinScreenUI(false);
        ToggleHealthUI(true);
    }

    public void ToggleHealthUI(bool isOn)
    {
        _healthDisplayObject.SetActive(isOn);
    }

    public void ToggleDeathScreenUI(bool isOn)
    {
        ToggleHealthUI(!isOn);
        Time.timeScale = isOn ? 0 : 1;
        _controller.canMove = !isOn;
        ToggleMouseCursorOnScreen(isOn);
        _deathScreenObject.SetActive(isOn);
    }

    public void TogglePauseScreenUI(bool isOn)
    {
        ToggleHealthUI(!isOn);
        Time.timeScale = isOn ? 0 : 1;
        _controller.canMove = !isOn;
        ToggleMouseCursorOnScreen(isOn);
        _pauseScreenObject.SetActive(isOn);
        
        
    }

    public void ToggleWinScreenUI(bool isOn)
    {
        ToggleHealthUI(!isOn);
        Time.timeScale = isOn ? 0 : 1;
        _controller.canMove = !isOn;
        ToggleMouseCursorOnScreen(isOn);
        _winScreenObject.SetActive(isOn);
        
        if(isOn)
            PlayerSoundManager.Instance.PlayPlayerWinAudio();
    }

    public void ToggleMouseCursorOnScreen(bool isOn)
    {
        if (isOn)
            _controller.ReleaseMouseCursor();
        else 
            _controller.LockMouseCursor();
    }
}
