using UnityEngine;

public class GhostHealthBarUI : HealthBarUI
{
    private void Start()
    {
        if (GhostHealth.Instance != null)
        {
            GhostHealth.Instance.OnHealthChanged += OnHealthChanged;

            OnHealthChanged(
                GhostHealth.Instance.currentHealth,
                GhostHealth.Instance.maxHealth
            );
        }
    }

    private void OnDestroy()
    {
        if (GhostHealth.Instance != null)
        {
            GhostHealth.Instance.OnHealthChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        UpdateBar(current, max);
    }
}
