using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseScreen : MonoBehaviour
{
    public void ResumeLevel()
    {
        PlayerController.Instance.GetComponent<PlayerUIHandler>().TogglePauseScreen(false);
        //Time.timeScale = 1;
    }
    public void RestartLevel()
    {
        Time.timeScale = 1;
        PlayerController.Instance.LockMouseCursor();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
