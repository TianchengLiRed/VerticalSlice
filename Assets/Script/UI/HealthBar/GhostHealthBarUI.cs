using UnityEngine;

public class GhostHealthBarUI : HealthBarUI
{
    private GhostHealth targetHealth;
    private Transform target;
    [SerializeField]private RectTransform rectTransform;
    [SerializeField]private Camera mainCam;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    public void Initiate(GhostHealth health)
    {

        targetHealth = health;
        target = health.transform;
        targetHealth.OnHealthChanged += OnHealthChanged;

        OnHealthChanged(
            targetHealth.currentHealth,
            targetHealth.maxHealth
        );
    }

    private void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= OnHealthChanged;
        }
    }

    private void LateUpdate()
    {
        if (target == null || mainCam == null || rectTransform == null)
        return;
        Vector3 screenPos =
        mainCam.WorldToScreenPoint(target.position + offset);

        rectTransform.position = screenPos;
    }



    private void OnHealthChanged(float current, float max)
    {
        UpdateBar(current, max);
    }
}
