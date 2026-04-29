using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestManager.Instance.SetQuest(QuestManager.QuestState.TalkToNPC);
            SceneManager.LoadScene("station");
        }
        
    }
}
