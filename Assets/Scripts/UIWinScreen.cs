using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIWinScreen : MonoBehaviour
{
    public void NextLevel()
    {
        if(SceneManager.GetActiveScene().name != "Level 5")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene("Level Selection");
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
