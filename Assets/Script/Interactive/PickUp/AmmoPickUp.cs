using UnityEngine;

public class AmmoPickUp : Collectible
{
    protected override void OnCollect()
    {
        if (AmmoManager.Instance.CurrentAmmo < 6)
        {
            AmmoManager.Instance.AddAmmo(1);
            Destroy(gameObject);
        }
        else
        {
            return;
        }
        

    }
}