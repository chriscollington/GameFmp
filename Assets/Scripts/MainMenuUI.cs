using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Info Panel")]
    public GameObject infoPanel; // Assign your info canvas/panel here

    // PLAY BUTTON
    public void PlayGame()
    {
        
        SceneManager.LoadScene("Game");
    }

    // QUIT BUTTON
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // Shows in editor
        Application.Quit();     // Works in build
    }

    // INFO BUTTON
    public void ToggleInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(!infoPanel.activeSelf);
        }
    }
}