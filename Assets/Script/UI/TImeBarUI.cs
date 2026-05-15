using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimeBarUI : MonoBehaviour
{
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI RoundCount;

    private void Start()
    {
        TurnManager.Instance.OnTurnStarted += RoundUpdate;
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

        float time = TurnManager.Instance.GetTimerPercent();
        timeSlider.value = time;
    }

    private void RoundUpdate(int roundcount)
    {
        RoundCount.text =""+ roundcount;
    }
}
