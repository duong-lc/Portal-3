using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseScreen : MonoBehaviour
{
    public void ResumeLevel()
    {
        PlayerSoundManager.Instance.PlayHudButtonSelectAudio();
        PlayerController.Instance.GetComponent<PlayerUIHandler>().TogglePauseScreenUI(false);
        //Time.timeScale = 1;
    }
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
