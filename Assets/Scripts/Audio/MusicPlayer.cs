using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    public enum MusicState
    {
        Menu,
        Patrol,
        Combat,
        Results
    }

    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    [Header("Looping Music Tracks")]
    [SerializeField] private AudioClip menuTrack;
    [SerializeField] private AudioClip patrolTrack;
    [SerializeField] private AudioClip combatTrack;
    [SerializeField] private AudioClip resultsTrack;

    [Header("Crossfade Settings")]
    [SerializeField] private float crossfadeSeconds = 1.2f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private bool usingA = true;
    private MusicState currentState = (MusicState)(-1);
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sourceA = CreateMusicSource("MusicSource_A");
        sourceB = CreateMusicSource("MusicSource_B");
    }

    private void Start()
    {
        SetMusicState(MusicState.Menu, immediate: true);
    }

    private AudioSource CreateMusicSource(string label)
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = true;
        src.priority = 0;       // Highest priority so Unity never drops music voices
        src.spatialBlend = 0f;  // Pure 2D stereo
        src.outputAudioMixerGroup = musicMixerGroup;
        // Music continues smoothly during PauseScreen while SFX suspend
        src.ignoreListenerPause = true;
        return src;
    }

    public void SetMusicState(MusicState newState, bool immediate = false)
    {
        if (newState == currentState) return;
        currentState = newState;

        AudioClip targetClip = GetClipForState(newState);
        if (targetClip == null) return;

        AudioSource active = usingA ? sourceA : sourceB;
        AudioSource incoming = usingA ? sourceB : sourceA;

        // If same clip is already playing on active source, keep seamless loop
        if (active.isPlaying && active.clip == targetClip) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        if (immediate)
        {
            active.Stop();
            incoming.clip = targetClip;
            incoming.volume = 1f;
            incoming.Play();
            usingA = !usingA;
        }
        else
        {
            fadeRoutine = StartCoroutine(EqualPowerCrossfade(active, incoming, targetClip));
            usingA = !usingA;
        }
    }

    private IEnumerator EqualPowerCrossfade(AudioSource from, AudioSource to, AudioClip nextClip)
    {
        to.clip = nextClip;
        to.volume = 0f;
        to.Play();

        float startFromVol = from.volume;
        float elapsed = 0f;

        while (elapsed < crossfadeSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(elapsed / crossfadeSeconds);
            float angle = u * Mathf.PI * 0.5f;

            from.volume = startFromVol * Mathf.Cos(angle);
            to.volume = Mathf.Sin(angle);
            yield return null;
        }

        from.volume = 0f;
        from.Stop();
        to.volume = 1f;
    }

    private AudioClip GetClipForState(MusicState state)
    {
        switch (state)
        {
            case MusicState.Menu:    return menuTrack;
            case MusicState.Patrol:  return patrolTrack != null ? patrolTrack : menuTrack;
            case MusicState.Combat:  return combatTrack != null ? combatTrack : patrolTrack;
            case MusicState.Results: return resultsTrack != null ? resultsTrack : menuTrack;
            default:                 return menuTrack;
        }
    }
}