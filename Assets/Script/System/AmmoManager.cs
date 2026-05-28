using System;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance;

    public event Action<int> OnAmmoChanged;

    [SerializeField] private int maxAmmo = 6;
    [SerializeField] private int currentAmmo = 3;
    public int CurrentAmmo => currentAmmo;

    private void Awake()
    {
        Instance = this;
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    public bool UseAmmo()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("û���ӵ�");
            return false;
        }

        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo);
        return true;
    }
}