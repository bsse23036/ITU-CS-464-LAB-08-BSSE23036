using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreenController : MonoBehaviour
{
    [Header("Header Economy Readout")]
    [SerializeField] private TMP_Text cashBalanceText;

    [Header("Shop Cards (0: Crimson Phantom, 1: Golden Titan, 2: Dual Laser Visor, 3: Shockwave Kick)")]
    [SerializeField] private Outline[] cardOutlines;
    [SerializeField] private Button[] buyButtons;
    [SerializeField] private TMP_Text[] buyButtonLabels;

    [Header("Purchase Feedback Toast")]
    [SerializeField] private CanvasGroup toastGroup;
    [SerializeField] private Image toastBackground;
    [SerializeField] private TMP_Text toastText;

    private readonly string[] itemNames = { "CRIMSON PHANTOM", "GOLDEN TITAN", "DUAL LASER VISOR", "SHOCKWAVE KICK" };
    private readonly int[] itemCosts = { 800, 1200, 600, 500 };

    private Coroutine toastRoutine;

    private void OnEnable()
    {
        if (toastGroup != null) toastGroup.alpha = 0f;

        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.OnDataChanged += RefreshShop;
            RefreshShop();
        }
    }

    private void OnDisable()
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.OnDataChanged -= RefreshShop;
        }
    }

    public void BuyItem(int index)
    {
        if (index < 0 || index >= itemNames.Length || ViceGameData.Instance == null) return;

        string item = itemNames[index];
        int cost = itemCosts[index];

        if (ViceGameData.Instance.ownedItems.Contains(item))
        {
            ShowToast($"{item} ALREADY UNLOCKED", new Color32(56, 189, 248, 255));
            return;
        }

        if (ViceGameData.Instance.TryPurchaseItem(item, cost))
        {
            ShowToast($"PURCHASED: {item} (-${cost:N0})", new Color32(34, 197, 94, 255));
        }
        else
        {
            ShowToast($"INSUFFICIENT VICE CASH (NEED ${cost:N0})", new Color32(239, 68, 68, 255));
        }
    }

    private void ShowToast(string message, Color32 bgColor)
    {
        if (toastGroup == null || toastText == null || toastBackground == null) return;

        if (toastRoutine != null) StopCoroutine(toastRoutine);
        toastRoutine = StartCoroutine(AnimateToastRoutine(message, bgColor));
    }

    private IEnumerator AnimateToastRoutine(string message, Color32 bgColor)
    {
        toastText.text = message;
        toastText.color = new Color32(11, 17, 32, 255);
        toastBackground.color = bgColor;

        UITweener.TweenAlpha(this, toastGroup, 0f, 1f, 0.2f, EaseType.OutCubic);
        UITweener.TweenScale(this, toastGroup.transform, Vector3.one * 0.9f, Vector3.one, 0.24f, EaseType.OutBack);

        yield return new WaitForSecondsRealtime(2.0f);

        UITweener.TweenAlpha(this, toastGroup, toastGroup.alpha, 0f, 0.25f, EaseType.InCubic);
    }

    public void RefreshShop()
    {
        var data = ViceGameData.Instance;
        if (data == null) return;

        if (cashBalanceText != null)
            cashBalanceText.text = $"VICE CASH: $ {data.viceCash:N0}";

        for (int i = 0; i < itemNames.Length && i < buyButtons.Length; i++)
        {
            bool owned = data.ownedItems.Contains(itemNames[i]);

            if (buyButtons[i] != null)
            {
                buyButtons[i].interactable = !owned;
                buyButtons[i].image.color = owned
                    ? new Color32(30, 41, 59, 255)      // Dark Slate when owned
                    : new Color32(250, 204, 21, 255);   // Bright Comic Gold when available
            }

            if (buyButtonLabels != null && i < buyButtonLabels.Length && buyButtonLabels[i] != null)
            {
                buyButtonLabels[i].text = owned ? "OWNED" : $"BUY  ${itemCosts[i]:N0}";
                buyButtonLabels[i].color = owned
                    ? new Color32(148, 163, 184, 255)
                    : new Color32(11, 17, 32, 255);
            }

            if (cardOutlines != null && i < cardOutlines.Length && cardOutlines[i] != null)
            {
                cardOutlines[i].effectColor = owned
                    ? new Color32(34, 197, 94, 255)
                    : new Color32(37, 99, 235, 255);
            }
        }
    }
}