using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float pressedScale = 0.94f;
    [SerializeField] private float pressDuration = 0.08f;
    [SerializeField] private float releaseDuration = 0.16f;

    private Button button;
    private Coroutine activeTween;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        RunScaleTween(Vector3.one * pressedScale, pressDuration, EaseType.OutCubic);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        RunScaleTween(Vector3.one, releaseDuration, EaseType.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (transform.localScale != Vector3.one)
        {
            RunScaleTween(Vector3.one, releaseDuration, EaseType.OutCubic);
        }
    }

    private void RunScaleTween(Vector3 targetScale, float duration, EaseType ease)
    {
        if (activeTween != null) StopCoroutine(activeTween);
        activeTween = UITweener.TweenScale(this, transform, transform.localScale, targetScale, duration, ease);
    }
}