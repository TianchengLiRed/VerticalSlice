using UnityEngine;

public class PlayerHealthBarUI : HealthBarUI
{
    private PlayerHealth currentPlayerHealth;
    private void Start()
    {
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.OnHealthChanged += OnHealthChanged;

            OnHealthChanged(
                PlayerHealth.Instance.currentHealth,
                PlayerHealth.Instance.maxHealth
            );
        }
    }

    private void BindPlayerHealth(PlayerHealth playerHealth)
    {
        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.OnHealthChanged -= OnHealthChanged;
        }

        currentPlayerHealth = playerHealth;

        currentPlayerHealth.OnHealthChanged += OnHealthChanged;

        OnHealthChanged(
            currentPlayerHealth.currentHealth,
            currentPlayerHealth.maxHealth
        );
    }

    private void OnEnable()
    {
        LevelSpawn.OnPlayerSpawned += BindPlayerHealth;
    }


    private void OnDestroy()
    {
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.OnHealthChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        UpdateBar(current, max); 
    }
}
