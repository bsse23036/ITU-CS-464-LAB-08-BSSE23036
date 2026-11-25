using UnityEngine;

public class HazardDamageTrigger : MonoBehaviour
{
    [SerializeField] private int damagePerHit = 25;

    private void OnTriggerEnter(Collider other)
    {
        ApplyHazardDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyHazardDamage(other);
    }

    private void ApplyHazardDamage(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        HeroHealth health = other.GetComponent<HeroHealth>();
        if (health != null)
        {
            health.TakeDamage(damagePerHit);
        }
    }
}