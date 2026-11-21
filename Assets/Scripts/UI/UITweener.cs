using System;
using System.Collections;
using UnityEngine;

public enum EaseType
{
    Linear,
    OutCubic,   // Smooth deceleration — ideal for screen slides & fades
    InCubic,    // Smooth acceleration — ideal for screen exits
    OutBack,    // Slight overshoot — ideal for modals, popups & button releases
    InOutSine   // Gentle symmetric curve — ideal for ambient pulsing prompts
}

public static class UITweener
{
    public static float Evaluate(EaseType ease, float t)
    {
        t = Mathf.Clamp01(t);
        switch (ease)
        {
            case EaseType.OutCubic:
                return 1f - Mathf.Pow(1f - t, 3f);

            case EaseType.InCubic:
                return t * t * t;

            case EaseType.OutBack:
                const float c1 = 1.70158f;
                const float c3 = c1 + 1f;
                return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

            case EaseType.InOutSine:
                return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;

            default:
                return t;
        }
    }

    public static Coroutine TweenAlpha(MonoBehaviour runner, CanvasGroup group, float from, float to, float duration, EaseType ease = EaseType.OutCubic, Action onComplete = null)
    {
        return runner.StartCoroutine(AlphaRoutine(group, from, to, duration, ease, onComplete));
    }

    public static Coroutine TweenAnchoredPos(MonoBehaviour runner, RectTransform rect, Vector2 from, Vector2 to, float duration, EaseType ease = EaseType.OutCubic, Action onComplete = null)
    {
        return runner.StartCoroutine(AnchoredPosRoutine(rect, from, to, duration, ease, onComplete));
    }

    public static Coroutine TweenScale(MonoBehaviour runner, Transform target, Vector3 from, Vector3 to, float duration, EaseType ease = EaseType.OutBack, Action onComplete = null)
    {
        return runner.StartCoroutine(ScaleRoutine(target, from, to, duration, ease, onComplete));
    }

    private static IEnumerator AlphaRoutine(CanvasGroup group, float from, float to, float duration, EaseType ease, Action onComplete)
    {
        if (group == null) yield break;
        float elapsed = 0f;
        group.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Evaluate(ease, elapsed / duration);
            group.alpha = Mathf.LerpUnclamped(from, to, k);
            yield return null;
        }

        group.alpha = to;
        onComplete?.Invoke();
    }

    private static IEnumerator AnchoredPosRoutine(RectTransform rect, Vector2 from, Vector2 to, float duration, EaseType ease, Action onComplete)
    {
        if (rect == null) yield break;
        float elapsed = 0f;
        rect.anchoredPosition = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Evaluate(ease, elapsed / duration);
            rect.anchoredPosition = Vector2.LerpUnclamped(from, to, k);
            yield return null;
        }

        rect.anchoredPosition = to;
        onComplete?.Invoke();
    }

    private static IEnumerator ScaleRoutine(Transform target, Vector3 from, Vector3 to, float duration, EaseType ease, Action onComplete)
    {
        if (target == null) yield break;
        float elapsed = 0f;
        target.localScale = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Evaluate(ease, elapsed / duration);
            target.localScale = Vector3.LerpUnclamped(from, to, k);
            yield return null;
        }

        target.localScale = to;
        onComplete?.Invoke();
    }
}