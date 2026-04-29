using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelSpawn : MonoBehaviour
{
    
    public static event Action<PlayerHealth> OnPlayerSpawned;
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Grid Position")]
    [SerializeField] private Vector2Int spawnGridPosition;
    private GameObject currentPlayer;
    
   private void OnEnable()
{
    RVcrash.OnGameStart -= SpawnPlayer; // 先移除，防止重复订阅
    RVcrash.OnGameStart += SpawnPlayer;
}

private void OnDisable()
{
    RVcrash.OnGameStart -= SpawnPlayer;
}

    private void SpawnPlayer()
    {
        //如果有current删除
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }
        //获取node坐标
        Node spawnNode = GridManager.Instance.GetNode(spawnGridPosition);
        //获取当前node的世界坐标
        Vector3 worldPosition = GridManager.Instance.GetWorldPosition(spawnNode);
        //生成玩家在这个位置
        currentPlayer = Instantiate(playerPrefab, worldPosition, Quaternion.identity);

        spawnNode.SetOccupied(true);

        PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            OnPlayerSpawned?.Invoke(playerHealth);
        }
    }
}
