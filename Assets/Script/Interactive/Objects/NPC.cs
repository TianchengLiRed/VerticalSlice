using UnityEngine;

public class NPC : Interactive
{
    public DialogueData talkDialogue;
    public DialogueData afterComputerDialogue;
    public DialogueData submitDialogue;

    public override void OnInteract()
    {
        switch (QuestManager.Instance.CurrentQuest)
        {
            case QuestManager.QuestState.TalkToNPC:
                DialogueManager.Instance.StartDialogue(talkDialogue);
                QuestManager.Instance.CompleteTalkToNPC();
                break;

            case QuestManager.QuestState.UseComputer:
                DialogueManager.Instance.StartDialogue(afterComputerDialogue);
                break;

            case QuestManager.QuestState.ReturnToSubmit:
                DialogueManager.Instance.StartDialogue(submitDialogue);
                QuestManager.Instance.SubmitMainItem();
                break;
        }
    }
}