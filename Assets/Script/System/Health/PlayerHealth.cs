using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerHealth : HealthManager
{
    public static PlayerHealth Instance;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Die()
    {
        Debug.Log("Player died. Game Over.");
        // 打开 GameOver UI
    }
    public override void TakeDamage(float damage)
   {
       base.TakeDamage(damage);
        PlayerAttacked.Instance.PlayerAttackedEffect();

       EventBus.Trigger("PlayerDamaged");
   }
}
