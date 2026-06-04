using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

public class GhostHealth : HealthManager
{
    private GhostController ghostController;
    private GhostColorChange colorC;

    private void Awake()
    {
        ghostController = GetComponent<GhostController>();
        colorC = GetComponentInChildren<GhostColorChange>();
    }

    public override void Heal(float amount)
    {
        return;
    }

    protected override void Die()
    {
        Debug.Log("Ghost died.");
        if (ghostController != null)
        ghostController.DestroyUI();
        gameObject.SetActive(false);

    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (ghostController != null)
            ghostController.ShowHitAlert();

        colorC.ShaderHealth(currentHealth, maxHealth);
    }
    
}