using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILevelSelection : MonoBehaviour
{
    public void Level1Button()
    {
        SceneManager.LoadScene("Level 1");
    }
    public void Level2Button()
    {
        SceneManager.LoadScene("Level 2");
    }
    public void Level3Button()
    {
        SceneManager.LoadScene("Level 3");
    }
    public void Level4Button()
    {
        SceneManager.LoadScene("Level 4");
    }
    public void Level5Button()
    {
        SceneManager.LoadScene("Level 5");
    }
    public void ReturnButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
