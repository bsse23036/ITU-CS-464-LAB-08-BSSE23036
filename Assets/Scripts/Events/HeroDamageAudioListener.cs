using UnityEngine;

public class HeroDamageAudioListener : MonoBehaviour
{
    [Header("Event Publisher")]
    [SerializeField] private HeroHealth heroHealth;

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
        if (SfxPlayer.Instance != null)
        {
            SfxPlayer.Instance.Play(SfxPlayer.Instance.hurtDamageClip, 1f);
        }
    }

    private void HandleDied()
    {
        if (SfxPlayer.Instance != null && SfxPlayer.Instance.deathClip != null)
        {
            SfxPlayer.Instance.Play(SfxPlayer.Instance.deathClip, 1f, varyPitch: false);
        }
    }
}