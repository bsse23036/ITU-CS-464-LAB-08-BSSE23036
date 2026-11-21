using TMPro;
using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    [Header("Top Header Readout")]
    [SerializeField] private TMP_Text activeModeText;

    private void OnEnable()
    {
        if (ViceGameData.Instance != null && activeModeText != null)
        {
            activeModeText.text = $"PROTOCOL: {ViceGameData.Instance.selectedMode}";
        }
    }

    public void SelectDistrictAndPlay(string districtLabel)
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.selectedDistrict = districtLabel;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.NavigateTo("Gameplay");
        }
    }
}