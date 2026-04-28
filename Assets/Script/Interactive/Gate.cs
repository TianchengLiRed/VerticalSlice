using UnityEngine;

public class Gate: MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (QuestManager.Instance.CurrentQuest == QuestManager.QuestState.ReturnToSubmit)
        {
            QuestManager.Instance.SubmitMainItem();
            Debug.Log("任务完成，离开成功");
        }
    }
}