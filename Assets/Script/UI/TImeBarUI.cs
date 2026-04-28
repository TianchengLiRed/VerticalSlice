using UnityEngine;
using UnityEngine.UI;

public class TurnTimeBarUI : MonoBehaviour
{
    [SerializeField] private Slider timeSlider;

    private void Start()
    {
        if (timeSlider != null)
        {
            timeSlider.minValue = 0f;
            timeSlider.maxValue = 1f;
            timeSlider.value = 1f;
        }
    }

    private void Update()
    {
        if (TurnManager.Instance == null) return;
        if (timeSlider == null) return;

        timeSlider.value = TurnManager.Instance.TimerPercent;
    }
}
