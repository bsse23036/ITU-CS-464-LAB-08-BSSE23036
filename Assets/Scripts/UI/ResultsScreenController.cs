using TMPro;
using UnityEngine;

public class ResultsScreenController : MonoBehaviour
{
    [Header("Results Readouts")]
    [SerializeField] private TMP_Text districtCompletedText;
    [SerializeField] private TMP_Text walletSummaryText;

    private void OnEnable()
    {
        var data = ViceGameData.Instance;
        if (data == null) return;

        if (districtCompletedText != null)
            districtCompletedText.text = $"{data.selectedDistrict}  |  {data.selectedMode}";

        if (walletSummaryText != null)
            walletSummaryText.text = $"TOTAL BALANCE:  $ {data.viceCash:N0}   |   ◆ {data.heroGems}";
    }

    public void GoToMainMenu()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.NavigateRoot("MainMenu");
    }

    public void GoToShop()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.NavigateRoot("MainMenu");

        if (UIManager.Instance != null)
            UIManager.Instance.NavigateTo("Shop");
    }

    public void GoToNextDistrict()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.NavigateRoot("MainMenu");

        if (UIManager.Instance != null)
            UIManager.Instance.NavigateTo("LevelSelect");
    }
}