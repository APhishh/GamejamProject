using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton; // For credits panel

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "MainScene"; // Your game scene name

    private void Start()
    {
        // Add listeners to buttons
        playButton.onClick.AddListener(PlayGame);
        creditsButton.onClick.AddListener(ShowCredits);
        exitButton.onClick.AddListener(ExitGame);
        backButton.onClick.AddListener(HideCredits);

        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    private void HideCredits()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    private void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
