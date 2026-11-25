using System;
using System.Collections.Generic;
using UnityEngine;

public class ViceGameData : MonoBehaviour
{
    public static ViceGameData Instance { get; private set; }

    public event Action OnDataChanged;

    [Header("Vice Town Economy")]
    public int viceCash = 2500;
    public int heroGems = 45;
    public string selectedDistrict = "01 • NEON BEACH";
    public string selectedMode = "VICE SANDBOX";

    [Header("Equipped Hero Gear")]
    public string equippedSuit = "BLUE VIGILANTE";
    public string equippedGadget = "TITANIUM GRAPPLE";

    [Header("Unlocked Arsenal")]
    public HashSet<string> ownedItems = new HashSet<string>
    {
        "BLUE VIGILANTE",
        "TITANIUM GRAPPLE"
    };

    [Header("System Settings")]
    [Range(0f, 1f)] public float masterVolume = 0.85f;
    [Range(0f, 1f)] public float sfxVolume = 0.90f;
    [Range(0.5f, 2f)] public float swingSensitivity = 1.0f;
    public int graphicsQualityIndex = 1; // 0 = Low, 1 = Medium, 2 = Ultra

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool TryPurchaseItem(string itemName, int cost)
    {
        if (ownedItems.Contains(itemName)) return true;
        if (viceCash < cost) return false;

        viceCash -= cost;
        ownedItems.Add(itemName);
        OnDataChanged?.Invoke();
        return true;
    }
    public void AddRewards(int cashAmount, int gemAmount = 0)
    {
        viceCash += cashAmount;
        OnDataChanged?.Invoke();
    }

    public void EquipSuit(string suitName)
    {
        if (!ownedItems.Contains(suitName)) return;
        equippedSuit = suitName;
        OnDataChanged?.Invoke();
    }

    public void EquipGadget(string gadgetName)
    {
        if (!ownedItems.Contains(gadgetName)) return;
        equippedGadget = gadgetName;
        OnDataChanged?.Invoke();
    }

    public void AddMissionReward(int cashEarned, int gemsEarned)
    {
        viceCash += cashEarned;
        heroGems += gemsEarned;
        OnDataChanged?.Invoke();
    }

    public void NotifySettingsUpdated()
    {
        AudioListener.volume = masterVolume;
        OnDataChanged?.Invoke();
    }
}