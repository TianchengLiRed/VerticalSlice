using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaughtedGhost : Interactive
{
    public DialogueData hateDialogue;

    public override void OnInteract()
    {
        DialogueManager.Instance.StartDialogue(hateDialogue);
    }
}
