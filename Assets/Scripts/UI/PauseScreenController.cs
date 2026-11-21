using TMPro;
using UnityEngine;

public class PauseScreenController : MonoBehaviour
{
    [Header("Modal Readout")]
    [SerializeField] private TMP_Text missionInfoText;

    [Header("Gameplay HUD Reference (For Clean Restart)")]
    [SerializeField] private GameplayHUDController gameplayHud;

    private void OnEnable()
    {
        var data = ViceGameData.Instance;
        if (data != null && missionInfoText != null)
        {
            missionInfoText.text = $"{data.selectedDistrict}  |  {data.equippedSuit}";
        }
    }

    public void ResumeGame()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.GoBack();
        }
    }

    public void RestartDistrict()
    {
        if (gameplayHud != null)
        {
            gameplayHud.ResetMissionClock();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.GoBack();
        }
    }

    public void OpenSettingsFromPause()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.NavigateTo("Settings");
        }
    }

    public void QuitToMainMenu()
    {
        if (gameplayHud != null)
        {
            gameplayHud.ResetMissionClock();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.NavigateRoot("MainMenu");
        }
    }
}