using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAlertUI : MonoBehaviour
{
    public static GhostAlertUI Instance;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private float flashTime = 1f;
    private RectTransform rectTransform;
    private Camera mainCam;
    private Coroutine alertRoutine;

    private void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        Vector3 screenPos =
        mainCam.WorldToScreenPoint(target.position + offset);

        rectTransform.position = screenPos;
    }

    public void ShowAlert()
    {
        gameObject.SetActive(true);
        alertRoutine = StartCoroutine(Alert());
    }

    private IEnumerator  Alert()
    {

        yield return new WaitForSeconds(flashTime);

        gameObject.SetActive(false);
    }
}
