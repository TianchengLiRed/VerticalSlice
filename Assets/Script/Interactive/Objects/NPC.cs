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
                AudioManager.Instance.PlayTalk();
                break;

            case QuestManager.QuestState.UseComputer:
                DialogueManager.Instance.StartDialogue(afterComputerDialogue);
                AudioManager.Instance.PlayTalk();
                break;

            case QuestManager.QuestState.ReturnToSubmit:
                DialogueManager.Instance.StartDialogue(submitDialogue);
                QuestManager.Instance.SubmitMainItem();
                AudioManager.Instance.PlayTalk();
                break;
        }
    }
}