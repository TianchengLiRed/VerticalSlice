using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image[] ammoImages;

    private void Start()
    {
        UpdateAmmoUI(AmmoManager.Instance.CurrentAmmo);

        if (AmmoManager.Instance != null)
        {
            AmmoManager.Instance.OnAmmoChanged += AmmoChanged;
        }
    }

    private void OnDestroy()
    {
        if (AmmoManager.Instance != null)
        {
            AmmoManager.Instance.OnAmmoChanged -= AmmoChanged;
        }
    }

    private void AmmoChanged(int ammo)
    {
        UpdateAmmoUI(ammo);
    }

    private void UpdateAmmoUI(int currentAmmo)
    {
        if (ammoText == null || AmmoManager.Instance == null) return;
        ammoText.text = "Ammo: " + currentAmmo;
        for (int i = 0; i < ammoImages.Length; i++)
        {
            ammoImages[i].gameObject.SetActive(i < currentAmmo);
        }


    }
}
