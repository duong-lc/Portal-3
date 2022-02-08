using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void LevelSelectionButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
