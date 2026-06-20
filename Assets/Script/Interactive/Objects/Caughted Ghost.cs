using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaughtedGhost : Interactive
{
    public DialogueData hateDialogue;

    public override void OnInteract()
    {
        AudioManager.Instance.PlayTalk();
        DialogueManager.Instance.StartDialogue(hateDialogue);
    }
}
