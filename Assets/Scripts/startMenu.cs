using UnityEngine;
using UnityEngine.SceneManagement;


public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsUI;
    [SerializeField] GameObject startMenu;
    [SerializeField] private string sceneName = "Level 1";

    public void StartGame()
    {
        // Load the game scene
        SceneManager.LoadScene(sceneName);
    }

    public void GoToOptions()
    {
        startMenu.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void BackToMain()
    {
        startMenu.SetActive(true);
        optionsUI.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}



