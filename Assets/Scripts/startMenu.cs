using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsUI;
    [SerializeField] GameObject startMenu;

    private string sceneName;

    public void StartGame()
    {
        // Load the game scene
        sceneName = "SampleScene";
        SceneManager.LoadScene(sceneName);
    }

    public void GoToOptions()
    {
        startMenu.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}



