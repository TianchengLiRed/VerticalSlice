using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] protected Slider slider;

    protected virtual void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    protected void UpdateBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}