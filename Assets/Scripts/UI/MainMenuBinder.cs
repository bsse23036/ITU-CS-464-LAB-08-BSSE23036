using TMPro;
using UnityEngine;

public class MainMenuBinder : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private TMP_Text heroTitleText;
    [SerializeField] private TMP_Text loadoutSummaryText;

    private void OnEnable()
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.OnDataChanged += RefreshMainMenu;
            RefreshMainMenu();
        }
    }

    private void OnDisable()
    {
        if (ViceGameData.Instance != null)
        {
            ViceGameData.Instance.OnDataChanged -= RefreshMainMenu;
        }
    }

    public void RefreshMainMenu()
    {
        var data = ViceGameData.Instance;
        if (data == null) return;

        if (currencyText != null)
            currencyText.SetText("<color=#22C55E>$ {0:N0}</color>   |   <color=#38BDF8>◆ {1}</color>", data.viceCash, data.heroGems);

        if (heroTitleText != null)
            heroTitleText.text = $"ACTIVE HERO: {data.equippedSuit}";

        if (loadoutSummaryText != null)
            loadoutSummaryText.text = $"EQUIPPED GADGET: {data.equippedGadget}";
    }
}