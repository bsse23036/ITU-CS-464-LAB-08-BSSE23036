using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutScreenController : MonoBehaviour
{
    [Header("Left Stats Panel")]
    [SerializeField] private TMP_Text equippedSummaryText;
    [SerializeField] private Image speedFill;
    [SerializeField] private Image armorFill;
    [SerializeField] private Image energyFill;

    [Header("Suit Cards (0: Blue Vigilante, 1: Crimson Phantom, 2: Golden Titan)")]
    [SerializeField] private Outline[] suitCardOutlines;
    [SerializeField] private Button[] suitButtons;
    [SerializeField] private TMP_Text[] suitButtonLabels;

    [Header("Gadget Cards (0: Titanium Grapple, 1: Dual Laser Visor, 2: Shockwave Kick)")]
    [SerializeField] private Outline[] gadgetCardOutlines;
    [SerializeField] private Button[] gadgetButtons;
    [SerializeField] private TMP_Text[] gadgetButtonLabels;

    private readonly string[] suitNames = { "BLUE VIGILANTE", "CRIMSON PHANTOM", "GOLDEN TITAN" };
    private readonly string[] gadgetNames = { "TITANIUM GRAPPLE", "DUAL LASER VISOR", "SHOCKWAVE KICK" };

    private void OnEnable()
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.OnDataChanged += RefreshUI;
            RefreshUI();
        }
    }

    private void OnDisable()
    {
        if (ViceGameData.Instance != null)
            ViceGameData.Instance.OnDataChanged -= RefreshUI;
    }

    public void SelectSuit(int index)
    {
        if (index < 0 || index >= suitNames.Length || ViceGameData.Instance == null) return;
        string target = suitNames[index];

        if (ViceGameData.Instance.ownedItems.Contains(target))
        {
            ViceGameData.Instance.EquipSuit(target);
            UIAudioPlayer.Instance?.PlaySuccess();
        }
        else
        {
            UIManager.Instance?.NavigateTo("Shop");
        }
    }

    public void SelectGadget(int index)
    {
        if (index < 0 || index >= gadgetNames.Length || ViceGameData.Instance == null) return;
        string target = gadgetNames[index];

        if (ViceGameData.Instance.ownedItems.Contains(target))
        {
            ViceGameData.Instance.EquipGadget(target);
            UIAudioPlayer.Instance?.PlaySuccess();
        }
        else
        {
            UIManager.Instance?.NavigateTo("Shop");
        }
    }

    public void RefreshUI()
    {
        var data = ViceGameData.Instance;
        if (data == null) return;

        if (equippedSummaryText != null)
            equippedSummaryText.text = $"SUIT: {data.equippedSuit}\nGADGET: {data.equippedGadget}";

        UpdateCategory(suitNames, data.equippedSuit, data.ownedItems, suitCardOutlines, suitButtons, suitButtonLabels);
        UpdateCategory(gadgetNames, data.equippedGadget, data.ownedItems, gadgetCardOutlines, gadgetButtons, gadgetButtonLabels);

        // Update Filled stat bars dynamically
        if (speedFill != null)
            speedFill.fillAmount = (data.equippedSuit == "CRIMSON PHANTOM") ? 0.95f : 0.72f;

        if (armorFill != null)
            armorFill.fillAmount = (data.equippedSuit == "GOLDEN TITAN") ? 0.95f : (data.equippedGadget == "SHOCKWAVE KICK" ? 0.78f : 0.60f);

        if (energyFill != null)
            energyFill.fillAmount = (data.equippedGadget == "DUAL LASER VISOR") ? 0.96f : 0.70f;
    }

    private void UpdateCategory(string[] names, string equippedItem, System.Collections.Generic.HashSet<string> ownedSet, Outline[] outlines, Button[] buttons, TMP_Text[] labels)
    {
        for (int i = 0; i < names.Length && i < buttons.Length; i++)
        {
            string item = names[i];
            bool isOwned = ownedSet.Contains(item);
            bool isEquipped = (equippedItem == item);

            if (labels[i] != null)
            {
                labels[i].text = isEquipped ? "EQUIPPED" : (isOwned ? "EQUIP" : "GET IN SHOP >");
                labels[i].color = isOwned ? new Color32(11, 17, 32, 255) : new Color32(248, 250, 252, 255);
            }

            if (buttons[i] != null)
            {
                buttons[i].image.color = isEquipped
                    ? new Color32(34, 197, 94, 255)    // Emerald Green (#22C55E)
                    : (isOwned ? new Color32(250, 204, 21, 255) : new Color32(30, 41, 59, 255));
            }

            if (outlines != null && i < outlines.Length && outlines[i] != null)
            {
                outlines[i].effectColor = isEquipped
                    ? new Color32(34, 197, 94, 255)
                    : new Color32(51, 65, 85, 255);
            }
        }
    }
}