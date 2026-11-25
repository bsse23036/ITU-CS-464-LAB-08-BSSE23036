using UnityEngine;

public class HitImpactVfxListener : MonoBehaviour
{
    [Header("Event Publisher")]
    [SerializeField] private HeroHealth heroHealth;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem hitSparks;

    private void OnEnable()
    {
        if (heroHealth != null)
        {
            heroHealth.Damaged += HandleDamaged;
        }
    }

    private void OnDisable()
    {
        if (heroHealth != null)
        {
            heroHealth.Damaged -= HandleDamaged;
        }
    }

    private void HandleDamaged(int damageAmount, int remainingHp)
    {
        if (hitSparks != null)
        {
            hitSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hitSparks.Play();
        }
    }
}