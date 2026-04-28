using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawn : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Grid Position")]
    [SerializeField] private Vector2Int spawnGridPosition;
    private GameObject currentPlayer;
    
    private void OnEnable()
    {
        RVcrash.OnGameStart += SpawnPlayer;//监听动画结束事件,结束后放置player
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
    }
}
