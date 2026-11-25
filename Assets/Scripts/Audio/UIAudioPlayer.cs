using UnityEngine;
using UnityEngine.Audio;

public class UIAudioPlayer : MonoBehaviour
{
    public static UIAudioPlayer Instance { get; private set; }

    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup uiMixerGroup;

    [Header("UI Audio Clips")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip backClip;
    [SerializeField] private AudioClip panelClip;
    [SerializeField] private AudioClip pauseClip;
    [SerializeField] private AudioClip resumeClip;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip errorClip;

    [Header("Pitch Subtlety")]
    [SerializeField] private float hoverPitchJitter = 0.04f;

    private AudioSource uiSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        uiSource = gameObject.GetComponent<AudioSource>();
        if (uiSource == null)
        {
            uiSource = gameObject.AddComponent<AudioSource>();
        }

        uiSource.playOnAwake = false;
        uiSource.loop = false;
        uiSource.spatialBlend = 0f;
        uiSource.outputAudioMixerGroup = uiMixerGroup;
        // Keep UI audio audible when AudioListener.pause = true during PauseScreen
        uiSource.ignoreListenerPause = true;
    }

    public void PlayHover()
    {
        if (hoverClip == null) return;
        uiSource.pitch = 1f + Random.Range(-hoverPitchJitter, hoverPitchJitter);
        uiSource.PlayOneShot(hoverClip, 0.65f);
    }

    public void PlayClick()
    {
        PlayStandard(clickClip, 0.95f);
    }

    public void PlayBack()
    {
        PlayStandard(backClip != null ? backClip : clickClip, 0.90f);
    }

    public void PlayPanelToggle()
    {
        PlayStandard(panelClip, 0.80f);
    }

    public void PlayPause()
    {
        PlayStandard(pauseClip != null ? pauseClip : panelClip, 1.0f);
    }

    public void PlayResume()
    {
        PlayStandard(resumeClip != null ? resumeClip : clickClip, 1.0f);
    }

    public void PlaySuccess()
    {
        PlayStandard(successClip, 1.0f);
    }

    public void PlayError()
    {
        PlayStandard(errorClip, 1.0f);
    }

    private void PlayStandard(AudioClip clip, float volume)
    {
        if (clip == null || uiSource == null) return;
        uiSource.pitch = 1f;
        uiSource.PlayOneShot(clip, volume);
    }
}