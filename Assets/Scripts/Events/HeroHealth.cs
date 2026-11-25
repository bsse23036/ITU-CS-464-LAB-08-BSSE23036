using System;
using UnityEngine;

public class HeroHealth : MonoBehaviour
{
    public event Action<int, int> HealthChanged; // (currentHp, maxHp)
    public event Action<int, int> Damaged;       // (damageAmount, currentHp)
    public event Action Died;

    [Header("Armor Health Settings")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private float hitCooldownSeconds = 0.8f;

    private int currentHp;
    private float nextAllowedHitTime;
    private bool isDead;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    private void Awake()
    {
        // Scale max HP slightly if Golden Titan suit is equipped
        if (ViceGameData.Instance != null && ViceGameData.Instance.equippedSuit == "GOLDEN TITAN")
        {
            maxHp = 130;
        }
        currentHp = maxHp;
    }

    private void Start()
    {
        HealthChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0) return;
        if (Time.time < nextAllowedHitTime) return;

        nextAllowedHitTime = Time.time + hitCooldownSeconds;
        currentHp = Mathf.Max(0, currentHp - amount);

        Damaged?.Invoke(amount, currentHp);
        HealthChanged?.Invoke(currentHp, maxHp);

        if (currentHp == 0)
        {
            isDead = true;
            Died?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;
        currentHp = Mathf.Min(maxHp, currentHp + amount);
        HealthChanged?.Invoke(currentHp, maxHp);
    }
}