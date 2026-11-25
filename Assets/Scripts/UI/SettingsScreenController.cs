using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenController : MonoBehaviour
{
    [Header("Audio Sliders & Readouts (Min = 0.0001, Max = 1)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TMP_Text masterValueText;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicValueText;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxValueText;

    [SerializeField] private Slider uiSlider;
    [SerializeField] private TMP_Text uiValueText;

    [Header("Gameplay & Graphics Controls")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityValueText;

    [SerializeField] private Button[] qualityButtons;
    [SerializeField] private TMP_Text[] qualityButtonLabels;
    [SerializeField] private Outline[] qualityButtonOutlines;

    private void OnEnable()
    {
        if (AudioMixerManager.Instance != null)
        {
            if (masterSlider != null)
                masterSlider.SetValueWithoutNotify(AudioMixerManager.Instance.GetSavedVolume(AudioMixerManager.KEY_MASTER, 0.85f));
            if (musicSlider != null)
                musicSlider.SetValueWithoutNotify(AudioMixerManager.Instance.GetSavedVolume(AudioMixerManager.KEY_MUSIC, 0.80f));
            if (sfxSlider != null)
                sfxSlider.SetValueWithoutNotify(AudioMixerManager.Instance.GetSavedVolume(AudioMixerManager.KEY_SFX, 0.90f));
            if (uiSlider != null)
                uiSlider.SetValueWithoutNotify(AudioMixerManager.Instance.GetSavedVolume(AudioMixerManager.KEY_UI, 0.90f));
        }

        if (ViceGameData.Instance != null)
        {
            if (sensitivitySlider != null)
                sensitivitySlider.SetValueWithoutNotify(ViceGameData.Instance.swingSensitivity);
            HighlightQuality(ViceGameData.Instance.graphicsQualityIndex);
        }

        UpdateLabels();
    }

    public void OnMasterChanged(float value)
    {
        if (AudioMixerManager.Instance != null)
            AudioMixerManager.Instance.SetMasterVolume(value);
        UpdateLabels();
    }

    public void OnMusicChanged(float value)
    {
        if (AudioMixerManager.Instance != null)
            AudioMixerManager.Instance.SetMusicVolume(value);
        UpdateLabels();
    }

    public void OnSfxChanged(float value)
    {
        if (AudioMixerManager.Instance != null)
            AudioMixerManager.Instance.SetSfxVolume(value);
        UpdateLabels();
    }

    public void OnUiChanged(float value)
    {
        if (AudioMixerManager.Instance != null)
            AudioMixerManager.Instance.SetUiVolume(value);
        UpdateLabels();
    }

    public void OnSensitivityChanged(float value)
    {
        if (ViceGameData.Instance != null)
            ViceGameData.Instance.swingSensitivity = value;
        UpdateLabels();
    }

    public void SetGraphicsQuality(int index)
    {
        if (ViceGameData.Instance != null)
            ViceGameData.Instance.graphicsQualityIndex = index;
        QualitySettings.SetQualityLevel(index);
        HighlightQuality(index);
    }

    public void ResetDefaults()
    {
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(0.85f);
        if (musicSlider != null)  musicSlider.SetValueWithoutNotify(0.80f);
        if (sfxSlider != null)    sfxSlider.SetValueWithoutNotify(0.90f);
        if (uiSlider != null)     uiSlider.SetValueWithoutNotify(0.90f);
        if (sensitivitySlider != null) sensitivitySlider.SetValueWithoutNotify(1.0f);

        OnMasterChanged(0.85f);
        OnMusicChanged(0.80f);
        OnSfxChanged(0.90f);
        OnUiChanged(0.90f);
        OnSensitivityChanged(1.0f);
        SetGraphicsQuality(1);
    }

    private void UpdateLabels()
    {
        FormatPercentOrMute(masterSlider, masterValueText);
        FormatPercentOrMute(musicSlider, musicValueText);
        FormatPercentOrMute(sfxSlider, sfxValueText);
        FormatPercentOrMute(uiSlider, uiValueText);

        if (sensitivitySlider != null && sensitivityValueText != null)
            sensitivityValueText.text = $"{sensitivitySlider.value:F1}x";
    }

    private void FormatPercentOrMute(Slider slider, TMP_Text label)
    {
        if (slider == null || label == null) return;
        label.text = slider.value <= 0.001f ? "MUTE" : $"{Mathf.RoundToInt(slider.value * 100f)}%";
    }

    private void HighlightQuality(int activeIndex)
    {
        for (int i = 0; i < qualityButtons.Length; i++)
        {
            bool isSelected = (i == activeIndex);

            if (qualityButtons[i] != null)
                qualityButtons[i].image.color = isSelected ? new Color32(250, 204, 21, 255) : new Color32(11, 17, 32, 255);

            if (qualityButtonLabels != null && i < qualityButtonLabels.Length && qualityButtonLabels[i] != null)
                qualityButtonLabels[i].color = isSelected ? new Color32(11, 17, 32, 255) : new Color32(148, 163, 184, 255);

            if (qualityButtonOutlines != null && i < qualityButtonOutlines.Length && qualityButtonOutlines[i] != null)
                qualityButtonOutlines[i].effectColor = isSelected ? new Color32(248, 250, 252, 255) : new Color32(51, 65, 85, 255);
        }
    }
}