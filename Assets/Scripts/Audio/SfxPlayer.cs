using UnityEngine;
using UnityEngine.Audio;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }

    [Header("Mixer Routing & Voice Pool")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private int poolSize = 6;

    [Header("Randomization (Lecture 09)")]
    [SerializeField] private float pitchRangeSemitones = 2f;
    [SerializeField] private float minVolumeScale = 0.85f;

    [Header("Gameplay SFX Library (Assign 4+ Clips)")]
    public AudioClip pickupClip;
    public AudioClip grappleBoostClip;
    public AudioClip laserAttackClip;
    public AudioClip hurtDamageClip;
    public AudioClip deathClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    private AudioSource[] sources;
    private int nextIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;
            src.outputAudioMixerGroup = sfxMixerGroup;
            // Gameplay SFX suspend automatically when AudioListener.pause = true
            src.ignoreListenerPause = false;
            sources[i] = src;
        }
    }

    public void Play(AudioClip clip, float volume = 1f, bool varyPitch = true)
    {
        if (clip == null || sources == null || sources.Length == 0) return;

        AudioSource source = sources[nextIndex];
        nextIndex = (nextIndex + 1) % sources.Length;

        if (varyPitch)
        {
            float semitones = Random.Range(-pitchRangeSemitones, pitchRangeSemitones);
            source.pitch = Mathf.Pow(2f, semitones / 12f);
        }
        else
        {
            source.pitch = 1f;
        }

        float volJitter = varyPitch ? Random.Range(minVolumeScale, 1f) : 1f;
        source.PlayOneShot(clip, volume * volJitter);
    }
}