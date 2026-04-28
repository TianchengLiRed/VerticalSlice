using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public enum QuestState
    {
        TalkToNPC,
        UseComputer,
        PickUpMainItem,
        ReturnToSubmit,
        Finished
    }

    [SerializeField] private TextMeshProUGUI questText;

    private QuestState currentQuest;
    private bool hasMainItem = false;

    public QuestState CurrentQuest => currentQuest;
    public bool HasMainItem => hasMainItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetQuest(QuestState.TalkToNPC);
    }

    public void SetQuest(QuestState newQuest)
    {
        currentQuest = newQuest;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (questText == null) return;

        switch (currentQuest)
        {
            case QuestState.TalkToNPC:
                questText.text = "Talk to the NPC";
                break;

            case QuestState.UseComputer:
                questText.text = "Use the computer";
                break;

            case QuestState.PickUpMainItem:
                questText.text = "Find the key item";
                break;

            case QuestState.ReturnToSubmit:
                questText.text = "Return and submit";
                break;

            case QuestState.Finished:
                questText.text = "Task Complete";
                break;
        }
    }

    public void CompleteTalkToNPC()
    {
        if (currentQuest == QuestState.TalkToNPC)
            SetQuest(QuestState.UseComputer);
    }

    public void CompleteUseComputer()
    {
        if (currentQuest == QuestState.UseComputer)
            SetQuest(QuestState.PickUpMainItem);
    }

    public void PickUpMainItem()
    {
        if (currentQuest == QuestState.PickUpMainItem)
        {
            hasMainItem = true;
            SetQuest(QuestState.ReturnToSubmit);
        }
    }

    public void SubmitMainItem()
    {
        if (currentQuest == QuestState.ReturnToSubmit && hasMainItem)
        {
            hasMainItem = false;
            SetQuest(QuestState.Finished);
        }
    }

    public void SetQuestText(TextMeshProUGUI newText)
    {
        questText = newText;
        UpdateUI();
    }
}