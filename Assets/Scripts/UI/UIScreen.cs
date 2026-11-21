using UnityEngine;

public enum ScreenTransitionStyle
{
    DirectionalSlide, // Slides horizontally based on forward/back navigation (OutCubic)
    ModalPop,         // Scales from 0.92 -> 1.0 with OutBack for overlays (Pause, Results)
    CrossFade         // Pure alpha fade for Title Screen
}

[RequireComponent(typeof(CanvasGroup))]
public class UIScreen : MonoBehaviour
{
    [Header("Screen Identity")]
    public string screenId;
    public ScreenTransitionStyle transitionStyle = ScreenTransitionStyle.DirectionalSlide;
    public float transitionDuration = 0.28f;

    [Header("Accessibility / Keyboard Navigation")]
    [Tooltip("First button selected when this screen opens")]
    public GameObject firstSelectedButton;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowImmediate()
    {
        gameObject.SetActive(true);
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

    public void HideImmediate()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    public void AnimateIn(bool isForward)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        switch (transitionStyle)
        {
            case ScreenTransitionStyle.DirectionalSlide:
                float startX = isForward ? 180f : -180f;
                rectTransform.localScale = Vector3.one;
                UITweener.TweenAlpha(this, canvasGroup, 0f, 1f, transitionDuration, EaseType.OutCubic);
                UITweener.TweenAnchoredPos(this, rectTransform, new Vector2(startX, 0f), Vector2.zero, transitionDuration, EaseType.OutCubic);
                break;

            case ScreenTransitionStyle.ModalPop:
                rectTransform.anchoredPosition = Vector2.zero;
                UITweener.TweenAlpha(this, canvasGroup, 0f, 1f, transitionDuration, EaseType.OutCubic);
                UITweener.TweenScale(this, rectTransform, Vector3.one * 0.92f, Vector3.one, transitionDuration, EaseType.OutBack);
                break;

            case ScreenTransitionStyle.CrossFade:
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one;
                UITweener.TweenAlpha(this, canvasGroup, 0f, 1f, transitionDuration, EaseType.OutCubic);
                break;
        }
    }

    public void AnimateOut(bool isForward)
    {
        if (!gameObject.activeSelf) return;
        StopAllCoroutines();

        // Immediately disable raycasts so outgoing screen never blocks incoming buttons
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float exitDuration = transitionDuration * 0.8f;

        switch (transitionStyle)
        {
            case ScreenTransitionStyle.DirectionalSlide:
                float endX = isForward ? -140f : 140f;
                UITweener.TweenAlpha(this, canvasGroup, canvasGroup.alpha, 0f, exitDuration, EaseType.InCubic);
                UITweener.TweenAnchoredPos(this, rectTransform, rectTransform.anchoredPosition, new Vector2(endX, 0f), exitDuration, EaseType.InCubic, () =>
                {
                    gameObject.SetActive(false);
                });
                break;

            case ScreenTransitionStyle.ModalPop:
                UITweener.TweenAlpha(this, canvasGroup, canvasGroup.alpha, 0f, exitDuration, EaseType.InCubic);
                UITweener.TweenScale(this, rectTransform, rectTransform.localScale, Vector3.one * 0.94f, exitDuration, EaseType.InCubic, () =>
                {
                    gameObject.SetActive(false);
                });
                break;

            case ScreenTransitionStyle.CrossFade:
                UITweener.TweenAlpha(this, canvasGroup, canvasGroup.alpha, 0f, exitDuration, EaseType.InCubic, () =>
                {
                    gameObject.SetActive(false);
                });
                break;
        }
    }
}