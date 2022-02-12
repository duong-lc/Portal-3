using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        PlayerController.Instance.GetComponent<PlayerUIHandler>().ToggleWinScreenUI(true);
    }
}
