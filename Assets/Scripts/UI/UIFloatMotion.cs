using UnityEngine;

public class UIFloatMotion : MonoBehaviour
{
    [SerializeField] private float amplitude = 8f;
    [SerializeField] private float cycleDuration = 2.4f;

    private RectTransform rect;
    private Vector2 basePos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        basePos = rect.anchoredPosition;
    }

    private void Update()
    {
        float wave = Mathf.PingPong(Time.unscaledTime / cycleDuration, 1f);
        float eased = UITweener.Evaluate(EaseType.InOutSine, wave);
        float offsetY = Mathf.Lerp(-amplitude, amplitude, eased);
        rect.anchoredPosition = basePos + new Vector2(0f, offsetY);
    }
}