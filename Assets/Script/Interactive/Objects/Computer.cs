using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Computer : Interactive
{
    public override void OnInteract()
    {
        SceneManager.LoadScene("HauntedHouse");
        if (QuestManager.Instance.CurrentQuest != QuestManager.QuestState.UseComputer)
            return;

        Debug.Log("Ê¹ÓÃµçÄÔ");
        QuestManager.Instance.CompleteUseComputer();
    }
}
