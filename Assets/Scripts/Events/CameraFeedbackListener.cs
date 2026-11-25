using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFeedbackListener : MonoBehaviour
{
    [Header("Event Publisher")]
    [SerializeField] private HeroHealth heroHealth;

    [Header("Feedback Targets")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Image damageFlashOverlay;
    [SerializeField] private ViceHeroController heroController;

    [Header("Shake Tuning")]
    [SerializeField] private float shakeDuration = 0.22f;
    [SerializeField] private float shakeMagnitude = 0.28f;

    private Vector3 originalCamLocalPos;
    private Coroutine feedbackRoutine;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        if (cameraTransform != null)
        {
            originalCamLocalPos = cameraTransform.localPosition;
        }
        if (damageFlashOverlay != null)
        {
            SetOverlayAlpha(0f);
        }
    }

    private void OnEnable()
    {
        if (heroHealth != null)
        {
            heroHealth.Damaged += HandleDamaged;
            heroHealth.Died += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (heroHealth != null)
        {
            heroHealth.Damaged -= HandleDamaged;
            heroHealth.Died -= HandleDied;
        }
    }

    private void HandleDamaged(int damageAmount, int remainingHp)
    {
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(ShakeAndFlashRoutine());
    }

    private void HandleDied()
    {
        if (heroController != null)
        {
            heroController.CompleteMissionLose();
        }
    }

    private IEnumerator ShakeAndFlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Clamp01(elapsed / shakeDuration);

            if (cameraTransform != null)
            {
                Vector3 offset = Random.insideUnitSphere * (shakeMagnitude * t);
                offset.z = 0f;
                cameraTransform.localPosition = originalCamLocalPos + offset;
            }

            if (damageFlashOverlay != null)
            {
                SetOverlayAlpha(0.35f * t);
            }

            yield return null;
        }

        if (cameraTransform != null) cameraTransform.localPosition = originalCamLocalPos;
        if (damageFlashOverlay != null) SetOverlayAlpha(0f);
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color c = damageFlashOverlay.color;
        c.a = alpha;
        damageFlashOverlay.color = c;
    }
}