using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles all UI screens and HUD values.
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameHUDPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD")]
    [Header("HUD")]
    [SerializeField] private Slider breathSlider;
    [SerializeField] private TextMeshProUGUI breathText;
    [SerializeField] private TextMeshProUGUI bubbleCountText;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        SetOnlyActive(mainMenuPanel);
    }

    public void ShowHUD()
    {
        SetOnlyActive(gameHUDPanel);
    }

    public void ShowVictory()
    {
        SetOnlyActive(victoryPanel);
    }

    public void ShowGameOver()
    {
        SetOnlyActive(gameOverPanel);
    }

    private void SetOnlyActive(GameObject target)
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(target == mainMenuPanel);

        if (gameHUDPanel != null)
            gameHUDPanel.SetActive(target == gameHUDPanel);

        if (victoryPanel != null)
            victoryPanel.SetActive(target == victoryPanel);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(target == gameOverPanel);
    }


    public void UpdateBreath(
    float current,
    float maximum)
    {
        if (breathSlider != null)
        {
            breathSlider.minValue = 0f;
            breathSlider.maxValue = maximum;
            breathSlider.value = current;
        }

        if (breathText != null)
        {
            breathText.text =
                $"Breath: {current:0}";
        }
    }
    public void UpdateBubbleCount(int count)
    {
        if (bubbleCountText != null)
        {
            bubbleCountText.text =
                $"Bubbles: {count}";
        }
    }

    #region Button Callbacks

    public void OnStartButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }

    public void OnRetryButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    public void OnMainMenuButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
    }

    #endregion
}