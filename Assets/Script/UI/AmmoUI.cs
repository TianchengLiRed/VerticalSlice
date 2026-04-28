using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    private void Start()
    {
        UpdateAmmoText();

        if (AmmoManager.Instance != null)
        {
            AmmoManager.Instance.OnAmmoChanged += HandleAmmoChanged;
        }
    }

    private void OnDestroy()
    {
        if (AmmoManager.Instance != null)
        {
            AmmoManager.Instance.OnAmmoChanged -= HandleAmmoChanged;
        }
    }

    private void HandleAmmoChanged(int ammo)
    {
        UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        if (ammoText == null || AmmoManager.Instance == null) return;

        ammoText.text = "Ammo: " + AmmoManager.Instance.CurrentAmmo;
    }
}
