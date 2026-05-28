using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerHealth : HealthManager
{
    public static PlayerHealth Instance;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player died. Game Over.");
        QuestManager.Instance.SetQuest(QuestManager.QuestState.TalkToNPC);
        SceneManager.LoadScene("Station");
        // 打开 GameOver UI
    }
    public override void TakeDamage(float damage)
   {
       base.TakeDamage(damage);
        PlayerAttacked.Instance.PlayerAttackedEffect();
        CameraShake.Instance.Shake();
        EventBus.Trigger("PlayerAttacked");
        EventBus.Trigger("PlayerDamaged");
   }
}
