using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIDeathScreen : MonoBehaviour
{
    public void RestartLevel()
    {
        PlayerSoundManager.Instance.PlayHudButtonSelectAudio();
        Time.timeScale = 1;
        PlayerController.Instance.LockMouseCursor();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        PlayerSoundManager.Instance.PlayHudButtonSelectAudio();
        SceneManager.LoadScene(0);
    }
}
