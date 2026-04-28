using UnityEngine;

public class AmmoPickUp : Collectible
{
    protected override void OnCollect()
    {
        AmmoManager.Instance.AddAmmo(amount);
        Debug.Log("ʰȡ Ammo +" + amount);
    }
}