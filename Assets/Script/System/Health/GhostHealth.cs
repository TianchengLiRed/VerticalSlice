using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHealth : HealthManager
{
    public static GhostHealth Instance;

    private void Awake()
    {
        Instance = this;
    }
    public override void Heal(float amount)
    {
        // Ghost 不回血
        return;
    }

    protected override void Die()
    {
        Debug.Log("Ghost died.");
        gameObject.SetActive(false);
    }
}
