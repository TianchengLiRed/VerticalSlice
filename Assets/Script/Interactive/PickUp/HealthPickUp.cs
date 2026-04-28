using UnityEngine;

public class HealPickUp : MonoBehaviour
{
    [SerializeField] private float healAmount = 25f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.Heal(healAmount);
        }

        Destroy(gameObject);
    }
}
