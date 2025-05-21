using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneButtonFunctions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void StartGame()
    {
        SceneManager.LoadScene("Level 1");            // ENTER STARTING SCENE NAME HERE
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
