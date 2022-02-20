using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    [Header("Canvas GameObjects")]
    [SerializeField] private GameObject _deathScreenObject;
    [SerializeField] private GameObject _healthDisplayObject;
    [SerializeField] private GameObject _pauseScreenObject;
    [SerializeField] private GameObject _winScreenObject;
    
    [SerializeField] private GameObject _crosshairMainObject;
    [SerializeField] private GameObject _crosshairBlueObject;
    [SerializeField] private GameObject _crosshairOrangeObject;
    private PlayerController _controller;
    private bool isCrosshairRoutineRun = false;
    
    public void Start()
    {
        _controller = PlayerController.Instance;
        if(_controller == null)
            print($"xd");
        TogglePauseScreenUI(false);
        ToggleDeathScreenUI(false);
        ToggleWinScreenUI(false);
        ToggleHealthUI(true);
        ToggleCrosshairMaster(true);
    }

    public void ToggleCrosshairMaster(bool isOn)
    {
        //Toggling
        _crosshairMainObject.SetActive(isOn);

        if (isOn)
        {
            isCrosshairRoutineRun = true;
            StartCoroutine(CheckActivePortalRoutine());
        }
        else
        {
            isCrosshairRoutineRun = false;
            StopCoroutine(CheckActivePortalRoutine());
            _crosshairBlueObject.SetActive(false);
            _crosshairOrangeObject.SetActive(false);
        }
            
    
    }

    private IEnumerator CheckActivePortalRoutine()
    {
        PortalBehavior[] arr = PortalRegistry.Instance.portalArray;
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (isCrosshairRoutineRun)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                if(i == 0)
                    _crosshairBlueObject.SetActive(arr[i].IsActive);
                else
                    _crosshairOrangeObject.SetActive(arr[i].IsActive);
            }

            yield return wait;
        }
        

    }
    
    public void ToggleHealthUI(bool isOn)
    {
        _healthDisplayObject.SetActive(isOn);
    }

    public void ToggleDeathScreenUI(bool isOn)
    {
        ToggleHealthUI(!isOn);
        ToggleCrosshairMaster(!isOn);
        Time.timeScale = isOn ? 0 : 1;
        _controller.canMove = !isOn;
        ToggleMouseCursorOnScreen(isOn);
        _deathScreenObject.SetActive(isOn);
    }

    public void TogglePauseScreenUI(bool isOn)
    {
        ToggleHealthUI(!isOn);
        ToggleCrosshairMaster(!isOn);
        Time.timeScale = isOn ? 0 : 1;
        _controller.canMove = !isOn;
        ToggleMouseCursorOnScreen(isOn);
        _pauseScreenObject.SetActive(isOn);
        
        
    }

    public void ToggleWinScreenUI(bool isOn)
    {
        ToggleHealthUI(!isOn);
        ToggleCrosshairMaster(!isOn);
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
