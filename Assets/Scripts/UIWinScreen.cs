using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIWinScreen : MonoBehaviour
{
    public void NextLevel()
    {
        PlayerSoundManager.Instance.PlayHudButtonSelectAudio();
        if(SceneManager.GetActiveScene().name != "Level 3")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene("Level Selection");
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
