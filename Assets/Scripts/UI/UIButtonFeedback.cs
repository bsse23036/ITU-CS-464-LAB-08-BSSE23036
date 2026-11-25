using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonFeedback : MonoBehaviour,
    IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, ISelectHandler
{
    public enum ButtonSoundRole
    {
        Standard,
        Back,
        Silent
    }

    [Header("Audio Role")]
    [SerializeField] private ButtonSoundRole soundRole = ButtonSoundRole.Standard;

    private Button button;
    private Vector3 originalScale;
    private Coroutine scaleRoutine;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        if (soundRole != ButtonSoundRole.Silent)
            UIAudioPlayer.Instance?.PlayHover();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (button != null && !button.interactable) return;
        if (soundRole != ButtonSoundRole.Silent)
            UIAudioPlayer.Instance?.PlayHover();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        if (soundRole == ButtonSoundRole.Back)
            UIAudioPlayer.Instance?.PlayBack();
        else if (soundRole == ButtonSoundRole.Standard)
            UIAudioPlayer.Instance?.PlayClick();

        AnimateScale(originalScale * 0.94f, 0.08f, false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateScale(originalScale, 0.16f, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(originalScale, 0.14f, false);
    }

    private void AnimateScale(Vector3 target, float duration, bool useOutBack)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleRoutine(target, duration, useOutBack));
    }

    private IEnumerator ScaleRoutine(Vector3 target, float duration, bool useOutBack)
    {
        Vector3 start = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = useOutBack ? EvaluateOutBack(t) : EvaluateOutCubic(t);
            transform.localScale = Vector3.LerpUnclamped(start, target, eased);
            yield return null;
        }

        transform.localScale = target;
    }

    private static float EvaluateOutCubic(float t)
    {
        float inv = 1f - t;
        return 1f - inv * inv * inv;
    }

    private static float EvaluateOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        float x = t - 1f;
        return 1f + c3 * x * x * x + c1 * x * x;
    }
}