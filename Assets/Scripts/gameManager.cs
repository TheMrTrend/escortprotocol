using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public PlayerController playerController;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPaused;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public bool isPaused;
    float timeScaleOrig;
    int gameGoalCount;

    private void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        timeScaleOrig = Time.timeScale;
    }

    private void Start()
    {
        menuPaused = UIManager.instance.pauseMenu;
        menuWin = UIManager.instance.winMenu;
        menuLose = UIManager.instance.loseMenu;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPaused;
                menuActive.SetActive(isPaused);
            } else if (menuActive == menuPaused)
            {
                StateUnpause();
            }
            
        } 
    }

    public void StatePause()
    {
        isPaused = !isPaused;

        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;

        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        menuActive.SetActive(false);
        menuActive = null;
    }

    public void LoseState()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void WinState()
    {
        StatePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;
        if (gameGoalCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }
}
