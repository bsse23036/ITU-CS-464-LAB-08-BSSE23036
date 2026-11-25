using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        SceneLoader loader = SceneLoader.Instance;
        if (loader == null)
        {
            loader = FindFirstObjectByType<SceneLoader>(FindObjectsInactive.Include);
        }

        if (loader != null)
        {
            // Ensure the GameObject hosting SceneLoader is active so StartCoroutine can run
            if (!loader.gameObject.activeInHierarchy)
            {
                loader.gameObject.SetActive(true);
            }
            loader.LoadGameplayScene("GameplayScene");
        }
        else
        {
            Debug.LogWarning("SceneLoader not found in MainMenuScene. Loading GameplayScene directly.");
            SceneManager.LoadScene("GameplayScene");
        }
    }
}