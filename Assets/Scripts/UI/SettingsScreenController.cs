using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenController : MonoBehaviour
{
    [Header("Sliders & Numeric Readouts")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TMP_Text masterValueText;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxValueText;

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityValueText;

    [Header("Graphics Quality Segmented Buttons (0: Low, 1: Medium, 2: Ultra)")]
    [SerializeField] private Button[] qualityButtons;
    [SerializeField] private TMP_Text[] qualityButtonLabels;
    [SerializeField] private Outline[] qualityButtonOutlines;

    private void OnEnable()
    {
        var data = ViceGameData.Instance;
        if (data == null) return;

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(data.masterVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(data.sfxVolume);
        if (sensitivitySlider != null) sensitivitySlider.SetValueWithoutNotify(data.swingSensitivity);

        UpdateLabels();
        HighlightQuality(data.graphicsQualityIndex);
    }

    public void OnMasterChanged(float value)
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.masterVolume = value;
            ViceGameData.Instance.NotifySettingsUpdated();
        }
        UpdateLabels();
    }

    public void OnSfxChanged(float value)
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.sfxVolume = value;
        }
        UpdateLabels();
    }

    public void OnSensitivityChanged(float value)
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.swingSensitivity = value;
        }
        UpdateLabels();
    }

    public void SetGraphicsQuality(int index)
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.graphicsQualityIndex = index;
        }
        QualitySettings.SetQualityLevel(index);
        HighlightQuality(index);
    }

    public void ResetDefaults()
    {
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(0.85f);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(0.90f);
        if (sensitivitySlider != null) sensitivitySlider.SetValueWithoutNotify(1.0f);

        OnMasterChanged(0.85f);
        OnSfxChanged(0.90f);
        OnSensitivityChanged(1.0f);
        SetGraphicsQuality(1);
    }

    private void UpdateLabels()
    {
        if (masterSlider != null && masterValueText != null)
            masterValueText.text = $"{Mathf.RoundToInt(masterSlider.value * 100f)}%";

        if (sfxSlider != null && sfxValueText != null)
            sfxValueText.text = $"{Mathf.RoundToInt(sfxSlider.value * 100f)}%";

        if (sensitivitySlider != null && sensitivityValueText != null)
            sensitivityValueText.text = $"{sensitivitySlider.value:F1}x";
    }

    private void HighlightQuality(int activeIndex)
    {
        for (int i = 0; i < qualityButtons.Length; i++)
        {
            bool isSelected = (i == activeIndex);

            if (qualityButtons[i] != null)
            {
                qualityButtons[i].image.color = isSelected
                    ? new Color32(250, 204, 21, 255)   // Active Comic Gold (#FACC15)
                    : new Color32(11, 17, 32, 255);    // Inactive Midnight (#0B1120)
            }

            if (qualityButtonLabels != null && i < qualityButtonLabels.Length && qualityButtonLabels[i] != null)
            {
                qualityButtonLabels[i].color = isSelected
                    ? new Color32(11, 17, 32, 255)
                    : new Color32(148, 163, 184, 255);
            }

            if (qualityButtonOutlines != null && i < qualityButtonOutlines.Length && qualityButtonOutlines[i] != null)
            {
                qualityButtonOutlines[i].effectColor = isSelected
                    ? new Color32(248, 250, 252, 255)
                    : new Color32(51, 65, 85, 255);
            }
        }
    }
}