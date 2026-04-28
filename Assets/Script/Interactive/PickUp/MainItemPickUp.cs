using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainItemPickUp : Collectible
{
    protected override void OnCollect()
    {
        QuestManager.Instance.PickUpMainItem();
        Debug.Log("拿到关键道具");
    }
}
