using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TitlePromptPulse : MonoBehaviour
{
    [SerializeField] private float cycleDuration = 1.4f;
    [SerializeField] private float minAlpha = 0.38f;
    [SerializeField] private float maxAlpha = 1.0f;

    private CanvasGroup group;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        float pingPong = Mathf.PingPong(Time.unscaledTime / cycleDuration, 1f);
        float eased = UITweener.Evaluate(EaseType.InOutSine, pingPong);
        group.alpha = Mathf.Lerp(minAlpha, maxAlpha, eased);
    }
}