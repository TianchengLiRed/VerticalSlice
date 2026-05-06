using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemindIcons : MonoBehaviour
{
    [SerializeField] private GameObject remindPanel;

    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        TurnManager.Instance.OnTurnStarted += TurnStartRemind;

        LevelSpawn.OnPlayerSpawned += SetPlayer;

        RemindHide();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.R))
            RemindHide();
    }

    private void LateUpdate()
    {
        if (player == null) return;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position + offset);
        rectTransform.position = screenPos;
    }

    private void TurnStartRemind(int roundcount)
    {
        if (player == null) return;
        remindPanel.gameObject.SetActive(true);
    }
    
    private void RemindHide()
    {
        remindPanel.gameObject.SetActive(false);
    }

    void SetPlayer(PlayerHealth health)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
}
