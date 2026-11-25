using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmorHudListener : MonoBehaviour
{
    [Header("Event Publisher")]
    [SerializeField] private HeroHealth heroHealth;

    [Header("HUD Elements")]
    [SerializeField] private Image armorFillBar;
    [SerializeField] private TMP_Text armorPercentText;

    private void OnEnable()
    {
        if (heroHealth != null)
        {
            heroHealth.HealthChanged += HandleHealthChanged;
            HandleHealthChanged(heroHealth.CurrentHp, heroHealth.MaxHp);
        }
    }

    private void OnDisable()
    {
        if (heroHealth != null)
        {
            heroHealth.HealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        float ratio = max > 0 ? (float)current / max : 0f;
        if (armorFillBar != null)
        {
            armorFillBar.fillAmount = ratio;
        }
        if (armorPercentText != null)
        {
            armorPercentText.text = $"ARMOR HP: {Mathf.RoundToInt(ratio * 100f)}%";
        }
    }
}