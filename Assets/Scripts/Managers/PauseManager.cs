using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool gamePaused = false;
    public GameObject pauseMenuUI;

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
            {
                ResumeGame();
            } else
            {
                PauseGame();
            }
        }
    }

    public void RestartGame()
    {
        SaveManager.DeleteData();
        SceneManager.LoadScene("Level1.1");
    }

    public void TryAgain()
    {
        SaveData saveData = SaveManager.Load();
        if (string.IsNullOrEmpty(saveData.lastSaveSpotID)) RestartGame();
        int sceneIndex = saveData.currentSceneIndex;
        SceneManager.LoadScene(sceneIndex);
    }
    // Resume game
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;

        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    // Pause game
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;

        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    // Options (not added yet)
    public void LoadOptions()
    {
        
    }

    // Main menu (not sure when this will be added but when it does just add this function ig)
    public void LoadMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        gamePaused = false;
        pauseMenuUI.SetActive(false);
        SceneManager.LoadScene("Main_Menu");
    }
}
