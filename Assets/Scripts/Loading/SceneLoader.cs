using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading Screen References")]
    [SerializeField] private Image progressFill;
    [SerializeField] private TMP_Text percentageText;
    [SerializeField] private TMP_Text districtSubtitleText;

    [Header("Minimum Visual Smoothing (Seconds)")]
    [SerializeField] private float minimumDisplaySeconds = 1.0f;

    private bool isLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void LoadGameplayScene(string sceneName = "GameplayScene")
    {
        if (isLoading) return;
        StartCoroutine(LoadAsyncRoutine(sceneName));
    }

    private IEnumerator LoadAsyncRoutine(string sceneName)
    {
        isLoading = true;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (ViceGameData.Instance != null && districtSubtitleText != null)
        {
            districtSubtitleText.text = $"{ViceGameData.Instance.selectedDistrict}  |  {ViceGameData.Instance.selectedMode}";
        }

        if (progressFill != null) progressFill.fillAmount = 0f;
        if (percentageText != null) percentageText.text = "LOADING SECTOR: 0%";

        if (UIManager.Instance != null)
        {
            UIManager.Instance.NavigateTo("Loading");
        }

        // Allow LoadingScreen CrossFade to finish before starting background load
        yield return new WaitForSecondsRealtime(0.28f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float elapsed = 0f;

        while (!op.isDone)
        {
            elapsed += Time.unscaledDeltaTime;

            // Map Unity's 0.0 -> 0.9 loading range to 0.0 -> 1.0
            float rawProgress = Mathf.Clamp01(op.progress / 0.9f);
            float smoothProgress = Mathf.Clamp01(elapsed / minimumDisplaySeconds);
            float displayProgress = Mathf.Min(rawProgress, smoothProgress);

            if (progressFill != null)
                progressFill.fillAmount = displayProgress;

            if (percentageText != null)
                percentageText.text = $"LOADING SECTOR: {Mathf.RoundToInt(displayProgress * 100f)}%";

            // Block gameplay entry until both async load and visual progress reach 100%
            if (op.progress >= 0.9f && displayProgress >= 0.999f)
            {
                if (progressFill != null) progressFill.fillAmount = 1f;
                if (percentageText != null) percentageText.text = "LOADING SECTOR: 100%";
                yield return new WaitForSecondsRealtime(0.15f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        isLoading = false;
    }
}