using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    public static AudioMixerManager Instance { get; private set; }

    [Header("Audio Mixer Reference")]
    [SerializeField] private AudioMixer gameMixer;

    // PlayerPrefs Keys
    public const string KEY_MASTER = "MasterVolume";
    public const string KEY_MUSIC  = "MusicVolume";
    public const string KEY_SFX    = "SfxVolume";
    public const string KEY_UI     = "UiVolume";

    // Exposed Mixer Parameter Names
    private const string PARAM_MASTER = "MasterVolume";
    private const string PARAM_MUSIC  = "MusicVolume";
    private const string PARAM_SFX    = "SfxVolume";
    private const string PARAM_UI     = "UiVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ApplySavedVolumes();
    }

    public void ApplySavedVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(KEY_MASTER, 0.85f), false);
        SetMusicVolume(PlayerPrefs.GetFloat(KEY_MUSIC, 0.80f), false);
        SetSfxVolume(PlayerPrefs.GetFloat(KEY_SFX, 0.90f), false);
        SetUiVolume(PlayerPrefs.GetFloat(KEY_UI, 0.90f), false);
    }

    public void SetMasterVolume(float sliderValue, bool save = true)
    {
        ApplyVolume(PARAM_MASTER, KEY_MASTER, sliderValue, save);
    }

    public void SetMusicVolume(float sliderValue, bool save = true)
    {
        ApplyVolume(PARAM_MUSIC, KEY_MUSIC, sliderValue, save);
    }

    public void SetSfxVolume(float sliderValue, bool save = true)
    {
        ApplyVolume(PARAM_SFX, KEY_SFX, sliderValue, save);
    }

    public void SetUiVolume(float sliderValue, bool save = true)
    {
        ApplyVolume(PARAM_UI, KEY_UI, sliderValue, save);
    }

    private void ApplyVolume(string exposedParam, string prefsKey, float linearValue, bool save)
    {
        // Clamp to 0.0001f so Mathf.Log10(0.0001f) * 20 = -80 dB (safe mute without -Infinity)
        float clamped = Mathf.Clamp(linearValue, 0.0001f, 1f);
        float dB = Mathf.Log10(clamped) * 20f;

        if (gameMixer != null)
        {
            gameMixer.SetFloat(exposedParam, dB);
        }

        if (save)
        {
            PlayerPrefs.SetFloat(prefsKey, clamped);
            PlayerPrefs.Save();
        }
    }

    public float GetSavedVolume(string prefsKey, float defaultValue = 0.85f)
    {
        return PlayerPrefs.GetFloat(prefsKey, defaultValue);
    }
}