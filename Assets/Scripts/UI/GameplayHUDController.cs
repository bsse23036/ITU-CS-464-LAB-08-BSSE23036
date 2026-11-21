using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHUDController : MonoBehaviour
{
    [Header("HUD Text Readouts")]
    [SerializeField] private TMP_Text suitGadgetText;
    [SerializeField] private TMP_Text districtProtocolText;
    [SerializeField] private TMP_Text timerScoreText;

    [Header("Vitals Filled Bars")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthLabel;
    [SerializeField] private Image energyFill;
    [SerializeField] private TMP_Text energyLabel;

    private float elapsedMissionTime;
    private int currentScore = 1250;
    private float ropeEnergy = 1f;

    private void OnEnable()
    {
        var data = ViceGameData.Instance;
        if (data != null)
        {
            if (suitGadgetText != null)
                suitGadgetText.text = $"{data.equippedSuit} • {data.equippedGadget}";

            if (districtProtocolText != null)
                districtProtocolText.text = $"{data.selectedDistrict}  |  {data.selectedMode}";
        }

        if (!UIManager.IsPaused && elapsedMissionTime <= 0.05f)
        {
            currentScore = 1250;
            ropeEnergy = 1f;
        }
        UpdateVitalsUI();
    }

    private void Update()
    {
        // Automatically freezes when PauseScreen sets Time.timeScale = 0
        elapsedMissionTime += Time.deltaTime;
        ropeEnergy = Mathf.MoveTowards(ropeEnergy, 1f, Time.deltaTime * 0.25f);
        UpdateVitalsUI();
    }

    public void TriggerGrappleSwing()
    {
        ropeEnergy = Mathf.Clamp01(ropeEnergy - 0.28f);
        currentScore += 150;
        UpdateVitalsUI();
    }

    public void TriggerLaserAttack()
    {
        ropeEnergy = Mathf.Clamp01(ropeEnergy - 0.35f);
        currentScore += 250;
        UpdateVitalsUI();
    }

    public void CompletePatrolMission()
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.AddMissionReward(450, 10);
        }

        elapsedMissionTime = 0f;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.NavigateTo("Results");
        }
    }

    public void ResetMissionClock()
    {
        elapsedMissionTime = 0f;
        currentScore = 1250;
        ropeEnergy = 1f;
        UpdateVitalsUI();
    }

    private void UpdateVitalsUI()
    {
        int mins = Mathf.FloorToInt(elapsedMissionTime / 60f);
        int secs = Mathf.FloorToInt(elapsedMissionTime % 60f);

        if (timerScoreText != null)
            timerScoreText.text = $"TIME: {mins:00}:{secs:00}   -   SCORE: {currentScore:N0}";

        if (healthFill != null) healthFill.fillAmount = 1.0f;
        if (healthLabel != null) healthLabel.text = "ARMOR HP: 100%";

        if (energyFill != null) energyFill.fillAmount = ropeEnergy;
        if (energyLabel != null) energyLabel.text = $"ROPE CHARGE: {Mathf.RoundToInt(ropeEnergy * 100f)}%";
    }
}