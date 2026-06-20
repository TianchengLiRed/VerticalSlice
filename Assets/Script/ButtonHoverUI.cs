using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class ButtonHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private float scaleSpeed = 10f;

    [SerializeField] private GameObject namePanel;
    [SerializeField] private string sceneName;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        if (namePanel != null)
            namePanel.SetActive(false);
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
            namePanel.SetActive(true);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHover();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
            namePanel.SetActive(false);
    }

    public void OnLevelButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
        }
        SceneManager.LoadScene(sceneName);
    }
}