using TMPro;
using UnityEngine;

public class ModeSelectController : MonoBehaviour
{
    [Header("Top-Right Continuity Readout")]
    [SerializeField] private TMP_Text activeHeroText;

    private void OnEnable()
    {
        if (ViceGameData.Instance != null && activeHeroText != null)
        {
            activeHeroText.text = $"SUIT: {ViceGameData.Instance.equippedSuit}";
        }
    }

    public void ChooseMode(string modeName)
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.selectedMode = modeName;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.NavigateTo("LevelSelect");
        }
    }
}