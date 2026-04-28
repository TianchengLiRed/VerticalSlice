using UnityEngine;
using TMPro;

public class QuestUIBinder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questText;

    private void Start()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetQuestText(questText);
        }

    }
}