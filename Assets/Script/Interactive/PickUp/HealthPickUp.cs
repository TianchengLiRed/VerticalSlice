using UnityEngine;

public class HealPickup : Collectible
{
    [SerializeField] private float healAmount = 25f;

    protected override void OnCollect()
    {
        HealthManager health = GetComponentFromPlayer();

        if (health != null)
        {
            health.Heal(healAmount);
        }
    }

    private HealthManager GetComponentFromPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return null;

        return player.GetComponent<HealthManager>();
    }
}